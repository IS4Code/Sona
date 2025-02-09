using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler
{
    partial class ScriptState
    {
        public sealed override void EnterErrorMissingExpression(ErrorMissingExpressionContext context)
        {
            Error("This construct requires an expression.");
        }

        public sealed override void ExitErrorMissingExpression(ErrorMissingExpressionContext context)
        {

        }
    }
}
