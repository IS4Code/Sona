using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
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

        ExpressionType IExpressionContext.Type => ExpressionType.Regular;

        public ImplementationType? ReturnOptionType { get; private set; }

        string? IComputationContext.BuilderVariable => null;

        bool IComputationContext.IsCollection => false;

        bool IStatementContext.TrailAllowed => true;

        bool hasType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasType = false;
            ReturnOptionType = default;
        }

        public override void EnterOptionSuffix(OptionSuffixContext context)
        {
            ReturnOptionType = OptionImplementationType;
        }

        public override void ExitOptionSuffix(OptionSuffixContext context)
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
            if(!hasType && ReturnOptionType is { } optionType && this is not CaseFunctionDeclState)
            {
                Out.WriteOperator(':');
                Out.Write("_ ");
                Out.WriteOptionAbbreviation(optionType);
            }
            Out.WriteOperator('=');
            Out.WriteLine('(');
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
            Out.WriteLine('(');
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

        public override void EnterOptionSuffix(OptionSuffixContext context)
        {
            Out.Write("|_");

            base.EnterOptionSuffix(context);
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

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void ExitParamList(ParamListContext context)
        {
            ExitState().ExitParamList(context);
        }
    }

    internal sealed class NewVariableState : NodeState, IExpressionContext
    {
        ExpressionType? type;

        ExpressionType IExpressionContext.Type => type ??= (FindContext<IExpressionContext>()?.Type ?? ExpressionType.Regular);
        
        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            type = null;
        }

        public override void EnterVariableDecl(VariableDeclContext context)
        {

        }

        public override void ExitVariableDecl(VariableDeclContext context)
        {
            ExitState().ExitVariableDecl(context);
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            if(LexerContext.GetState<RecursivePragma>()?.Value ?? false)
            {
                Out.Write("rec ");
            }
            EnterState<DeclarationState>().EnterDeclaration(context);
        }

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void EnterMultiDeclAssignment(MultiDeclAssignmentContext context)
        {
            if(type == ExpressionType.Literal || (LexerContext.GetState<RecursivePragma>()?.Value ?? false))
            {
                EnterState<RecursiveMultiDeclarationState>().EnterMultiDeclAssignment(context);
            }
            else
            {
                EnterState<MultiDeclarationState>().EnterMultiDeclAssignment(context);
            }
        }

        public override void ExitMultiDeclAssignment(MultiDeclAssignmentContext context)
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
            type = ExpressionType.Literal;
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

    internal class DeclarationState : NodeState
    {
        public override void EnterDeclaration(DeclarationContext context)
        {

        }

        public override void ExitDeclaration(DeclarationContext context)
        {
            ExitState().ExitDeclaration(context);
        }

        public sealed override void EnterPattern(PatternContext context)
        {
            Out.Write('(');
            EnterState<PatternState>().EnterPattern(context);
        }

        public sealed override void ExitPattern(PatternContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterOptionalName(OptionalNameContext context)
        {
            Out.Write('?');
        }

        public sealed override void ExitOptionalName(OptionalNameContext context)
        {

        }

        public sealed override void EnterType(TypeContext context)
        {
            Out.WriteOperator(':');
            base.EnterType(context);
        }

        public sealed override void ExitType(TypeContext context)
        {
            base.ExitType(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            if(node.Symbol.Type == SonaLexer.INLINE)
            {
                Out.Write("[<");
                Out.WriteCoreName("InlineIfLambdaAttribute");
                Out.Write(">]");
            }
        }
    }

    internal sealed class MultiDeclarationState : DeclarationState
    {
        bool first;
        ISourceCapture? valueCapture;
        readonly List<ISourceCapture> valueCaptures = new();

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            valueCapture = null;
            valueCaptures.Clear();
        }

        public override void EnterMultiDeclAssignment(MultiDeclAssignmentContext context)
        {
            Out.Write('(');
        }

        public override void ExitMultiDeclAssignment(MultiDeclAssignmentContext context)
        {
            Out.Write(')');
            Out.WriteOperator('=');
            Out.EnterNestedScope();
            Out.Write('(');
            var first = true;
            foreach(var capture in valueCaptures)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.Write(',');
                }
                capture.Play(Out);
            }
            Out.ExitNestedScope();
            Out.Write(')');
            ExitState().ExitMultiDeclAssignment(context);
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
        }

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void EnterExpression(ExpressionContext context)
        {
            valueCapture = Out.StartCapture();
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            if(valueCapture != null)
            {
                Out.StopCapture(valueCapture);
                valueCaptures.Add(valueCapture);
                valueCapture = null;
            }
        }
    }

    internal sealed class RecursiveMultiDeclarationState : DeclarationState
    {
        bool first, isLiteral;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            isLiteral = FindContext<IExpressionContext>()?.Type == ExpressionType.Literal;
        }

        public override void EnterMultiDeclAssignment(MultiDeclAssignmentContext context)
        {

        }

        public override void ExitMultiDeclAssignment(MultiDeclAssignmentContext context)
        {
            ExitState().ExitMultiDeclAssignment(context);
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            if(first)
            {
                first = false;
                Out.Write("rec ");
            }
            else
            {
                Out.WriteLine();
                Out.Write("and ");
                if(isLiteral)
                {
                    Out.Write("[<");
                    Out.WriteCoreName("LiteralAttribute");
                    Out.Write(">]");
                }
            }
        }

        public override void ExitDeclaration(DeclarationContext context)
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
    }
}
