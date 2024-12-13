using System;
using System.IO;
using IS4.Sona.Compiler.Tools;
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

        public override void ExitIgnoredStatements(IgnoredStatementsContext context)
        {
            if(!first)
            {
                Out.WriteLine();
            }
            Out.ExitScope();
            Out.WriteLine("end");
            ExitState()?.ExitIgnoredStatements(context);
        }

        public override void EnterFinalStatements(FinalStatementsContext context)
        {

        }

        public override void ExitFinalStatements(FinalStatementsContext context)
        {
            if(!first)
            {
                Out.WriteLine();
            }
            ExitState()?.ExitFinalStatements(context);
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

        public override void EnterStatement(StatementContext context)
        {
            OnEnter();
        }

        public override void ExitStatement(StatementContext context)
        {

        }

        public override void EnterReturningStatement(ReturningStatementContext context)
        {
            OnEnter();
        }

        public override void ExitReturningStatement(ReturningStatementContext context)
        {

        }

        public override void EnterConditionallyReturningStatement(ConditionallyReturningStatementContext context)
        {
            OnEnter();
        }

        public override void ExitConditionallyReturningStatement(ConditionallyReturningStatementContext context)
        {

        }

        public override void EnterTerminatingStatement(TerminatingStatementContext context)
        {
            OnEnter();
        }

        public override void ExitTerminatingStatement(TerminatingStatementContext context)
        {

        }

        public override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnEnter();
            Out.Write("()");
        }

        public override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
        {

        }
    }

    internal abstract class ControlStatement : NodeState
    {
        public override void EnterValuelessBlock(ValuelessBlockContext context)
        {
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterValuelessBlock(context);
        }

        public override void ExitValuelessBlock(ValuelessBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }

        public override void EnterReturningBlock(ReturningBlockContext context)
        {
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterReturningBlock(context);
        }

        public override void ExitReturningBlock(ReturningBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }

        public override void EnterClosedBlock(ClosedBlockContext context)
        {
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterClosedBlock(context);
        }

        public override void ExitClosedBlock(ClosedBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }

        public override void EnterTerminatingBlock(TerminatingBlockContext context)
        {
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterTerminatingBlock(context);
        }

        public override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }

        public override void EnterOpenBlock(OpenBlockContext context)
        {
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterOpenBlock(context);
        }

        public override void ExitOpenBlock(OpenBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
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

    internal class IfSimpleStatement : IfStatement
    {
        bool hasElse;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasElse = false;
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

        public override void EnterFinalStatements(FinalStatementsContext context)
        {
            if(!hasElse)
            {
                Out.WriteLine();
                Out.WriteLine("else begin");
                Out.EnterScope();
            }
            EnterState<TrailingStatements>().EnterFinalStatements(context);
        }

        public override void ExitFinalStatements(FinalStatementsContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }
    }

    internal class IfComplexStatement : IfStatement
    {
#nullable disable
        string successVariable, returnVariable;
#nullable restore

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            successVariable = null;
            returnVariable = null;
        }

        public override void EnterIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            successVariable = Out.GetTemporarySymbol();
            returnVariable = Out.GetTemporarySymbol();
            Out.WriteLine("begin");
            Out.EnterScope();
            // let success = false
            Out.Write("let mutable ");
            Out.WriteSymbol(successVariable);
            Out.WriteOperator("=");
            Out.WriteLine("false");
            // let result = default
            Out.Write("let mutable ");
            Out.WriteSymbol(returnVariable);
            Out.WriteOperator("=");
            Out.WriteOperatorName("Unchecked");
            Out.WriteLine(".defaultof<_>");
        }

        public override void ExitIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            Out.WriteLine();
            Out.ExitScope();
            Out.Write("end");
            ExitState().ExitIfStatementReturningOpenComplex(context);
        }

        void OnEnterValueBranch()
        {
            Out.Write("begin");
            Out.WriteLine();
            Out.EnterScope();
            // success = true
            Out.WriteSymbol(successVariable);
            Out.WriteOperator("<-");
            Out.WriteLine("true");
            // result = ...
            Out.WriteSymbol(returnVariable);
            Out.WriteOperator("<-");
        }

        void OnExitValueBranch()
        {
            Out.WriteLine();
            Out.ExitScope();
            Out.WriteLine("end");
        }

        public override void EnterReturningBlock(ReturningBlockContext context)
        {
            OnEnterValueBranch();
            base.EnterReturningBlock(context);
        }

        public override void ExitReturningBlock(ReturningBlockContext context)
        {
            base.ExitReturningBlock(context);
            OnExitValueBranch();
        }

        public override void EnterClosedBlock(ClosedBlockContext context)
        {
            OnEnterValueBranch();
            base.EnterClosedBlock(context);
        }

        public override void ExitClosedBlock(ClosedBlockContext context)
        {
            base.ExitClosedBlock(context);
            OnExitValueBranch();
        }

        public override void EnterOpenBlock(OpenBlockContext context)
        {
            base.EnterOpenBlock(context);
        }

        public override void ExitOpenBlock(OpenBlockContext context)
        {
            base.ExitOpenBlock(context);
        }

        public override void EnterFinalStatements(FinalStatementsContext context)
        {
            // if success then result
            Out.Write("if ");
            Out.WriteSymbol(successVariable);
            Out.Write(" then ");
            Out.WriteSymbol(returnVariable);
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
