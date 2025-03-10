using System.Collections.Generic;
using Antlr4.Runtime;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal abstract class ControlStatement : NodeState, IStatementContext
    {
        sealed class TrailingStatements : BlockState
        {
            bool first;
            IComputationContext? scope;
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
                scope = FindContext<IComputationContext>();

                Out.WriteCoreOperator("ignore");
                Out.Write(" ");
                scope?.WriteBeginBlockExpression();
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
                scope?.WriteEndBlockExpression();
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

            protected override void OnEnterStatement(StatementFlags flags, ParserRuleContext context)
            {
                if(!StatementsAllowed)
                {
                    Error("A statement is not allowed in this context.", context);
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

            protected override void OnExitStatement(StatementFlags flags, ParserRuleContext context)
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
        protected override bool IgnoreContext => exited;

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

        protected virtual void OnEnter(StatementFlags flags, ParserRuleContext context)
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

        protected virtual void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                Out.WriteLine();
                Out.Write("if ");
                Out.WriteIdentifier(ScopeReturningVariable ?? Error("Returning from a scope that does not support return.", context));
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
                    Out.WriteIdentifier(interruptScope.InterruptingVariable ?? Error("COMPILER ERROR: Interrupting variable not provided.", context));
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
                Out.WriteIdentifier(interruptScope.InterruptingVariable ?? Error("COMPILER ERROR: Interrupting variable not provided.", context));
                Out.WriteLine(" then ()");
                Out.Write("else ");
            }
        }

        protected virtual void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        protected virtual void OnExitBlock(StatementFlags flags, ParserRuleContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        #region Block types
        public sealed override void EnterValueBlock(ValueBlockContext context)
        {
            OnEnterBlock(StatementFlags.None, context);
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public sealed override void ExitValueBlock(ValueBlockContext context)
        {
            OnExitBlock(StatementFlags.None, context);
        }

        public sealed override void EnterFreeBlock(FreeBlockContext context)
        {
            OnEnterBlock(StatementFlags.OpenPath, context);
            EnterState<BlockState>().EnterFreeBlock(context);
        }

        public sealed override void ExitFreeBlock(FreeBlockContext context)
        {
            OnExitBlock(StatementFlags.OpenPath, context);
        }

        public sealed override void EnterOpenBlock(OpenBlockContext context)
        {
            OnEnterBlock(StatementFlags.OpenPath, context);
            EnterState<BlockState>().EnterOpenBlock(context);
        }

        public sealed override void ExitOpenBlock(OpenBlockContext context)
        {
            OnExitBlock(StatementFlags.OpenPath, context);
        }

        public sealed override void EnterReturningBlock(ReturningBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            EnterState<BlockState>().EnterReturningBlock(context);
        }

        public sealed override void ExitReturningBlock(ReturningBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterTerminatingBlock(TerminatingBlockContext context)
        {
            OnEnterBlock(StatementFlags.Terminating, context);
            EnterState<BlockState>().EnterTerminatingBlock(context);
        }

        public sealed override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            OnExitBlock(StatementFlags.Terminating, context);
        }

        public sealed override void EnterInterruptingBlock(InterruptingBlockContext context)
        {
            OnEnterBlock(StatementFlags.InterruptPath, context);
            EnterState<BlockState>().EnterInterruptingBlock(context);
        }

        public sealed override void ExitInterruptingBlock(InterruptingBlockContext context)
        {
            OnExitBlock(StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterClosingBlock(ClosingBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            EnterState<BlockState>().EnterClosingBlock(context);
        }

        public sealed override void ExitClosingBlock(ClosingBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterConditionalBlock(ConditionalBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            EnterState<BlockState>().EnterConditionalBlock(context);
        }

        public sealed override void ExitConditionalBlock(ConditionalBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void EnterInterruptibleBlock(InterruptibleBlockContext context)
        {
            OnEnterBlock(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            EnterState<BlockState>().EnterInterruptibleBlock(context);
        }

        public sealed override void ExitInterruptibleBlock(InterruptibleBlockContext context)
        {
            OnExitBlock(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void EnterFullBlock(FullBlockContext context)
        {
            OnEnterBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            EnterState<BlockState>().EnterFullBlock(context);
        }

        public sealed override void ExitFullBlock(FullBlockContext context)
        {
            OnExitBlock(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }
        #endregion

        public sealed override void EnterIgnoredTrail(IgnoredTrailContext context)
        {
            try
            {
                OnExitInner(enterFlags, context);
                if((enterFlags & StatementFlags.OpenPath) == 0)
                {
                    // `if true` was written previously
                    Out.WriteLine();
                    Out.Write("else ");
                }
            }
            finally
            {
                Out.WriteLine(_begin_);
                Out.EnterScope();
                EnterState<TrailingStatements>().EnterIgnoredTrail(context);
            }
        }

        public sealed override void ExitIgnoredTrail(IgnoredTrailContext context)
        {
            Out.WriteCoreOperator("Unchecked");
            Out.WriteLine(".defaultof<_>");
            Out.ExitScope();
            Out.Write(_end_);
        }

        public sealed override void EnterIgnoredEmptyTrail(IgnoredEmptyTrailContext context)
        {
            OnExitInner(enterFlags, context);
            if((enterFlags & StatementFlags.OpenPath) == 0)
            {
                // `if true` was written previously
                Out.WriteLine();
                Out.Write("else ");
            }
            Out.WriteCoreOperator("Unchecked");
            Out.Write(".defaultof<_>");
            // Possible to elide even further if empty trail is predicted
        }

        public sealed override void ExitIgnoredEmptyTrail(IgnoredEmptyTrailContext context)
        {

        }

        protected virtual void OnEnterTrail(StatementFlags flags, ParserRuleContext context)
        {
            OnExitInner(enterFlags, context);
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        protected virtual void OnExitTrail(StatementFlags flags, ParserRuleContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        #region Trail types
        public sealed override void EnterOpenTrail(OpenTrailContext context)
        {
            OnEnterTrail(StatementFlags.OpenPath, context);
            EnterState<TrailingStatements>().EnterOpenTrail(context);
        }

        public sealed override void ExitOpenTrail(OpenTrailContext context)
        {
            OnExitTrail(StatementFlags.OpenPath, context);
        }

        public sealed override void EnterReturningTrail(ReturningTrailContext context)
        {
            OnEnterTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            EnterState<TrailingStatements>().EnterReturningTrail(context);
        }

        public sealed override void ExitReturningTrail(ReturningTrailContext context)
        {
            OnExitTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterClosingTrail(ClosingTrailContext context)
        {
            OnEnterTrail(StatementFlags.Terminating, context);
            EnterState<TrailingStatements>().EnterClosingTrail(context);
        }

        public sealed override void ExitClosingTrail(ClosingTrailContext context)
        {
            OnExitTrail(StatementFlags.Terminating, context);
        }

        public sealed override void EnterTerminatingTrail(TerminatingTrailContext context)
        {
            OnEnterTrail(StatementFlags.Terminating, context);
            EnterState<TrailingStatements>().EnterTerminatingTrail(context);
        }

        public sealed override void ExitTerminatingTrail(TerminatingTrailContext context)
        {
            OnExitTrail(StatementFlags.Terminating, context);
        }

        public sealed override void EnterInterruptingTrail(InterruptingTrailContext context)
        {
            OnEnterTrail(StatementFlags.InterruptPath, context);
            EnterState<TrailingStatements>().EnterInterruptingTrail(context);
        }

        public sealed override void ExitInterruptingTrail(InterruptingTrailContext context)
        {
            OnExitTrail(StatementFlags.InterruptPath, context);
        }

        public sealed override void EnterConditionalTrail(ConditionalTrailContext context)
        {
            OnEnterTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            EnterState<TrailingStatements>().EnterConditionalTrail(context);
        }

        public sealed override void ExitConditionalTrail(ConditionalTrailContext context)
        {
            OnExitTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void EnterInterruptibleTrail(InterruptibleTrailContext context)
        {
            OnEnterTrail(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            EnterState<TrailingStatements>().EnterInterruptibleTrail(context);
        }

        public sealed override void ExitInterruptibleTrail(InterruptibleTrailContext context)
        {
            OnExitTrail(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void EnterFullTrail(FullTrailContext context)
        {
            OnEnterTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            EnterState<TrailingStatements>().EnterFullTrail(context);
        }

        public sealed override void ExitFullTrail(FullTrailContext context)
        {
            OnExitTrail(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }
        #endregion
        
        private void OnEnterInner(StatementFlags flags, ParserRuleContext context)
        {
            if(this is IReturnableStatementContext)
            {
                flags |= StatementFlags.OpenPath;
            }
            enterFlags = flags;
            OnEnter(flags, context);
        }

        private void OnExitInner(StatementFlags flags, ParserRuleContext context)
        {
            if(this is IReturnableStatementContext)
            {
                flags |= StatementFlags.OpenPath;
            }
            if(!exited)
            {
                exited = true;
                OnExit(flags, context);
            }
        }

        #region Statement types
        public sealed override void EnterIfStatementFree(IfStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public sealed override void ExitIfStatementFree(IfStatementFreeContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementFree(context);
            }
        }

        public sealed override void EnterIfStatementReturning(IfStatementReturningContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitIfStatementReturning(IfStatementReturningContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementReturning(context);
            }
        }

        public sealed override void EnterIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementReturningTrail(context);
            }
        }

        public sealed override void EnterIfStatementReturningTrailFromElse(IfStatementReturningTrailFromElseContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitIfStatementReturningTrailFromElse(IfStatementReturningTrailFromElseContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementReturningTrailFromElse(context);
            }
        }

        public sealed override void EnterIfStatementInterrupting(IfStatementInterruptingContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitIfStatementInterrupting(IfStatementInterruptingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementInterrupting(context);
            }
        }

        public sealed override void EnterIfStatementInterruptingTrail(IfStatementInterruptingTrailContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitIfStatementInterruptingTrail(IfStatementInterruptingTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementInterruptingTrail(context);
            }
        }

        public sealed override void EnterIfStatementInterruptible(IfStatementInterruptibleContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitIfStatementInterruptible(IfStatementInterruptibleContext context)
        {
            try
            {
                OnExitInner(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementInterruptible(context);
            }
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating, context);
        }

        public sealed override void ExitIfStatementTerminating(IfStatementTerminatingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.Terminating, context);
            }
            finally
            {
                ExitState().ExitIfStatementTerminating(context);
            }
        }

        public sealed override void EnterIfStatementConditional(IfStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitIfStatementConditional(IfStatementConditionalContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitIfStatementConditional(context);
            }
        }

        public sealed override void EnterDoStatementFree(DoStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public sealed override void ExitDoStatementFree(DoStatementFreeContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementFree(context);
            }
        }

        public sealed override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating, context);
        }

        public sealed override void ExitDoStatementTerminating(DoStatementTerminatingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementTerminating(context);
            }
        }

        public sealed override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitDoStatementReturning(DoStatementReturningContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementReturning(context);
            }
        }

        public sealed override void EnterDoStatementInterrupting(DoStatementInterruptingContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitDoStatementInterrupting(DoStatementInterruptingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementInterrupting(context);
            }
        }

        public sealed override void EnterDoStatementInterruptingTrail(DoStatementInterruptingTrailContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitDoStatementInterruptingTrail(DoStatementInterruptingTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementInterruptingTrail(context);
            }
        }

        public sealed override void EnterDoStatementInterruptible(DoStatementInterruptibleContext context)
        {
            OnEnterInner(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitDoStatementInterruptible(DoStatementInterruptibleContext context)
        {
            try
            {
                OnExitInner(StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementInterruptible(context);
            }
        }

        public sealed override void EnterDoStatementConditional(DoStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitDoStatementConditional(DoStatementConditionalContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitDoStatementConditional(context);
            }
        }

        public override void EnterWhileStatementFree(WhileStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public override void ExitWhileStatementFree(WhileStatementFreeContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitWhileStatementFree(context);
            }
        }

        public override void EnterWhileStatementFreeInterrupted(WhileStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public override void ExitWhileStatementFreeInterrupted(WhileStatementFreeInterruptedContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitWhileStatementFreeInterrupted(context);
            }
        }

        public override void EnterWhileStatementTerminating(WhileStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating, context);
        }

        public override void ExitWhileStatementTerminating(WhileStatementTerminatingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.Terminating, context);
            }
            finally
            {
                ExitState().ExitWhileStatementTerminating(context);
            }
        }

        public override void EnterWhileStatementReturningTrail(WhileStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public override void ExitWhileStatementReturningTrail(WhileStatementReturningTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitWhileStatementReturningTrail(context);
            }
        }

        public override void EnterWhileStatementConditional(WhileStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public override void ExitWhileStatementConditional(WhileStatementConditionalContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitWhileStatementConditional(context);
            }
        }

        public override void EnterRepeatStatementFree(RepeatStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public override void ExitRepeatStatementFree(RepeatStatementFreeContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitRepeatStatementFree(context);
            }
        }

        public override void EnterRepeatStatementFreeInterrupted(RepeatStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public override void ExitRepeatStatementFreeInterrupted(RepeatStatementFreeInterruptedContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitRepeatStatementFreeInterrupted(context);
            }
        }

        public override void EnterRepeatStatementTerminating(RepeatStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating, context);
        }

        public override void ExitRepeatStatementTerminating(RepeatStatementTerminatingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.Terminating, context);
            }
            finally
            {
                ExitState().ExitRepeatStatementTerminating(context);
            }
        }

        public override void EnterRepeatStatementReturningTrail(RepeatStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public override void ExitRepeatStatementReturningTrail(RepeatStatementReturningTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitRepeatStatementReturningTrail(context);
            }
        }

        public override void EnterRepeatStatementConditional(RepeatStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public override void ExitRepeatStatementConditional(RepeatStatementConditionalContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitRepeatStatementConditional(context);
            }
        }

        public override void EnterForStatementFree(ForStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public override void ExitForStatementFree(ForStatementFreeContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitForStatementFree(context);
            }
        }

        public override void EnterForStatementFreeInterrupted(ForStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public override void ExitForStatementFreeInterrupted(ForStatementFreeInterruptedContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitForStatementFreeInterrupted(context);
            }
        }

        public override void EnterForStatementReturningTrail(ForStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public override void ExitForStatementReturningTrail(ForStatementReturningTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitForStatementReturningTrail(context);
            }
        }

        public override void EnterForStatementConditional(ForStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public override void ExitForStatementConditional(ForStatementConditionalContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitForStatementConditional(context);
            }
        }

        public sealed override void EnterSwitchStatementFree(SwitchStatementFreeContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public sealed override void ExitSwitchStatementFree(SwitchStatementFreeContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementFree(context);
            }
        }

        public sealed override void EnterSwitchStatementFreeInterrupted(SwitchStatementFreeInterruptedContext context)
        {
            OnEnterInner(StatementFlags.OpenPath, context);
        }

        public sealed override void ExitSwitchStatementFreeInterrupted(SwitchStatementFreeInterruptedContext context)
        {
            try
            {
                OnExitInner(StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementFreeInterrupted(context);
            }
        }

        public sealed override void EnterSwitchStatementReturning(SwitchStatementReturningContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitSwitchStatementReturning(SwitchStatementReturningContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementReturning(context);
            }
        }

        public sealed override void EnterSwitchStatementReturningTrail(SwitchStatementReturningTrailContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
        }

        public sealed override void ExitSwitchStatementReturningTrail(SwitchStatementReturningTrailContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementReturningTrail(context);
            }
        }

        public sealed override void EnterSwitchStatementTerminating(SwitchStatementTerminatingContext context)
        {
            OnEnterInner(StatementFlags.Terminating, context);
        }

        public sealed override void ExitSwitchStatementTerminating(SwitchStatementTerminatingContext context)
        {
            try
            {
                OnExitInner(StatementFlags.Terminating, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementTerminating(context);
            }
        }

        public sealed override void EnterSwitchStatementTerminatingInterrupted(SwitchStatementTerminatingInterruptedContext context)
        {
            OnEnterInner(StatementFlags.Terminating, context);
        }

        public sealed override void ExitSwitchStatementTerminatingInterrupted(SwitchStatementTerminatingInterruptedContext context)
        {
            try
            {
                OnExitInner(StatementFlags.Terminating, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementTerminatingInterrupted(context);
            }
        }

        public sealed override void EnterSwitchStatementConditional(SwitchStatementConditionalContext context)
        {
            OnEnterInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
        }

        public sealed override void ExitSwitchStatementConditional(SwitchStatementConditionalContext context)
        {
            try
            {
                OnExitInner(StatementFlags.ReturnPath | StatementFlags.InterruptPath | StatementFlags.OpenPath, context);
            }
            finally
            {
                ExitState().ExitSwitchStatementConditional(context);
            }
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
        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
                return;
            }
            base.OnExit(flags, context);
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
        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) != 0)
            {
                Out.WriteLine(_begin_);
                Out.EnterScope();
                Out.Write("if true then ");
            }
            base.OnEnterBlock(flags, context);
        }

        protected override void OnExitBlock(StatementFlags flags, ParserRuleContext context)
        {
            base.OnExitBlock(flags, context);
            if((flags & StatementFlags.OpenPath) != 0)
            {
                Out.WriteLine();
            }
        }

        protected override void OnEnterTrail(StatementFlags flags, ParserRuleContext context)
        {
            if(!HasElse)
            {
                Out.WriteLine();
                Out.Write("else ");
                base.OnEnterTrail(flags, context);
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
        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            Out.Write("if true then ");
            base.OnEnterBlock(flags, context);
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

        protected sealed override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            base.OnEnterBlock(flags, context);
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

        public virtual void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                Error("`break` in a `while` statement does not take an expression.", context);
            }
            Error("COMPILER ERROR: `break` used in a wrong version of `while`.", context);
        }

        public virtual void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                Error("`break` in a `while` statement does not take an expression.", context);
            }
            Error("COMPILER ERROR: `continue` used in a wrong version of `while`.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

        }
    }

    /// <summary>
    /// <c>while</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class WhileStatementNoTrail : WhileStatement
    {
        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
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
            base.OnExit(flags, context);
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

        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            continuingVariable = Out.CreateTemporaryIdentifier();
            // var continuing = true
            Out.Write("let mutable ");
            Out.WriteIdentifier(continuingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("true");
            base.OnEnter(flags, context);
        }

        public sealed override void EnterWhile(WhileContext context)
        {
            Out.Write("while ");
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `while` used without a condition variable.", context));
            Out.WriteOperator("&&");
            Out.Write('(');
        }

        public sealed override void EnterWhileTrue(WhileTrueContext context)
        {
            Out.Write("while ");
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `while` used without a condition variable.", context));
            Out.Write(" do ");
        }

        public sealed override void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                base.WriteBreak(hasExpression, context);
                return;
            }
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
            Out.Write("true");
        }

        public sealed override void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                base.WriteContinue(hasExpression, context);
                return;
            }
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to.", context));
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
            Out.WriteIdentifier(ContinuingVariable ?? Error("COMPILER ERROR: `repeat` used without a condition variable.", context));
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

        protected sealed override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
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

            base.OnEnterBlock(flags, context);

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

        protected sealed override void OnExitBlock(StatementFlags flags, ParserRuleContext context)
        {
            // Exited in ExitUntil
        }

        public virtual void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                Error("`break` in a `repeat` statement does not take an expression.", context);
            }
            Error("COMPILER ERROR: `break` used in a wrong version of `repeat`.", context);
        }

        public virtual void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                Error("`break` in a `repeat` statement does not take an expression.", context);
            }
            Error("COMPILER ERROR: `continue` used in a wrong version of `repeat`.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

        }
    }

    /// <summary>
    /// <c>repeat</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class RepeatStatementNoTrail : RepeatStatement
    {
        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
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
            base.OnExit(flags, context);
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
            Out.WriteIdentifier(ContinuingVariable ?? Error("COMPILER ERROR: `repeat` used without a condition variable.", context));
            Out.Write(" then ");
        }

        public sealed override void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                base.WriteBreak(hasExpression, context);
                return;
            }
            Out.WriteIdentifier(ContinuingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
            Out.Write("true");
        }

        public sealed override void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                base.WriteContinue(hasExpression, context);
                return;
            }
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to.", context));
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
            Error("`for` statement cannot use `by` with a non-range sequence.", context);
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

        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            base.OnEnterBlock(flags, context);
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

        public virtual void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                Error("`break` in a `for` statement does not take an expression.", context);
            }
            Error("COMPILER ERROR: `break` used in a wrong version of `for`.", context);
        }

        public virtual void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                Error("`break` in a `for` statement does not take an expression.", context);
            }
            Error("COMPILER ERROR: `continue` used in a wrong version of `for`.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

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
        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
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
            base.OnExit(flags, context);
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
                (capture ?? ErrorCapture(captureError, context)).Play(Out);
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
                    Out.StopCapture(capture ?? ErrorCapture(captureError, context));
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
                    Out.StopCapture(leftCapture ?? ErrorCapture(captureError, context));
                }
            }

            public override void EnterPrimitiveExpr(PrimitiveExprContext context)
            {
                Out.StopCapture(rightCapture ?? ErrorCapture(captureError, context));
                (leftCapture ?? ErrorCapture(captureError, context)).Play(Out);
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
                (rightCapture ?? ErrorCapture(captureError, context)).Play(Out);
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

        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            continuingVariable = Out.CreateTemporaryIdentifier();
            // var continuing = true
            Out.Write("let mutable ");
            Out.WriteIdentifier(continuingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("true");
            base.OnEnter(flags, context);

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
            Out.StopCapture(declarationCapture ?? ErrorCapture(captureError, context));
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
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `for` used without a condition variable.", context));
            Out.WriteOperator("&&");
            Out.WriteIdentifier(enumeratorVariable ?? Error(enumeratorError, context));
            Out.Write(".MoveNext() do ");
        }

        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            base.OnEnterBlock(flags, context);

            Out.Write("let ");
            (declarationCapture ?? ErrorCapture(captureError, context)).Play(Out);
            Out.WriteOperator('=');
            Out.WriteIdentifier(enumeratorVariable ?? Error(enumeratorError, context));
            Out.WriteLine(".Current");
        }

        protected sealed override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            Out.ExitScope();
            Out.WriteLine();
            Out.Write("finally ");
            Out.WriteNamespacedName("Sona.Runtime.SequenceHelpers", "DisposeEnumerator");
            Out.Write('(');
            Out.WriteNamespacedName("Sona.Runtime.SequenceHelpers", "Marker");
            Out.Write(',');
            Out.WriteIdentifier(enumeratorVariable ?? Error(enumeratorError, context));
            Out.Write(")");
            base.OnExit(flags, context);
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

        public sealed override void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                base.WriteBreak(hasExpression, context);
                return;
            }
            Out.WriteIdentifier(continuingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `break` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
            Out.Write("true");
        }

        public sealed override void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(hasExpression)
            {
                base.WriteContinue(hasExpression, context);
                return;
            }
            Out.WriteIdentifier(InterruptingVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to.", context));
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

    internal abstract class SwitchStatementBase : ControlStatement
    {
        protected bool HasBranches { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            HasBranches = false;
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            if(!HasBranches)
            {
                Out.WriteLine();
                Out.Write("| _ when false");
                Out.WriteOperator("->");
                Out.WriteCoreOperator("Unchecked");
                Out.Write(".defaultof<_>");
            }
            base.OnExit(flags, context);
        }

        public abstract override void EnterSwitch(SwitchContext context);
        public abstract override void ExitSwitch(SwitchContext context);

        public sealed override void EnterCase(CaseContext context)
        {
            Out.WriteLine();
            Out.Write("| ");
        }

        public sealed override void ExitCase(CaseContext context)
        {
            Out.WriteOperator("->");
            HasBranches = true;
        }

        public sealed override void EnterEmptyPattern(EmptyPatternContext context)
        {
            Out.Write('_');
        }

        public sealed override void ExitEmptyPattern(EmptyPatternContext context)
        {

        }

        public sealed override void EnterWhenClause(WhenClauseContext context)
        {
            Out.Write(" when(");
        }

        public sealed override void ExitWhenClause(WhenClauseContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterElse(ElseContext context)
        {
            Out.WriteLine();
            Out.Write("| _");
        }

        public sealed override void ExitElse(ElseContext context)
        {
            Out.WriteOperator("->");
            HasBranches = true;
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {

        }
    }

    internal abstract class SwitchStatement : SwitchStatementBase
    {
        public sealed override void EnterSwitch(SwitchContext context)
        {
            Out.Write("match(");
        }

        public sealed override void ExitSwitch(SwitchContext context)
        {
            Out.Write(")with");
        }
    }

    internal abstract class SwitchStatementInterrupted : SwitchStatementBase, IInterruptibleStatementContext
    {
        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.CanContinue;

        string? matchingVariable, matchedVariable, interruptingVariable;

        string? IInterruptibleStatementContext.InterruptingVariable => interruptingVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            matchingVariable = null;
            matchedVariable = null;
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            Out.ExitScope();
            Out.WriteLine();
            Out.Write(_end_);
            base.OnExit(flags, context);
        }

        public sealed override void EnterSwitch(SwitchContext context)
        {
            matchedVariable = Out.CreateTemporaryIdentifier();
            matchingVariable = Out.CreateTemporaryIdentifier();

            // var matched = <expression>
            Out.Write("let mutable ");
            Out.WriteIdentifier(matchedVariable);
            Out.WriteOperator('=');
        }

        public sealed override void ExitSwitch(SwitchContext context)
        {
            Out.WriteLine();
            // var matched = <expression>
            var matchingVariable = this.matchingVariable ?? Error("COMPILER ERROR: Matching variable missing", context);
            Out.Write("let mutable ");
            Out.WriteIdentifier(matchingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("true");
            // while matching do
            Out.Write("while ");
            Out.WriteIdentifier(matchingVariable);
            Out.Write(" do ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
            // matching = false
            Out.WriteIdentifier(matchingVariable);
            Out.WriteOperator("<-");
            Out.WriteLine("false");
            // var interrupting = false
            interruptingVariable = Out.CreateTemporaryIdentifier();
            Out.Write("let mutable ");
            Out.WriteIdentifier(interruptingVariable);
            Out.WriteOperator('=');
            Out.WriteLine("false");
            Out.Write("match ");
            Out.WriteIdentifier(matchedVariable ?? Error("COMPILER ERROR: Matched variable missing", context));
            Out.Write(" with");
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Error("COMPILER ERROR: `break` used in `switch`.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            if(!hasExpression)
            {
                Error("`continue` in a `switch` statement must take an expression.", context);
            }
            Out.WriteIdentifier(matchedVariable ?? Error("COMPILER ERROR: `continue` used without a variable to assign to.", context));
            Out.WriteOperator("<-");
        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {
            Out.WriteLine();
            Out.WriteIdentifier(matchingVariable ?? Error("COMPILER ERROR: `continue` used without a matching variable.", context));
            Out.WriteOperator("<-");
            Out.WriteLine("true");
            Out.WriteIdentifier(interruptingVariable ?? Error("COMPILER ERROR: `continue` used without an interrupting variable.", context));
            Out.WriteOperator("<-");
            Out.Write("true");
        }
    }

    /// <summary>
    /// <c>switch</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class SwitchStatementNoTrail : SwitchStatement
    {
        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            base.OnExit(flags, context);
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
            }
        }
    }

    /// <summary>
    /// <c>switch</c> with no trailing statements (free or ignored).
    /// </summary>
    internal class SwitchStatementInterruptedNoTrail : SwitchStatementInterrupted
    {
        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Not open, there will be ignored statements
                Out.Write("if true then ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
                return;
            }
            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            base.OnExit(flags, context);
            if((flags & StatementFlags.OpenPath) == 0)
            {
                // Exit for ignored statements
                Out.ExitScope();
                Out.WriteLine();
                Out.Write(_end_);
            }
        }
    }

    internal sealed class SwitchStatementControl : SwitchStatementInterrupted, IReturnableStatementContext
    {
        string? IReturnableStatementContext.ReturnVariable => ScopeReturnVariable;
        string? IReturnableStatementContext.ReturningVariable => ScopeReturningVariable;
    }
}
