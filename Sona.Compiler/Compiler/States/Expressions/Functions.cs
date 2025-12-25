using Antlr4.Runtime;
using Sona.Compiler.Tools;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class FunctionExprState : NodeState, IFunctionContext
    {
        BlockFlags IBlockContext.Flags => BlockFlags.None;

        ReturnFlags IReturnableContext.Flags => ReturnFlags.None | (returnOptionType != null ? ReturnFlags.Optional : 0);

        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;

        string? IInterruptibleContext.InterruptingVariable => null;

        ComputationFlags IComputationContext.Flags => ComputationFlags.None;

        ExpressionFlags IExpressionContext.Flags => ExpressionFlags.IsValue;

        ImplementationType? returnOptionType;

        bool IStatementContext.TrailAllowed => true;

        string? name;

        bool hasType;
        BindingSet bindings;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            name = null;

            hasType = false;
            bindings = new(FindContext<IBindingContext>());
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

            name = Syntax.GetIdentifierFromName(context.GetText());
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
            returnOptionType = OptionImplementationType;
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
            if(returnOptionType is { } optionType)
            {
                Out.WriteOptionType(optionType);
                Out.Write('<');
            }
            base.EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {
            base.ExitType(context);
            if(returnOptionType != null)
            {
                Out.Write('>');
            }
            Out.Write('>');
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            if(!hasType && returnOptionType is { } optionType)
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

        void IBindingContext.Set(string name, BindingKind kind)
        {
            bindings.Set(name, kind);
        }

        BindingKind IBindingContext.Get(string name)
        {
            return bindings.Get(name);
        }

        void IComputationContext.WriteBeginBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteBeginBlockExpression(context);
        }

        void IComputationContext.WriteEndBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteEndBlockExpression(context);
        }

        void IReturnableContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteReturnStatement(context);
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnStatement(context);
        }

        void IReturnableContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            if(returnOptionType is { } optionType)
            {
                if(isOption)
                {
                    isOption = false;
                }
                else
                {
                    Out.WriteOptionSome(optionType);
                }
            }
            Defaults.WriteReturnValue(isOption, context);
        }

        void IReturnableContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnValue(context);
        }

        void IReturnableContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            if(returnOptionType is { } optionType)
            {
                Out.WriteOptionNone(optionType);
            }
            else
            {
                Defaults.WriteEmptyReturnValue(context);
            }
        }

        void IBlockContext.WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if(returnOptionType is { } optionType)
            {
                Out.WriteOptionNone(optionType);
            }
            else
            {
                Defaults.WriteImplicitReturnStatement(context);
            }
        }

        void IInterruptibleContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteBreak(hasExpression, context);
        }

        void IInterruptibleContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteContinue(hasExpression, context);
        }

        void IInterruptibleContext.WriteAfterBreak(ParserRuleContext context)
        {
            Defaults.WriteAfterBreak(context);
        }

        void IInterruptibleContext.WriteAfterContinue(ParserRuleContext context)
        {
            Defaults.WriteAfterContinue(context);
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
            Out.WriteNext('|', ref firstName);

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
