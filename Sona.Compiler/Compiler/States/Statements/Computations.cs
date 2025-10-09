using System;
using Antlr4.Runtime;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class WithStatementState : NodeState, IComputationContext
    {
        bool IStatementContext.TrailAllowed => true;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;
        string? IInterruptibleStatementContext.InterruptingVariable => null;

        ReturnFlags IReturnableStatementContext.Flags => ReturnFlags.None;

        bool IComputationContext.IsCollection => false;
        public string? BuilderVariable { get; private set; }

        IReturnableStatementContext? returnScope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            returnScope = FindContext<IReturnableStatementContext>();
        }

        public override void EnterWithStatement(WithStatementContext context)
        {
            BuilderVariable = Out.CreateTemporaryIdentifier();

            if(returnScope != null)
            {
                returnScope.WriteReturnStatement(context);
            }
            else
            {
                Defaults.WriteReturnStatement(context);
            }

            Out.EnterNestedScope(true);
            Out.Write("(let ");
            Out.WriteIdentifier(BuilderVariable);
            Out.WriteOperator('=');
        }

        public override void ExitWithStatement(WithStatementContext context)
        {
            Out.ExitScope();
            Out.WriteLine();
            Out.Write(_end_);
            Out.Write(" }");

            Out.ExitNestedScope();
            Out.Write(')');

            if(returnScope != null)
            {
                returnScope.WriteAfterReturnStatement(context);
            }
            else
            {
                Defaults.WriteAfterReturnStatement(context);
            }

            ExitState().ExitWithStatement(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            Out.Write(" in ");
            Out.WriteIdentifier(BuilderVariable!);
            Out.Write(" { ");
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public override void EnterValueTrail(ValueTrailContext context)
        {
            EnterState<Trail>().EnterValueTrail(context);
        }

        public override void ExitValueTrail(ValueTrailContext context)
        {

        }

        public void WriteBeginBlockExpression(ParserRuleContext context)
        {
            Out.EnterNestedScope();
            Out.Write('(');
            Out.WriteIdentifier(BuilderVariable ?? Error("COMPILER ERROR: Computation block is not properly initialized.", context));
            Out.WriteLine('{');
        }

        public void WriteEndBlockExpression(ParserRuleContext context)
        {
            Out.ExitNestedScope();
            Out.Write("})");
        }

        void IReturnableStatementContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableStatementContext.WriteReturnStatement(ParserRuleContext context)
        {
            Out.Write("return ");
        }

        void IReturnableStatementContext.WriteAfterReturnStatement(ParserRuleContext context)
        {

        }

        void IReturnableStatementContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            if(returnScope != null)
            {
                returnScope.WriteReturnValue(isOption, context);
            }
            else
            {
                Defaults.WriteReturnValue(isOption, context);
            }
        }

        void IReturnableStatementContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            if(returnScope != null)
            {
                returnScope.WriteAfterReturnValue(context);
            }
            else
            {
                Defaults.WriteAfterReturnValue(context);
            }
        }

        void IReturnableStatementContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            if(returnScope != null)
            {
                returnScope.WriteEmptyReturnValue(context);
            }
            else
            {
                Defaults.WriteEmptyReturnValue(context);
            }
        }

        void IBlockStatementContext.WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if(FindContext<IBlockStatementContext>() is { } blockContext)
            {
                if(blockContext == FindContext<IComputationContext>())
                {
                    // At the end of function/computation
                    blockContext.WriteImplicitReturnStatement(context);
                    return;
                }
            }
            
            Error("It is not possible to escape from a computation block directly to the outside code. Use `return` to return explicitly.", context);
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteBreak(hasExpression, context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteContinue(hasExpression, context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {
            Defaults.WriteAfterBreak(context);
        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {
            Defaults.WriteAfterContinue(context);
        }

        sealed class Trail : BlockState
        {
            protected override bool IgnoreContext => true;

            public override void EnterValueTrail(SonaParser.ValueTrailContext context)
            {

            }

            public override void ExitValueTrail(SonaParser.ValueTrailContext context)
            {
                ExitState()!.ExitValueTrail(context);
            }
        }
    }

    internal sealed class FollowState : NodeState
    {
        public override void EnterFollowStatement(FollowStatementContext context)
        {
            if(FindContext<IComputationContext>() is not { BuilderVariable: not null })
            {
                Error("`follow` is not allowed outside a computation.", context);
            }
            Out.Write("do! ");
        }

        public override void ExitFollowStatement(FollowStatementContext context)
        {
            ExitState().ExitFollowStatement(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }
    }
}
