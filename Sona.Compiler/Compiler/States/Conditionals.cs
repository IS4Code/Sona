﻿using System.Collections.Generic;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal abstract class ControlStatement : NodeState, IStatementContext
    {
        sealed class TrailingStatements : BlockState
        {
            bool first;
            IFunctionContext? scope;
            bool? statementsAllowed;

            bool StatementsAllowed => statementsAllowed ??= (Parent!.FindContext<IStatementContext>()?.TrailAllowed ?? true);

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
                scope = null;
                statementsAllowed = null;
            }

            public override void EnterIgnoredTrail(IgnoredTrailContext context)
            {
                scope = FindContext<IFunctionContext>();

                Out.WriteCoreOperator("ignore");
                Out.Write(" ");
                scope?.WriteBegin();
            }

            void OnExitTrail()
            {
                if(!first)
                {
                    Out.WriteLine();
                }
            }

            public override void ExitIgnoredTrail(IgnoredTrailContext context)
            {
                OnExitTrail();
                scope?.WriteEnd();
                Out.WriteLine();
                ExitState()?.ExitIgnoredTrail(context);
            }

            #region Trail types
            public sealed override void EnterOpenTrail(OpenTrailContext context)
            {

            }

            public sealed override void ExitOpenTrail(OpenTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitOpenTrail(context);
            }

            public sealed override void EnterReturningTrail(ReturningTrailContext context)
            {

            }

            public sealed override void ExitReturningTrail(ReturningTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitReturningTrail(context);
            }

            public sealed override void EnterTerminatingTrail(TerminatingTrailContext context)
            {

            }

            public sealed override void ExitTerminatingTrail(TerminatingTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitTerminatingTrail(context);
            }

            public sealed override void EnterInterruptingTrail(InterruptingTrailContext context)
            {

            }

            public sealed override void ExitInterruptingTrail(InterruptingTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitInterruptingTrail(context);
            }

            public sealed override void EnterClosingTrail(ClosingTrailContext context)
            {

            }

            public sealed override void ExitClosingTrail(ClosingTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitClosingTrail(context);
            }

            public sealed override void EnterConditionalTrail(ConditionalTrailContext context)
            {

            }

            public sealed override void ExitConditionalTrail(ConditionalTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitConditionalTrail(context);
            }

            public sealed override void EnterInterruptibleTrail(InterruptibleTrailContext context)
            {

            }

            public sealed override void ExitInterruptibleTrail(InterruptibleTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitInterruptibleTrail(context);
            }

            public sealed override void EnterFullTrail(FullTrailContext context)
            {

            }

            public sealed override void ExitFullTrail(FullTrailContext context)
            {
                OnExitTrail();
                ExitState()?.ExitFullTrail(context);
            }
            #endregion

            protected override void OnEnterStatement(StatementFlags flags)
            {
                if(!StatementsAllowed)
                {
                    Error("A statement is not allowed in this context.");
                }
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.WriteLine();
                }
            }

            protected override void OnExitStatement(StatementFlags flags)
            {
            
            }

            public override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
            {
                if(StatementsAllowed)
                {
                    base.EnterImplicitReturnStatement(context);
                }
            }

            public override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
            {
                if(StatementsAllowed)
                {
                    base.ExitImplicitReturnStatement(context);
                }
            }
        }

#nullable disable
        protected string ReturnVariable { get; private set; }
        protected string ReturningVariable { get; private set; }
#nullable restore
        IReturnableStatementContext? returnScope;
        IInterruptibleStatementContext? interruptScope;

        protected string? OriginalReturnVariable => returnScope?.ReturnVariable;
        protected string? OriginalReturningVariable => returnScope?.ReturningVariable;

        // Nested returns will be stored here
        protected string? ScopeReturnVariable => ReturnVariable ?? OriginalReturnVariable;
        protected string? ScopeReturningVariable => ReturningVariable ?? OriginalReturningVariable;

        bool IStatementContext.TrailAllowed => true;

        StatementFlags enterFlags;
        bool exited;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            enterFlags = StatementFlags.None;
            exited = false;

            returnScope = FindContext<IReturnableStatementContext>();
            interruptScope = FindContext<IInterruptibleStatementContext>();
            if(interruptScope?.Flags == 0)
            {
                interruptScope = null;
            }
        }

        const StatementFlags conditionalFlags = StatementFlags.OpenPath | StatementFlags.ReturnPath;
        const StatementFlags interruptibleFlags = StatementFlags.OpenPath | StatementFlags.InterruptPath;

        protected virtual void OnEnter(StatementFlags flags)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                if(OriginalReturnVariable is null)
                {
                    // Initialize variables for conditional return
                    ReturningVariable = Out.CreateTemporaryIdentifier();
                    ReturnVariable = Out.CreateTemporaryIdentifier();
                    // var success = false
                    Out.Write("let mutable ");
                    Out.WriteIdentifier(ReturningVariable);
                    Out.WriteOperator('=');
                    Out.WriteLine("false");
                    // var result = default
                    Out.Write("let mutable ");
                    Out.WriteIdentifier(ReturnVariable);
                    Out.WriteOperator('=');
                    Out.WriteCoreOperator("Unchecked");
                    Out.WriteLine(".defaultof<_>");
                }
            }
        }

        protected virtual void OnExit(StatementFlags flags)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                Out.WriteLine();
                Out.Write("if ");
                Out.WriteIdentifier(ScopeReturningVariable ?? Error("Returning from a scope that does not support return."));
                Out.Write(" then ");
                if(OriginalReturnVariable is null)
                {
                    // Nowhere to assign, return
                    Out.WriteIdentifier(ReturnVariable);
                }
                else
                {
                    // Variables already assigned
                    Out.Write("()");
                }
                Out.WriteLine();
                if(interruptScope != null && (flags & interruptibleFlags) == interruptibleFlags)
                {
                    Out.Write("elif ");
                    Out.WriteIdentifier(interruptScope.InterruptingVariable ?? Error("COMPILER ERROR: Interrupting variable not provided."));
                    Out.WriteLine(" then ()");
                }
                // else ...
                Out.Write("else ");
                // Own declared variables no longer used
                ReturnVariable = null;
                ReturningVariable = null;
            }
            else if(interruptScope != null && (flags & interruptibleFlags) == interruptibleFlags)
            {
                Out.WriteLine();
                Out.Write("if ");
                Out.WriteIdentifier(interruptScope.InterruptingVariable ?? Error("COMPILER ERROR: Interrupting variable not provided."));
                Out.WriteLine(" then ()");
                Out.Write("else ");
            }
        }

        protected virtual void OnEnterBlock(StatementFlags flags)
        {
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        protected virtual void OnExitBlock(StatementFlags flags)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        #region Block types
        public sealed override void EnterValueBlock(ValueBlockContext context)
        {
            OnEnterBlock(StatementFlags.None);
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public sealed override void ExitValueBlock(ValueBlockContext context)
        {
            OnExitBlock(StatementFlags.None);
        }

        public sealed override void EnterFreeBlock(FreeBlockContext context)
        {
            OnEnterBlock(StatementFlags.OpenPath);
            EnterState<BlockState>().EnterFreeBlock(context);
        }

        public sealed override void ExitFreeBlock(FreeBlockContext context)
        {
            OnExitBlock(StatementFlags.OpenPath);
        }

        public sealed override void EnterOpenBlock(OpenBlockContext context)
        {
            OnEnterBlock(StatementFlags.OpenPath);
            EnterState<BlockState>().EnterOpenBlock(context);
        }

        public sealed override void ExitOpenBlock(OpenBlockContext context)
        {
            OnExitBlock(StatementFlags.OpenPath);
        }

        public sealed override void EnterReturningBlock(ReturningBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            EnterState<BlockState>().EnterReturningBlock(context);
        }

        public sealed override void ExitReturningBlock(ReturningBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void EnterTerminatingBlock(TerminatingBlockContext context)
        {
            OnEnterBlock(StatementFlags.Terminating);
            EnterState<BlockState>().EnterTerminatingBlock(context);
        }

        public sealed override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            OnExitBlock(StatementFlags.Terminating);
        }

        public sealed override void EnterInterruptingBlock(InterruptingBlockContext context)
        {
            OnEnterBlock(StatementFlags.InterruptPath);
            EnterState<BlockState>().EnterInterruptingBlock(context);
        }

        public sealed override void ExitInterruptingBlock(InterruptingBlockContext context)
        {
            OnExitBlock(StatementFlags.InterruptPath);
        }

        public sealed override void EnterClosingBlock(ClosingBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            EnterState<BlockState>().EnterClosingBlock(context);
        }

        public sealed override void ExitClosingBlock(ClosingBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void EnterConditionalBlock(ConditionalBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            EnterState<BlockState>().EnterConditionalBlock(context);
        }

        public sealed override void ExitConditionalBlock(ConditionalBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void EnterInterruptibleBlock(InterruptibleBlockContext context)
        {
            OnEnterBlock(StatementFlags.InterruptPath | StatementFlags.OpenPath);
            EnterState<BlockState>().EnterInterruptibleBlock(context);
        }

        public sealed override void ExitInterruptibleBlock(InterruptibleBlockContext context)
        {
            OnExitBlock(StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void EnterFullBlock(FullBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            EnterState<BlockState>().EnterFullBlock(context);
        }

        public sealed override void ExitFullBlock(FullBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }
        #endregion

        public sealed override void EnterIgnoredTrail(IgnoredTrailContext context)
        {
            OnExitInner(enterFlags);
            Out.WriteLine();
            Out.Write("else ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterIgnoredTrail(context);
        }

        public sealed override void ExitIgnoredTrail(IgnoredTrailContext context)
        {
            Out.WriteCoreOperator("Unchecked");
            Out.WriteLine(".defaultof<_>");
            Out.ExitScope();
            Out.Write("end");
        }

        public sealed override void EnterIgnoredEmptyTrail(IgnoredEmptyTrailContext context)
        {
            OnExitInner(enterFlags);
            Out.WriteLine();
            Out.Write("else ");
            Out.WriteCoreOperator("Unchecked");
            Out.Write(".defaultof<_>");
            // Possible to elide even further if empty trail is predicted
        }

        public sealed override void ExitIgnoredEmptyTrail(IgnoredEmptyTrailContext context)
        {

        }

        protected virtual void OnEnterTrail(StatementFlags flags)
        {
            OnExitInner(enterFlags);
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        protected virtual void OnExitTrail(StatementFlags flags)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        #region Trail types
        public sealed override void EnterOpenTrail(OpenTrailContext context)
        {
            OnEnterTrail(StatementFlags.OpenPath);
            EnterState<TrailingStatements>().EnterOpenTrail(context);
        }

        public sealed override void ExitOpenTrail(OpenTrailContext context)
        {
            OnExitTrail(StatementFlags.OpenPath);
        }

        public sealed override void EnterReturningTrail(ReturningTrailContext context)
        {
            OnEnterTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            EnterState<TrailingStatements>().EnterReturningTrail(context);
        }

        public sealed override void ExitReturningTrail(ReturningTrailContext context)
        {
            OnExitTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void EnterClosingTrail(ClosingTrailContext context)
        {
            OnEnterTrail(StatementFlags.Terminating);
            EnterState<TrailingStatements>().EnterClosingTrail(context);
        }

        public sealed override void ExitClosingTrail(ClosingTrailContext context)
        {
            OnExitTrail(StatementFlags.Terminating);
        }

        public sealed override void EnterTerminatingTrail(TerminatingTrailContext context)
        {
            OnEnterTrail(StatementFlags.Terminating);
            EnterState<TrailingStatements>().EnterTerminatingTrail(context);
        }

        public sealed override void ExitTerminatingTrail(TerminatingTrailContext context)
        {
            OnExitTrail(StatementFlags.Terminating);
        }

        public sealed override void EnterInterruptingTrail(InterruptingTrailContext context)
        {
            OnEnterTrail(StatementFlags.InterruptPath);
            EnterState<TrailingStatements>().EnterInterruptingTrail(context);
        }

        public sealed override void ExitInterruptingTrail(InterruptingTrailContext context)
        {
            OnExitTrail(StatementFlags.InterruptPath);
        }

        public sealed override void EnterConditionalTrail(ConditionalTrailContext context)
        {
            OnEnterTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            EnterState<TrailingStatements>().EnterConditionalTrail(context);
        }

        public sealed override void ExitConditionalTrail(ConditionalTrailContext context)
        {
            OnExitTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void EnterInterruptibleTrail(InterruptibleTrailContext context)
        {
            OnEnterTrail(StatementFlags.InterruptPath | StatementFlags.OpenPath);
            EnterState<TrailingStatements>().EnterInterruptibleTrail(context);
        }

        public sealed override void ExitInterruptibleTrail(InterruptibleTrailContext context)
        {
            OnExitTrail(StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void EnterFullTrail(FullTrailContext context)
        {
            OnEnterTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            EnterState<TrailingStatements>().EnterFullTrail(context);
        }

        public sealed override void ExitFullTrail(FullTrailContext context)
        {
            OnExitTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }
        #endregion
        
        private void OnEnterInner(StatementFlags flags)
        {
            if(this is IReturnableStatementContext)
            {
                flags |= StatementFlags.OpenPath;
            }
            enterFlags = flags;
            OnEnter(flags);
        }

        private void OnExitInner(StatementFlags flags)
        {
            if(this is IReturnableStatementContext)
            {
                flags |= StatementFlags.OpenPath;
            }
            if(!exited)
            {
                exited = true;
                OnExit(flags);
            }
        }

        #region Statement types
        public sealed override void EnterIfStatementFree(IfStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public sealed override void ExitIfStatementFree(IfStatementFreeContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitIfStatementFree(context);
        }

        public sealed override void EnterIfStatementReturning(IfStatementReturningContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void ExitIfStatementReturning(IfStatementReturningContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitIfStatementReturning(context);
        }

        public sealed override void EnterIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void ExitIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitIfStatementReturningTrail(context);
        }

        public sealed override void EnterIfStatementReturningTrailFromElse(IfStatementReturningTrailFromElseContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void ExitIfStatementReturningTrailFromElse(IfStatementReturningTrailFromElseContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitIfStatementReturningTrailFromElse(context);
        }

        public sealed override void EnterIfStatementInterrupting(IfStatementInterruptingContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath);
        }

        public sealed override void ExitIfStatementInterrupting(IfStatementInterruptingContext context)
        {
            OnExitInner(StatementFlags.InterruptPath);
            ExitState().ExitIfStatementInterrupting(context);
        }

        public sealed override void EnterIfStatementInterruptingTrail(IfStatementInterruptingTrailContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath);
        }

        public sealed override void ExitIfStatementInterruptingTrail(IfStatementInterruptingTrailContext context)
        {
            OnExitInner(StatementFlags.InterruptPath);
            ExitState().ExitIfStatementInterruptingTrail(context);
        }

        public sealed override void EnterIfStatementInterruptible(IfStatementInterruptibleContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void ExitIfStatementInterruptible(IfStatementInterruptibleContext context)
        {
            OnExitInner(StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitIfStatementInterruptible(context);
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating);
        }

        public sealed override void ExitIfStatementTerminating(IfStatementTerminatingContext context)
        {
            OnExitInner(StatementFlags.Terminating);
            ExitState().ExitIfStatementTerminating(context);
        }

        public sealed override void EnterIfStatementConditional(IfStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void ExitIfStatementConditional(IfStatementConditionalContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitIfStatementConditional(context);
        }

        public sealed override void EnterDoStatementFree(DoStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public sealed override void ExitDoStatementFree(DoStatementFreeContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitDoStatementFree(context);
        }

        public sealed override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating);
        }

        public sealed override void ExitDoStatementTerminating(DoStatementTerminatingContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitDoStatementTerminating(context);
        }

        public sealed override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public sealed override void ExitDoStatementReturning(DoStatementReturningContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitDoStatementReturning(context);
        }

        public sealed override void EnterDoStatementInterrupting(DoStatementInterruptingContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath);
        }

        public sealed override void ExitDoStatementInterrupting(DoStatementInterruptingContext context)
        {
            OnExitInner(StatementFlags.InterruptPath);
            ExitState().ExitDoStatementInterrupting(context);
        }

        public sealed override void EnterDoStatementInterruptingTrail(DoStatementInterruptingTrailContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath);
        }

        public sealed override void ExitDoStatementInterruptingTrail(DoStatementInterruptingTrailContext context)
        {
            OnExitInner(StatementFlags.InterruptPath);
            ExitState().ExitDoStatementInterruptingTrail(context);
        }

        public sealed override void EnterDoStatementInterruptible(DoStatementInterruptibleContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void ExitDoStatementInterruptible(DoStatementInterruptibleContext context)
        {
            OnExitInner(StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitDoStatementInterruptible(context);
        }

        public sealed override void EnterDoStatementConditional(DoStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public sealed override void ExitDoStatementConditional(DoStatementConditionalContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitDoStatementConditional(context);
        }

        public override void EnterWhileStatementFree(WhileStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public override void ExitWhileStatementFree(WhileStatementFreeContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitWhileStatementFree(context);
        }

        public override void EnterWhileStatementFreeInterrupted(WhileStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public override void ExitWhileStatementFreeInterrupted(WhileStatementFreeInterruptedContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitWhileStatementFreeInterrupted(context);
        }

        public override void EnterWhileStatementTerminating(WhileStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating);
        }

        public override void ExitWhileStatementTerminating(WhileStatementTerminatingContext context)
        {
            OnExitInner(StatementFlags.Terminating);
            ExitState().ExitWhileStatementTerminating(context);
        }

        public override void EnterWhileStatementReturningTrail(WhileStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public override void ExitWhileStatementReturningTrail(WhileStatementReturningTrailContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitWhileStatementReturningTrail(context);
        }

        public override void EnterWhileStatementConditional(WhileStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public override void ExitWhileStatementConditional(WhileStatementConditionalContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitWhileStatementConditional(context);
        }

        public override void EnterRepeatStatementFree(RepeatStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public override void ExitRepeatStatementFree(RepeatStatementFreeContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitRepeatStatementFree(context);
        }

        public override void EnterRepeatStatementFreeInterrupted(RepeatStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public override void ExitRepeatStatementFreeInterrupted(RepeatStatementFreeInterruptedContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitRepeatStatementFreeInterrupted(context);
        }

        public override void EnterRepeatStatementTerminating(RepeatStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating);
        }

        public override void ExitRepeatStatementTerminating(RepeatStatementTerminatingContext context)
        {
            OnExitInner(StatementFlags.Terminating);
            ExitState().ExitRepeatStatementTerminating(context);
        }

        public override void EnterRepeatStatementReturningTrail(RepeatStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public override void ExitRepeatStatementReturningTrail(RepeatStatementReturningTrailContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitRepeatStatementReturningTrail(context);
        }

        public override void EnterRepeatStatementConditional(RepeatStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public override void ExitRepeatStatementConditional(RepeatStatementConditionalContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitRepeatStatementConditional(context);
        }

        public override void EnterForStatementFree(ForStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public override void ExitForStatementFree(ForStatementFreeContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitForStatementFree(context);
        }

        public override void EnterForStatementFreeInterrupted(ForStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath);
        }

        public override void ExitForStatementFreeInterrupted(ForStatementFreeInterruptedContext context)
        {
            OnExitInner(StatementFlags.OpenPath);
            ExitState().ExitForStatementFreeInterrupted(context);
        }

        public override void EnterForStatementReturningTrail(ForStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
        }

        public override void ExitForStatementReturningTrail(ForStatementReturningTrailContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath);
            ExitState().ExitForStatementReturningTrail(context);
        }

        public override void EnterForStatementConditional(ForStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
        }

        public override void ExitForStatementConditional(ForStatementConditionalContext context)
        {
            OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath);
            ExitState().ExitForStatementConditional(context);
        }
        #endregion
    }

    internal abstract class IfStatement : ControlStatement
    {
        protected bool HasElse { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            HasElse = false;
        }

        public sealed override void EnterIf(IfContext context)
        {
            Out.Write("if(");
        }

        public sealed override void EnterElseif(ElseifContext context)
        {
            Out.WriteLine();
            Out.Write("elif(");
        }

        public sealed override void EnterElse(ElseContext context)
        {
            Out.WriteLine();
            Out.Write("else ");
        }

        public sealed override void ExitIf(IfContext context)
        {

        }

        public sealed override void ExitElseif(ElseifContext context)
        {

        }

        public sealed override void ExitElse(ElseContext context)
        {
            HasElse = true;
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            Out.Write(")then ");
        }
    }

    /// <summary>
    /// <c>if</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class IfStatementNoTrail : IfStatement
    {
        protected override void OnEnter(StatementFlags flags)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags);
        }

        protected override void OnExit(StatementFlags flags)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
                return;
            }
            base.OnEnter(flags);
        }
    }

    /// <summary>
    /// <c>if</c> with trailing statements.
    /// </summary>
    internal sealed class IfStatementTrail : IfStatement
    {

    }
    
    /// <summary>
    /// <c>if</c> with trailing statements as a part of <c>else</c>.
    /// </summary>
    internal sealed class IfStatementTrailFromElse : IfStatement
    {
        protected override void OnEnterBlock(StatementFlags flags)
        {
            if((flags & StatementFlags.OpenPath) != 0)
            {
                Out.WriteLine(_begin_);
                Out.EnterScope();
                Out.Write("do ");
            }
            base.OnEnterBlock(flags);
        }

        protected override void OnExitBlock(StatementFlags flags)
        {
            base.OnExitBlock(flags);
            if((flags & StatementFlags.OpenPath) != 0)
            {
                Out.WriteLine();
            }
        }

        protected override void OnEnterTrail(StatementFlags flags)
        {
            if(!HasElse)
            {
                Out.WriteLine();
                Out.Write("else ");
                base.OnEnterTrail(flags);
            }
        }
    }

    internal sealed class IfStatementControl : IfStatement, IReturnableStatementContext
    {
        string? IReturnableStatementContext.ReturnVariable => ScopeReturnVariable;
        string? IReturnableStatementContext.ReturningVariable => ScopeReturningVariable;
    }

    internal abstract class DoStatement : ControlStatement
    {
        protected override void OnEnterBlock(StatementFlags flags)
        {
            Out.Write("if true then ");
            base.OnEnterBlock(flags);
        }
    }

    internal sealed class DoStatementNoTrail : DoStatement
    {

    }

    internal sealed class DoStatementTrail : DoStatement
    {

    }

    internal sealed class DoStatementControl : DoStatement, IReturnableStatementContext
    {
        string? IReturnableStatementContext.ReturnVariable => ScopeReturnVariable;
        string? IReturnableStatementContext.ReturningVariable => ScopeReturningVariable;
    }

    internal abstract class WhileStatement : ControlStatement, IInterruptibleStatementContext
    {
        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.CanBreak | InterruptFlags.CanContinue;

        public string? InterruptingVariable { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InterruptingVariable = null;
        }

        public abstract override void EnterWhile(WhileContext context);
        public abstract override void EnterWhileTrue(WhileTrueContext context);

        public sealed override void ExitWhile(WhileContext context)
        {

        }

        public sealed override void ExitWhileTrue(WhileTrueContext context)
        {

        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            Out.Write(")do ");
        }

        protected sealed override void OnEnterBlock(StatementFlags flags)
        {
            base.OnEnterBlock(flags);
            if((flags & StatementFlags.InterruptPath) != 0)
            {
                InterruptingVariable = Out.CreateTemporaryIdentifier();
                // var interrupting = false
                Out.Write("let mutable ");
                Out.WriteIdentifier(InterruptingVariable);
                Out.WriteOperator('=');
                Out.WriteLine("false");
            }
        }

        public virtual void WriteBreak(bool hasExpression)
        {
            if(hasExpression)
            {
                Error("`break` in a `while` statement does not take an expression.");
            }
            Error("COMPILER ERROR: `break` used in a wrong version of `while`.");
        }

        public virtual void WriteContinue(bool hasExpression)
        {
            if(hasExpression)
            {
                Error("`break` in a `while` statement does not take an expression.");
            }
            Error("COMPILER ERROR: `continue` used in a wrong version of `while`.");
        }
    }

    /// <summary>
    /// <c>while</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class WhileStatementNoTrail : WhileStatement
    {
        protected override void OnEnter(StatementFlags flags)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags);
        }

        protected override void OnExit(StatementFlags flags)
        {
            if(flags == StatementFlags.Terminating)
            {
                // Return any value
                Out.WriteLine();
                Out.WriteCoreOperator("Unchecked");
                Out.Write(".defaultof<_>");
            }
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
                return;
            }
            base.OnEnter(flags);
        }

        public override void EnterWhile(WhileContext context)
        {
            Out.Write("while(");
        }

        public override void EnterWhileTrue(WhileTrueContext context)
        {
            Out.Write("while true do ");
        }
    }

    internal abstract class WhileStatementTrailInterrupted : WhileStatement
    {
        string? continuingVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            continuingVariable = null;
        }

        protected override void OnEnter(StatementFlags flags)
        {
            continuingVariable = Out.CreateTemporaryIdentifier();
            // var continuing = true
            Out.Write("let mutable ");
            Out.WriteIdentifier(continuingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("true");
            base.OnEnter(flags);
        }

        public sealed override void EnterWhile(WhileContext context)
        {
            Out.Write("while ");
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `while` used without a condition variable."));
            Out.WriteOperator("&&");
            Out.Write('(');
        }

        public sealed override void EnterWhileTrue(WhileTrueContext context)
        {
            Out.Write("while ");
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `while` used without a condition variable."));
            Out.Write(" do ");
        }

        public sealed override void WriteBreak(bool hasExpression)
        {
            if(hasExpression)
            {
                base.WriteBreak(hasExpression);
                return;
            }
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.Write("true");
        }

        public sealed override void WriteContinue(bool hasExpression)
        {
            if(hasExpression)
            {
                base.WriteContinue(hasExpression);
                return;
            }
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.Write("true");
        }
    }

    internal sealed class WhileStatementFreeInterrupted : WhileStatementTrailInterrupted
    {

    }

    internal sealed class WhileStatementControl : WhileStatementTrailInterrupted, IReturnableStatementContext
    {
        string? IReturnableStatementContext.ReturnVariable => ScopeReturnVariable;
        string? IReturnableStatementContext.ReturningVariable => ScopeReturningVariable;
    }

    internal abstract class RepeatStatement : ControlStatement, IInterruptibleStatementContext
    {
        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.CanBreak | InterruptFlags.CanContinue;

        public string? InterruptingVariable { get; private set; }

        public string? ContinuingVariable { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InterruptingVariable = null;
            ContinuingVariable = null;
        }

        public abstract override void EnterUntil(UntilContext context);

        public sealed override void ExitUntil(UntilContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            Out.WriteIdentifier(ContinuingVariable ?? Error("COMPILER ERROR: `repeat` used without a condition variable."));
            Out.WriteOperator("<-");
            Out.WriteCoreOperator("not");
            Out.Write('(');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            Out.Write(')');
            Out.WriteLine();
        }

        protected sealed override void OnEnterBlock(StatementFlags flags)
        {
            ContinuingVariable = Out.CreateTemporaryIdentifier();
            // var continuing = true
            Out.Write("let mutable ");
            Out.WriteIdentifier(ContinuingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("true");

            Out.Write("while ");
            Out.WriteIdentifier(ContinuingVariable);
            Out.Write(" do ");

            base.OnEnterBlock(flags);

            if((flags & StatementFlags.InterruptPath) != 0)
            {
                InterruptingVariable = Out.CreateTemporaryIdentifier();
                // var interrupting = false
                Out.Write("let mutable ");
                Out.WriteIdentifier(InterruptingVariable);
                Out.WriteOperator('=');
                Out.WriteLine("false");
            }
        }

        protected sealed override void OnExitBlock(StatementFlags flags)
        {
            // Exited in ExitUntil
        }

        public virtual void WriteBreak(bool hasExpression)
        {
            if(hasExpression)
            {
                Error("`break` in a `repeat` statement does not take an expression.");
            }
            Error("COMPILER ERROR: `break` used in a wrong version of `repeat`.");
        }

        public virtual void WriteContinue(bool hasExpression)
        {
            if(hasExpression)
            {
                Error("`break` in a `repeat` statement does not take an expression.");
            }
            Error("COMPILER ERROR: `continue` used in a wrong version of `repeat`.");
        }
    }

    /// <summary>
    /// <c>repeat</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class RepeatStatementNoTrail : RepeatStatement
    {
        protected override void OnEnter(StatementFlags flags)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags);
        }

        protected override void OnExit(StatementFlags flags)
        {
            if(flags == StatementFlags.Terminating)
            {
                // Return any value
                Out.WriteLine();
                Out.WriteCoreOperator("Unchecked");
                Out.Write(".defaultof<_>");
            }
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
                return;
            }
            base.OnEnter(flags);
        }

        public override void EnterUntil(UntilContext context)
        {

        }
    }

    internal abstract class RepeatStatementTrailInterrupted : RepeatStatement
    {
        public sealed override void EnterUntil(UntilContext context)
        {
            Out.Write("if ");
            Out.WriteIdentifier(ContinuingVariable ?? Error("COMPILER ERROR: `repeat` used without a condition variable."));
            Out.Write(" then ");
        }

        public sealed override void WriteBreak(bool hasExpression)
        {
            if(hasExpression)
            {
                base.WriteBreak(hasExpression);
                return;
            }
            Out.WriteIdentifier(ContinuingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.Write("true");
        }

        public sealed override void WriteContinue(bool hasExpression)
        {
            if(hasExpression)
            {
                base.WriteContinue(hasExpression);
                return;
            }
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.Write("true");
        }
    }

    internal sealed class RepeatStatementFreeInterrupted : RepeatStatementTrailInterrupted
    {

    }

    internal sealed class RepeatStatementControl : RepeatStatementTrailInterrupted, IReturnableStatementContext
    {
        string? IReturnableStatementContext.ReturnVariable => ScopeReturnVariable;
        string? IReturnableStatementContext.ReturningVariable => ScopeReturningVariable;
    }

    internal abstract class ForStatement : ControlStatement, IInterruptibleStatementContext
    {
        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.CanBreak | InterruptFlags.CanContinue;

        public string? InterruptingVariable { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InterruptingVariable = null;
        }

        public sealed override void EnterFor(ForContext context)
        {

        }

        public abstract override void ExitFor(ForContext context);

        public abstract override void EnterForSimple(ForSimpleContext context);

        public sealed override void EnterForSimpleStep(ForSimpleStepContext context)
        {
            Error("`for` statement cannot use `by` with a non-range sequence.");
        }

        public abstract override void EnterForRange(ForRangeContext context);
        public abstract override void EnterForRangeStep(ForRangeStepContext context);
        public abstract override void EnterForRangePrimitiveStep(ForRangePrimitiveStepContext context);
        
        public override void ExitForSimple(ForSimpleContext context)
        {

        }

        public sealed override void ExitForSimpleStep(ForSimpleStepContext context)
        {

        }

        public override void ExitForRange(ForRangeContext context)
        {

        }

        public override void ExitForRangeStep(ForRangeStepContext context)
        {

        }

        public override void ExitForRangePrimitiveStep(ForRangePrimitiveStepContext context)
        {

        }

        public abstract override void EnterExpression(ExpressionContext context);
        public abstract override void ExitExpression(ExpressionContext context);
        public abstract override void EnterForRangeExpression(ForRangeExpressionContext context);
        public abstract override void ExitForRangeExpression(ForRangeExpressionContext context);
        public abstract override void EnterPrimitiveExpr(PrimitiveExprContext context);
        public abstract override void ExitPrimitiveExpr(PrimitiveExprContext context);

        private protected const string captureError = "COMPILER ERROR: `for` without captured declaration.";

        public sealed override void EnterDeclaration(DeclarationContext context)
        {
            EnterState<DeclarationState>().EnterDeclaration(context);
        }

        public abstract override void ExitDeclaration(DeclarationContext context);

        protected override void OnEnterBlock(StatementFlags flags)
        {
            base.OnEnterBlock(flags);
            if((flags & StatementFlags.InterruptPath) != 0)
            {
                InterruptingVariable = Out.CreateTemporaryIdentifier();
                // var interrupting = false
                Out.Write("let mutable ");
                Out.WriteIdentifier(InterruptingVariable);
                Out.WriteOperator('=');
                Out.WriteLine("false");
            }
        }

        public virtual void WriteBreak(bool hasExpression)
        {
            if(hasExpression)
            {
                Error("`break` in a `for` statement does not take an expression.");
            }
            Error("COMPILER ERROR: `break` used in a wrong version of `for`.");
        }

        public virtual void WriteContinue(bool hasExpression)
        {
            if(hasExpression)
            {
                Error("`break` in a `for` statement does not take an expression.");
            }
            Error("COMPILER ERROR: `continue` used in a wrong version of `for`.");
        }

        protected sealed class PrimitiveExprState : ExpressionState
        {
            public override void EnterPrimitiveExpr(PrimitiveExprContext context)
            {
                base.EnterPrimitiveExpr(context);
            }

            public override void ExitPrimitiveExpr(PrimitiveExprContext context)
            {
                base.ExitPrimitiveExpr(context);
                ExitState().ExitPrimitiveExpr(context);
            }
        }

        protected sealed class ForRangeExprState : ExpressionState
        {
            public override void EnterForRangeExpression(ForRangeExpressionContext context)
            {

            }

            public override void ExitForRangeExpression(ForRangeExpressionContext context)
            {
                ExitState().ExitForRangeExpression(context);
            }
        }
    }

    /// <summary>
    /// <c>for</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class ForStatementNoTrail : ForStatement
    {
        protected override void OnEnter(StatementFlags flags)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags);
        }

        protected override void OnExit(StatementFlags flags)
        {
            if(flags == StatementFlags.Terminating)
            {
                // Return any value
                Out.WriteLine();
                Out.WriteCoreOperator("Unchecked");
                Out.Write(".defaultof<_>");
            }
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
                return;
            }
            base.OnEnter(flags);
        }

        public override void EnterForSimple(ForSimpleContext context)
        {
            Out.Write("for ");
        }

        public override void EnterForRange(ForRangeContext context)
        {
            EnterState<Range>().EnterForRange(context);
        }

        public override void EnterForRangeStep(ForRangeStepContext context)
        {
            EnterState<RangeStep>().EnterForRangeStep(context);
        }

        public override void EnterForRangePrimitiveStep(ForRangePrimitiveStepContext context)
        {
            EnterState<RangePrimitiveStep>().EnterForRangePrimitiveStep(context);
        }

        public sealed override void ExitDeclaration(DeclarationContext context)
        {
            Out.Write(" in(");
        }

        public override void ExitFor(ForContext context)
        {
            Out.Write(")do ");
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        public override void EnterForRangeExpression(ForRangeExpressionContext context)
        {
            EnterState<ForRangeExprState>().EnterForRangeExpression(context);
        }

        public override void ExitForRangeExpression(ForRangeExpressionContext context)
        {

        }

        public override void EnterPrimitiveExpr(PrimitiveExprContext context)
        {
            EnterState<PrimitiveExprState>().EnterPrimitiveExpr(context);
        }

        public override void ExitPrimitiveExpr(PrimitiveExprContext context)
        {

        }

        sealed class Range : ForStatementNoTrail
        {
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
            }

            public override void EnterForRange(ForRangeContext context)
            {
                Out.Write("for ");
            }

            public override void ExitForRange(ForRangeContext context)
            {
                ExitState().ExitForRange(context);
            }

            public override void ExitForRangeExpression(ForRangeExpressionContext context)
            {
                base.ExitForRangeExpression(context);
                if(first)
                {
                    Out.Write(')');
                    Out.WriteOperator("..");
                    Out.Write('(');
                    first = false;
                }
            }
        }

        sealed class RangeStep : ForStatementNoTrail
        {
            ISourceCapture? capture;
            bool first;
            readonly List<string> parts = new();

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                capture = null;
                first = true;
                parts.Clear();
            }

            public override void EnterForRangeStep(ForRangeStepContext context)
            {
                capture = Out.StartCapture();
                Out.Write("for ");
            }

            public override void ExitForRangeStep(ForRangeStepContext context)
            {
                (capture ?? ErrorCapture(captureError)).Play(Out);
                Out.WriteIdentifier(parts[0]);
                Out.Write(')');
                Out.WriteOperator("..");
                Out.WriteIdentifier(parts[2]);
                Out.WriteOperator("..");
                Out.Write('(');
                Out.WriteIdentifier(parts[1]);
                ExitState().ExitForRangeStep(context);
            }

            public override void EnterForRangeExpression(ForRangeExpressionContext context)
            {
                if(first)
                {
                    Out.StopCapture(capture ?? ErrorCapture(captureError));
                }
                var name = Out.CreateTemporaryIdentifier();
                parts.Add(name);
                Out.Write("let ");
                Out.WriteIdentifier(name);
                Out.WriteOperator('=');
                base.EnterForRangeExpression(context);
            }

            public override void ExitForRangeExpression(ForRangeExpressionContext context)
            {
                base.ExitForRangeExpression(context);
                first = false;
                Out.WriteLine();
            }

            public override void EnterExpression(ExpressionContext context)
            {
                var name = Out.CreateTemporaryIdentifier();
                parts.Add(name);
                Out.Write("let ");
                Out.WriteIdentifier(name);
                Out.WriteOperator('=');
                base.EnterExpression(context);
            }

            public override void ExitExpression(ExpressionContext context)
            {
                base.ExitExpression(context);
                Out.WriteLine();
            }
        }

        sealed class RangePrimitiveStep : ForStatementNoTrail
        {
            ISourceCapture? leftCapture;
            ISourceCapture? rightCapture;
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                leftCapture = null;
                rightCapture = null;
                first = true;
            }

            public override void EnterForRangePrimitiveStep(ForRangePrimitiveStepContext context)
            {
                leftCapture = Out.StartCapture();
                Out.Write("for ");
            }

            public override void ExitForRangePrimitiveStep(ForRangePrimitiveStepContext context)
            {
                ExitState().ExitForRangePrimitiveStep(context);
            }

            public override void EnterForRangeExpression(ForRangeExpressionContext context)
            {
                if(!first)
                {
                    rightCapture = Out.StartCapture();
                }
                base.EnterForRangeExpression(context);
            }

            public override void ExitForRangeExpression(ForRangeExpressionContext context)
            {
                base.ExitForRangeExpression(context);
                if(first)
                {
                    first = false;
                    Out.StopCapture(leftCapture ?? ErrorCapture(captureError));
                }
            }

            public override void EnterPrimitiveExpr(PrimitiveExprContext context)
            {
                Out.StopCapture(rightCapture ?? ErrorCapture(captureError));
                (leftCapture ?? ErrorCapture(captureError)).Play(Out);
                Out.Write(')');
                Out.WriteOperator("..");
                Out.Write('(');
                base.EnterPrimitiveExpr(context);
            }

            public override void ExitPrimitiveExpr(PrimitiveExprContext context)
            {
                base.ExitPrimitiveExpr(context);
                Out.Write(')');
                Out.WriteOperator("..");
                Out.Write('(');
                (rightCapture ?? ErrorCapture(captureError)).Play(Out);
            }
        }
    }

    internal class ForStatementTrailInterrupted : ForStatement
    {
        string? continuingVariable;
        string? enumeratorVariable;
        ISourceCapture? declarationCapture;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            continuingVariable = null;
            enumeratorVariable = null;
            declarationCapture = null;
        }

        protected override void OnEnter(StatementFlags flags)
        {
            continuingVariable = Out.CreateTemporaryIdentifier();
            // var continuing = true
            Out.Write("let mutable ");
            Out.WriteIdentifier(continuingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("true");
            base.OnEnter(flags);

            enumeratorVariable = Out.CreateTemporaryIdentifier();
            // var enumerator = ...
            Out.Write("let mutable ");
            Out.WriteIdentifier(enumeratorVariable);
            Out.WriteOperator('=');
        }
        
        public sealed override void EnterForSimple(ForSimpleContext context)
        {
            declarationCapture = Out.StartCapture();
        }

        public sealed override void EnterForRange(ForRangeContext context)
        {
            Out.Write("((..)");
            declarationCapture = Out.StartCapture();
        }

        private void EnterForRangeStep()
        {
            var start = Out.CreateTemporaryIdentifier();
            var end = Out.CreateTemporaryIdentifier();
            var step = Out.CreateTemporaryIdentifier();
            // ((fun start end step -> (.. ..)start step end)
            Out.Write("((fun ");
            Out.WriteIdentifier(start);
            Out.Write(' ');
            Out.WriteIdentifier(end);
            Out.Write(' ');
            Out.WriteIdentifier(step);
            Out.WriteOperator("->");
            Out.Write("(.. ..)");
            Out.WriteIdentifier(start);
            Out.Write(' ');
            Out.WriteIdentifier(step);
            Out.Write(' ');
            Out.WriteIdentifier(end);
            Out.Write(')');
            declarationCapture = Out.StartCapture();
        }

        public sealed override void EnterForRangeStep(ForRangeStepContext context)
        {
            EnterForRangeStep();
        }

        public sealed override void EnterForRangePrimitiveStep(ForRangePrimitiveStepContext context)
        {
            EnterForRangeStep();
        }

        public sealed override void ExitDeclaration(DeclarationContext context)
        {
            Out.StopCapture(declarationCapture ?? ErrorCapture(captureError));
        }

        public override void ExitForSimple(ForSimpleContext context)
        {
            base.ExitForSimple(context);
        }

        public override void ExitForRange(ForRangeContext context)
        {
            base.ExitForRange(context);
            Out.Write(')');
        }

        public override void ExitForRangeStep(ForRangeStepContext context)
        {
            base.ExitForRangeStep(context);
            Out.Write(')');
        }

        public override void ExitForRangePrimitiveStep(ForRangePrimitiveStepContext context)
        {
            base.ExitForRangePrimitiveStep(context);
            Out.Write(')');
        }

        const string enumeratorError = "COMPILER ERROR: `for` without enumerator variable.";

        public sealed override void ExitFor(ForContext context)
        {
            Out.WriteSpecialMember("each()");
            Out.WriteLine("().GetEnumerator()");
            Out.WriteLine("try");
            Out.EnterScope();
            Out.Write("while ");
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `for` used without a condition variable."));
            Out.WriteOperator("&&");
            Out.WriteIdentifier(enumeratorVariable ?? Error(enumeratorError));
            Out.Write(".MoveNext() do ");
        }

        protected override void OnEnterBlock(StatementFlags flags)
        {
            base.OnEnterBlock(flags);

            Out.Write("let ");
            (declarationCapture ?? ErrorCapture(captureError)).Play(Out);
            Out.WriteOperator('=');
            Out.WriteIdentifier(enumeratorVariable ?? Error(enumeratorError));
            Out.WriteLine(".Current");
        }

        protected sealed override void OnExit(StatementFlags flags)
        {
            Out.ExitScope();
            Out.WriteLine();
            Out.Write("finally ");
            Out.WriteNamespacedName("Sona.Runtime.SequenceHelpers", "DisposeEnumerator");
            Out.Write('(');
            Out.WriteNamespacedName("Sona.Runtime.SequenceHelpers", "Marker");
            Out.Write(',');
            Out.WriteIdentifier(enumeratorVariable ?? Error(enumeratorError));
            Out.Write(")");
            base.OnExit(flags);
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            Out.Write('(');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterForRangeExpression(ForRangeExpressionContext context)
        {
            Out.Write('(');
            EnterState<ForRangeExprState>().EnterForRangeExpression(context);
        }

        public sealed override void ExitForRangeExpression(ForRangeExpressionContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterPrimitiveExpr(PrimitiveExprContext context)
        {
            Out.Write('(');
            EnterState<PrimitiveExprState>().EnterPrimitiveExpr(context);
        }

        public sealed override void ExitPrimitiveExpr(PrimitiveExprContext context)
        {
            Out.Write(')');
        }

        public sealed override void WriteBreak(bool hasExpression)
        {
            if(hasExpression)
            {
                base.WriteBreak(hasExpression);
                return;
            }
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.Write("true");
        }

        public sealed override void WriteContinue(bool hasExpression)
        {
            if(hasExpression)
            {
                base.WriteContinue(hasExpression);
                return;
            }
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to."));
            Out.WriteOperator("<-");
            Out.Write("true");
        }
    }

    internal sealed class ForStatementFreeInterrupted : ForStatementTrailInterrupted
    {

    }

    internal sealed class ForStatementControl : ForStatementTrailInterrupted, IReturnableStatementContext
    {
        string? IReturnableStatementContext.ReturnVariable => ScopeReturnVariable;
        string? IReturnableStatementContext.ReturningVariable => ScopeReturningVariable;
    }
}
