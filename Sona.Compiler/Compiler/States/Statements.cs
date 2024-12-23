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

        protected virtual void OnEnterStatement()
        {

        }

        protected virtual void OnExitStatement()
        {
            Out.WriteLine();
        }

        public sealed override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnEnterStatement();
            Out.Write("()");
        }

        public sealed override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            OnExitStatement();
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

        public sealed override void EnterIfStatementFree(IfStatementFreeContext context)
        {
            EnterState<IfStatementFree>().EnterIfStatementFree(context);
        }

        public sealed override void EnterIfStatementReturning(IfStatementReturningContext context)
        {
            EnterState<IfStatementFree>().EnterIfStatementReturning(context);
        }

        public sealed override void EnterIfStatementReturningTrail(IfStatementReturningTrailContext context)
        {
            EnterState<IfSimpleStatement>().EnterIfStatementReturningTrail(context);
        }

        public sealed override void EnterIfStatementConditionalSimple(IfStatementConditionalSimpleContext context)
        {
            EnterState<IfSimpleStatement>().EnterIfStatementConditionalSimple(context);
        }

        public sealed override void EnterIfStatementConditionalComplex(IfStatementConditionalComplexContext context)
        {
            EnterState<IfControlStatement>().EnterIfStatementConditionalComplex(context);
        }

        public sealed override void EnterIfStatementTerminating(IfStatementTerminatingContext context)
        {
            EnterState<IfStatementFree>().EnterIfStatementTerminating(context);
        }

        public override void EnterDoStatementFree(DoStatementFreeContext context)
        {
            EnterState<DoSimpleStatement>().EnterDoStatementFree(context);
        }

        public override void EnterDoStatementReturning(DoStatementReturningContext context)
        {
            EnterState<DoSimpleStatement>().EnterDoStatementReturning(context);
        }

        public override void EnterDoStatementConditional(DoStatementConditionalContext context)
        {
            EnterState<DoControlStatement>().EnterDoStatementConditional(context);
        }

        public override void EnterDoStatementTerminating(DoStatementTerminatingContext context)
        {
            EnterState<DoSimpleStatement>().EnterDoStatementTerminating(context);
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
            OnEnterStatement();
        }

        public sealed override void ExitStatement(StatementContext context)
        {
            OnExitStatement();
        }

        public sealed override void EnterReturningStatement(ReturningStatementContext context)
        {
            OnEnterStatement();
        }

        public sealed override void ExitReturningStatement(ReturningStatementContext context)
        {
            OnExitStatement();
        }

        public sealed override void EnterConditionalStatement(ConditionalStatementContext context)
        {
            OnEnterStatement();
        }

        public sealed override void ExitConditionalStatement(ConditionalStatementContext context)
        {
            OnExitStatement();
        }

        public sealed override void EnterTerminatingStatement(TerminatingStatementContext context)
        {
            OnEnterStatement();
        }

        public sealed override void ExitTerminatingStatement(TerminatingStatementContext context)
        {
            OnExitStatement();
        }

        public sealed override void EnterTopLevelStatement(TopLevelStatementContext context)
        {
            OnEnterStatement();
        }

        public sealed override void ExitTopLevelStatement(TopLevelStatementContext context)
        {
            OnExitStatement();
        }

        public sealed override void ExitFreeBlock(FreeBlockContext context)
        {
            ExitState()?.ExitFreeBlock(context);
        }

        public sealed override void ExitTerminatingBlock(TerminatingBlockContext context)
        {
            ExitState()?.ExitTerminatingBlock(context);
        }

        public sealed override void ExitReturnSafeBlock(ReturnSafeBlockContext context)
        {
            ExitState()?.ExitReturnSafeBlock(context);
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

        public sealed override void ExitConditionalBlock(ConditionalBlockContext context)
        {
            ExitState()?.ExitConditionalBlock(context);
        }
    }

    internal sealed class ChunkState : BlockState, IReturnScope, IFunctionScope, IExecutionScope
    {
        // Main block return is currently ignored
        string? IReturnScope.ReturnVariable => null;
        string? IReturnScope.SuccessVariable => null;
        bool IExecutionScope.IsLiteral => false;
        bool IExecutionScope.IsInline => false;

        public ChunkState(ScriptEnvironment environment)
        {
            Initialize(environment, null);
        }

        public override void ExitMainBlock(MainBlockContext context)
        {

        }

        void IFunctionScope.WriteBegin()
        {
            Out.WriteLine("begin");
            Out.EnterScope();
        }

        void IFunctionScope.WriteEnd()
        {
            Out.ExitScope();
            Out.Write("end");
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
        IReturnScope? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = FindScope<IReturnScope>();
        }

        public override void EnterExprList(ExprListContext context)
        {
            if(scope?.ReturnVariable is { } result)
            {
                // Store result in variable
                Out.WriteSymbol(result);
                Out.WriteOperator("<-");
            }
            Out.Write('(');
            base.EnterExprList(context);
        }

        public override void ExitReturnStatement(ReturnStatementContext context)
        {
            if(!HasExpression)
            {
                if(scope?.ReturnVariable is { } result)
                {
                    Out.WriteSymbol(result);
                    Out.WriteOperator("<-");
                }
                Out.Write("()");
            }
            else
            {
                Out.Write(")");
            }

            if(scope?.SuccessVariable is { } success)
            {
                // Value returned
                Out.WriteLine();
                Out.WriteSymbol(success);
                Out.WriteOperator("<-");
                Out.Write("true");
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
