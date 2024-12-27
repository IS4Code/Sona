using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class LogicPartExpression : ExpressionState
    {
        public override void EnterLogicExprArg(LogicExprArgContext context)
        {

        }

        public override void ExitLogicExprArg(LogicExprArgContext context)
        {
            ExitState().ExitLogicExprArg(context);
        }

        public override void EnterOuterExpr(OuterExprContext context)
        {
            Out.Write('(');
            base.EnterOuterExpr(context);
        }

        public override void ExitOuterExpr(OuterExprContext context)
        {
            base.ExitOuterExpr(context);
            Out.Write(')');
        }
    }

    internal sealed class LogicExpression : ExpressionState
    {
        string op = null!;
        bool first;

        protected sealed override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            op = null!;
        }

        public override void EnterDisjunctiveExpr(DisjunctiveExprContext context)
        {
            op = "||";
        }

        public override void EnterConjunctiveExpr(ConjunctiveExprContext context)
        {
            op = "&&";
        }

        public override void ExitDisjunctiveExpr(DisjunctiveExprContext context)
        {
            ExitState().ExitDisjunctiveExpr(context);
        }

        public override void ExitConjunctiveExpr(ConjunctiveExprContext context)
        {
            ExitState().ExitConjunctiveExpr(context);
        }

        void OnOperand()
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.WriteOperator(op);
            }
        }

        public override void EnterLogicExprArg(LogicExprArgContext context)
        {
            OnOperand();
            EnterState<LogicPartExpression>().EnterLogicExprArg(context);
        }

        public override void EnterNegatedExpr(NegatedExprContext context)
        {
            OnOperand();
            base.EnterNegatedExpr(context);
        }
    }

    internal sealed class LogicFormExpression : NodeState
    {
        string op = null!;
        bool first;

        protected sealed override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            op = null!;
        }

        public override void EnterDnfExpr(DnfExprContext context)
        {
            op = "||";
        }

        public override void EnterCnfExpr(CnfExprContext context)
        {
            op = "&&";
        }

        public override void ExitDnfExpr(DnfExprContext context)
        {
            ExitState().ExitDnfExpr(context);
        }

        public override void ExitCnfExpr(CnfExprContext context)
        {
            ExitState().ExitCnfExpr(context);
        }

        void OnOperand()
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.WriteOperator(op);
            }
        }

        public override void EnterDisjunctiveExpr(DisjunctiveExprContext context)
        {
            OnOperand();
            Out.Write('(');
            EnterState<LogicExpression>().EnterDisjunctiveExpr(context);
        }

        public override void EnterConjunctiveExpr(ConjunctiveExprContext context)
        {
            OnOperand();
            Out.Write('(');
            EnterState<LogicExpression>().EnterConjunctiveExpr(context);
        }

        public override void EnterLogicExprArg(LogicExprArgContext context)
        {
            OnOperand();
            EnterState<LogicPartExpression>().EnterLogicExprArg(context);
        }

        public override void ExitDisjunctiveExpr(DisjunctiveExprContext context)
        {
            Out.Write(')');
        }

        public override void ExitConjunctiveExpr(ConjunctiveExprContext context)
        {
            Out.Write(')');
        }

        public override void ExitLogicExprArg(LogicExprArgContext context)
        {

        }
    }

    internal sealed class NegatedExpression : ExpressionState
    {
        int level;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);
            level = 0;
        }

        void OnOperand()
        {
            Out.WriteCoreOperator("not");
            Out.Write('(');
        }

        public override void EnterNegatedExpr(NegatedExprContext context)
        {
            OnOperand();
        }

        public override void EnterDoubleNegation(DoubleNegationContext context)
        {
            OnOperand();
            level++;
        }

        public override void ExitDoubleNegation(DoubleNegationContext context)
        {

        }

        public override void ExitNegatedExpr(NegatedExprContext context)
        {
            Out.Write(")");
            while(level > 0)
            {
                Out.Write(")");
                level--;
            }
            ExitState().ExitNegatedExpr(context);
        }

        public override void EnterLogicExprArg(LogicExprArgContext context)
        {

        }

        public override void ExitLogicExprArg(LogicExprArgContext context)
        {

        }
    }
}
