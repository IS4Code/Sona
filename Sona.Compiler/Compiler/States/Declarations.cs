﻿using static IS4.Sona.Grammar.SonaParser;

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

    internal sealed class FunctionDeclState : NodeState, IReturnScope, IInterruptibleScope, IFunctionScope
    {
        // Establish a scope to return from
        string? IReturnScope.ReturnVariable => null;
        string? IReturnScope.ReturningVariable => null;

        InterruptFlags IInterruptibleScope.Flags => InterruptFlags.None;

        string? IInterruptibleScope.InterruptingVariable => null;

        public override void ExitFuncDecl(FuncDeclContext context)
        {
            ExitState().ExitFuncDecl(context);
        }

        public override void EnterParamList(ParamListContext context)
        {
            EnterState<ParamListState>().EnterParamList(context);
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            Out.WriteOperator('=');
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitScope();
            Out.Write("end");
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

        void IInterruptibleScope.WriteBreak(bool hasExpression)
        {
            Error("`break` must be used in a statement that supports it.");
        }

        void IInterruptibleScope.WriteContinue(bool hasExpression)
        {
            Error("`continue` must be used in a statement that supports it.");
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

    internal sealed class NewVariableState : NodeState, IExecutionScope
    {
        bool first;
        bool isliteral;

        bool IExecutionScope.IsLiteral => isliteral || (FindScope<IExecutionScope>()?.IsLiteral ?? false);
        bool IExecutionScope.IsInline => false;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = false;
            isliteral = false;
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

        public override void EnterConstDecl(ConstDeclContext context)
        {
            Out.Write("[<");
            Out.WriteNamespacedName("Microsoft.FSharp.Core", "LiteralAttribute");
            Out.WriteLine(">]");
            Out.Write("let ");
            isliteral = true;
        }

        public override void EnterExprList(ExprListContext context)
        {
            Out.WriteOperator('=');
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
