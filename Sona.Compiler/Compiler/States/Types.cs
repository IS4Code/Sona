﻿using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
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
            Out.WriteCoreName("unit");
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
                        Out.WriteSystemName("Void");
                        return;
                    case "exception":
                        Out.WriteSystemName("Exception");
                        return;
                }
                Out.WriteCoreName(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }
    }
}
