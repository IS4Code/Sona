using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class YieldState : NodeState
    {
        public override void EnterYieldStatement(YieldStatementContext context)
        {
            if(FindContext<IComputationContext>() is not ({ IsCollection: true } or { BuilderVariable: not null }))
            {
                Error("`yield` is not allowed outside a collection or computation.", context);
            }
            Out.Write("yield ");
        }

        public override void ExitYieldStatement(YieldStatementContext context)
        {
            ExitState().ExitYieldStatement(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }
    }

    internal sealed class YieldEachState : NodeState
    {
        public override void EnterYieldEachStatement(YieldEachStatementContext context)
        {
            if(FindContext<IComputationContext>() is not ({ IsCollection: true } or { BuilderVariable: not null }))
            {
                Error("`yield` is not allowed outside a collection or computation.", context);
            }
            Out.Write("yield! ");
        }

        public override void ExitYieldEachStatement(YieldEachStatementContext context)
        {
            ExitState().ExitYieldEachStatement(context);
        }

        public override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            EnterState<SpreadExpression>().EnterSpreadExpression(context);
        }

        public override void ExitSpreadExpression(SpreadExpressionContext context)
        {

        }

        sealed class SpreadExpression : ExpressionState
        {
            public override void EnterSpreadExpression(SpreadExpressionContext context)
            {

            }

            public override void ExitSpreadExpression(SpreadExpressionContext context)
            {
                ExitState().ExitSpreadExpression(context);
            }
        }
    }

    internal sealed class YieldBreakState : ReturnState
    {
        public override void EnterYieldBreakStatement(YieldBreakStatementContext context)
        {
            if(FindContext<IComputationContext>() is not ({ IsCollection: true } or { BuilderVariable: not null }))
            {
                Error("`yield break` is not allowed outside a collection or computation.", context);
            }
        }

        public override void ExitYieldBreakStatement(YieldBreakStatementContext context)
        {
            try
            {
                if(HasExpression)
                {
                    Error("`yield break` cannot be used with an expression outside of a computation.", context);
                }
                OnExit(context);
            }
            finally
            {
                ExitState().ExitYieldBreakStatement(context);
            }
        }
    }
}
