using System;
using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    [Flags]
    internal enum StatementFlags
    {
        None = 0,
        Terminating = 1,
        OpenPath = 2,
        ReturnPath = 4,
        InterruptPath = 8
    }

    internal class BlockState : ScriptState, IStatementContext
    {
        bool IStatementContext.TrailAllowed => true;

        ISourceWriter IScopeContext.GlobalWriter => GlobalOut;
        ISourceWriter IScopeContext.LocalWriter => Out;

        public sealed override void EnterMultiFuncDecl(MultiFuncDeclContext context)
        {
            EnterState<MultiFunctionState>().EnterMultiFuncDecl(context);
        }

        public sealed override void ExitMultiFuncDecl(MultiFuncDeclContext context)
        {

        }

        public sealed override void EnterInlineFuncDecl(InlineFuncDeclContext context)
        {
            Out.Write("let inline ");
            EnterState<FunctionDeclState>().EnterInlineFuncDecl(context);
        }

        public sealed override void ExitInlineFuncDecl(InlineFuncDeclContext context)
        {

        }

        public sealed override void EnterInlineCaseFuncDecl(InlineCaseFuncDeclContext context)
        {
            Out.Write("let inline ");
            EnterState<CaseFunctionDeclState>().EnterInlineCaseFuncDecl(context);
        }

        public sealed override void ExitInlineCaseFuncDecl(InlineCaseFuncDeclContext context)
        {

        }

        protected virtual void OnEnterStatement(StatementFlags flags, ParserRuleContext context)
        {

        }

        protected virtual void OnExitStatement(StatementFlags flags, ParserRuleContext context)
        {
            Out.WriteLine();
        }

        public override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            (FindContext<IBlockContext>() ?? Defaults).WriteImplicitReturnStatement(context);
        }

        public override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
        {

        }

        public sealed override void EnterReturnStatement(ReturnStatementContext context)
        {
            EnterState<ReturnState>().EnterReturnStatement(context);
        }

        public sealed override void EnterReturnOptionStatement(ReturnOptionStatementContext context)
        {
            EnterState<ReturnOptionState>().EnterReturnOptionStatement(context);
        }

        public sealed override void EnterReturnFollowStatement(ReturnFollowStatementContext context)
        {
            EnterState<ReturnFollowState>().EnterReturnFollowStatement(context);
        }

        public sealed override void EnterThrowStatement(ThrowStatementContext context)
        {
            EnterState<ThrowState>().EnterThrowStatement(context);
        }

        public sealed override void EnterBreakStatement(BreakStatementContext context)
        {
            EnterState<BreakState>().EnterBreakStatement(context);
        }

        public sealed override void EnterContinueStatement(ContinueStatementContext context)
        {
            EnterState<ContinueState>().EnterContinueStatement(context);
        }

        public sealed override void EnterEchoStatement(EchoStatementContext context)
        {
            EnterState<EchoState>().EnterEchoStatement(context);
        }

        public sealed override void EnterYieldStatement(YieldStatementContext context)
        {
            EnterState<YieldState>().EnterYieldStatement(context);
        }

        public sealed override void EnterYieldFollowStatement(YieldFollowStatementContext context)
        {
            EnterState<YieldFollowState>().EnterYieldFollowStatement(context);
        }

        public sealed override void EnterYieldEachStatement(YieldEachStatementContext context)
        {
            EnterState<YieldEachState>().EnterYieldEachStatement(context);
        }

        public sealed override void EnterYieldBreakStatement(YieldBreakStatementContext context)
        {
            EnterState<YieldBreakState>().EnterYieldBreakStatement(context);
        }

        public sealed override void EnterYieldReturnStatement(YieldReturnStatementContext context)
        {
            EnterState<YieldReturnState>().EnterYieldReturnStatement(context);
        }

        public sealed override void EnterYieldReturnFollowStatement(YieldReturnFollowStatementContext context)
        {
            EnterState<YieldReturnFollowState>().EnterYieldReturnFollowStatement(context);
        }

        public sealed override void EnterWithStatement(WithStatementContext context)
        {
            EnterState<WithStatementState>().EnterWithStatement(context);
        }

        public sealed override void EnterFollowWithTrailing(FollowWithTrailingContext context)
        {
            EnterState<FollowWithStatementState>().EnterFollowWithTrailing(context);
        }

        public sealed override void EnterFollowWithTerminating(FollowWithTerminatingContext context)
        {
            EnterState<FollowWithStatementState>().EnterFollowWithTerminating(context);
        }

        public sealed override void EnterFollowWithInterrupting(FollowWithInterruptingContext context)
        {
            EnterState<FollowWithStatementState>().EnterFollowWithInterrupting(context);
        }

        public sealed override void EnterFollowWithReturning(FollowWithReturningContext context)
        {
            EnterState<FollowWithStatementState>().EnterFollowWithReturning(context);
        }

        public sealed override void EnterFollowWithInterruptible(FollowWithInterruptibleContext context)
        {
            EnterState<FollowWithStatementState>().EnterFollowWithInterruptible(context);
        }

        public sealed override void EnterFollowWithConditional(FollowWithConditionalContext context)
        {
            EnterState<FollowWithStatementState>().EnterFollowWithConditional(context);
        }

        public sealed override void EnterYieldWithTrailing(YieldWithTrailingContext context)
        {
            EnterState<YieldWithStatementState>().EnterYieldWithTrailing(context);
        }

        public sealed override void EnterYieldWithTerminating(YieldWithTerminatingContext context)
        {
            EnterState<YieldWithStatementState>().EnterYieldWithTerminating(context);
        }

        public sealed override void EnterYieldWithInterrupting(YieldWithInterruptingContext context)
        {
            EnterState<YieldWithStatementState>().EnterYieldWithInterrupting(context);
        }

        public sealed override void EnterYieldWithReturning(YieldWithReturningContext context)
        {
            EnterState<YieldWithStatementState>().EnterYieldWithReturning(context);
        }

        public sealed override void EnterYieldWithInterruptible(YieldWithInterruptibleContext context)
        {
            EnterState<YieldWithStatementState>().EnterYieldWithInterruptible(context);
        }

        public sealed override void EnterYieldWithConditional(YieldWithConditionalContext context)
        {
            EnterState<YieldWithStatementState>().EnterYieldWithConditional(context);
        }

        public sealed override void EnterFollowDiscardStatement(FollowDiscardStatementContext context)
        {
            EnterState<FollowDiscardState>().EnterFollowDiscardStatement(context);
        }

        public sealed override void EnterFollowStatement(FollowStatementContext context)
        {
            EnterState<FollowState>().EnterFollowStatement(context);
        }

        public sealed override void EnterVariableDecl(VariableDeclContext context)
        {
            EnterState<NewVariableState>().EnterVariableDecl(context);
        }

        public override void EnterLazyVariableDecl(LazyVariableDeclContext context)
        {
            EnterState<LazyDeclarationState>().EnterLazyVariableDecl(context);
        }

        public sealed override void EnterFollowVariableDecl(FollowVariableDeclContext context)
        {
            EnterState<NewFollowVariableState>().EnterFollowVariableDecl(context);
        }

        public sealed override void EnterMemberOrAssignment(MemberOrAssignmentContext context)
        {
            EnterState<MemberOrAssignmentState>().EnterMemberOrAssignment(context);
        }

        public sealed override void EnterMemberDiscard(MemberDiscardContext context)
        {
            EnterState<MemberDiscardState>().EnterMemberDiscard(context);
        }

        public sealed override void EnterIfStatementFree(IfStatementFreeContext context)
        {
            EnterState<IfStatementNoTrail>().EnterIfStatementFree(context);
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            EnterState<IfStatementNoTrail>().EnterIfStatementTerminating(context);
        }

        public sealed override void EnterIfStatementInterrupting(IfStatementInterruptingContext context)
        {
            EnterState<IfStatementNoTrail>().EnterIfStatementInterrupting(context);
        }

        public sealed override void EnterIfStatementInterruptingTrail(IfStatementInterruptingTrailContext context)
        {
            EnterState<IfStatementTrail>().EnterIfStatementInterruptingTrail(context);
        }

        public sealed override void EnterIfStatementInterruptible(IfStatementInterruptibleContext context)
        {
            EnterState<IfStatementTrail>().EnterIfStatementInterruptible(context);
        }

        public sealed override void EnterIfStatementReturning(IfStatementReturningContext context)
        {
            EnterState<IfStatementNoTrail>().EnterIfStatementReturning(context);
        }

        public sealed override void EnterIfStatementReturningTrailFromElse(IfStatementReturningTrailFromElseContext context)
        {
            IfStatement state;
            if(FindContext<IInterruptibleContext>()?.Flags is null or 0)
            {
                state = EnterState<IfStatementTrailFromElse>();
            }
            else
            {
                // Full controls are needed in a loop
                state = EnterState<IfStatementControl>();
            }
            state.EnterIfStatementReturningTrailFromElse(context);
        }

        public sealed override void EnterIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            EnterState<IfStatementControl>().EnterIfStatementReturningTrail(context);
        }

        public sealed override void EnterIfStatementConditional(IfStatementConditionalContext context)
        {
            EnterState<IfStatementControl>().EnterIfStatementConditional(context);
        }

        public sealed override void EnterDoStatementFree(DoStatementFreeContext context)
        {
            EnterState<DoStatementNoTrail>().EnterDoStatementFree(context);
        }

        public sealed override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            EnterState<DoStatementNoTrail>().EnterDoStatementTerminating(context);
        }

        public sealed override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            EnterState<DoStatementNoTrail>().EnterDoStatementReturning(context);
        }

        public sealed override void EnterDoStatementReturningTrail(DoStatementReturningTrailContext context)
        {
            EnterState<DoStatementControl>().EnterDoStatementReturningTrail(context);
        }

        public sealed override void EnterDoStatementInterrupting(DoStatementInterruptingContext context)
        {
            EnterState<DoStatementNoTrail>().EnterDoStatementInterrupting(context);
        }

        public sealed override void EnterDoStatementInterruptingTrail(DoStatementInterruptingTrailContext context)
        {
            EnterState<DoStatementTrail>().EnterDoStatementInterruptingTrail(context);
        }

        public sealed override void EnterDoStatementInterruptible(DoStatementInterruptibleContext context)
        {
            EnterState<DoStatementTrail>().EnterDoStatementInterruptible(context);
        }

        public sealed override void EnterDoStatementConditional(DoStatementConditionalContext context)
        {
            EnterState<DoStatementControl>().EnterDoStatementConditional(context);
        }

        public sealed override void EnterWhileStatementFree(WhileStatementFreeContext context)
        {
            EnterState<WhileStatementNoTrail>().EnterWhileStatementFree(context);
        }

        public sealed override void EnterWhileStatementFreeInterrupted(WhileStatementFreeInterruptedContext context)
        {
            EnterState<WhileStatementFreeInterrupted>().EnterWhileStatementFreeInterrupted(context);
        }

        public sealed override void EnterWhileStatementTerminating(WhileStatementTerminatingContext context)
        {
            EnterState<WhileStatementNoTrail>().EnterWhileStatementTerminating(context);
        }

        public sealed override void EnterWhileStatementReturningTrail(WhileStatementReturningTrailContext context)
        {
            EnterState<WhileStatementControl>().EnterWhileStatementReturningTrail(context);
        }

        public sealed override void EnterWhileStatementConditional(WhileStatementConditionalContext context)
        {
            EnterState<WhileStatementControl>().EnterWhileStatementConditional(context);
        }

        public sealed override void EnterRepeatStatementFree(RepeatStatementFreeContext context)
        {
            EnterState<RepeatStatementNoTrail>().EnterRepeatStatementFree(context);
        }

        public sealed override void EnterRepeatStatementFreeInterrupted(RepeatStatementFreeInterruptedContext context)
        {
            EnterState<RepeatStatementFreeInterrupted>().EnterRepeatStatementFreeInterrupted(context);
        }

        public sealed override void EnterRepeatStatementTerminating(RepeatStatementTerminatingContext context)
        {
            EnterState<RepeatStatementNoTrail>().EnterRepeatStatementTerminating(context);
        }

        public sealed override void EnterRepeatStatementReturningTrail(RepeatStatementReturningTrailContext context)
        {
            EnterState<RepeatStatementControl>().EnterRepeatStatementReturningTrail(context);
        }

        public sealed override void EnterRepeatStatementConditional(RepeatStatementConditionalContext context)
        {
            EnterState<RepeatStatementControl>().EnterRepeatStatementConditional(context);
        }

        public sealed override void EnterForStatementFree(ForStatementFreeContext context)
        {
            EnterState<ForStatementNoTrail>().EnterForStatementFree(context);
        }

        public sealed override void EnterForStatementFreeInterrupted(ForStatementFreeInterruptedContext context)
        {
            EnterState<ForStatementFreeInterrupted>().EnterForStatementFreeInterrupted(context);
        }

        public sealed override void EnterForStatementReturningTrail(ForStatementReturningTrailContext context)
        {
            EnterState<ForStatementControl>().EnterForStatementReturningTrail(context);
        }

        public sealed override void EnterForStatementConditional(ForStatementConditionalContext context)
        {
            EnterState<ForStatementControl>().EnterForStatementConditional(context);
        }

        public sealed override void EnterSwitchStatementFree(SwitchStatementFreeContext context)
        {
            EnterState<SwitchStatementNoTrail>().EnterSwitchStatementFree(context);
        }

        public sealed override void EnterSwitchStatementFreeInterrupted(SwitchStatementFreeInterruptedContext context)
        {
            EnterState<SwitchStatementInterruptedNoTrail>().EnterSwitchStatementFreeInterrupted(context);
        }

        public sealed override void EnterSwitchStatementTerminating(SwitchStatementTerminatingContext context)
        {
            EnterState<SwitchStatementNoTrail>().EnterSwitchStatementTerminating(context);
        }

        public sealed override void EnterSwitchStatementTerminatingInterrupted(SwitchStatementTerminatingInterruptedContext context)
        {
            EnterState<SwitchStatementInterruptedNoTrail>().EnterSwitchStatementTerminatingInterrupted(context);
        }

        public sealed override void EnterSwitchStatementReturning(SwitchStatementReturningContext context)
        {
            EnterState<SwitchStatementControl>().EnterSwitchStatementReturning(context);
        }

        public sealed override void EnterSwitchStatementReturningTrail(SwitchStatementReturningTrailContext context)
        {
            EnterState<SwitchStatementControl>().EnterSwitchStatementReturningTrail(context);
        }

        public sealed override void EnterSwitchStatementConditional(SwitchStatementConditionalContext context)
        {
            EnterState<SwitchStatementControl>().EnterSwitchStatementConditional(context);
        }

        public sealed override void EnterTryCatchStatementFree(TryCatchStatementFreeContext context)
        {
            EnterState<TryCatchStatementNoTrail>().EnterTryCatchStatementFree(context);
        }

        public sealed override void EnterTryCatchStatementTerminating(TryCatchStatementTerminatingContext context)
        {
            EnterState<TryCatchStatementNoTrail>().EnterTryCatchStatementTerminating(context);
        }

        public sealed override void EnterTryCatchStatementReturning(TryCatchStatementReturningContext context)
        {
            EnterState<TryCatchStatementControl>().EnterTryCatchStatementReturning(context);
        }

        public sealed override void EnterTryCatchStatementReturningTrail(TryCatchStatementReturningTrailContext context)
        {
            EnterState<TryCatchStatementControl>().EnterTryCatchStatementReturningTrail(context);
        }

        public sealed override void EnterTryCatchStatementConditional(TryCatchStatementConditionalContext context)
        {
            EnterState<TryCatchStatementControl>().EnterTryCatchStatementConditional(context);
        }

        public sealed override void EnterTryFinallyStatementFree(TryFinallyStatementFreeContext context)
        {
            EnterState<TryFinallyStatementNoTrail>().EnterTryFinallyStatementFree(context);
        }

        public sealed override void EnterTryFinallyStatementTerminating(TryFinallyStatementTerminatingContext context)
        {
            EnterState<TryFinallyStatementNoTrail>().EnterTryFinallyStatementTerminating(context);
        }

        public sealed override void EnterTryFinallyStatementReturning(TryFinallyStatementReturningContext context)
        {
            EnterState<TryFinallyStatementControl>().EnterTryFinallyStatementReturning(context);
        }

        public sealed override void EnterTryFinallyStatementReturningTrail(TryFinallyStatementReturningTrailContext context)
        {
            EnterState<TryFinallyStatementControl>().EnterTryFinallyStatementReturningTrail(context);
        }

        public sealed override void EnterTryFinallyStatementConditional(TryFinallyStatementConditionalContext context)
        {
            EnterState<TryFinallyStatementControl>().EnterTryFinallyStatementConditional(context);
        }

        public sealed override void EnterTryCatchFinallyStatementFree(TryCatchFinallyStatementFreeContext context)
        {
            EnterState<TryCatchFinallyStatementNoTrail>().EnterTryCatchFinallyStatementFree(context);
        }

        public sealed override void EnterTryCatchFinallyStatementTerminating(TryCatchFinallyStatementTerminatingContext context)
        {
            EnterState<TryCatchFinallyStatementNoTrail>().EnterTryCatchFinallyStatementTerminating(context);
        }

        public sealed override void EnterTryCatchFinallyStatementReturning(TryCatchFinallyStatementReturningContext context)
        {
            EnterState<TryCatchFinallyStatementControl>().EnterTryCatchFinallyStatementReturning(context);
        }

        public sealed override void EnterTryCatchFinallyStatementReturningTrail(TryCatchFinallyStatementReturningTrailContext context)
        {
            EnterState<TryCatchFinallyStatementControl>().EnterTryCatchFinallyStatementReturningTrail(context);
        }

        public sealed override void EnterTryCatchFinallyStatementConditional(TryCatchFinallyStatementConditionalContext context)
        {
            EnterState<TryCatchFinallyStatementControl>().EnterTryCatchFinallyStatementConditional(context);
        }

        public sealed override void EnterInlineSourceFree(InlineSourceFreeContext context)
        {
            EnterState<InlineSourceStatement>().EnterInlineSourceFree(context);
        }

        public sealed override void EnterInlineSourceReturning(InlineSourceReturningContext context)
        {
            EnterState<InlineSourceStatement>().EnterInlineSourceReturning(context);
        }

        public sealed override void EnterInlineSourceTerminating(InlineSourceTerminatingContext context)
        {
            EnterState<InlineSourceStatement>().EnterInlineSourceTerminating(context);
        }

        public sealed override void EnterStatement(StatementContext context)
        {
            OnEnterStatement(StatementFlags.None, context);
        }

        public sealed override void ExitStatement(StatementContext context)
        {
            OnExitStatement(StatementFlags.None, context);
        }

        public sealed override void EnterTrailingStatement(TrailingStatementContext context)
        {
            OnEnterStatement(StatementFlags.None, context);
        }

        public sealed override void ExitTrailingStatement(TrailingStatementContext context)
        {
            OnExitStatement(StatementFlags.None, context);
        }

        public sealed override void EnterReturningStatement(ReturningStatementContext context)
        {
            OnEnterStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitReturningStatement(ReturningStatementContext context)
        {
            OnExitStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterInterruptingStatement(InterruptingStatementContext context)
        {
            OnEnterStatement(StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitInterruptingStatement(InterruptingStatementContext context)
        {
            OnExitStatement(StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterInterruptibleStatement(InterruptibleStatementContext context)
        {
            OnEnterStatement(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitInterruptibleStatement(InterruptibleStatementContext context)
        {
            OnExitStatement(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void EnterConditionalStatement(ConditionalStatementContext context)
        {
            OnEnterStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitConditionalStatement(ConditionalStatementContext context)
        {
            OnExitStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void EnterTerminatingStatement(TerminatingStatementContext context)
        {
            OnEnterStatement(StatementFlags.Terminating, context);
        }

        public sealed override void ExitTerminatingStatement(TerminatingStatementContext context)
        {
            OnExitStatement(StatementFlags.Terminating, context);
        }

        public sealed override void EnterTopLevelStatement(TopLevelStatementContext context)
        {
            OnEnterStatement(StatementFlags.None, context);
            EnterState<TopLevelStatement>().EnterTopLevelStatement(context);
        }

        public sealed override void ExitTopLevelStatement(TopLevelStatementContext context)
        {
            OnExitStatement(StatementFlags.None, context);
        }

        #region Block types
        public sealed override void ExitValueBlock(ValueBlockContext context)
        {
            ExitState()?.ExitValueBlock(context);
        }

        public sealed override void ExitFreeBlock(FreeBlockContext context)
        {
            ExitState()?.ExitFreeBlock(context);
        }

        public sealed override void ExitOpenBlock(OpenBlockContext context)
        {
            ExitState()?.ExitOpenBlock(context);
        }

        public sealed override void ExitReturningBlock(ReturningBlockContext context)
        {
            ExitState()?.ExitReturningBlock(context);
        }

        public sealed override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            ExitState()?.ExitTerminatingBlock(context);
        }

        public sealed override void ExitInterruptingBlock(InterruptingBlockContext context)
        {
            ExitState()?.ExitInterruptingBlock(context);
        }

        public sealed override void ExitReturningCoverBlock(ReturningCoverBlockContext context)
        {
            ExitState()?.ExitReturningCoverBlock(context);
        }

        public sealed override void ExitConditionalBlock(ConditionalBlockContext context)
        {
            ExitState()?.ExitConditionalBlock(context);
        }

        public sealed override void ExitInterruptibleBlock(InterruptibleBlockContext context)
        {
            ExitState()?.ExitInterruptibleBlock(context);
        }

        public sealed override void ExitConditionalCoverBlock(ConditionalCoverBlockContext context)
        {
            ExitState()?.ExitConditionalCoverBlock(context);
        }

        public sealed override void ExitInterruptingCoverBlock(InterruptingCoverBlockContext context)
        {
            ExitState()?.ExitInterruptingCoverBlock(context);
        }

        public sealed override void ExitInterruptibleCoverBlock(InterruptibleCoverBlockContext context)
        {
            ExitState()?.ExitInterruptibleCoverBlock(context);
        }

        public sealed override void ExitOpenCoverBlock(OpenCoverBlockContext context)
        {
            ExitState()?.ExitOpenCoverBlock(context);
        }

        public sealed override void ExitOpenToInterruptibleBlock(OpenToInterruptibleBlockContext context)
        {
            ExitState()?.ExitOpenToInterruptibleBlock(context);
        }

        public sealed override void ExitOpenToConditionalBlock(OpenToConditionalBlockContext context)
        {
            ExitState()?.ExitOpenToConditionalBlock(context);
        }

        public sealed override void ExitInterruptingToInterruptibleBlock(InterruptingToInterruptibleBlockContext context)
        {
            ExitState()?.ExitInterruptingToInterruptibleBlock(context);
        }

        public sealed override void ExitReturningToConditionalBlock(ReturningToConditionalBlockContext context)
        {
            ExitState()?.ExitReturningToConditionalBlock(context);
        }
        #endregion
    }

    internal sealed class ChunkState : BlockState, IFunctionContext, IDeclarationsBlockContext
    {
        ExpressionFlags IExpressionContext.Flags => ExpressionFlags.IsValue;

        BlockFlags IBlockContext.Flags => BlockFlags.None;

        ReturnFlags IReturnableContext.Flags => ReturnFlags.None;

        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;

        string? IInterruptibleContext.InterruptingVariable => null;

        ComputationFlags IComputationContext.Flags => ComputationFlags.None;

        bool IDeclarationsBlockContext.Recursive => false;

        public ChunkState(ScriptEnvironment environment)
        {
            Initialize(environment, null);
        }

        public override void ExitMainBlock(MainBlockContext context)
        {

        }

        void IComputationContext.WriteBeginBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteBeginBlockExpression(context);
        }

        void IComputationContext.WriteEndBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteEndBlockExpression(context);
        }

        void IReturnableContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            Out.Write("do ");
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {

        }

        void IBlockContext.WriteImplicitReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteImplicitReturnStatement(context);
        }

        void IReturnableContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            Defaults.WriteReturnValue(isOption, context);
        }

        void IReturnableContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnValue(context);
        }

        void IReturnableContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            Defaults.WriteEmptyReturnValue(context);
        }

        void IInterruptibleContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteBreak(hasExpression, context);
        }

        void IInterruptibleContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteContinue(hasExpression, context);
        }

        void IInterruptibleContext.WriteAfterBreak(ParserRuleContext context)
        {
            Defaults.WriteAfterBreak(context);
        }

        void IInterruptibleContext.WriteAfterContinue(ParserRuleContext context)
        {
            Defaults.WriteAfterContinue(context);
        }
    }

    internal sealed class MemberOrAssignmentState : MemberExprState
    {
        public override void EnterMemberOrAssignment(MemberOrAssignmentContext context)
        {
            if(FindContext<IStatementContext>() is IDeclarationsBlockContext)
            {
                // Module-level statement requires unit return
                Out.Write("do ");
            }
        }

        public override void ExitMemberOrAssignment(MemberOrAssignmentContext context)
        {
            ExitState().ExitMemberOrAssignment(context);
        }

        public override void EnterMemberExpr(MemberExprContext context)
        {
            EnterState<MemberExprState>().EnterMemberExpr(context);
        }

        public override void ExitMemberExpr(MemberExprContext context)
        {

        }

        public override void EnterAltMemberExpr(AltMemberExprContext context)
        {
            EnterState<AltMemberExprState>().EnterAltMemberExpr(context);
        }

        public override void ExitAltMemberExpr(AltMemberExprContext context)
        {

        }

        public override void EnterAssignment(AssignmentContext context)
        {
            Out.WriteOperator("<-");
        }

        public override void ExitAssignment(AssignmentContext context)
        {

        }
    }

    internal sealed class MemberDiscardState : MemberExprState
    {
        public override void EnterMemberDiscard(MemberDiscardContext context)
        {
            Out.Write("let _");
            Out.WriteOperator('=');
        }

        public override void ExitMemberDiscard(MemberDiscardContext context)
        {
            ExitState().ExitMemberDiscard(context);
        }

        public override void EnterMemberExpr(MemberExprContext context)
        {
            EnterState<MemberExprState>().EnterMemberExpr(context);
        }

        public override void ExitMemberExpr(MemberExprContext context)
        {

        }

        public override void EnterAltMemberExpr(AltMemberExprContext context)
        {
            EnterState<AltMemberExprState>().EnterAltMemberExpr(context);
        }

        public override void ExitAltMemberExpr(AltMemberExprContext context)
        {

        }
    }
}
