using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class WithStatementBase : ControlStatement
    {
        public string? BuilderVariable { get; private set; }

        protected override bool IgnoreContext => false;

        protected override bool IgnoreTrailContext => true;

        protected new IReturnableStatementContext ReturnScope => base.ReturnScope ?? Defaults;
        protected new IInterruptibleStatementContext InterruptScope => base.InterruptScope ?? Defaults;

        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            base.OnEnter(flags, context);

            BuilderVariable = Out.CreateTemporaryIdentifier();

            Out.EnterNestedScope();
            Out.Write("(let ");
            Out.WriteIdentifier(BuilderVariable);
            Out.WriteOperator('=');
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            Out.ExitScope();
            Out.WriteLine();
            Out.Write(_end_);
            Out.ExitNestedScope();
            Out.Write("})");

            base.OnExit(flags, context);
        }

        protected sealed override void OnEnterTrail(StatementFlags flags, ParserRuleContext context)
        {
            
        }

        protected sealed override void OnExitTrail(StatementFlags flags, ParserRuleContext context)
        {
            
        }

        protected sealed override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {

        }

        protected sealed override void OnExitBlock(StatementFlags flags, ParserRuleContext context)
        {

        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            Out.Write(" in ");
            Out.WriteIdentifier(BuilderVariable!);
            Out.Write(" { ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
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

        public new virtual void WriteImplicitReturnStatement(ParserRuleContext context)
        {
            Out.Write("return ");
            ReturnScope.WriteEmptyReturnValue(context);
        }
    }

    internal sealed class WithStatementState : WithStatementBase, IComputationContext
    {
        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;
        string? IInterruptibleStatementContext.InterruptingVariable => null;

        ReturnFlags IReturnableStatementContext.Flags => ReturnFlags;

        bool IComputationContext.IsCollection => false;
        
        public override void EnterWithStatement(WithStatementContext context)
        {
            ReturnScope.WriteReturnStatement(context);

            OnEnter(StatementFlags.None, context);
        }

        public override void ExitWithStatement(WithStatementContext context)
        {
            OnExit(StatementFlags.None, context);

            ReturnScope.WriteAfterReturnStatement(context);

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
            ReturnScope.WriteReturnValue(isOption, context);
        }

        void IReturnableStatementContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            ReturnScope.WriteAfterReturnValue(context);
        }

        void IReturnableStatementContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            ReturnScope.WriteEmptyReturnValue(context);
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
    }

    internal sealed class FollowWithStatementState : WithStatementBase, IComputationContext
    {
        InterruptFlags IInterruptibleStatementContext.Flags => InterruptScope?.Flags ?? InterruptFlags.None;
        string? IInterruptibleStatementContext.InterruptingVariable => InterruptScope?.InterruptingVariable;

        ReturnFlags IReturnableStatementContext.Flags => ReturnFlags;

        bool IComputationContext.IsCollection => false;

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

            OnEnter(StatementFlags.None, context);
        }

        public override void ExitFollowWithTrailing(FollowWithTrailingContext context)
        {
            OnExit(StatementFlags.None, context);
            ExitState().ExitFollowWithTrailing(context);
        }

        public override void EnterFollowWithTerminating(FollowWithTerminatingContext context)
        {
            CheckScope(context);

            Out.Write("return! ");

            OnEnter(StatementFlags.Terminating, context);
        }

        public override void ExitFollowWithTerminating(FollowWithTerminatingContext context)
        {
            OnExit(StatementFlags.Terminating, context);
            ExitState().ExitFollowWithTerminating(context);
        }

        public override void EnterFollowWithInterrupting(FollowWithInterruptingContext context)
        {
            CheckScope(context);

            Out.Write("do! ");

            OnEnter(StatementFlags.InterruptPath, context);
        }

        public override void ExitFollowWithInterrupting(FollowWithInterruptingContext context)
        {
            OnExit(StatementFlags.InterruptPath, context);
            ExitState().ExitFollowWithInterrupting(context);
        }

        public override void EnterFollowWithReturning(FollowWithReturningContext context)
        {
            CheckScope(context);

            Out.Write("return! ");

            OnEnter(StatementFlags.InterruptPath | StatementFlags.ReturnPath, context);
        }

        public override void ExitFollowWithReturning(FollowWithReturningContext context)
        {
            OnExit(StatementFlags.InterruptPath | StatementFlags.ReturnPath, context);
            ExitState().ExitFollowWithReturning(context);
        }

        public override void EnterFollowWithInterruptible(FollowWithInterruptibleContext context)
        {
            CheckScope(context);

            Out.Write("do! ");

            OnEnter(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public override void ExitFollowWithInterruptible(FollowWithInterruptibleContext context)
        {
            OnExit(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            ExitState().ExitFollowWithInterruptible(context);
        }

        public override void EnterFollowWithConditional(FollowWithConditionalContext context)
        {
            CheckScope(context);

            Out.Write("do! ");

            OnEnter(StatementFlags.InterruptPath | StatementFlags.ReturnPath | StatementFlags.OpenPath, context);
        }

        public override void ExitFollowWithConditional(FollowWithConditionalContext context)
        {
            OnExit(StatementFlags.InterruptPath | StatementFlags.ReturnPath | StatementFlags.OpenPath, context);
            ExitState().ExitFollowWithConditional(context);
        }

        void IReturnableStatementContext.WriteReturnStatement(ParserRuleContext context)
        {
            Out.Write("return ");
        }

        void IReturnableStatementContext.WriteAfterReturnStatement(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            InterruptScope.WriteBreak(hasExpression, context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            InterruptScope.WriteContinue(hasExpression, context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {
            InterruptScope.WriteAfterBreak(context);
        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {
            InterruptScope.WriteAfterContinue(context);
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
