using Antlr4.Runtime.Misc;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler
{
    partial class ScriptState
    {
        public sealed override void EnterErrorMissingExpression(ErrorMissingExpressionContext context)
        {

        }

        public sealed override void EnterErrorUnderscoreReserved(ErrorUnderscoreReservedContext context)
        {

        }

        public sealed override void EnterErrorUnsupportedNumberSuffix(ErrorUnsupportedNumberSuffixContext context)
        {
            Environment.EnableParseTree();
        }

        public sealed override void EnterErrorUnsupportedStringSuffix(ErrorUnsupportedStringSuffixContext context)
        {
            Environment.EnableParseTree();
        }

        public sealed override void EnterErrorUnsupportedEndInterpolatedStringSuffix(ErrorUnsupportedEndInterpolatedStringSuffixContext context)
        {
            Environment.EnableParseTree();
        }

        public sealed override void ExitErrorMissingExpression(ErrorMissingExpressionContext context)
        {
            Error("This construct requires an expression.");
        }

        public sealed override void ExitErrorUnderscoreReserved(ErrorUnderscoreReservedContext context)
        {
            Error("A single underscore is a reserved identifier.");
        }

        public sealed override void ExitErrorUnsupportedNumberSuffix(ErrorUnsupportedNumberSuffixContext context)
        {
            try
            {
                Error($"The literal '{context.GetText()}' has an unsupported suffix.");
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void ExitErrorUnsupportedStringSuffix(ErrorUnsupportedStringSuffixContext context)
        {
            try
            {
                Error($"The literal '{context.GetText()}' has an unsupported suffix.");
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void ExitErrorUnsupportedEndInterpolatedStringSuffix(ErrorUnsupportedEndInterpolatedStringSuffixContext context)
        {
            try
            {
                Error($"The literal '{context.GetText()}' has an unsupported suffix.");
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }
    }
}
