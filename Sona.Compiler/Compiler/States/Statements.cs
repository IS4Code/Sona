using System;
using System.IO;
using Antlr4.Runtime;
using IS4.Sona.Compiler.Tools;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
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

        protected virtual void OnEnterStatement(StatementFlags flags, ParserRuleContext context)
        {

        }

        protected virtual void OnExitStatement(StatementFlags flags, ParserRuleContext context)
        {
            Out.WriteLine();
        }

        public override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnEnterStatement(StatementFlags.None, context);
            Out.Write("()");
        }

        public override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnExitStatement(StatementFlags.None, context);
        }

        public sealed override void EnterReturnStatement(ReturnStatementContext context)
        {
            EnterState<ReturnState>().EnterReturnStatement(context);
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

        public sealed override void EnterYieldEachStatement(YieldEachStatementContext context)
        {
            EnterState<YieldEachState>().EnterYieldEachStatement(context);
        }

        public sealed override void EnterYieldBreakStatement(YieldBreakStatementContext context)
        {
            EnterState<YieldBreakState>().EnterYieldBreakStatement(context);
        }

        public sealed override void EnterVariableDecl(VariableDeclContext context)
        {
            EnterState<NewVariableState>().EnterVariableDecl(context);
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
            if(FindContext<IInterruptibleStatementContext>()?.Flags is null or 0)
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

        public sealed override void EnterInlineSourceFree(InlineSourceFreeContext context)
        {
            EnterState<InlineSource>().EnterInlineSourceFree(context);
        }

        public sealed override void EnterInlineSourceReturning(InlineSourceReturningContext context)
        {
            EnterState<InlineSource>().EnterInlineSourceReturning(context);
        }

        public sealed override void EnterInlineSourceTerminating(InlineSourceTerminatingContext context)
        {
            EnterState<InlineSource>().EnterInlineSourceTerminating(context);
        }

        public sealed override void EnterImportStatement(ImportStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterImportStatement(context);
        }

        public sealed override void EnterImportTypeStatement(ImportTypeStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterImportTypeStatement(context);
        }

        public sealed override void EnterImportFileStatement(ImportFileStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterImportFileStatement(context);
        }

        public sealed override void EnterIncludeStatement(IncludeStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterIncludeStatement(context);
        }

        public sealed override void EnterRequireStatement(RequireStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterRequireStatement(context);
        }

        public sealed override void EnterStatement(StatementContext context)
        {
            OnEnterStatement(StatementFlags.None, context);
        }

        public sealed override void ExitStatement(StatementContext context)
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

        public sealed override void ExitClosingBlock(ClosingBlockContext context)
        {
            ExitState()?.ExitClosingBlock(context);
        }

        public sealed override void ExitConditionalBlock(ConditionalBlockContext context)
        {
            ExitState()?.ExitConditionalBlock(context);
        }

        public sealed override void ExitInterruptibleBlock(InterruptibleBlockContext context)
        {
            ExitState()?.ExitInterruptibleBlock(context);
        }

        public sealed override void ExitFullBlock(FullBlockContext context)
        {
            ExitState()?.ExitFullBlock(context);
        }
        #endregion
    }

    internal sealed class ChunkState : BlockState, IFunctionContext
    {
        // Main block return is currently ignored
        string? IReturnableStatementContext.ReturnVariable => null;
        string? IReturnableStatementContext.ReturningVariable => null;
        bool IExpressionContext.IsLiteral => false;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;

        string? IInterruptibleStatementContext.InterruptingVariable => null;

        bool IFunctionContext.IsOptionalReturn => false;

        bool IComputationContext.IsCollection => false;

        string? IComputationContext.BuilderVariable => null;

        public ChunkState(ScriptEnvironment environment)
        {
            Initialize(environment, null);
        }

        public override void ExitMainBlock(MainBlockContext context)
        {

        }

        void IComputationContext.WriteBeginBlockExpression()
        {
            Out.EnterNestedScope();
            Out.WriteLine("(");
        }

        void IComputationContext.WriteEndBlockExpression()
        {
            Out.ExitNestedScope();
            Out.Write(')');
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Error("`break` must be used in a statement that supports it.", context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Error("`continue` must be used in a statement that supports it.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

        }
    }

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
            HasExpression = true;
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {

        }
    }

    internal class ReturnState : ArgumentStatementState
    {
        IReturnableStatementContext? returnScope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            returnScope = FindContext<IReturnableStatementContext>();
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            if(returnScope?.ReturnVariable is { } result)
            {
                // Store result in variable
                Out.WriteIdentifier(result);
                Out.WriteOperator("<-");
            }
            Out.Write('(');
            base.EnterExpression(context);
        }

        public sealed override void EnterReturnStatement(ReturnStatementContext context)
        {
            if(FindContext<IComputationContext>() is { IsCollection: true, BuilderVariable: null })
            {
                Error("`return` is not allowed in a collection without a builder.", context);
            }
        }

        protected virtual void OnExit(ParserRuleContext context)
        {
            if(!HasExpression)
            {
                if(returnScope?.ReturnVariable is { } result)
                {
                    Out.WriteIdentifier(result);
                    Out.WriteOperator("<-");
                }
                Out.Write("()");
            }
            else
            {
                Out.Write(")");
            }

            // Control variables must be set last, in case the expression throws

            if(returnScope?.ReturningVariable is { } success)
            {
                // Value returned
                Out.WriteLine();
                Out.WriteIdentifier(success);
                Out.WriteOperator("<-");
                Out.Write("true");
            }

            var interruptScope = FindContext<IInterruptibleStatementContext>();
            if(interruptScope != null && (interruptScope.Flags & InterruptFlags.CanBreak) == 0)
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
            if(scope != null && (scope.Flags & InterruptFlags.CanBreak) == 0)
            {
                scope = null;
            }
        }

        public override void EnterExpression(ExpressionContext context)
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
            base.EnterExpression(context);
        }

        public override void EnterBreakStatement(BreakStatementContext context)
        {

        }

        public override void ExitBreakStatement(BreakStatementContext context)
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

            ExitState().ExitBreakStatement(context);
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
            if(scope == null)
            {
                Error("`continue` must be used in a statement that supports it.", context);
            }
            else
            {
                scope.WriteContinue(true, context);
            }
            Out.Write('(');
            base.EnterExpression(context);
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

    internal sealed class EchoState : NodeState
    {
        public override void EnterEchoStatement(EchoStatementContext context)
        {
            var identifier = LexerContext.GetState<EchoPragma>()?.Identifier ?? "printfn";

            Out.WriteCoreName("ExtraTopLevelOperators");
            Out.Write('.');
            Out.WriteIdentifier(identifier);
        }

        public override void ExitEchoStatement(EchoStatementContext context)
        {
            ExitState().ExitEchoStatement(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.Write('(');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            Out.Write(')');
        }
    }

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

    internal sealed class MemberOrAssignmentState : MemberExprState
    {
        public override void EnterMemberOrAssignment(MemberOrAssignmentContext context)
        {

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

        }

        public override void ExitMemberDiscard(MemberDiscardContext context)
        {
            ExitState().ExitMemberDiscard(context);
        }

        public override void EnterMemberExpr(MemberExprContext context)
        {
            Out.Write("let _");
            Out.WriteOperator('=');
            EnterState<MemberExprState>().EnterMemberExpr(context);
        }

        public override void ExitMemberExpr(MemberExprContext context)
        {

        }

        public override void EnterAltMemberExpr(AltMemberExprContext context)
        {
            Out.Write("let _");
            Out.WriteOperator('=');
            EnterState<AltMemberExprState>().EnterAltMemberExpr(context);
        }

        public override void ExitAltMemberExpr(AltMemberExprContext context)
        {

        }
    }

    internal sealed class TopLevelStatement : NodeState
    {
        string? argument;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            argument = null;
        }

        public override void EnterImportStatement(ImportStatementContext context)
        {
            Out.Write("open ");
        }

        public override void EnterImportTypeStatement(ImportTypeStatementContext context)
        {
            Out.Write("open type ");
        }

        public override void EnterImportFileStatement(ImportFileStatementContext context)
        {
            Out.Write("#load ");
        }

        public override void EnterIncludeStatement(IncludeStatementContext context)
        {
            Out.Write("#load ");
        }

        public override void EnterRequireStatement(RequireStatementContext context)
        {
            Out.Write("#r ");
        }

        public override void ExitImportStatement(ImportStatementContext context)
        {
            ExitState().ExitImportStatement(context);
        }

        public override void ExitImportTypeStatement(ImportTypeStatementContext context)
        {
            ExitState().ExitImportTypeStatement(context);
        }

        public override void ExitImportFileStatement(ImportFileStatementContext context)
        {
            Out.WriteLine();
            Out.Write("open ");

            var path = Syntax.GetStringLiteralValue(argument!);
            var name = Path.GetFileNameWithoutExtension(path);
            if(name.Length > 0)
            {
                char c = name[0];
                char uc = Char.ToUpperInvariant(c);
                if(uc != c)
                {
                    name = uc + name.Substring(1);
                }
            }

            Out.WriteIdentifier(name);

            ExitState().ExitImportFileStatement(context);
        }

        public override void ExitIncludeStatement(IncludeStatementContext context)
        {
            ExitState().ExitIncludeStatement(context);
        }

        public override void ExitRequireStatement(RequireStatementContext context)
        {
            ExitState().ExitRequireStatement(context);
        }

        public override void EnterString(StringContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitString(StringContext context)
        {
            try
            {
                Out.Write(argument = context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }
    }
}
