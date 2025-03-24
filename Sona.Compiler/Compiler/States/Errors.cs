using Antlr4.Runtime.Misc;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler
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

        public sealed override void EnterErrorUnsupportedEndStringSuffix(ErrorUnsupportedEndStringSuffixContext context)
        {
            Environment.EnableParseTree();
        }

        public sealed override void ExitErrorMissingExpression(ErrorMissingExpressionContext context)
        {
            Error("This construct requires an expression.", context);
        }

        public sealed override void ExitErrorUnderscoreReserved(ErrorUnderscoreReservedContext context)
        {
            Error("A single underscore is a reserved identifier.", context);
        }

        public sealed override void ExitErrorUnsupportedNumberSuffix(ErrorUnsupportedNumberSuffixContext context)
        {
            try
            {
                Error($"The literal '{context.GetText()}' has an unsupported suffix.", context);
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
                Error($"The literal '{context.GetText()}' has an unsupported suffix.", context);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public sealed override void ExitErrorUnsupportedEndStringSuffix(ErrorUnsupportedEndStringSuffixContext context)
        {
            try
            {
                Error($"Unsupported string suffix '{context.GetText()}'.", context);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }
    }
}
