﻿using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class AttributeState : NodeState, IExpressionContext
    {
        bool firstGroup;
        bool firstArgument;

        ExpressionType IExpressionContext.Type => ExpressionType.Literal;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            firstGroup = true;
        }

        public override void EnterLocalAttribute(LocalAttributeContext context)
        {
            Out.Write("[<");
        }

        public override void ExitLocalAttribute(LocalAttributeContext context)
        {
            Out.Write(">]");
            ExitState().ExitLocalAttribute(context);
        }

        public override void EnterGlobalAttribute(GlobalAttributeContext context)
        {
            Out.Write("[<");
        }

        public override void ExitGlobalAttribute(GlobalAttributeContext context)
        {
            Out.WriteLine(">]do()");
            ExitState().ExitGlobalAttribute(context);
        }

        public override void EnterLocalAttrTarget(LocalAttrTargetContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitLocalAttrTarget(LocalAttrTargetContext context)
        {
            try
            {
                var target = Tools.Syntax.GetAttributeTargetFromToken(context.GetText());
                switch(target)
                {
                    case "item":
                        return;
                }
                Out.WriteIdentifier(target);
                Out.Write(':');
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterGlobalAttrTarget(GlobalAttrTargetContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitGlobalAttrTarget(GlobalAttrTargetContext context)
        {
            try
            {
                var target = Tools.Syntax.GetAttributeTargetFromToken(context.GetText());
                switch(target)
                {
                    case "entry":
                        target = "method";
                        break;
                    case "program":
                        return;
                }
                Out.WriteIdentifier(target);
                Out.Write(':');
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterAttrGroup(AttrGroupContext context)
        {
            if(firstGroup)
            {
                firstGroup = false;
            }
            else
            {
                Out.Write(';');
            }
            firstArgument = true;
        }

        public override void ExitAttrGroup(AttrGroupContext context)
        {
            Out.Write(')');
        }

        public override void ExitCompoundName(CompoundNameContext context)
        {
            base.ExitCompoundName(context);
            Out.Write('(');
        }

        public override void EnterCompiledNameAttr(CompiledNameAttrContext context)
        {
            Out.WriteCoreName("CompiledNameAttribute");
            Out.Write('(');
        }

        public override void ExitCompiledNameAttr(CompiledNameAttrContext context)
        {
            firstArgument = false;
        }

        private void EnterAttrArgument()
        {
            if(firstArgument)
            {
                firstArgument = false;
            }
            else
            {
                Out.Write(',');
            }
        }

        public override void EnterAttrPosArg(AttrPosArgContext context)
        {
            EnterAttrArgument();
            EnterState<Argument>().EnterAttrPosArg(context);
        }

        public override void EnterAttrNamedArg(AttrNamedArgContext context)
        {
            EnterAttrArgument();
            EnterState<Argument>().EnterAttrNamedArg(context);
        }

        public override void ExitAttrPosArg(AttrPosArgContext context)
        {

        }

        public override void ExitAttrNamedArg(AttrNamedArgContext context)
        {

        }

        sealed class Argument : ExpressionState
        {
            bool isNamed;

            public override void EnterAttrNamedArg(AttrNamedArgContext context)
            {
                isNamed = true;
            }

            public override void ExitAttrNamedArg(AttrNamedArgContext context)
            {
                ExitState().ExitAttrNamedArg(context);
            }

            public override void EnterAttrPosArg(AttrPosArgContext context)
            {
                isNamed = false;
            }

            public override void ExitAttrPosArg(AttrPosArgContext context)
            {
                ExitState().ExitAttrPosArg(context);
            }

            public override void EnterExpression(ExpressionContext context)
            {
                EnterState<ExpressionState>().EnterExpression(context);
            }

            public override void ExitExpression(ExpressionContext context)
            {

            }

            public override void EnterUnaryExpr(UnaryExprContext context)
            {
                if(isNamed)
                {
                    Out.WriteOperator('=');
                    isNamed = false;
                }
            }

            public override void ExitUnaryExpr(UnaryExprContext context)
            {

            }
        }
    }
}
