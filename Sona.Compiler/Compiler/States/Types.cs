using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal class TypeState : NodeState
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

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {
            try
            {
                var text = context.GetText();
                switch(text)
                {
                    case "object":
                        text = "obj";
                        break;
                    case "void":
                        Out.WriteNamespacedName("System", "Void");
                        return;
                    case "exception":
                        Out.WriteNamespacedName("System", "Exception");
                        return;
                }
                Out.WriteNamespacedName("Microsoft.FSharp.Core", text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }
    }
}
