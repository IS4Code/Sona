using Antlr4.Runtime.Misc;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class TrailingStatements : BlockState
    {
        bool first;
        IFunctionScope? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            scope = null;
        }

        public override void EnterIgnoredTrail(IgnoredTrailContext context)
        {
            scope = FindScope<IFunctionScope>();

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
    }

    internal abstract class ControlStatement : NodeState
    {
#nullable disable
        protected string ReturnVariable { get; private set; }
        protected string SuccessVariable { get; private set; }
#nullable restore
        protected string? OriginalReturnVariable { get; private set; }
        protected string? OriginalSuccessVariable { get; private set; }

        // Nested returns will be stored here
        protected string? ScopeReturnVariable => ReturnVariable ?? OriginalReturnVariable;
        protected string? ScopeSuccessVariable => SuccessVariable ?? OriginalSuccessVariable;

        StatementFlags enterFlags;
        bool exited;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            enterFlags = StatementFlags.None;
            exited = false;
        }

        protected void InitializeReturnControl()
        {
            if(FindScope<IReturnScope>() is { } scope)
            {
                OriginalReturnVariable = scope.ReturnVariable;
                OriginalSuccessVariable = scope.SuccessVariable;
            }
        }

        protected void EnterReturnScope()
        {
            if(OriginalReturnVariable is null)
            {
                // Initialize variables for conditional return
                SuccessVariable = Out.CreateTemporaryIdentifier();
                ReturnVariable = Out.CreateTemporaryIdentifier();
                // var success = false
                Out.Write("let mutable ");
                Out.WriteIdentifier(SuccessVariable);
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

        protected void ReturnFromScope()
        {
            Out.Write("if ");
            Out.WriteIdentifier(ScopeSuccessVariable);
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
            // else ...
            Out.Write("else ");
            // Own declared variables no longer used
            ReturnVariable = null;
            SuccessVariable = null;
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

        protected virtual void OnEnter(StatementFlags flags)
        {

        }

        protected virtual void OnExit(StatementFlags flags)
        {

        }

        private void OnEnterInner(StatementFlags flags)
        {
            if(this is IReturnScope)
            {
                flags |= StatementFlags.OpenPath;
            }
            enterFlags = flags;
            OnEnter(flags);
        }

        private void OnExitInner(StatementFlags flags)
        {
            if(this is IReturnScope)
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

        const StatementFlags conditionalFlags = StatementFlags.OpenPath | StatementFlags.ReturnPath;

        protected override void OnEnter(StatementFlags flags)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                // Open and returning - prepare variables
                EnterReturnScope();
            }
        }

        protected override void OnExit(StatementFlags flags)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                // Open and returning - conditional return
                Out.WriteLine();
                ReturnFromScope();
            }
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

    internal sealed class IfStatementControl : IfStatement, IReturnScope
    {
        string? IReturnScope.ReturnVariable => ScopeReturnVariable;
        string? IReturnScope.SuccessVariable => ScopeSuccessVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }
    }

    internal abstract class DoStatement : ControlStatement
    {
        const StatementFlags conditionalFlags = StatementFlags.OpenPath | StatementFlags.ReturnPath;

        protected override void OnEnter(StatementFlags flags)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                // Open and returning - prepare variables
                EnterReturnScope();
            }
        }

        protected override void OnExit(StatementFlags flags)
        {
            if((flags & conditionalFlags) == conditionalFlags)
            {
                // Open and returning - conditional return
                Out.WriteLine();
                ReturnFromScope();
            }
        }

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

    internal sealed class DoStatementControl : DoStatement, IReturnScope
    {
        string? IReturnScope.ReturnVariable => ScopeReturnVariable;
        string? IReturnScope.SuccessVariable => ScopeSuccessVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }
    }
}
