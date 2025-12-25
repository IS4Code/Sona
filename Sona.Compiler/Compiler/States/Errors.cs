using static Sona.Grammar.SonaParser;

namespace Sona.Compiler
{
    partial class ScriptState
    {
        public sealed override void EnterErrorMissingExpression(ErrorMissingExpressionContext context)
        {

        }

        public sealed override void ExitErrorMissingExpression(ErrorMissingExpressionContext context)
        {
            Error("This construct requires an expression.", context);
        }

        public override void EnterErrorUnsupportedFollow(ErrorUnsupportedFollowContext context)
        {

        }

        public override void ExitErrorUnsupportedFollow(ErrorUnsupportedFollowContext context)
        {
            Error("The `follow` keyword is usable only in specific contexts.", context);
        }

        public sealed override void EnterErrorUnderscoreReserved(ErrorUnderscoreReservedContext context)
        {

        }

        public sealed override void ExitErrorUnderscoreReserved(ErrorUnderscoreReservedContext context)
        {
            Error("A single underscore is a reserved identifier.", context);
        }

        public sealed override void EnterErrorUnsupportedNumberSuffix(ErrorUnsupportedNumberSuffixContext context)
        {
            StartCaptureInput(context);
        }

        public sealed override void ExitErrorUnsupportedNumberSuffix(ErrorUnsupportedNumberSuffixContext context)
        {
            Error($"The literal '{StopCaptureInput(context)}' has an unsupported suffix.", context);
        }

        public sealed override void EnterErrorUnsupportedEndCharSuffix(ErrorUnsupportedEndCharSuffixContext context)
        {
            StartCaptureInput(context);
        }

        public sealed override void ExitErrorUnsupportedEndCharSuffix(ErrorUnsupportedEndCharSuffixContext context)
        {
            Error($"Unsupported character suffix '{StopCaptureInput(context)}'.", context);
        }

        public sealed override void EnterErrorUnsupportedEndStringSuffix(ErrorUnsupportedEndStringSuffixContext context)
        {
            StartCaptureInput(context);
        }

        public sealed override void ExitErrorUnsupportedEndStringSuffix(ErrorUnsupportedEndStringSuffixContext context)
        {
            Error($"Unsupported string suffix '{StopCaptureInput(context)}'.", context);
        }
    }
}
