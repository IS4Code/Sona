using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class ArgumentStatementState : NodeState
    {
        protected bool HasExpression { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            HasExpression = false;
        }

        public override void EnterExpression(ExpressionContext context)
        {
            OnEnterExpression(context);
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {

        }

        protected virtual void OnEnterExpression(ParserRuleContext context)
        {
            HasExpression = true;
        }

        protected virtual void OnExitExpression(ParserRuleContext context)
        {

        }
    }

    internal class ReturnState : ArgumentStatementState
    {
        IReturnableStatementContext? returnScope;

        IReturnableStatementContext ReturnScope => returnScope ?? Defaults;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            returnScope = FindContext<IReturnableStatementContext>();
        }

        protected sealed override void OnEnterExpression(ParserRuleContext context)
        {
            bool isOption = this is ReturnOptionState;

            ReturnScope.WriteDirectReturnStatement(isOption, context);
            base.OnEnterExpression(context);
        }

        public sealed override void EnterReturnStatement(ReturnStatementContext context)
        {
            OnEnter(context);
        }

        protected virtual void OnEnter(ParserRuleContext context)
        {

        }

        protected virtual void OnExit(ParserRuleContext context)
        {
            if(!HasExpression)
            {
                ReturnScope.WriteEmptyReturnStatement(context);
            }
            else
            {
                ReturnScope.WriteAfterDirectReturnStatement(context);
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

        public sealed override void ExitReturnStatement(ReturnStatementContext context)
        {
            OnExit(context);
            ExitState().ExitReturnStatement(context);
        }
    }

    internal sealed class ReturnOptionState : ReturnState
    {
        public override void EnterReturnOptionStatement(ReturnOptionStatementContext context)
        {
            OnEnter(context);
        }

        public override void ExitReturnOptionStatement(ReturnOptionStatementContext context)
        {
            OnExit(context);
            ExitState().ExitReturnOptionStatement(context);
        }

        public sealed override void EnterAtomicExpr(AtomicExprContext context)
        {
            OnEnterExpression(context);
            EnterState<AtomicExpressionState>().EnterAtomicExpr(context);
        }

        public sealed override void ExitAtomicExpr(AtomicExprContext context)
        {
            OnExitExpression(context);
        }
    }

    internal sealed class ThrowState : ArgumentStatementState
    {
        public override void EnterExpression(ExpressionContext context)
        {
            Out.Write('(');
            base.EnterExpression(context);
        }

        public override void EnterThrowStatement(ThrowStatementContext context)
        {

        }

        public override void ExitThrowStatement(ThrowStatementContext context)
        {
            if(!HasExpression)
            {
                Out.WriteCoreOperatorName("reraise");
                Out.Write("()");
            }
            else
            {
                Out.Write(")");
                Out.WriteSpecialOperator("Throw");
            }

            ExitState().ExitThrowStatement(context);
        }
    }

    internal sealed class BreakState : ArgumentStatementState
    {
        IInterruptibleStatementContext? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = FindContext<IInterruptibleStatementContext>();
            if(scope?.HasFlag(InterruptFlags.CanBreak) != true)
            {
                scope = null;
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            try
            {
                if(scope == null)
                {
                    Error("`break` must be used in a statement that supports it.", context);
                }
                else
                {
                    scope.WriteBreak(true, context);
                }
                Out.Write('(');
            }
            finally
            {
                base.EnterExpression(context);
            }
        }

        public override void EnterBreakStatement(BreakStatementContext context)
        {

        }

        public override void ExitBreakStatement(BreakStatementContext context)
        {
            try
            {
                if(!HasExpression)
                {
                    if(scope == null)
                    {
                        Error("`break` must be used in a statement that supports it.", context);
                    }
                    else
                    {
                        scope.WriteBreak(false, context);
                    }
                }
                else
                {
                    Out.Write(")");
                    if(scope != null)
                    {
                        scope.WriteAfterContinue(context);
                    }
                }
            }
            finally
            {
                ExitState().ExitBreakStatement(context);
            }
        }
    }

    internal sealed class ContinueState : ArgumentStatementState
    {
        IInterruptibleStatementContext? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = FindContext<IInterruptibleStatementContext>();
            if(scope != null && (scope.Flags & InterruptFlags.CanContinue) == 0)
            {
                scope = null;
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            try
            {
                if(scope == null)
                {
                    Error("`continue` must be used in a statement that supports it.", context);
                }
                else
                {
                    scope.WriteContinue(true, context);
                }
                Out.Write('(');
            }
            finally
            {
                base.EnterExpression(context);
            }
        }

        public override void EnterContinueStatement(ContinueStatementContext context)
        {

        }

        public override void ExitContinueStatement(ContinueStatementContext context)
        {
            try
            {
                if(!HasExpression)
                {
                    if(scope == null)
                    {
                        Error("`continue` must be used in a statement that supports it.", context);
                    }
                    else
                    {
                        scope.WriteContinue(false, context);
                    }
                }
                else
                {
                    Out.Write(")");
                    if(scope != null)
                    {
                        scope.WriteAfterContinue(context);
                    }
                }
            }
            finally
            {
                ExitState().ExitContinueStatement(context);
            }
        }
    }
}
