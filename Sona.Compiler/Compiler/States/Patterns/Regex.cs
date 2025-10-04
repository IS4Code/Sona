using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class RegexPatternState : NodeState
    {
        readonly StringBuilder regexBuilder = new();
        readonly StringBuilder flagsBuilder = new();
        RegexPosition position;
        IToken? lastToken;

        readonly List<ISourceCapture> captures = new();
        ISourceCapture? patternCapture;
        int groups;

        const RegexOptions validateRegexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            regexBuilder.Clear();
            flagsBuilder.Clear();
            position = RegexPosition.Start;
            lastToken = null;
            captures.Clear();
            patternCapture = null;
            groups = 0;
        }

        public override void EnterRegexPattern(RegexPatternContext context)
        {

        }

        public override void ExitRegexPattern(RegexPatternContext context)
        {
            if(flagsBuilder.Length > 0)
            {
                flagsBuilder.Append(')');

                // Validate flags
                var flags = flagsBuilder.ToString();
                try
                {
                    new Regex(flags, validateRegexOptions);
                    regexBuilder.Insert(0, flagsBuilder);
                }
                catch(ArgumentException e)
                {
                    Error("Unrecognized regular expression options: " + e.Message, context);
                }
            }

            var pattern = regexBuilder.ToString();

            // Validate regex
            try
            {
                var regex = new Regex(pattern, validateRegexOptions);

                var groupNames = regex.GetGroupNames();
                if(groupNames.Length != groups + 1 || groupNames[0] != "0" || groupNames.Skip(1).Any(n => n.Length == 0 || n[0] is not (>= '0' and <= '9')) || Enumerable.Range(1, groups).Any(n => regex.GroupNumberFromName(n.ToString()) == -1))
                {
                    Error("Regular expressions in patterns may not contain more capturing groups than those indicated by pattern captures.", context);
                }
            }
            catch(ArgumentException e)
            {
                Error("Invalid regular expression: " + e.Message, context);
            }

            // Hash collisions will be exposed
            var storageIdentifier = "Regex " + GetStringKey(pattern);

            if(Environment.DefineGlobalSymbol(storageIdentifier))
            {
                // Define a type whose static constructor creates the regex
                GlobalOut.Write("type [<");
                GlobalOut.WriteCoreName("AbstractClassAttribute");
                GlobalOut.Write(';');
                GlobalOut.WriteCoreName("SealedAttribute");
                GlobalOut.Write(">]");
                GlobalOut.WriteIdentifier(storageIdentifier);
                GlobalOut.WriteLine(" private() =");
                GlobalOut.EnterScope();
                GlobalOut.Write("static member val Instance");
                GlobalOut.WriteOperator('=');
                GlobalOut.WriteCustomPattern("CreateRegex");
                GlobalOut.Write("(\"");
                GlobalOut.WriteStringPart(pattern);
                GlobalOut.Write("\")");
                GlobalOut.ExitScope();
                GlobalOut.WriteLine();
            }

            // Refer to the stored instance
            Out.WriteCustomPattern("MatchRegex");
            Out.Write('(');
            Out.WriteIdentifier(Environment.GlobalModuleIdentifier);
            Out.Write('.');
            Out.WriteIdentifier(storageIdentifier);
            Out.Write(".Instance");
            Out.Write(")([| ");

            // Play back the captures
            bool first = true;
            foreach(var capture in captures)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.Write(';');
                }

                // Target-type to result
                Out.WriteCustomPattern("UnpackRegexGroup");
                Out.Write('(');
                capture.Play(Out);
                Out.Write(')');
            }

            Out.Write(" |])");

            ExitState().ExitRegexPattern(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            // Do not call base to prevent additional validation

            var token = node.Symbol;

            // Parser state must be updated however
            OnEnterToken(token);

            // Check all previous tokens
            OnNextToken(token);

            ProcessToken(token, true);
        }

        private void OnNextToken(IToken token)
        {
            if(lastToken != null && LexerContext.Whitespace?.ReadTokens() is { Count: > 0 } previous)
            {
                int lastIndex = lastToken.TokenIndex;
                int tokenIndex = token.TokenIndex;

                // Previous whitespace tokens to process
                foreach(var previousToken in previous)
                {
                    var index = previousToken.TokenIndex;
                    if(index > lastIndex && index < tokenIndex)
                    {
                        // A whitespace token
                        ProcessToken(previousToken, true);
                    }
                }
            }
        }

        private void ProcessToken(IToken token, bool write)
        {
            if(lastToken != null)
            {
                if(token.Line != lastToken.Line)
                {
                    Error("Regular expressions may not span multiple lines.", token);
                }
                else if(position == RegexPosition.Middle)
                {
                    int distance = token.StartIndex - lastToken.StopIndex - 1;
                    if(distance > 0)
                    {
                        // Fill with spaces (other whitespace tokens are preserved)
                        regexBuilder.Append(' ', distance);
                    }
                }
            }
            lastToken = token;

            switch(token.Type)
            {
                case SonaLexer.COMMENT:
                case SonaLexer.DOC_COMMENT:
                    Error("Comments are not allowed within regular expressions.", token);
                    break;
                case SonaLexer.BEGIN_STRING:
                case SonaLexer.BEGIN_VERBATIM_STRING:
                case SonaLexer.BEGIN_INTERPOLATED_STRING:
                case SonaLexer.BEGIN_VERBATIM_INTERPOLATED_STRING:
                case SonaLexer.STRING_LITERAL:
                case SonaLexer.VERBATIM_STRING_LITERAL:
                    Error("The `\"` character in a regular expression must be escaped.", token);
                    break;
                case SonaLexer.BEGIN_CHAR:
                case SonaLexer.CHAR_LITERAL:
                    Error("The `'` character in a regular expression must be escaped.", token);
                    break;
            }

            var text = token.Text;

            if(text.StartsWith("#", StringComparison.Ordinal))
            {
                Error("The `#` character in a regular expression must be escaped.", token);
            }

            if(token.Type == SonaLexer.SLASH)
            {
                switch(position)
                {
                    case RegexPosition.Start:
                        position = RegexPosition.Middle;
                        return;
                    case RegexPosition.Middle:
                        position = RegexPosition.End;
                        return;
                    default:
                        Error("Invalid regular expression syntax.", token);
                        return;
                }
            }

            switch(position)
            {
                case RegexPosition.Start:
                    Error($"COMPILER ERROR: Unexpected token `{text}` at the beginning of regular expression.", token);
                    break;
                case RegexPosition.Middle when write:
                    regexBuilder.Append(text);
                    break;
                case RegexPosition.End:
                    if(flagsBuilder.Length == 0)
                    {
                        flagsBuilder.Append("(?");
                    }
                    flagsBuilder.Append(text);
                    break;
            }
        }

        private static string GetStringKey(string str)
        {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            var trimmed = base64.AsSpan().TrimEnd('=');
            if(trimmed.Length != base64.Length)
            {
                base64 = trimmed.ToString();
            }
            return base64.Replace('+', '-').Replace('/', '_');
        }

        public override void EnterRegexGroupStart(RegexGroupStartContext context)
        {
            var token = context.Start;
            OnNextToken(token);
            ProcessToken(token, false);

            regexBuilder.Append("(?<");
            regexBuilder.Append(++groups);

            // The full regex needs to be known first
            captures.Add(patternCapture = Out.StartCapture());
            EnterState<Group>().EnterRegexGroupStart(context);
        }

        public override void ExitRegexGroupStart(RegexGroupStartContext context)
        {
            Out.StopCapture(patternCapture ?? ErrorCapture("COMPILER ERROR: Missing pattern capture.", context));
            patternCapture = null;

            regexBuilder.Append('>');

            lastToken = context.Stop;
        }

        enum RegexPosition
        {
            Start,
            Middle,
            End
        }

        sealed class Group : NodeState
        {
            public override void EnterRegexGroupStart(RegexGroupStartContext context)
            {
                
            }

            public override void ExitRegexGroupStart(RegexGroupStartContext context)
            {
                ExitState().ExitRegexGroupStart(context);
            }

            public override void EnterPatternArgument(PatternArgumentContext context)
            {
                EnterState<PatternState.Argument>().EnterPatternArgument(context);
            }

            public override void ExitPatternArgument(PatternArgumentContext context)
            {
                
            }
        }
    }
}
