using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class TrailingStatements : BlockState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
        }

        public override void EnterIgnoredStatements(IgnoredStatementsContext context)
        {
            Out.WriteOperatorName("ignore");
            Out.WriteLine(" begin");
            Out.EnterScope();
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
            Out.ExitScope();
            Out.WriteLine("end");
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
#nullable restore
        protected string? OriginalReturnVariable { get; private set; }

        // Nested returns will be stored here
        protected string? ScopeVariableName => ReturnVariable ?? OriginalReturnVariable;

        protected void InitializeReturnControl()
        {
            OriginalReturnVariable = FindScope<IReturnScope>()?.VariableName;
        }

        protected void EnterReturnScope()
        {
            if(OriginalReturnVariable is null)
            {
                Out.WriteLine("begin");
                Out.EnterScope();
                ReturnVariable = Out.GetTemporarySymbol();
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
            if(OriginalReturnVariable is null)
            {
                Out.WriteLine();
                Out.ExitScope();
                Out.Write("end");
            }
        }

        protected void ReturnFromScope()
        {
            if(OriginalReturnVariable is null)
            {
                Out.WriteSymbol(ReturnVariable);
            }
            else
            {
                // The result is control
                Out.Write("true");
            }
            // Own declared return no longer used
            ReturnVariable = null;
        }

        void OnEnterBlock()
        {
            Out.WriteLine("begin");
            Out.EnterScope();
        }

        void OnExitBlock()
        {
            Out.ExitScope();
            Out.Write("end");
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

        public override void EnterIgnoredStatements(IgnoredStatementsContext context)
        {
            EnterState<TrailingStatements>().EnterIgnoredStatements(context);
        }

        public override void ExitIgnoredStatements(IgnoredStatementsContext context)
        {
            Out.WriteOperatorName("Unchecked");
            Out.WriteLine(".defaultof<_>");
        }
    }

    internal class IfStatement : ControlStatement
    {
        public sealed override void EnterIfStatement(IfStatementContext context)
        {

        }

        public sealed override void EnterIfStatementReturningClosed(IfStatementReturningClosedContext context)
        {
            Out.WriteLine("if true then begin");
            Out.EnterScope();
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            Out.WriteLine("if true then begin");
            Out.EnterScope();
        }

        public sealed override void ExitIfStatement(IfStatementContext context)
        {
            ExitState().ExitIfStatement(context);
        }

        public sealed override void ExitIfStatementReturningClosed(IfStatementReturningClosedContext context)
        {
            Out.ExitScope();
            Out.Write("end");
            ExitState().ExitIfStatementReturningClosed(context);
        }

        public sealed override void ExitIfStatementTerminating(IfStatementTerminatingContext context)
        {
            Out.ExitScope();
            Out.Write("end");
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

        public sealed override void EnterIgnoredStatements(IgnoredStatementsContext context)
        {
            Out.WriteLine();
            Out.ExitScope();
            Out.WriteLine("end");
            Out.WriteLine("else begin");
            Out.EnterScope();
            base.EnterIgnoredStatements(context);
        }

        public sealed override void ExitIgnoredStatements(IgnoredStatementsContext context)
        {
            base.ExitIgnoredStatements(context);
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
            Out.WriteLine("begin");
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
                Out.WriteLine("else begin");
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterClosedFinalStatements(context);
        }

        public override void ExitClosedFinalStatements(ClosedFinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }

        public override void EnterOpenFinalStatements(OpenFinalStatementsContext context)
        {
            if(!hasElse)
            {
                Out.WriteLine();
                Out.WriteLine("else begin");
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterOpenFinalStatements(context);
        }

        public override void ExitOpenFinalStatements(OpenFinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }
    }

    internal sealed class IfComplexStatement : IfStatement, IReturnScope
    {
        string? IReturnScope.VariableName => ScopeVariableName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }

        public override void EnterIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            EnterReturnScope();
            Out.WriteLine("if begin");
            Out.EnterScope();
        }

        public override void ExitIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            ExitReturnScope();
            ExitState().ExitIfStatementReturningOpenComplex(context);
        }

        public override void EnterFinalStatements(FinalStatementsContext context)
        {
            // end then result
            Out.WriteLine();
            Out.ExitScope();
            Out.Write("end then ");
            ReturnFromScope();
            Out.WriteLine();
            // else ...
            Out.WriteLine("else begin");
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterFinalStatements(context);
        }

        public override void ExitFinalStatements(FinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }
    }

    internal sealed class DoStatement : ControlStatement
    {
        public override void EnterDoStatement(DoStatementContext context)
        {

        }

        public override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            Out.WriteLine("if true then begin");
            Out.EnterScope();
        }

        public override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            Out.WriteLine("if true then begin");
            Out.EnterScope();
        }

        public override void ExitDoStatement(DoStatementContext context)
        {
            ExitState().ExitDoStatement(context);
        }

        public override void ExitDoStatementReturning(DoStatementReturningContext context)
        {
            Out.ExitScope();
            Out.Write("end");
            ExitState().ExitDoStatementReturning(context);
        }

        public override void ExitDoStatementTerminating(DoStatementTerminatingContext context)
        {
            Out.ExitScope();
            Out.Write("end");
            ExitState().ExitDoStatementTerminating(context);
        }

        public override void EnterIgnoredStatements(IgnoredStatementsContext context)
        {
            Out.WriteLine();
            Out.ExitScope();
            Out.WriteLine("end");
            Out.WriteLine("else begin");
            Out.EnterScope();
            base.EnterIgnoredStatements(context);
        }

        public override void ExitIgnoredStatements(IgnoredStatementsContext context)
        {
            base.ExitIgnoredStatements(context);
        }
    }

    internal sealed class DoControlStatement : ControlStatement, IReturnScope
    {
        string? IReturnScope.VariableName => ScopeVariableName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            InitializeReturnControl();
        }

        public override void EnterDoStatementConditionallyReturning(DoStatementConditionallyReturningContext context)
        {
            EnterReturnScope();
            Out.Write("if ");
        }

        public override void ExitDoStatementConditionallyReturning(DoStatementConditionallyReturningContext context)
        {
            ExitReturnScope();
            ExitState().ExitDoStatementConditionallyReturning(context);
        }

        public override void EnterFinalStatements(FinalStatementsContext context)
        {
            // end then result
            Out.Write(" then ");
            ReturnFromScope();
            Out.WriteLine();
            // else ...
            Out.WriteLine("else begin");
            Out.EnterScope();
            EnterState<TrailingStatements>().EnterFinalStatements(context);
        }

        public override void ExitFinalStatements(FinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }
    }
}
