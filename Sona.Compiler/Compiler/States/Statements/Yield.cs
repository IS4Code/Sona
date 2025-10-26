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
            EnterState<ExpressionState.Spread>().EnterSpreadExpression(context);
        }

        public override void ExitSpreadExpression(SpreadExpressionContext context)
        {

        }
    }

    internal sealed class YieldBreakState : NodeState
    {
        IReturnableContext? returnScope;
        IComputationContext? computationScope;

        IReturnableContext ReturnScope => returnScope ?? Defaults;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            returnScope = FindContext<IReturnableContext>();
            computationScope = FindContext<IComputationContext>();
        }

        public override void EnterYieldBreakStatement(YieldBreakStatementContext context)
        {
            if(computationScope?.HasFlag(ComputationFlags.IsCollection) ?? false)
            {
                // Always supported
            }
            else if(computationScope?.HasFlag(ComputationFlags.IsComputation) ?? false)
            {
                if(returnScope?.HasFlag(ReturnFlags.Indirect) ?? false)
                {
                    // There is no mechanism to indicate whether computation ends in `return` or `yield break`.
                    Error("`yield break` in a computation block is not supported in other positions than the final returning statement.", context);
                }
            }
            else
            {
                Error("`yield break` is not allowed outside a collection or computation.", context);
            }
        }

        public override void ExitYieldBreakStatement(YieldBreakStatementContext context)
        {
            if(computationScope?.HasFlag(ComputationFlags.IsCollection) ?? false)
            {
                // Safe to rely on collection not using `return`.
                ReturnScope.WriteReturnStatement(context);
                Defaults.WriteEmptyReturnValue(context);
                ReturnScope.WriteAfterReturnStatement(context);
            }
            else
            {
                // Bypass normal returning
                Defaults.WriteEmptyReturnStatement(context);
            }

            var interruptScope = FindContext<IInterruptibleContext>();
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

            ExitState().ExitYieldBreakStatement(context);
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
            if(FindContext<IReturnableContext>()?.HasFlag(ReturnFlags.Optional) ?? false)
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
