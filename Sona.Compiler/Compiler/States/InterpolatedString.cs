using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using IS4.Sona.Compiler.Tools;
using IS4.Sona.Grammar;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class InterpolatedString : NodeState
    {
        readonly List<string> parts = new();
        string? fillName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            parts.Clear();
            fillName = null;
        }

        protected override void OnEnterToken(IToken token)
        {
            // Do not inform writer about new lines
        }

        public override void EnterInterpolatedString(InterpolatedStringContext context)
        {
            Out.Write('(');
            parts.Add("$\"");
        }

        public override void EnterVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            Out.Write('(');
            parts.Add("$@\"");
        }

        private void OnExit()
        {
            foreach(var part in parts)
            {
                Out.Write(part);
            }
            parts.Clear();
            Out.Write("\")");
        }

        public override void ExitInterpolatedString(InterpolatedStringContext context)
        {
            OnExit();
            ExitState().ExitInterpolatedString(context);
        }

        public override void ExitVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            OnExit();
            ExitState().ExitVerbatimInterpolatedString(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            var token = node.Symbol;
            switch(token.Type)
            {
                case SonaLexer.PERCENT:
                    parts.Add("%%");
                    break;
                case SonaLexer.STRING_PART:
                    parts.Add(token.Text);
                    break;
                case SonaLexer.LITERAL_NEWLINE:
                    parts.Add(LexerContext.GetState<NewlinePragma>()?.NewLineSequence ?? ScriptEnvironment.DefaultNewLineSequence);
                    break;
                case SonaLexer.LITERAL_ESCAPE_NEWLINE:
                    parts.Add(token.Text.Substring(1));
                    break;
            }
        }

        public override void EnterInterpStrAlignment(InterpStrAlignmentContext context)
        {
            AddFill();
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrAlignment(InterpStrAlignmentContext context)
        {
            try
            {
                parts.Add(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void EnterInterpStrGeneralFormat(InterpStrGeneralFormatContext context)
        {
            Environment.EnableParseTree();
        }

        public sealed override void ExitInterpStrGeneralFormat(InterpStrGeneralFormatContext context)
        {
            try
            {
                var text = context.GetText().TrimStart(':');
                parts.Add("%");
                parts.Add(text);
                AddFill();
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void EnterInterpStrStandardFormat(InterpStrStandardFormatContext context)
        {
            AddFill();
            Environment.EnableParseTree();
        }

        public sealed override void ExitInterpStrStandardFormat(InterpStrStandardFormatContext context)
        {
            try
            {
                var text = context.GetText().TrimStart(':');
                var parsedText = Syntax.GetStringLiteralValue(text);
                if(parsedText.Length == 1 && !Char.IsLetter(parsedText[0]))
                {
                    var suggestion = parsedText[0] switch
                    {
                        '"' => "\"\\\"\"",
                        '\'' => "\"'\"",
                        _ => text.Replace('\'', '\"')
                    };
                    Error($"Standard format specifiers (as '…') must be a single letter. Use `{suggestion}` if you wish to use a custom format specifier.", context);
                }
                AddUncheckedFormat(parsedText, context);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void EnterInterpStrCustomFormat(InterpStrCustomFormatContext context)
        {
            AddFill();
            Environment.EnableParseTree();
        }

        public sealed override void ExitInterpStrCustomFormat(InterpStrCustomFormatContext context)
        {
            try
            {
                var text = context.GetText().TrimStart(':');
                var parsedText = Syntax.GetStringLiteralValue(text);
                if(parsedText.Length == 1 && Char.IsLetter(parsedText[0]))
                {
                    Error($"Custom format specifiers (as \"…\") are not expressed as a single letter. Use the format-specific method (such as `\"%{parsedText[0]}\"`) to express it, or `'{parsedText[0]}'` if you wanted to use a standard format specifier.\"", context);
                }
                AddUncheckedFormat(parsedText, context);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void EnterInterpStrNumberFormat(InterpStrNumberFormatContext context)
        {
            AddFill();
            Out.WriteOperator(':');
            Out.WriteNamespacedName("Sona.Runtime.Traits", "trait number");
            Out.Write("<_>");
            Environment.EnableParseTree();
        }

        public sealed override void ExitInterpStrNumberFormat(InterpStrNumberFormatContext context)
        {
            try
            {
                var text = context.GetText().TrimStart(':');
                AddUncheckedFormat(text, context);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void EnterInterpStrComponentFormat(InterpStrComponentFormatContext context)
        {
            AddFill();
            EnterState<Components>().EnterInterpStrComponentFormat(context);
            base.EnterInterpStrComponentFormat(context);
        }

        public sealed override void ExitInterpStrComponentFormat(InterpStrComponentFormatContext context)
        {

        }

        public void ExitInterpStrComponentFormat(InterpStrComponentFormatContext context, string format)
        {
            AddUncheckedFormat(format, context);
        }

        private void AddUncheckedFormat(string text, ParserRuleContext context)
        {
            if(text.IndexOf(')') != -1)
            {
                Error("The ')' character cannot be represented in a format string.", context);
            }
            if(Syntax.IsValidIdentifierName(text))
            {
                // Add as-is
                parts.Add(":");
                parts.Add(text);
                return;
            }
            if(!Syntax.IsValidEnclosedIdentifierName(text))
            {
                // Cannot be enclosed
                Error($"The format '{text}' cannot be syntactically represented.", context);
                return;
            }
            parts.Add(":``");
            parts.Add(text);
            parts.Add("``");
        }

        private void AddFill()
        {
            if(fillName is not null)
            {
                Out.Write(')');
                parts.Add("{");
                if(Syntax.IsValidIdentifierName(fillName))
                {
                    parts.Add(fillName);
                }
                else
                {
                    parts.Add("``");
                    parts.Add(fillName);
                    parts.Add("``");
                }
                fillName = null;
            }
        }

        public override void EnterInterpStrExpression(InterpStrExpressionContext context)
        {
            // let valI = ...
            fillName = Out.CreateTemporaryIdentifier();
            Out.Write("let ");
            Out.WriteIdentifier(fillName);
            Out.WriteOperator('=');
            Out.Write('(');
        }

        public override void ExitInterpStrExpression(InterpStrExpressionContext context)
        {
            AddFill();
            parts.Add("}");
            Out.Write(" in ");
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        sealed class Components : NodeState
        {
            StringBuilder text = new();
            State instantState = new State(false), durationState = new State(true);

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                text.Clear();
                instantState = new(instantState);
                durationState = new(durationState);
            }

            protected sealed override void OnEnterToken(IToken token)
            {

            }

            public sealed override void EnterInterpStrComponentFormat(InterpStrComponentFormatContext context)
            {

            }

            public sealed override void ExitInterpStrComponentFormat(InterpStrComponentFormatContext context)
            {
                try
                {
                    OnEnd(ref instantState);
                    OnEnd(ref durationState);
                    if((instantState.Traits, durationState.Traits) is (Traits.None, _) or (_, Traits.None) or (Traits.Invalid, Traits.Invalid))
                    {
                        // If one is None, there was no character that could specify the trait
                        Error($"The format string '{text}' does not contain any recognized specifiers. To use it without validation, enclose it in quotes.", context);
                    }
                    else if(instantState.Traits == Traits.Invalid)
                    {
                        // Duration is valid
                        Out.WriteOperator(':');
                        Out.WriteNamespacedName("Sona.Runtime.Traits", "trait " + durationState.TraitName);
                        Out.Write("<_>");
                    }
                    else if(durationState.Traits == Traits.Invalid)
                    {
                        // Instant is valid
                        Out.WriteOperator(':');
                        Out.WriteNamespacedName("Sona.Runtime.Traits", "trait " + instantState.TraitName);
                        Out.Write("<_>");
                    }
                    else
                    {
                        // Both are valid
                        Out.WriteOperator("|>");
                        Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Inference");
                        Out.Write('.');
                        Out.WriteIdentifier(instantState.TraitName + "|" + durationState.TraitName);
                    }
                }
                finally
                {
                    ((InterpolatedString)ExitState()).ExitInterpStrComponentFormat(context, text.ToString());
                }
            }

            public override void VisitTerminal(ITerminalNode node)
            {
                var token = node.Symbol.Text;

                char type = token[0];

                if(type is '\\' or '\'')
                {
                    // Escape characters do not contribute
                    text.Append(token);
                    return;
                }
                
                if(type == '%')
                {
                    text.Append(token);

                    // Singleton - just trim %
                    type = token[1];
                    token = token.Substring(1);
                }
                else if(!Char.IsAsciiLetter(type) && type is not (':' or '/'))
                {
                    // Escape literal characters
                    if(token.Length == 1)
                    {
                        text.Append('\\');
                        text.Append(token);
                    }
                    else
                    {
                        text.Append('\'');
                        text.Append(token);
                        text.Append('\'');
                    }
                    return;
                }
                else
                {
                    text.Append(token);
                }

                if(instantState.Traits != Traits.Invalid)
                {
                    OnSymbol(type, token, ref instantState);
                }
                if(durationState.Traits != Traits.Invalid)
                {
                    OnSymbol(type, token, ref durationState);
                }
                if(instantState.Traits == Traits.Invalid && durationState.Traits == Traits.Invalid)
                {
                    // Last specifier that could be valid
                    Error($"Format specifier '{token}' is not recognized in this context.", node);
                }
            }

            private void OnSymbol(char type, string symbol, ref State state)
            {
                var (traits, maxLength, categoryLengthDivisor, section) = state.GetTypeInfo(ref type);
                if(symbol.Length > maxLength)
                {
                    // Longer than permitted
                    state.Require(Traits.Invalid);
                    return;
                }
                if(categoryLengthDivisor != 0)
                {
                    // Check the category for this specifier (type may be mutated previously)
                    // The divisor is used for dd/dddd etc. to get the category from length
                    var category = (type, (symbol.Length - 1) / categoryLengthDivisor);
                    if(!state.VisitedCategories.Add(category))
                    {
                        // Category already visited
                        state.Require(Traits.Invalid);
                        return;
                    }
                    if(state.RequiredSection != Section.None && section != state.RequiredSection)
                    {
                        // A section was required but this is a wrong one
                        state.Require(Traits.Invalid);
                        return;
                    }
                    // No section required next
                    state.RequiredSection = Section.None;
                }
                else
                {
                    // This is a delimiter
                    if(state.CurrentSection != section)
                    {
                        // But in a wrong section
                        state.Require(Traits.Invalid);
                        return;
                    }
                    if(state.RequiredSection != Section.None)
                    {
                        // A section was required from previous delimiter but this is a delimiter too
                        state.Require(Traits.Invalid);
                        return;
                    }
                    // Require the section to be next
                    state.RequiredSection = section;
                }
                if(section != state.CurrentSection)
                {
                    // This changes the section
                    if(!state.VisitedSections.Add(section))
                    {
                        // But the section was already visited once
                        state.Require(Traits.Invalid);
                        return;
                    }
                    state.CurrentSection = section;
                }
                state.Require(traits);
            }

            private void OnEnd(ref State state)
            {
                if(state.VisitedCategories.Count == 0)
                {
                    // A letter must be visited
                    state.Require(Traits.Invalid);
                    return;
                }
                if(state.RequiredSection != Section.None)
                {
                    // A section was required to be next
                    return;
                }
            }

            struct State
            {
                public Traits Traits;
                public Section CurrentSection;
                public Section RequiredSection;
                public bool IsDuration { get; }
                public HashSet<(char type, int category)> VisitedCategories { get; }
                public HashSet<Section> VisitedSections { get; }

                public string TraitName => Traits.ToString().ToLowerInvariant() + (IsDuration ? "span" : "");

                public State(bool isDuration) : this()
                {
                    IsDuration = isDuration;
                    VisitedCategories = new();
                    VisitedSections = new();
                }

                public State(State previous) : this()
                {
                    IsDuration = previous.IsDuration;
                    VisitedCategories = previous.VisitedCategories;
                    VisitedCategories.Clear();
                    VisitedSections = previous.VisitedSections;
                    VisitedSections.Clear();
                }

                public void Require(Traits traits)
                {
                    Traits |= traits;
                }

                public (Traits traits, int maxLength, int categoryLengthDivisor, Section section) GetTypeInfo(ref char type)
                {
                    if(IsDuration)
                    {
                        switch(type)
                        {
                            case 'y':
                                return (Traits.Date, 8, Int32.MaxValue, Section.Date);
                            case 'M':
                                return (Traits.Date, 2, Int32.MaxValue, Section.Date);
                            case 'd':
                                return (Traits.Time, 8, Int32.MaxValue, Section.Date);
                            case 'h':
                            case 'm':
                            case 's':
                                return (Traits.Time, 2, Int32.MaxValue, Section.Time);
                            case 'f':
                                return (Traits.Time, 7, Int32.MaxValue, Section.Time);
                            case 'F':
                                type = 'f';
                                goto case 'f';
                            default:
                                return (Traits.Invalid, 0, 0, Section.None);
                        }
                    }
                    else
                    {
                        switch(type)
                        {
                            case 'd':
                            case 'M':
                                return (Traits.Date, 4, 2, Section.Date);
                            case 'y':
                                return (Traits.Date, 5, Int32.MaxValue, Section.Date);
                            case 'z':
                                return (Traits.DateTime, 3, Int32.MaxValue, Section.TimeZone);
                            case 'H':
                                type = 'h';
                                goto case 'h';
                            case 'h':
                            case 'm':
                            case 's':
                            case 't':
                                return (Traits.Time, 2, Int32.MaxValue, Section.Time);
                            case 'g':
                                return (Traits.Date, 2, Int32.MaxValue, Section.Era);
                            case 'f':
                                return (Traits.Time, 7, Int32.MaxValue, Section.Time);
                            case 'F':
                                type = 'f';
                                goto case 'f';
                            case 'K':
                                return (Traits.DateTime, 1, Int32.MaxValue, Section.TimeZone);
                            case ':':
                                return (Traits.Time, 1, 0, Section.Time);
                            case '/':
                                return (Traits.Date, 1, 0, Section.Date);
                            default:
                                return (Traits.Invalid, 0, 0, Section.None);
                        }
                    }
                }
            }

            [Flags]
            enum Traits
            {
                None = 0,
                Time = 1,
                Date = 2,
                DateTime = 3,
                Invalid = -1
            }

            enum Section
            {
                None,
                Time,
                Date,
                TimeZone,
                Era
            }
        }
    }

    internal abstract class LiteralInterpolatedString : NodeState
    {
        protected sealed override void OnEnterToken(IToken token)
        {
            // Do not inform writer about new lines
        }

        public sealed override void EnterInterpolatedString(InterpolatedStringContext context)
        {
            Out.Write("(\"");
        }

        public sealed override void EnterVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            Out.Write("(@\"");
        }

        private void OnExit()
        {
            Out.Write("\")");
        }

        public sealed override void ExitInterpolatedString(InterpolatedStringContext context)
        {
            OnExit();
            ExitState().ExitInterpolatedString(context);
        }

        public sealed override void ExitVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            OnExit();
            ExitState().ExitVerbatimInterpolatedString(context);
        }

        public sealed override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            var token = node.Symbol;
            switch(token.Type)
            {
                case SonaLexer.PERCENT:
                    Out.Write("%");
                    break;
                case SonaLexer.STRING_PART:
                    Out.Write(token.Text);
                    break;
                case SonaLexer.LITERAL_NEWLINE:
                    Out.Write(LexerContext.GetState<NewlinePragma>()?.NewLineSequence ?? ScriptEnvironment.DefaultNewLineSequence);
                    break;
                case SonaLexer.LITERAL_ESCAPE_NEWLINE:
                    Out.Write(token.Text.Substring(1));
                    break;
            }
        }

        public sealed override void EnterInterpStrAlignment(InterpStrAlignmentContext context)
        {
            Error("The alignment cannot be specified in a literal interpolated string expression.", context);
        }

        public sealed override void ExitInterpStrAlignment(InterpStrAlignmentContext context)
        {

        }

        private void FormatError(ParserRuleContext context)
        {
            Error("The format cannot be specified in a literal interpolated string expression.", context);
        }

        public sealed override void EnterInterpStrGeneralFormat(InterpStrGeneralFormatContext context)
        {
            FormatError(context);
        }

        public sealed override void ExitInterpStrGeneralFormat(InterpStrGeneralFormatContext context)
        {

        }

        public sealed override void EnterInterpStrStandardFormat(InterpStrStandardFormatContext context)
        {
            FormatError(context);
        }

        public sealed override void ExitInterpStrStandardFormat(InterpStrStandardFormatContext context)
        {

        }

        public sealed override void EnterInterpStrCustomFormat(InterpStrCustomFormatContext context)
        {
            FormatError(context);
        }

        public sealed override void ExitInterpStrCustomFormat(InterpStrCustomFormatContext context)
        {

        }

        public sealed override void EnterInterpStrNumberFormat(InterpStrNumberFormatContext context)
        {
            FormatError(context);
        }

        public sealed override void ExitInterpStrNumberFormat(InterpStrNumberFormatContext context)
        {

        }

        public sealed override void EnterInterpStrComponentFormat(InterpStrComponentFormatContext context)
        {
            FormatError(context);
        }

        public sealed override void ExitInterpStrComponentFormat(InterpStrComponentFormatContext context)
        {

        }

        public sealed override void EnterInterpStrExpression(InterpStrExpressionContext context)
        {
            Out.Write("\"+(");
        }

        public abstract override void ExitInterpStrExpression(InterpStrExpressionContext context);

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {

        }
    }

    internal sealed class LiteralNormalInterpolatedString : LiteralInterpolatedString
    {
        public override void ExitInterpStrExpression(InterpStrExpressionContext context)
        {
            Out.Write(")+\"");
        }
    }

    internal sealed class LiteralVerbatimInterpolatedString : LiteralInterpolatedString
    {
        public override void ExitInterpStrExpression(InterpStrExpressionContext context)
        {
            Out.Write(")+@\"");
        }
    }
}
