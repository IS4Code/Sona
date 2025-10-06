using System;
using Antlr4.Runtime;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class WithStatementState : NodeState, IComputationContext, IReturnableStatementContext
    {
        bool IStatementContext.TrailAllowed => true;
        ImplementationType? IStatementContext.ReturnOptionType => null;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;
        string? IInterruptibleStatementContext.InterruptingVariable => null;

        string? IReturnableStatementContext.ReturnVariable => null;
        string? IReturnableStatementContext.ReturningVariable => null;

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

            if(returnScope?.ReturnVariable is { } result)
            {
                // Store result in variable
                Out.WriteIdentifier(result);
                Out.WriteOperator("<-");
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

            if(returnScope?.ReturningVariable is { } success)
            {
                // Value returned
                Out.WriteLine();
                Out.WriteIdentifier(success);
                Out.WriteOperator("<-");
                Out.Write("true");
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

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Error("`break` cannot be used accross a computation block boundary.", context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Error("`continue` cannot be used accross a computation block boundary.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

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
