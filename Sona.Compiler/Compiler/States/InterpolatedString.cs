using System.Collections.Generic;
using Antlr4.Runtime;
using IS4.Sona.Compiler.Tools;
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

        protected override void UpdateOnToken(IToken token)
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

        public override void EnterInterpStrPart(InterpStrPartContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrPart(InterpStrPartContext context)
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

        public override void EnterInterpStrPercent(InterpStrPercentContext context)
        {

        }

        public override void ExitInterpStrPercent(InterpStrPercentContext context)
        {
            parts.Add("%%");
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

        public override void EnterInterpStrUncheckedFormat(InterpStrUncheckedFormatContext context)
        {
            AddFill();
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrUncheckedFormat(InterpStrUncheckedFormatContext context)
        {
            try
            {
                var text = context.GetText().Substring(1);
                AddUncheckedFormat(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInterpStrUncheckedFormatString(InterpStrUncheckedFormatStringContext context)
        {
            AddFill();
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrUncheckedFormatString(InterpStrUncheckedFormatStringContext context)
        {
            try
            {
                var text = context.GetText().Substring(1);
                AddUncheckedFormat(Syntax.GetStringLiteralValue(text));
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInterpStrCheckedFormat(InterpStrCheckedFormatContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrCheckedFormat(InterpStrCheckedFormatContext context)
        {
            try
            {
                var text = context.GetText().Substring(1);
                parts.Add("%");
                parts.Add(text);
                AddFill();
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        private void AddUncheckedFormat(string text)
        {
            if(text.IndexOf(')') != -1)
            {
                Error("The ')' character cannot be represented in a format string.");
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
                Error($"The format '{text}' cannot be syntactically represented.");
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
    }

    internal abstract class LiteralInterpolatedString : NodeState
    {
        protected sealed override void UpdateOnToken(IToken token)
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

        public sealed override void EnterInterpStrPart(InterpStrPartContext context)
        {
            Environment.EnableParseTree();
        }

        public sealed override void ExitInterpStrPart(InterpStrPartContext context)
        {
            try
            {
                Out.Write(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void EnterInterpStrPercent(InterpStrPercentContext context)
        {

        }

        public sealed override void ExitInterpStrPercent(InterpStrPercentContext context)
        {
            Out.Write('%');
        }

        public sealed override void EnterInterpStrAlignment(InterpStrAlignmentContext context)
        {
            Error("The alignment cannot be specified in a literal interpolated string expression.");
        }

        public sealed override void ExitInterpStrAlignment(InterpStrAlignmentContext context)
        {

        }

        public sealed override void EnterInterpStrUncheckedFormat(InterpStrUncheckedFormatContext context)
        {
            Error("The format cannot be specified in a literal interpolated string expression.");
        }

        public sealed override void ExitInterpStrUncheckedFormat(InterpStrUncheckedFormatContext context)
        {

        }

        public sealed override void EnterInterpStrUncheckedFormatString(InterpStrUncheckedFormatStringContext context)
        {
            Error("The format cannot be specified in a literal interpolated string expression.");
        }

        public sealed override void ExitInterpStrUncheckedFormatString(InterpStrUncheckedFormatStringContext context)
        {

        }

        public sealed override void EnterInterpStrCheckedFormat(InterpStrCheckedFormatContext context)
        {
            Error("The format cannot be specified in a literal interpolated string expression.");
        }

        public sealed override void ExitInterpStrCheckedFormat(InterpStrCheckedFormatContext context)
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
