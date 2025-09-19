using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class MultiFunctionState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
        }

        private void OnEnter(ParserRuleContext context)
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
        }

        public override void EnterFuncDecl(FuncDeclContext context)
        {
            OnEnter(context);
            EnterState<FunctionDeclState>().EnterFuncDecl(context);
        }

        public override void ExitFuncDecl(FuncDeclContext context)
        {

        }

        public override void EnterCaseFuncDecl(CaseFuncDeclContext context)
        {
            OnEnter(context);
            EnterState<CaseFunctionDeclState>().EnterCaseFuncDecl(context);
        }

        public override void ExitCaseFuncDecl(CaseFuncDeclContext context)
        {

        }

        public override void EnterMultiFuncDecl(MultiFuncDeclContext context)
        {

        }

        public override void ExitMultiFuncDecl(MultiFuncDeclContext context)
        {
            ExitState().ExitMultiFuncDecl(context);
        }
    }

    internal abstract class FunctionBaseState : NodeState, IFunctionContext
    {
        // Establish a scope to return from
        string? IReturnableStatementContext.ReturnVariable => null;
        string? IReturnableStatementContext.ReturningVariable => null;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;

        string? IInterruptibleStatementContext.InterruptingVariable => null;

        bool IExpressionContext.IsLiteral => false;

        public ImplementationType? ReturnOptionType { get; private set; }

        string? IComputationContext.BuilderVariable => null;

        bool IComputationContext.IsCollection => false;

        bool IStatementContext.TrailAllowed => true;

        bool hasType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasType = false;
        }

        public override void EnterOptionalTypeSuffix(OptionalTypeSuffixContext context)
        {
            ReturnOptionType = OptionImplementationType;
        }

        public override void ExitOptionalTypeSuffix(OptionalTypeSuffixContext context)
        {

        }

        public sealed override void EnterParamList(ParamListContext context)
        {
            EnterState<ParamListState>().EnterParamList(context);
        }

        public sealed override void ExitParamList(ParamListContext context)
        {

        }

        public sealed override void EnterType(TypeContext context)
        {
            hasType = true;
            Out.WriteOperator(':');
            if(ReturnOptionType is { } optionType)
            {
                Out.WriteOptionType(optionType);
                Out.Write('<');
            }
            base.EnterType(context);
        }

        public sealed override void ExitType(TypeContext context)
        {
            base.ExitType(context);
            if(ReturnOptionType != null)
            {
                Out.Write('>');
            }
        }

        public sealed override void EnterValueBlock(ValueBlockContext context)
        {
            if(!hasType && ReturnOptionType is { } optionType)
            {
                Out.WriteOperator(':');
                Out.Write("_ ");
                Out.WriteOptionAbbreviation(optionType);
            }
            Out.WriteOperator('=');
            Out.WriteLine("(");
            Out.EnterScope();
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public sealed override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitScope();
            Out.Write(')');
        }

        public override void EnterFuncBody(FuncBodyContext context)
        {

        }

        public override void ExitFuncBody(FuncBodyContext context)
        {

        }

        void IComputationContext.WriteBeginBlockExpression()
        {
            Out.EnterNestedScope();
            Out.WriteLine("(");
        }

        void IComputationContext.WriteEndBlockExpression()
        {
            Out.ExitNestedScope();
            Out.Write(')');
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Error("`break` must be used in a statement that supports it.", context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Error("`continue` must be used in a statement that supports it.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

        }
    }

    internal sealed class FunctionDeclState : FunctionBaseState
    {
        public override void EnterFuncDecl(FuncDeclContext context)
        {

        }

        public override void ExitFuncDecl(FuncDeclContext context)
        {
            ExitState().ExitFuncDecl(context);
        }

        public override void EnterInlineFuncDecl(InlineFuncDeclContext context)
        {

        }

        public override void ExitInlineFuncDecl(InlineFuncDeclContext context)
        {
            ExitState().ExitInlineFuncDecl(context);
        }
    }

    internal sealed class CaseFunctionDeclState : FunctionBaseState
    {
        bool firstName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            firstName = true;
        }

        public override void EnterCaseFuncDecl(CaseFuncDeclContext context)
        {

        }

        public override void ExitCaseFuncDecl(CaseFuncDeclContext context)
        {
            ExitState().ExitCaseFuncDecl(context);
        }

        public override void EnterInlineCaseFuncDecl(InlineCaseFuncDeclContext context)
        {

        }

        public override void ExitInlineCaseFuncDecl(InlineCaseFuncDeclContext context)
        {
            ExitState().ExitInlineCaseFuncDecl(context);
        }

        public override void EnterCaseFuncName(CaseFuncNameContext context)
        {
            Out.Write("(|");
        }

        public override void EnterName(NameContext context)
        {
            if(firstName)
            {
                firstName = false;
            }
            else
            {
                Out.Write('|');
            }

            base.EnterName(context);
        }

        public override void EnterOptionalTypeSuffix(OptionalTypeSuffixContext context)
        {
            Out.Write("|_");

            base.EnterOptionalTypeSuffix(context);
        }

        public override void EnterFuncBody(FuncBodyContext context)
        {
            Out.Write("|)");

            base.EnterFuncBody(context);
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

    internal sealed class NewVariableState : NodeState, IExpressionContext
    {
        bool first;
        bool? isliteral;

        bool IExpressionContext.IsLiteral => isliteral ??= (FindContext<IExpressionContext>()?.IsLiteral ?? false);
        
        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = false;
            isliteral = null;
        }

        public override void EnterVariableDecl(VariableDeclContext context)
        {
            first = true;
        }

        public override void ExitVariableDecl(VariableDeclContext context)
        {
            ExitState().ExitVariableDecl(context);
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

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void EnterLetDecl(LetDeclContext context)
        {
            Out.Write("let ");
        }

        public override void ExitLetDecl(LetDeclContext context)
        {

        }

        public override void EnterVarDecl(VarDeclContext context)
        {
            Out.Write("let mutable ");
        }

        public override void ExitVarDecl(VarDeclContext context)
        {

        }

        public override void EnterConstDecl(ConstDeclContext context)
        {
            Out.Write("[<");
            Out.WriteCoreName("LiteralAttribute");
            Out.WriteLine(">]");
            Out.Write("let ");
            isliteral = true;
        }

        public override void ExitConstDecl(ConstDeclContext context)
        {

        }

        public override void EnterUseDecl(UseDeclContext context)
        {
            Out.Write("use ");
        }

        public override void ExitUseDecl(UseDeclContext context)
        {

        }

        public override void EnterUseVarDecl(UseVarDeclContext context)
        {
            Out.Write("use mutable ");
        }

        public override void ExitUseVarDecl(UseVarDeclContext context)
        {

        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.WriteOperator('=');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

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

        public override void EnterType(TypeContext context)
        {
            Out.WriteOperator(':');
            base.EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {
            base.ExitType(context);
        }
    }
}
