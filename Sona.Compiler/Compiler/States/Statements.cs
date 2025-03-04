﻿using System;
using System.IO;
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

        protected virtual void OnEnterStatement(StatementFlags flags)
        {

        }

        protected virtual void OnExitStatement(StatementFlags flags)
        {
            Out.WriteLine();
        }

        public override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnEnterStatement(StatementFlags.None);
            Out.Write("()");
        }

        public override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnExitStatement(StatementFlags.None);
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
            OnEnterStatement(StatementFlags.None);
        }

        public sealed override void ExitStatement(StatementContext context)
        {
            OnExitStatement(StatementFlags.None);
        }

        public sealed override void EnterReturningStatement(ReturningStatementContext context)
        {
            OnEnterStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void ExitReturningStatement(ReturningStatementContext context)
        {
            OnExitStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void EnterInterruptingStatement(InterruptingStatementContext context)
        {
            OnEnterStatement(StatementFlags.InterruptPath);
        }

        public sealed override void ExitInterruptingStatement(InterruptingStatementContext context)
        {
            OnExitStatement(StatementFlags.InterruptPath);
        }

        public sealed override void EnterInterruptibleStatement(InterruptibleStatementContext context)
        {
            OnEnterStatement(StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void ExitInterruptibleStatement(InterruptibleStatementContext context)
        {
            OnExitStatement(StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void EnterConditionalStatement(ConditionalStatementContext context)
        {
            OnEnterStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void ExitConditionalStatement(ConditionalStatementContext context)
        {
            OnExitStatement(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void EnterTerminatingStatement(TerminatingStatementContext context)
        {
            OnEnterStatement(StatementFlags.Terminating);
        }

        public sealed override void ExitTerminatingStatement(TerminatingStatementContext context)
        {
            OnExitStatement(StatementFlags.Terminating);
        }

        public sealed override void EnterTopLevelStatement(TopLevelStatementContext context)
        {
            OnEnterStatement(StatementFlags.None);
        }

        public sealed override void ExitTopLevelStatement(TopLevelStatementContext context)
        {
            OnExitStatement(StatementFlags.None);
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

        void IFunctionContext.WriteBegin()
        {
            Out.WriteLine("begin");
            Out.EnterScope();
        }

        void IFunctionContext.WriteEnd()
        {
            Out.ExitScope();
            Out.Write("end");
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression)
        {
            Error("`break` must be used in a statement that supports it.");
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression)
        {
            Error("`continue` must be used in a statement that supports it.");
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

    internal sealed class ReturnState : ArgumentStatementState
    {
        IReturnableStatementContext? returnScope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            returnScope = FindContext<IReturnableStatementContext>();
        }

        public override void EnterExpression(ExpressionContext context)
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

        public override void EnterReturnStatement(ReturnStatementContext context)
        {
            if(FindContext<IComputationContext>() is { IsCollection: true, BuilderVariable: null })
            {
                Error("`return` is not allowed in a collection without a builder.");
            }
        }

        public override void ExitReturnStatement(ReturnStatementContext context)
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
                interruptScope.WriteBreak(false);
            }

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
                Out.WriteCoreOperator("reraise");
                Out.Write("()");
            }
            else
            {
                Out.Write(")");
                Out.WriteSpecialMember("throw()");
                Out.Write("()");
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
                Error("`break` must be used in a statement that supports it.");
            }
            else
            {
                scope.WriteBreak(true);
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
                    Error("`break` must be used in a statement that supports it.");
                }
                else
                {
                    scope.WriteBreak(false);
                }
            }
            else
            {
                Out.Write(")");
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
                Error("`continue` must be used in a statement that supports it.");
            }
            else
            {
                scope.WriteContinue(true);
            }
            Out.Write('(');
            base.EnterExpression(context);
        }

        public override void EnterContinueStatement(ContinueStatementContext context)
        {

        }

        public override void ExitContinueStatement(ContinueStatementContext context)
        {
            if(!HasExpression)
            {
                if(scope == null)
                {
                    Error("`continue` must be used in a statement that supports it.");
                }
                else
                {
                    scope.WriteContinue(false);
                }
            }
            else
            {
                Out.Write(")");
            }

            ExitState().ExitContinueStatement(context);
        }
    }

    internal sealed class EchoState : NodeState
    {
        public override void EnterEchoStatement(EchoStatementContext context)
        {
            var identifier = LexerContext.GetState<EchoPragma>()?.Identifier ?? "printfn";

            Out.WriteNamespacedName("Microsoft.FSharp.Core", "ExtraTopLevelOperators");
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
