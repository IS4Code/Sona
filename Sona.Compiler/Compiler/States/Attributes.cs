using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class AttributeState : NodeState, IExpressionContext
    {
        bool firstGroup;
        bool firstArgument;
        bool isNamed;

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

        static readonly char[] targetTrimChars = { '#', ':', ' ', '\t', '\xc' };
        public override void ExitLocalAttrTarget(LocalAttrTargetContext context)
        {
            try
            {
                var target = context.GetText().Trim(targetTrimChars);
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
                var target = context.GetText().Trim(targetTrimChars);
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
            Out.Write('(');
        }

        public void EnterAttrArgument()
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
            isNamed = false;
        }

        public override void EnterAttrNamedArg(AttrNamedArgContext context)
        {
            EnterAttrArgument();
            isNamed = true;
        }

        public override void EnterUnaryExpr(UnaryExprContext context)
        {
            if(isNamed)
            {
                Out.WriteOperator('=');
            }
            EnterState<Expression>().EnterUnaryExpr(context);
        }

        public override void ExitUnaryExpr(UnaryExprContext context)
        {

        }

        sealed class Expression : ExpressionState
        {
            public override void ExitExpression(ExpressionContext context)
            {

            }

            public override void ExitUnaryExpr(UnaryExprContext context)
            {
                ExitState().ExitUnaryExpr(context);
            }
        }
    }
}
