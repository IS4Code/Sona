using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class YieldState : NodeState
    {
        public override void EnterYieldStatement(YieldStatementContext context)
        {
            if(FindContext<IComputationContext>()?.HasAnyFlag(ComputationFlags.IsCollection | ComputationFlags.IsComputation) != true)
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
            if(FindContext<IComputationContext>()?.HasAnyFlag(ComputationFlags.IsCollection | ComputationFlags.IsComputation) != true)
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

    internal sealed class YieldBreakState : ArgumentStatementState
    {
        IComputationContext? computationScope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            computationScope = FindContext<IComputationContext>();
        }

        public override void EnterYieldBreakStatement(YieldBreakStatementContext context)
        {
            if(computationScope?.HasAnyFlag(ComputationFlags.IsCollection | ComputationFlags.IsComputation) != true)
            {
                Error("`yield break` is not allowed outside a collection or computation.", context);
            }
        }

        public override void ExitYieldBreakStatement(YieldBreakStatementContext context)
        {
            OnExit(context);

            ExitState().ExitYieldBreakStatement(context);
        }

        protected sealed override void OnEnterExpression(ParserRuleContext context)
        {
            if(computationScope?.HasFlag(ComputationFlags.IsComputation) != true)
            {
                // Not a computation - there is no use to this
                Error("`yield break` with an argument cannot be used outside of a computation.", context);
            }
            else
            {
                var returnScope = FindContext<IReturnableStatementContext>();
                if(returnScope?.HasFlag(ReturnFlags.Indirect) ?? false)
                {
                    // There is no mechanism to indicate whether computation ends in `return` or `yield break`.
                    Error("`yield break` with an argument is not supported in other positions than the final returning statement of a function.", context);
                }
                if(returnScope?.HasFlag(ReturnFlags.Optional) ?? false)
                {
                    Error("`yield break` with an argument cannot be used in an optional function.", context);
                }
            }
            // Bypass normal returning and just produce the value
            Defaults.WriteDirectReturnStatement(false, context);
            base.OnEnterExpression(context);
        }

        private void OnExit(ParserRuleContext context)
        {
            if(!HasExpression)
            {
                Defaults.WriteEmptyReturnStatement(context);
            }
            else
            {
                Defaults.WriteAfterDirectReturnStatement(context);
            }

            var interruptScope = FindContext<IInterruptibleStatementContext>();
            if(interruptScope?.HasFlag(InterruptFlags.CanBreak) != true)
            {
                // No need for break
                interruptScope = null;
            }

            if(interruptScope != null)
            {
                Out.WriteLine();
                interruptScope.WriteBreak(false, context);
            }
        }
    }

    internal sealed class YieldReturnState : NodeState
    {
        public override void EnterYieldReturnStatement(YieldReturnStatementContext context)
        {
            if(FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) != true)
            {
                Error("`yield return` is not allowed outside a computation.", context);
            }
            if(FindContext<IReturnableStatementContext>()?.HasFlag(ReturnFlags.Optional) ?? false)
            {
                Error("`yield return` cannot be used in an optional function.", context);
            }
            Out.Write("return ");
        }

        public override void ExitYieldReturnStatement(YieldReturnStatementContext context)
        {
            ExitState().ExitYieldReturnStatement(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
           EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }
    }
}
