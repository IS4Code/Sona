﻿using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class WithStatementBase : ControlStatement
    {
        public string? BuilderVariable { get; private set; }
        bool isCollection;

        [MemberNotNullWhen(true, nameof(BuilderVariable))]
        protected bool IsComputation => BuilderVariable != null;

        protected override bool IgnoreContext => false;

        protected override bool IgnoreTrailContext => true;

        protected new IReturnableContext ReturnScope => base.ReturnScope ?? Defaults;
        protected new IInterruptibleContext InterruptScope => base.InterruptScope ?? Defaults;

        public ComputationFlags Flags =>
            (IsComputation ? ComputationFlags.IsComputation : 0) |
            (isCollection ? ComputationFlags.IsCollection : 0);

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            BuilderVariable = null;
            isCollection = false;
        }

        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            base.OnEnter(flags, context);

            OnComputationEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            Out.WriteLine();
            Out.ExitNestedScope();
            if(IsComputation || isCollection)
            {
                Out.Write('}');
            }
            Out.Write(')');

            OnComputationExit(flags, context);

            base.OnExit(flags, context);
        }

        protected abstract void OnComputationEnter(StatementFlags flags, ParserRuleContext context);

        protected abstract void OnComputationExit(StatementFlags flags, ParserRuleContext context);

        protected sealed override void OnEnterTrail(StatementFlags flags, ParserRuleContext context)
        {
            OnEnterBlock(flags, context);
        }

        protected sealed override void OnExitTrail(StatementFlags flags, ParserRuleContext context)
        {
            OnExitBlock(flags, context);
        }

        /*protected sealed override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {

        }

        protected sealed override void OnExitBlock(StatementFlags flags, ParserRuleContext context)
        {

        }*/

        protected override void ModifyFlags(ref StatementFlags flags)
        {
            // Leave as is
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            OnEnterExpression(context);
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            OnExitExpression(context);
        }

        public sealed override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            isCollection = true;
            OnEnterExpression(context);
            EnterState<ExpressionState.Spread>().EnterSpreadExpression(context);
        }

        public sealed override void ExitSpreadExpression(SpreadExpressionContext context)
        {
            OnExitExpression(context);
        }

        private void OnEnterExpression(ParserRuleContext context)
        {
            BuilderVariable = Out.CreateTemporaryIdentifier();

            Out.EnterNestedScope();
            Out.Write("(let ");
            Out.WriteIdentifier(BuilderVariable);
            Out.WriteOperator('=');
        }

        private void OnExitExpression(ParserRuleContext context)
        {
            Out.Write(" in ");
            Out.WriteIdentifier(BuilderVariable!);
            Out.Write(" { ");
        }

        public sealed override void EnterWithDefaultArgument(WithDefaultArgumentContext context)
        {
            Out.EnterNestedScope();
            Out.Write("(");
        }

        public sealed override void ExitWithDefaultArgument(WithDefaultArgumentContext context)
        {

        }

        public sealed override void EnterWithDefaultSequenceArgument(WithDefaultSequenceArgumentContext context)
        {
            isCollection = true;
            Out.EnterNestedScope();
            Out.Write('(');
            Out.WriteCoreOperatorName("seq");
            Out.Write('{');
        }

        public sealed override void ExitWithDefaultSequenceArgument(WithDefaultSequenceArgumentContext context)
        {

        }

        public void WriteBeginBlockExpression(ParserRuleContext context)
        {
            if(IsComputation)
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteIdentifier(BuilderVariable);
                Out.WriteLine('{');
            }
            else if(isCollection)
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteCoreOperatorName("seq");
                Out.WriteLine('{');
            }
            else
            {
                Defaults.WriteBeginBlockExpression(context);
            }
        }

        public void WriteEndBlockExpression(ParserRuleContext context)
        {
            if(IsComputation || isCollection)
            {
                Out.ExitNestedScope();
                Out.Write("})");
            }
            else
            {
                Defaults.WriteEndBlockExpression(context);
            }
        }

        public override void WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation)
            {
                // Nothing
                base.WriteImplicitReturnStatement(context);
            }
            else
            {
                Out.Write("return ");
                ReturnScope.WriteEmptyReturnValue(context);
            }
        }
    }

    internal sealed class WithStatementState : WithStatementBase, IComputationContext
    {
        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;
        string? IInterruptibleContext.InterruptingVariable => null;

        // Does not provide its own conditional return variables
        ReturnFlags IReturnableContext.Flags => ReturnFlags & ~ReturnFlags.Indirect;

        protected override void OnEnter(StatementFlags flags, ParserRuleContext context)
        {
            ReturnScope.WriteReturnStatement(context);

            base.OnEnter(flags, context);
        }

        protected override void OnExit(StatementFlags flags, ParserRuleContext context)
        {
            base.OnExit(flags, context);

            ReturnScope.WriteAfterReturnStatement(context);
        }

        protected override void OnComputationEnter(StatementFlags flags, ParserRuleContext context)
        {
            if(FindContext<IBlockContext>()?.HasFlag(BlockFlags.HasTrySemantics) ?? false)
            {
                Error("The `with` statement cannot be used in `try` because it is not possible to transfer execution to outside code.", context);
            }
        }

        protected override void OnComputationExit(StatementFlags flags, ParserRuleContext context)
        {

        }

        public override void WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if(ReturnScope?.HasFlag(ReturnFlags.Indirect) ?? false)
            {
                // Other paths lead past this block so this is not the final result
                Error("It is not possible to escape from a computation block directly to the outside code. Use `return` to return explicitly.", context);
            }
            base.WriteImplicitReturnStatement(context);
        }

        void IReturnableContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            if(IsComputation)
            {
                Out.Write("return ");
            }
            else
            {
                Defaults.WriteReturnStatement(context);
            }
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {
            if(!IsComputation)
            {
                Defaults.WriteAfterReturnStatement(context);
            }
        }

        void IReturnableContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            ReturnScope.WriteReturnValue(isOption, context);
        }

        void IReturnableContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            ReturnScope.WriteAfterReturnValue(context);
        }

        void IReturnableContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            ReturnScope.WriteEmptyReturnValue(context);
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

    internal sealed class FollowWithStatementState : WithStatementBase, IComputationContext
    {
        InterruptFlags IInterruptibleContext.Flags => InterruptScope?.Flags ?? InterruptFlags.None;
        string? IInterruptibleContext.InterruptingVariable => InterruptScope?.InterruptingVariable;

        ReturnFlags IReturnableContext.Flags => ReturnFlags;

        bool closeReturnStatement;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            closeReturnStatement = false;
        }

        protected override void OnComputationEnter(StatementFlags flags, ParserRuleContext context)
        {
            if((ReturnFlags & ReturnFlags.Indirect) == 0)
            {
                // No conditional variables
                if(FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) ?? false)
                {
                    // Can utilize `ReturnFrom`
                    Out.Write("return! ");
                }
                else
                {
                    // Unwrap directly
                    ReturnScope.WriteReturnStatement(context);
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                    closeReturnStatement = true;
                }
            }
            else
            {
                // Returns are handled indirectly
                if(FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) ?? false)
                {
                    Out.Write("let! () = ");
                }
                else
                {
                    Out.Write("do ");
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                }
            }
        }

        protected override void OnComputationExit(StatementFlags flags, ParserRuleContext context)
        {
            if(closeReturnStatement)
            {
                ReturnScope.WriteAfterReturnStatement(context);
            }
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation)
            {
                WriteReturnStatement(context);
            }
            else
            {
                Out.Write("return ");
            }
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation)
            {
                WriteAfterReturnStatement(context);
            }
        }

        void IInterruptibleContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            InterruptScope.WriteBreak(hasExpression, context);
        }

        void IInterruptibleContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            InterruptScope.WriteContinue(hasExpression, context);
        }

        void IInterruptibleContext.WriteAfterBreak(ParserRuleContext context)
        {
            InterruptScope.WriteAfterBreak(context);
        }

        void IInterruptibleContext.WriteAfterContinue(ParserRuleContext context)
        {
            InterruptScope.WriteAfterContinue(context);
        }
    }

    internal sealed class FollowState : NodeState
    {
        public override void EnterFollowStatement(FollowStatementContext context)
        {
            if(FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) ?? false)
            {
                // Not `do!` because that sometimes abuses `Return`.
                Out.Write("let! ()");
                Out.WriteOperator('=');
            }
            else
            {
                Out.Write("do ");
                Out.WriteGlobalComputationOperator("ReturnFrom");
            }
            Out.Write('(');
        }

        public override void ExitFollowStatement(FollowStatementContext context)
        {
            Out.Write(')');
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

    internal sealed class FollowDiscardState : ExpressionState
    {
        public override void EnterFollowDiscardStatement(FollowDiscardStatementContext context)
        {
            if(FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) ?? false)
            {
                Out.Write("let! _");
                Out.WriteOperator('=');
            }
            else
            {
                Out.Write("let _");
                Out.WriteOperator('=');
                Out.WriteGlobalComputationOperator("ReturnFrom");
            }
            Out.Write('(');
        }

        public override void ExitFollowDiscardStatement(FollowDiscardStatementContext context)
        {
            Out.Write(')');
            ExitState().ExitFollowDiscardStatement(context);
        }
    }
}
