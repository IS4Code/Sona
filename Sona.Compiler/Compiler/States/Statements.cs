﻿using System;
using System.IO;
using IS4.Sona.Compiler.Tools;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal class BlockState : ScriptState
    {
        public sealed override void EnterMultiFuncDecl(MultiFuncDeclContext context)
        {
            EnterState<MultiFunctionState>().EnterMultiFuncDecl(context);
        }

        public override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            Out.Write("()");
        }

        public override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            Out.WriteLine();
        }

        public sealed override void EnterReturnStatement(ReturnStatementContext context)
        {
            EnterState<ReturnState>().EnterReturnStatement(context);
        }

        public sealed override void EnterThrowStatement(ThrowStatementContext context)
        {
            EnterState<ThrowState>().EnterThrowStatement(context);
        }

        public sealed override void EnterVariableDecl(VariableDeclContext context)
        {
            EnterState<NewVariableState>().EnterVariableDecl(context);
        }

        public sealed override void EnterAssignmentOrCall(AssignmentOrCallContext context)
        {
            EnterState<AssignmentOrCallState>().EnterAssignmentOrCall(context);
        }

        public sealed override void EnterIfStatement(IfStatementContext context)
        {
            EnterState<IfStatement>().EnterIfStatement(context);
        }

        public sealed override void EnterIfStatementReturningClosed(IfStatementReturningClosedContext context)
        {
            EnterState<IfStatement>().EnterIfStatementReturningClosed(context);
        }

        public sealed override void EnterIfStatementReturningOpenSimple(IfStatementReturningOpenSimpleContext context)
        {
            EnterState<IfSimpleStatement>().EnterIfStatementReturningOpenSimple(context);
        }

        public sealed override void EnterIfStatementReturningOpenComplex(IfStatementReturningOpenComplexContext context)
        {
            EnterState<IfComplexStatement>().EnterIfStatementReturningOpenComplex(context);
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            EnterState<IfStatement>().EnterIfStatementTerminating(context);
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

        public override void EnterStatement(StatementContext context)
        {

        }

        public override void ExitStatement(StatementContext context)
        {
            Out.WriteLine();
        }

        public override void EnterReturningStatement(ReturningStatementContext context)
        {

        }

        public override void ExitReturningStatement(ReturningStatementContext context)
        {
            Out.WriteLine();
        }

        public override void EnterConditionallyReturningStatement(ConditionallyReturningStatementContext context)
        {

        }

        public override void ExitConditionallyReturningStatement(ConditionallyReturningStatementContext context)
        {
            Out.WriteLine();
        }

        public override void EnterTerminatingStatement(TerminatingStatementContext context)
        {

        }

        public override void ExitTerminatingStatement(TerminatingStatementContext context)
        {
            Out.WriteLine();
        }

        public sealed override void EnterTopLevelStatement(TopLevelStatementContext context)
        {

        }

        public sealed override void ExitTopLevelStatement(TopLevelStatementContext context)
        {
            Out.WriteLine();
        }

        public sealed override void ExitValuelessBlock(ValuelessBlockContext context)
        {
            ExitState()?.ExitValuelessBlock(context);
        }

        public sealed override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            ExitState()?.ExitTerminatingBlock(context);
        }

        public sealed override void ExitClosedBlock(ClosedBlockContext context)
        {
            ExitState()?.ExitClosedBlock(context);
        }

        public sealed override void ExitReturningBlock(ReturningBlockContext context)
        {
            ExitState()?.ExitReturningBlock(context);
        }

        public sealed override void ExitValueBlock(ValueBlockContext context)
        {
            ExitState()?.ExitValueBlock(context);
        }

        public sealed override void ExitOpenBlock(OpenBlockContext context)
        {
            ExitState()?.ExitOpenBlock(context);
        }

        public sealed override void ExitControlBlock(ControlBlockContext context)
        {
            ExitState()?.ExitControlBlock(context);
        }
    }

    internal sealed class ChunkState : BlockState
    {
        public ChunkState(ScriptEnvironment environment)
        {
            Initialize(environment, null);
        }

        public override void ExitMainBlock(MainBlockContext context)
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

        public override void EnterExprList(ExprListContext context)
        {
            HasExpression = true;
            EnterState<ExpressionListState>().EnterExprList(context);
        }
    }

    internal sealed class ReturnState : ArgumentStatementState
    {
        public override void EnterExprList(ExprListContext context)
        {
            Out.Write('(');
            base.EnterExprList(context);
        }

        public override void ExitReturnStatement(ReturnStatementContext context)
        {
            if(!HasExpression)
            {
                Out.Write("()");
            }
            else
            {
                Out.Write(")");
            }

            ExitState().ExitReturnStatement(context);
        }
    }

    internal sealed class ThrowState : ArgumentStatementState
    {
        public override void EnterExprList(ExprListContext context)
        {
            Out.Write('(');
            base.EnterExprList(context);
        }

        public override void ExitExprList(ExprListContext context)
        {

        }

        public override void EnterThrowStatement(ThrowStatementContext context)
        {

        }

        public override void ExitThrowStatement(ThrowStatementContext context)
        {
            if(!HasExpression)
            {
                Out.WriteOperatorName("reraise");
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

            Out.WriteSymbol(name);

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
