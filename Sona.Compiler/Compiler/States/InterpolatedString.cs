using Antlr4.Runtime;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class InterpolatedString : NodeState
    {
        protected override void UpdateOnToken(IToken token)
        {
            // Do not inform writer about new lines
        }

        public override void EnterInterpolatedString(InterpolatedStringContext context)
        {
            Out.WriteLeftOperator("$\"");
        }

        public override void EnterVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            Out.WriteLeftOperator("$@\"");
        }

        public override void ExitInterpolatedString(InterpolatedStringContext context)
        {
            Out.Write('"');
            ExitState().ExitInterpolatedString(context);
        }

        public override void EnterInterpStrPart(InterpStrPartContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrPart(InterpStrPartContext context)
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

        public override void EnterInterpStrPercent(InterpStrPercentContext context)
        {

        }

        public override void ExitInterpStrPercent(InterpStrPercentContext context)
        {
            Out.Write("%%");
        }

        public override void EnterInterpStrAlignment(InterpStrAlignmentContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrAlignment(InterpStrAlignmentContext context)
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

        public override void EnterInterpStrFormat(InterpStrFormatContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInterpStrFormat(InterpStrFormatContext context)
        {
            try
            {
                var text = context.GetText().Substring(1);
                Out.Write(':');
                Out.WriteSymbol(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInterpStrExpression(InterpStrExpressionContext context)
        {
            Out.Write('{');
        }

        public override void ExitInterpStrExpression(InterpStrExpressionContext context)
        {
            Out.Write('}');
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }
    }
}
