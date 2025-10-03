using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        int line, column;

        readonly List<ISourceCapture> captures = new();
        ISourceCapture? patternCapture;
        int groups;

        const RegexOptions validateRegexOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            regexBuilder.Clear();
            flagsBuilder.Clear();
            position = RegexPosition.Start;
            captures.Clear();
            patternCapture = null;
            groups = 0;
        }

        public override void EnterRegexPattern(RegexPatternContext context)
        {
            var token = context.Start;
            line = token.Line;
            column = token.Column;

            //Out.WriteCustomPattern("MatchRegex");
            //Out.Write('(');
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
                if(groupNames.Length != groups + 1 || groupNames[0] != "0" || groupNames.Skip(1).Any(n => !n.StartsWith("_", StringComparison.Ordinal)) || Enumerable.Range(1, groups).Any(n => regex.GroupNumberFromName("_" + n) == -1))
                {
                    Error("Regular expressions may not contain additional capturing groups.", context);
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
            // Do not call base

            var token = node.Symbol;

            if(token.Line != line)
            {
                Error("Regular expressions may not span multiple lines.", node);
                line = token.Line;
                column = token.Column;
            }
            else if(token.Column != column)
            {
                Error("Regular expressions may not contain unescaped whitespace characters.", node);
                column = token.Column;
            }

            var text = token.Text;
            column += text.Length;

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
                        Error("Invalid regular expression syntax.", node);
                        return;
                }
            }

            switch(position)
            {
                case RegexPosition.Start:
                    Error($"COMPILER ERROR: Unexpected token '{text}' at the beginning of regular expression.", node);
                    break;
                case RegexPosition.Middle:
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
            regexBuilder.Append("(?<_");
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

            var token = context.Stop;
            column = token.Column + token.Text.Length;
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
