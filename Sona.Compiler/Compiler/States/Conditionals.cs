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

        public override void EnterIgnoredStatements(IgnoredStatementsContext context)
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

        public override void ExitIgnoredStatements(IgnoredStatementsContext context)
        {
            OnExitStatements();
            scope?.WriteEnd();
            Out.WriteLine();
            ExitState()?.ExitIgnoredStatements(context);
        }

        public override void EnterFinalStatements(FinalStatementsContext context)
        {

        }

        public override void ExitFinalStatements(FinalStatementsContext context)
        {
            OnExitStatements();
            ExitState()?.ExitFinalStatements(context);
        }

        public override void EnterClosedFinalStatements(ClosedFinalStatementsContext context)
        {

        }

        public override void ExitClosedFinalStatements(ClosedFinalStatementsContext context)
        {
            OnExitStatements();
            ExitState()?.ExitClosedFinalStatements(context);
        }

        public override void EnterOpenFinalStatements(OpenFinalStatementsContext context)
        {

        }

        public override void ExitOpenFinalStatements(OpenFinalStatementsContext context)
        {
            OnExitStatements();
            ExitState()?.ExitOpenFinalStatements(context);
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

        public override void EnterValuelessBlock(ValuelessBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterValuelessBlock(context);
        }

        public override void ExitValuelessBlock(ValuelessBlockContext context)
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

        public override void EnterClosedBlock(ClosedBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterClosedBlock(context);
        }

        public override void ExitClosedBlock(ClosedBlockContext context)
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

        public override void EnterControlBlock(ControlBlockContext context)
        {
            OnEnterBlock();
            EnterState<BlockState>().EnterControlBlock(context);
        }

        public override void ExitControlBlock(ControlBlockContext context)
        {
            OnExitBlock();
        }

        public sealed override void EnterIgnoredStatements(IgnoredStatementsContext context)
        {
            Out.WriteLine();
            Out.ExitScope();
            Out.WriteLine(_end_);
            Out.Write("else ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterIgnoredStatements(context);
        }

        public sealed override void ExitIgnoredStatements(IgnoredStatementsContext context)
        {
            Out.WriteOperatorName("Unchecked");
            Out.WriteLine(".defaultof<_>");
        }

        public sealed override void EnterFinalStatements(FinalStatementsContext context)
        {
            Out.WriteLine();
            ReturnFromScope();
            Out.WriteLine(_begin_);
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterFinalStatements(context);
        }

        public sealed override void ExitFinalStatements(FinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }
    }

    internal class IfStatement : ControlStatement
    {
        public sealed override void EnterIfStatement(IfStatementContext context)
        {

        }

        public sealed override void EnterIfStatementReturningClosed(IfStatementReturningClosedContext context)
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

        public sealed override void ExitIfStatement(IfStatementContext context)
        {
            ExitState().ExitIfStatement(context);
        }

        public sealed override void ExitIfStatementReturningClosed(IfStatementReturningClosedContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
            ExitState().ExitIfStatementReturningClosed(context);
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
    }

    internal sealed class IfSimpleStatement : IfStatement
    {
        bool hasElse;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasElse = false;
        }

        public override void EnterIfStatementReturningClosedTrail(IfStatementReturningClosedTrailContext context)
        {

        }

        public override void ExitIfStatementReturningClosedTrail(IfStatementReturningClosedTrailContext context)
        {
            ExitState().ExitIfStatementReturningClosedTrail(context);
        }

        public override void EnterIfStatementReturningOpenSimple(IfStatementReturningOpenSimpleContext context)
        {

        }

        public override void ExitIfStatementReturningOpenSimple(IfStatementReturningOpenSimpleContext context)
        {
            ExitState().ExitIfStatementReturningOpenSimple(context);
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

        public override void EnterClosedFinalStatements(ClosedFinalStatementsContext context)
        {
            if(!hasElse)
            {
                Out.WriteLine();
                Out.Write("else ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterClosedFinalStatements(context);
        }

        public override void ExitClosedFinalStatements(ClosedFinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        public override void EnterOpenFinalStatements(OpenFinalStatementsContext context)
        {
            if(!hasElse)
            {
                Out.WriteLine();
                Out.Write("else ");
                Out.WriteLine(_begin_);
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterOpenFinalStatements(context);
        }

        public override void ExitOpenFinalStatements(OpenFinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }
    }

    internal sealed class IfControlStatement : IfStatement, IReturnScope
    {
        string? IReturnScope.ReturnVariable => ScopeReturnVariable;
        string? IReturnScope.SuccessVariable => ScopeSuccessVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }

        public override void EnterIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            EnterReturnScope();
        }

        public override void ExitIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            ExitReturnScope();
            ExitState().ExitIfStatementReturningOpenComplex(context);
        }
    }

    internal abstract class DoStatement : ControlStatement
    {
        IFunctionScope? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = FindScope<IFunctionScope>();
        }

        protected override void OnEnterBlock()
        {
            scope?.WriteBegin();
        }

        protected override void OnExitBlock()
        {
            scope?.WriteEnd();
        }
    }

    internal sealed class DoSimpleStatement : DoStatement
    {
        public override void EnterDoStatement(DoStatementContext context)
        {

        }

        public override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            Out.Write("if true then ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            Out.Write("if true then ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public override void ExitDoStatement(DoStatementContext context)
        {
            ExitState().ExitDoStatement(context);
        }

        public override void ExitDoStatementReturning(DoStatementReturningContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
            ExitState().ExitDoStatementReturning(context);
        }

        public override void ExitDoStatementTerminating(DoStatementTerminatingContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
            ExitState().ExitDoStatementTerminating(context);
        }
    }

    internal sealed class DoControlStatement : DoStatement, IReturnScope
    {
        string? IReturnScope.ReturnVariable => ScopeReturnVariable;
        string? IReturnScope.SuccessVariable => ScopeSuccessVariable;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }

        public override void EnterDoStatementConditionallyReturning(DoStatementConditionallyReturningContext context)
        {
            EnterReturnScope();
        }

        public override void ExitDoStatementConditionallyReturning(DoStatementConditionallyReturningContext context)
        {
            ExitReturnScope();
            ExitState().ExitDoStatementConditionallyReturning(context);
        }
    }
}
