using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class MultiFunctionState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
        }

        public override void EnterFuncDecl(FuncDeclContext context)
        {
            if(first)
            {
                Out.Write("let rec ");
                first = false;
            }
            else
            {
                Out.WriteLine();
                Out.Write("and ");
            }
            EnterState<FunctionDeclState>().EnterFuncDecl(context);
        }

        public override void ExitMultiFuncDecl(MultiFuncDeclContext context)
        {
            ExitState().ExitMultiFuncDecl(context);
        }
    }

    internal sealed class FunctionDeclState : NodeState
    {
        public override void ExitFuncDecl(FuncDeclContext context)
        {
            ExitState().ExitFuncDecl(context);
        }

        public override void EnterParamList(ParamListContext context)
        {
            EnterState<ParamListState>().EnterParamList(context);
        }

        public override void EnterFuncBlock(FuncBlockContext context)
        {
            Out.WriteOperator("=");
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterFuncBlock(context);
        }

        public override void ExitFuncBlock(FuncBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
        }
    }

    internal sealed class ParamListState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = false;
        }

        public override void EnterParamTuple(ParamTupleContext context)
        {
            Out.Write('(');
            first = true;
        }

        public override void ExitParamTuple(ParamTupleContext context)
        {
            Out.Write(')');
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(',');
            }

            EnterState<DeclarationState>().EnterDeclaration(context);
        }

        public override void ExitParamList(ParamListContext context)
        {
            ExitState().ExitParamList(context);
        }
    }

    internal sealed class NewVariableState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = false;
        }

        public override void EnterVariableDecl(VariableDeclContext context)
        {
            first = true;
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(',');
            }

            EnterState<DeclarationState>().EnterDeclaration(context);
        }

        public override void EnterLetDecl(LetDeclContext context)
        {
            Out.Write("let ");
        }

        public override void EnterVarDecl(VarDeclContext context)
        {
            Out.Write("let mutable ");
        }

        public override void EnterExprList(ExprListContext context)
        {
            Out.WriteOperator("=");
            EnterState<ExpressionListState>().EnterExprList(context);
        }

        public override void ExitVariableDecl(VariableDeclContext context)
        {
            ExitState().ExitVariableDecl(context);
        }

        public override void ExitLocalAttribute(LocalAttributeContext context)
        {
            Out.WriteLine();
            base.ExitLocalAttribute(context);
        }
    }

    internal sealed class DeclarationState : NodeState
    {
        public override void ExitDeclaration(DeclarationContext context)
        {
            ExitState().ExitDeclaration(context);
        }
    }
}
