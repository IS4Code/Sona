using Antlr4.Runtime;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class WithStatementBase : NodeState, IComputationContext
    {
        bool IStatementContext.TrailAllowed => true;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;
        string? IInterruptibleStatementContext.InterruptingVariable => null;

        ReturnFlags IReturnableStatementContext.Flags => ReturnFlags.None;

        bool IComputationContext.IsCollection => false;
        public string? BuilderVariable { get; private set; }

        protected IReturnableStatementContext? ReturnScope { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            ReturnScope = FindContext<IReturnableStatementContext>();
        }

        protected void OnEnter(ParserRuleContext context)
        {
            BuilderVariable = Out.CreateTemporaryIdentifier();

            Out.EnterNestedScope();
            Out.Write("(let ");
            Out.WriteIdentifier(BuilderVariable);
            Out.WriteOperator('=');
        }

        protected void OnExit(ParserRuleContext context)
        {
            Out.ExitScope();
            Out.WriteLine();
            Out.Write(_end_);
            Out.ExitNestedScope();
            Out.Write("})");
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            Out.Write(" in ");
            Out.WriteIdentifier(BuilderVariable!);
            Out.Write(" { ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public override void EnterValueTrail(ValueTrailContext context)
        {
            EnterState<Trail>().EnterValueTrail(context);
        }

        public override void ExitValueTrail(ValueTrailContext context)
        {

        }

        public override void EnterFreeTrail(FreeTrailContext context)
        {
            EnterState<Trail>().EnterFreeTrail(context);
        }

        public override void ExitFreeTrail(FreeTrailContext context)
        {

        }

        public override void EnterReturningTrail(ReturningTrailContext context)
        {
            EnterState<Trail>().EnterReturningTrail(context);
        }

        public override void ExitReturningTrail(ReturningTrailContext context)
        {

        }

        public void WriteBeginBlockExpression(ParserRuleContext context)
        {
            Out.EnterNestedScope();
            Out.Write('(');
            Out.WriteIdentifier(BuilderVariable ?? Error("COMPILER ERROR: Computation block is not properly initialized.", context));
            Out.WriteLine('{');
        }

        public void WriteEndBlockExpression(ParserRuleContext context)
        {
            Out.ExitNestedScope();
            Out.Write("})");
        }

        void IReturnableStatementContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableStatementContext.WriteReturnStatement(ParserRuleContext context)
        {
            Out.Write("return ");
        }

        void IReturnableStatementContext.WriteAfterReturnStatement(ParserRuleContext context)
        {

        }

        void IReturnableStatementContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            if(ReturnScope != null)
            {
                ReturnScope.WriteReturnValue(isOption, context);
            }
            else
            {
                Defaults.WriteReturnValue(isOption, context);
            }
        }

        void IReturnableStatementContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            if(ReturnScope != null)
            {
                ReturnScope.WriteAfterReturnValue(context);
            }
            else
            {
                Defaults.WriteAfterReturnValue(context);
            }
        }

        void IReturnableStatementContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            if(ReturnScope != null)
            {
                ReturnScope.WriteEmptyReturnValue(context);
            }
            else
            {
                Defaults.WriteEmptyReturnValue(context);
            }
        }

        public virtual void WriteImplicitReturnStatement(ParserRuleContext context)
        {
            Out.Write("return ");
            if(ReturnScope != null)
            {
                ReturnScope.WriteEmptyReturnValue(context);
            }
            else
            {
                Defaults.WriteEmptyReturnValue(context);
            }
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteBreak(hasExpression, context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteContinue(hasExpression, context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {
            Defaults.WriteAfterBreak(context);
        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {
            Defaults.WriteAfterContinue(context);
        }

        sealed class Trail : BlockState
        {
            protected override bool IgnoreContext => true;

            public override void EnterValueTrail(ValueTrailContext context)
            {

            }

            public override void ExitValueTrail(ValueTrailContext context)
            {
                ExitState()!.ExitValueTrail(context);
            }

            public override void EnterFreeTrail(FreeTrailContext context)
            {

            }

            public override void ExitFreeTrail(FreeTrailContext context)
            {
                ExitState()!.ExitFreeTrail(context);
            }

            public override void EnterReturningTrail(ReturningTrailContext context)
            {

            }

            public override void ExitReturningTrail(ReturningTrailContext context)
            {
                ExitState()!.ExitReturningTrail(context);
            }
        }
    }

    internal sealed class WithStatementState : WithStatementBase
    {
        public override void EnterWithStatement(WithStatementContext context)
        {
            if(ReturnScope != null)
            {
                ReturnScope.WriteReturnStatement(context);
            }
            else
            {
                Defaults.WriteReturnStatement(context);
            }

            OnEnter(context);
        }

        public override void ExitWithStatement(WithStatementContext context)
        {
            OnExit(context);

            if(ReturnScope != null)
            {
                ReturnScope.WriteAfterReturnStatement(context);
            }
            else
            {
                Defaults.WriteAfterReturnStatement(context);
            }

            ExitState().ExitWithStatement(context);
        }

        public override void WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if(FindContext<IBlockStatementContext>() != FindContext<IComputationContext>())
            {
                // Not at the end of function/computation
                Error("It is not possible to escape from a computation block directly to the outside code. Use `return` to return explicitly.", context);
            }
            base.WriteImplicitReturnStatement(context);
        }
    }

    internal sealed class FollowWithStatementState : WithStatementBase
    {
        private void CheckScope(ParserRuleContext context)
        {
            if(FindContext<IComputationContext>() is not { BuilderVariable: not null })
            {
                Error("`follow` is not allowed outside a computation.", context);
            }
        }

        public override void EnterFollowWithTrailing(FollowWithTrailingContext context)
        {
            CheckScope(context);

            Out.Write("do! ");

            OnEnter(context);
        }

        public override void ExitFollowWithTrailing(FollowWithTrailingContext context)
        {
            OnExit(context);
            ExitState().ExitFollowWithTrailing(context);
        }

        public override void EnterFollowWithReturning(FollowWithReturningContext context)
        {
            CheckScope(context);

            Out.Write("return! ");

            OnEnter(context);
        }

        public override void ExitFollowWithReturning(FollowWithReturningContext context)
        {
            OnExit(context);
            ExitState().ExitFollowWithReturning(context);
        }
    }

    internal sealed class FollowState : NodeState
    {
        public override void EnterFollowStatement(FollowStatementContext context)
        {
            if(FindContext<IComputationContext>() is not { BuilderVariable: not null })
            {
                Error("`follow` is not allowed outside a computation.", context);
            }
            Out.Write("do! ");
        }

        public override void ExitFollowStatement(FollowStatementContext context)
        {
            ExitState().ExitFollowStatement(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }
    }

    internal sealed class FollowDiscardState : ExpressionState
    {
        public override void EnterFollowDiscardStatement(FollowDiscardStatementContext context)
        {
            if(FindContext<IComputationContext>() is not { BuilderVariable: not null })
            {
                Error("`follow` is not allowed outside a computation.", context);
            }
            Out.Write("let! _");
            Out.WriteOperator('=');
        }

        public override void ExitFollowDiscardStatement(FollowDiscardStatementContext context)
        {
            ExitState().ExitFollowDiscardStatement(context);
        }
    }
}
