using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class FunctionExprState : NodeState, IFunctionContext
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

        string? name;

        bool hasType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            name = null;

            hasType = false;
        }

        public override void EnterName(NameContext context)
        {
            Out.EnterNestedScope(true);
            Out.Write("(let rec ");

            base.EnterName(context);
        }

        public override void ExitName(NameContext context)
        {
            base.ExitName(context);

            name = Tools.Syntax.GetIdentifierFromName(context.GetText());
            Out.WriteOperator('=');
        }

        public override void EnterFuncExpr(FuncExprContext context)
        {

        }

        public override void ExitFuncExpr(FuncExprContext context)
        {
            ExitState().ExitFuncExpr(context);
        }

        public override void EnterOptionSuffix(OptionSuffixContext context)
        {
            ReturnOptionType = OptionImplementationType;
        }

        public override void ExitOptionSuffix(OptionSuffixContext context)
        {

        }

        public override void EnterParamList(ParamListContext context)
        {
            if(name == null)
            {
                // Scope not entered before
                Out.EnterNestedScope();
                Out.Write('(');
            }
            Out.Write("fun ");

            EnterState<ParamListState>().EnterParamList(context);
        }

        public override void ExitParamList(ParamListContext context)
        {
            Out.WriteOperator("->");
        }

        public override void EnterType(TypeContext context)
        {
            hasType = true;
            Out.WriteCoreName("Operators");
            Out.Write(".id<");
            if(ReturnOptionType is { } optionType)
            {
                Out.WriteOptionType(optionType);
                Out.Write('<');
            }
            base.EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {
            base.ExitType(context);
            if(ReturnOptionType != null)
            {
                Out.Write('>');
            }
            Out.Write('>');
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            if(!hasType && ReturnOptionType is { } optionType)
            {
                Out.WriteCoreName("Operators");
                Out.Write(".id<");
                Out.Write("_ ");
                Out.WriteOptionAbbreviation(optionType);
                Out.Write('>');
            }
            Out.EnterNestedScope();
            Out.WriteLine('(');
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitNestedScope();
            if(name != null)
            {
                Out.Write(')');
                Out.WriteLine();
                Out.ExitNestedScope();
                Out.Write(" in ");
                Out.WriteIdentifier(name);
            }
            else
            {
                Out.ExitNestedScope();
                Out.Write(')');
            }
            Out.Write(')');
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

    internal sealed class CaseFunctionRefExprState : MemberExprState
    {
        bool firstName;
        bool hasParent;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            firstName = true;
            hasParent = false;
        }

        public override void EnterCaseFuncRefExpr(CaseFuncRefExprContext context)
        {

        }

        public override void ExitCaseFuncRefExpr(CaseFuncRefExprContext context)
        {
            Out.Write("|)");
            ExitState().ExitCaseFuncRefExpr(context);
        }

        public override void EnterCompoundNameGeneric(CompoundNameGenericContext context)
        {
            hasParent = true;
            base.EnterCompoundNameGeneric(context);
        }

        private void OnEnterCaseName()
        {
            if(hasParent == true)
            {
                Out.Write('.');
            }
            Out.Write("(|");
        }

        public override void EnterMemberName(MemberNameContext context)
        {
            OnEnterCaseName();
            firstName = false;
            base.EnterMemberName(context);
        }

        public override void EnterCaseFuncName(CaseFuncNameContext context)
        {
            OnEnterCaseName();
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
        }

        public override void ExitOptionSuffix(OptionSuffixContext context)
        {

        }
    }
}
