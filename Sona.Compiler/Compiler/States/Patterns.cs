using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class PatternState : NodeState
    {
        public override void EnterPattern(PatternContext context)
        {

        }

        public override void ExitPattern(PatternContext context)
        {
            ExitState().ExitPattern(context);
        }

        public override void EnterNestedPattern(NestedPatternContext context)
        {
            EnterState<NestedPattern>().EnterNestedPattern(context);
        }

        public override void ExitNestedPattern(NestedPatternContext context)
        {

        }

        sealed class NestedPattern : PatternState
        {
            public sealed override void EnterNestedPattern(NestedPatternContext context)
            {

            }

            public sealed override void ExitNestedPattern(NestedPatternContext context)
            {
                ExitState().ExitNestedPattern(context);
            }

            public override void EnterPattern(PatternContext context)
            {
                EnterState<PatternState>().EnterPattern(context);
            }

            public override void ExitPattern(PatternContext context)
            {

            }
        }
    }
}
