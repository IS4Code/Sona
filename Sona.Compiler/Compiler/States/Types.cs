using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class TypeState : NodeState
    {
        public override void EnterType(TypeContext context)
        {

        }

        public override void ExitType(TypeContext context)
        {
            ExitState().ExitType(context);
        }

        public override void EnterUnit(UnitContext context)
        {
            Out.WriteNamespacedName("Microsoft.FSharp.Core", "unit");
        }

        public override void ExitUnit(UnitContext context)
        {

        }
    }
}
