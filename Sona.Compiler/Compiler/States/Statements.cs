using System;
using System.IO;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Tokenization;
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

        public sealed override void EnterImplicitReturnStatement(ImplicitReturnStatementContext context)
        {
            Out.Write("()");
        }

        public sealed override void ExitImplicitReturnStatement(ImplicitReturnStatementContext context)
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

        public override void EnterImportStatement(ImportStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterImportStatement(context);
        }

        public override void EnterImportTypeStatement(ImportTypeStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterImportTypeStatement(context);
        }

        public override void EnterImportFileStatement(ImportFileStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterImportFileStatement(context);
        }

        public override void EnterIncludeStatement(IncludeStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterIncludeStatement(context);
        }

        public override void EnterRequireStatement(RequireStatementContext context)
        {
            EnterState<TopLevelStatement>().EnterRequireStatement(context);
        }

        public sealed override void ExitStatement(StatementContext context)
        {
            Out.WriteLine();
        }

        public sealed override void ExitFuncFinalStatement(FuncFinalStatementContext context)
        {
            Out.WriteLine();
        }

        public sealed override void ExitBlockFinalStatement(BlockFinalStatementContext context)
        {
            Out.WriteLine();
        }

        public sealed override void ExitTopLevelStatement(TopLevelStatementContext context)
        {
            Out.WriteLine();
        }

        public override void ExitSubBlock(SubBlockContext context)
        {
            ExitState()?.ExitSubBlock(context);
        }

        public override void ExitFuncBlock(FuncBlockContext context)
        {
            ExitState()?.ExitFuncBlock(context);
        }

        public override void ExitMainBlock(MainBlockContext context)
        {
            ExitState()?.ExitMainBlock(context);
        }
    }

    internal sealed class ChunkState : BlockState
    {
        public ChunkState(ScriptEnvironment environment)
        {
            Initialize(environment, null);
        }

        public override void ExitSubBlock(SubBlockContext context)
        {

        }

        public override void ExitMainBlock(MainBlockContext context)
        {

        }

        public override void ExitFuncBlock(FuncBlockContext context)
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
        public override void ExitReturnStatement(ReturnStatementContext context)
        {
            if(!HasExpression)
            {
                Out.Write("()");
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

        public override void EnterThrowStatement(ThrowStatementContext context)
        {
            Out.Write("do ");
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
