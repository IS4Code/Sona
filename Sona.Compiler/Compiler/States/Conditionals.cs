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

        public override void EnterIgnoredTrailingStatements(IgnoredTrailingStatementsContext context)
        {
            scope = FindScope<IFunctionScope>();

            Out.WriteOperatorName("ignore");
            Out.Write(" ");
            scope?.WriteBegin();
        }

        void OnExitStatements()
        {
            if(!first)
            {
                Out.WriteLine();
            }
        }

        public override void ExitIgnoredTrailingStatements(IgnoredTrailingStatementsContext context)
        {
            OnExitStatements();
            scope?.WriteEnd();
            Out.WriteLine();
            ExitState()?.ExitIgnoredTrailingStatements(context);
        }

        public override void EnterConditionalTrailingStatements(ConditionalTrailingStatementsContext context)
        {

        }

        public override void ExitConditionalTrailingStatements(ConditionalTrailingStatementsContext context)
        {
            OnExitStatements();
            ExitState()?.ExitConditionalTrailingStatements(context);
        }

        public override void EnterReturnSafeTrailingStatements(ReturnSafeTrailingStatementsContext context)
        {

        }

        public override void ExitReturnSafeTrailingStatements(ReturnSafeTrailingStatementsContext context)
        {
            OnExitStatements();
            ExitState()?.ExitReturnSafeTrailingStatements(context);
        }

        public override void EnterOpenTrailingStatements(OpenTrailingStatementsContext context)
        {

        }

        public override void ExitOpenTrailingStatements(OpenTrailingStatementsContext context)
        {
            OnExitStatements();
            ExitState()?.ExitOpenTrailingStatements(context);
        }

        void OnEnter()
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

        protected override void OnEnterStatement()
        {
            OnEnter();
        }

        protected override void OnExitStatement()
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

        protected void InitializeReturnControl()
        {
            if(FindScope<IReturnScope>() is { } scope)
            {
                OriginalReturnVariable = scope.ReturnVariable;
                OriginalSuccessVariable = scope.ReturnVariable;
            }
        }

        protected void EnterReturnScope()
        {
            if(OriginalReturnVariable is null)
            {
                // Initialize variables for conditional return
                SuccessVariable = Out.GetTemporarySymbol();
                ReturnVariable = Out.GetTemporarySymbol();
                // var success = false
                Out.Write("let mutable ");
                Out.WriteSymbol(SuccessVariable);
                Out.WriteOperator("=");
                Out.WriteLine("false");
                // var result = default
                Out.Write("let mutable ");
                Out.WriteSymbol(ReturnVariable);
                Out.WriteOperator("=");
                Out.WriteOperatorName("Unchecked");
                Out.WriteLine(".defaultof<_>");
            }
        }

        protected void ExitReturnScope()
        {

        }

        protected void ReturnFromScope()
        {
            Out.Write("if ");
            Out.WriteSymbol(SuccessVariable);
            Out.Write(" then ");
            if(OriginalReturnVariable is null)
            {
                // Nowhere to assign, return
                Out.WriteSymbol(ReturnVariable);
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

        protected virtual void OnEnterBlock()
        {
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        protected virtual void OnExitBlock()
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        public override void EnterFreeBlock(FreeBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterFreeBlock(context);
        }

        public override void ExitFreeBlock(FreeBlockContext context)
        {
            OnExitBlock();
        }

        public override void EnterReturningBlock(ReturningBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterReturningBlock(context);
        }

        public override void ExitReturningBlock(ReturningBlockContext context)
        {
            OnExitBlock();
        }

        public override void EnterReturnSafeBlock(ReturnSafeBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterReturnSafeBlock(context);
        }

        public override void ExitReturnSafeBlock(ReturnSafeBlockContext context)
        {
            OnExitBlock();
        }

        public override void EnterTerminatingBlock(TerminatingBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterTerminatingBlock(context);
        }

        public override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            OnExitBlock();
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public override void ExitValueBlock(ValueBlockContext context)
        {
            OnExitBlock();
        }

        public override void EnterOpenBlock(OpenBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterOpenBlock(context);
        }

        public override void ExitOpenBlock(OpenBlockContext context)
        {
            OnExitBlock();
        }

        public override void EnterConditionalBlock(ConditionalBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterConditionalBlock(context);
        }

        public override void ExitConditionalBlock(ConditionalBlockContext context)
        {
            OnExitBlock();
        }

        public sealed override void EnterIgnoredTrailingStatements(IgnoredTrailingStatementsContext context)
        {
            OnExitStatement();
            Out.WriteLine();
            Out.Write("else ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterIgnoredTrailingStatements(context);
        }

        public sealed override void ExitIgnoredTrailingStatements(IgnoredTrailingStatementsContext context)
        {
            Out.WriteOperatorName("Unchecked");
            Out.WriteLine(".defaultof<_>");
        }

        public sealed override void EnterConditionalTrailingStatements(ConditionalTrailingStatementsContext context)
        {
            OnExitStatement();
            Out.WriteLine();
            ReturnFromScope();
            Out.WriteLine(_begin_);
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterConditionalTrailingStatements(context);
        }

        public sealed override void ExitConditionalTrailingStatements(ConditionalTrailingStatementsContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        protected virtual void OnEnterStatement()
        {

        }

        protected virtual void OnExitStatement()
        {

        }
    }

    internal class IfStatementFree : ControlStatement
    {
        public sealed override void EnterIfStatementFree(IfStatementFreeContext context)
        {

        }

        public sealed override void EnterIfStatementReturning(IfStatementReturningContext context)
        {
            Out.Write("if true then ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            Out.Write("if true then ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public sealed override void ExitIfStatementFree(IfStatementFreeContext context)
        {
            ExitState().ExitIfStatementFree(context);
        }

        public sealed override void ExitIfStatementReturning(IfStatementReturningContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
            ExitState().ExitIfStatementReturning(context);
        }

        public sealed override void ExitIfStatementTerminating(IfStatementTerminatingContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
            ExitState().ExitIfStatementTerminating(context);
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

        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            Out.Write(")then ");
        }

        protected override void OnExitStatement()
        {
            Out.WriteLine();
            Out.ExitScope();
            Out.Write(_end_);
        }
    }

    internal sealed class IfSimpleStatement : IfStatementFree
    {
        bool hasElse;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasElse = false;
        }

        public override void EnterIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {

        }

        public override void ExitIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            ExitState().ExitIfStatementReturningTrail(context);
        }

        public override void EnterIfStatementConditionalSimple(IfStatementConditionalSimpleContext context)
        {

        }

        public override void ExitIfStatementConditionalSimple(IfStatementConditionalSimpleContext context)
        {
            ExitState().ExitIfStatementConditionalSimple(context);
        }

        public override void EnterOpenBlock(OpenBlockContext context)
        {
            hasElse = true;
            Out.WriteLine(_begin_);
            Out.EnterScope();
            Out.Write("do ");
            base.EnterOpenBlock(context);
        }

        public override void ExitOpenBlock(OpenBlockContext context)
        {
            base.ExitOpenBlock(context);
            Out.WriteLine();
        }

        public override void EnterReturnSafeTrailingStatements(ReturnSafeTrailingStatementsContext context)
        {
            if(!hasElse)
            {
                Out.WriteLine();
                Out.Write("else ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterReturnSafeTrailingStatements(context);
        }

        public override void ExitReturnSafeTrailingStatements(ReturnSafeTrailingStatementsContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        public override void EnterOpenTrailingStatements(OpenTrailingStatementsContext context)
        {
            if(!hasElse)
            {
                Out.WriteLine();
                Out.Write("else ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterOpenTrailingStatements(context);
        }

        public override void ExitOpenTrailingStatements(OpenTrailingStatementsContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }
    }

    internal sealed class IfControlStatement : IfStatementFree, IReturnScope
    {
        string? IReturnScope.ReturnVariable => ScopeReturnVariable;
        string? IReturnScope.SuccessVariable => ScopeSuccessVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }

        public override void EnterIfStatementConditionalComplex(IfStatementConditionalComplexContext context)
        {
            EnterReturnScope();
        }

        public override void ExitIfStatementConditionalComplex(IfStatementConditionalComplexContext context)
        {
            ExitReturnScope();
            ExitState().ExitIfStatementConditionalComplex(context);
        }

        protected override void OnExitStatement()
        {

        }
    }

    internal abstract class DoStatementFree : ControlStatement
    {
        protected sealed override void OnEnterBlock()
        {

        }

        protected sealed override void OnExitBlock()
        {

        }

        protected sealed override void OnEnterStatement()
        {
            Out.Write("if true then ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        protected sealed override void OnExitStatement()
        {
            Out.ExitScope();
            Out.Write(_end_);
        }
    }

    internal sealed class DoSimpleStatement : DoStatementFree
    {
        public override void EnterDoStatementFree(DoStatementFreeContext context)
        {
            OnEnterStatement();
        }

        public override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            OnEnterStatement();
        }

        public override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            OnEnterStatement();
        }

        public override void ExitDoStatementFree(DoStatementFreeContext context)
        {
            OnExitStatement();
            ExitState().ExitDoStatementFree(context);
        }

        public override void ExitDoStatementReturning(DoStatementReturningContext context)
        {
            OnExitStatement();
            ExitState().ExitDoStatementReturning(context);
        }

        public override void ExitDoStatementTerminating(DoStatementTerminatingContext context)
        {
            OnExitStatement();
            ExitState().ExitDoStatementTerminating(context);
        }
    }

    internal sealed class DoControlStatement : DoStatementFree, IReturnScope
    {
        string? IReturnScope.ReturnVariable => ScopeReturnVariable;
        string? IReturnScope.SuccessVariable => ScopeSuccessVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }

        public override void EnterDoStatementConditional(DoStatementConditionalContext context)
        {
            EnterReturnScope();
            OnEnterStatement();
        }

        public override void ExitDoStatementConditional(DoStatementConditionalContext context)
        {
            ExitReturnScope();
            ExitState().ExitDoStatementConditional(context);
        }
    }
}
