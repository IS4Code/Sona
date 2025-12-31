using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class WithStatementBase : ControlStatement
    {
        public string? BuilderVariable { get; private set; }
        protected bool IsCollection { get; private set; }

        [MemberNotNullWhen(true, nameof(BuilderVariable))]
        protected bool IsComputation => BuilderVariable != null;

        protected override bool IgnoreContext => false;

        protected override bool IgnoreTrailContext => true;

        protected new IReturnableContext ReturnScope => base.ReturnScope ?? Defaults;
        protected new IInterruptibleContext InterruptScope => base.InterruptScope ?? Defaults;

        public ComputationFlags Flags =>
            (IsComputation ? ComputationFlags.IsComputation : 0) |
            (IsCollection ? ComputationFlags.IsCollection : 0);

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            BuilderVariable = null;
            IsCollection = false;
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
            if(IsComputation || IsCollection)
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

        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            base.OnEnterBlock(flags, context);
            if(IsCollection)
            {
                // Yield at least once to disambiguate from value construction
                Out.Write("if false then yield ");
                Out.WriteDefaultValue();
                Out.WriteLine();
            }
        }

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
            IsCollection = true;
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
            IsCollection = true;
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
            else if(IsCollection)
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
            if(IsComputation || IsCollection)
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
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation || IsCollection)
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
            var computationScope = FindContext<IComputationContext>();
            if(computationScope?.HasFlag(ComputationFlags.IsCollection) ?? false)
            {
                const string msg = "`with` cannot be used in a collection construction because returning is not supported.";
                if(computationScope?.HasFlag(ComputationFlags.IsComputation) != true)
                {
                    // Can't return from a sequence
                    Error(msg + " Use `yield with` instead.", context);
                }
                else
                {
                    // No mechanism to indicate returning only
                    Error(msg + " Use `follow with` or `yield with` instead.", context);
                }
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
            if(!IsComputation || IsCollection)
            {
                Defaults.WriteAfterReturnStatement(context);
            }
            else
            {
                Out.Write("return ");
            }
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {
            if(!IsComputation || IsCollection)
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

        bool closeReturnStatement, closeVariableStatement, closeGlobalFollowOperator;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            closeReturnStatement = false;
            closeVariableStatement = false;
            closeGlobalFollowOperator = false;
        }

        protected override void OnComputationEnter(StatementFlags flags, ParserRuleContext context)
        {
            var computationScope = FindContext<IComputationContext>();
            if((flags & StatementFlags.ReturnPath) != 0 && (ReturnFlags & ReturnFlags.Indirect) == 0)
            {
                // No conditional variables
                if(computationScope?.HasFlag(ComputationFlags.IsComputation) ?? false)
                {
                    // Can utilize shorter syntax
                    Out.Write("return! ");
                }
                else
                {
                    // Unwrap directly
                    ReturnScope.WriteReturnStatement(context);
                    closeReturnStatement = true;
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                    closeGlobalFollowOperator = true;
                }
            }
            else
            {
                // Returns are handled indirectly
                if(computationScope?.HasFlag(ComputationFlags.IsComputation) ?? false)
                {
                    Out.Write("let! () = ");
                    closeVariableStatement = true;
                }
                else
                {
                    Out.Write("do ");
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                    closeGlobalFollowOperator = true;
                }
            }
        }

        protected override void OnComputationExit(StatementFlags flags, ParserRuleContext context)
        {
            if(closeGlobalFollowOperator)
            {
                Out.WriteAfterGlobalComputationOperator();
            }
            if(closeReturnStatement)
            {
                ReturnScope.WriteAfterReturnStatement(context);
            }
            if(closeVariableStatement)
            {
                Out.Write(" in ()");
            }
        }

        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            if((FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsCollection) ?? false) && !IsCollection && (flags & StatementFlags.ReturnPath) != 0)
            {
                Error("`follow with` statement used in a collection construction must either not return or use the collection form, i.e. `follow with..`.", context);
            }
            base.OnEnterBlock(flags, context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation || IsCollection)
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
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation || IsCollection)
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

    internal sealed class YieldWithStatementState : WithStatementBase, IComputationContext
    {
        InterruptFlags IInterruptibleContext.Flags => InterruptScope?.Flags ?? InterruptFlags.None;
        string? IInterruptibleContext.InterruptingVariable => InterruptScope?.InterruptingVariable;

        ReturnFlags IReturnableContext.Flags => ReturnFlags;

        protected override void OnComputationEnter(StatementFlags flags, ParserRuleContext context)
        {
            if(FindContext<IComputationContext>()?.HasAnyFlag(ComputationFlags.IsCollection | ComputationFlags.IsComputation) != true)
            {
                Error("`yield with` is not allowed outside a collection or computation.", context);
            }
            
            // Unwrap the inner sequence
            Out.Write("yield! ");
        }

        protected override void OnComputationExit(StatementFlags flags, ParserRuleContext context)
        {

        }

        protected override void OnEnterBlock(StatementFlags flags, ParserRuleContext context)
        {
            if((FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsCollection) ?? false) && !IsCollection && (flags & StatementFlags.ReturnPath) != 0)
            {
                Error("`yield with` statement used in a collection construction must either not return or use the collection form, i.e. `yield with..`.", context);
            }
            base.OnEnterBlock(flags, context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation || IsCollection)
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
            if((ReturnFlags & ReturnFlags.Indirect) != 0 || !IsComputation || IsCollection)
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

    internal sealed class FollowState : ExpressionState
    {
        bool first;
        bool inComputation;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            inComputation = false;
        }

        public override void EnterFollowStatement(FollowStatementContext context)
        {
            inComputation = FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) ?? false;
        }

        public override void ExitFollowStatement(FollowStatementContext context)
        {
            ExitState().ExitFollowStatement(context);
        }

        private void OnEnterExpression(ParserRuleContext context)
        {
            if(inComputation)
            {
                if(first)
                {
                    Out.Write("let! ");
                    first = false;
                }
                else
                {
                    Out.WriteLine();
                    Out.Write("and! ");
                }
            }
            else
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.WriteLine();
                    Error("Multiple operands for the `follow` statement are supported only in a computation.", context);
                }
            }
        }

        private void OnExitExpression(ParserRuleContext context)
        {
            if(inComputation)
            {
                Out.Write(')');
            }
            else
            {
                Out.WriteAfterGlobalComputationOperator();
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            OnEnterExpression(context);
            if(inComputation)
            {
                // Not `do!` because that sometimes abuses `Return`.
                Out.Write("()");
                Out.WriteOperator('=');
                Out.Write('(');
            }
            else
            {
                Out.Write("do ");
                Out.WriteGlobalComputationOperator("ReturnFrom");
            }
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            OnExitExpression(context);
        }

        public override void EnterMemberDiscard(MemberDiscardContext context)
        {
            OnEnterExpression(context);
            if(inComputation)
            {
                Out.Write("_");
                Out.WriteOperator('=');
                Out.Write('(');
            }
            else
            {
                Out.Write("let _");
                Out.WriteOperator('=');
                Out.WriteGlobalComputationOperator("ReturnFrom");
            }
        }

        public override void ExitMemberDiscard(MemberDiscardContext context)
        {
            OnExitExpression(context);
        }
    }

    internal abstract class FollowStatementState : NodeState
    {
        string? variableName;

        protected bool UsesComputationForm { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            variableName = null;
            UsesComputationForm = false;
        }

        protected void OnEnter(ParserRuleContext context)
        {
            var computationScope = FindContext<IComputationContext>();
            if(computationScope?.HasFlag(ComputationFlags.IsComputation) ?? false)
            {
                if(OnComputationStatement(context))
                {
                    // Computation form was used
                    UsesComputationForm = true;
                    return;
                }
                variableName = Out.CreateTemporaryIdentifier();
                Out.Write("let! ");
                Out.WriteIdentifier(variableName);
                Out.WriteOperator('=');
            }
            else
            {
                OnStatement(context);
                Out.WriteGlobalComputationOperator("ReturnFrom");
            }
        }

        protected void OnExit(ParserRuleContext context)
        {
            if(UsesComputationForm)
            {
                return;
            }
            if(variableName == null)
            {
                // Non-computation form
                Out.WriteAfterGlobalComputationOperator();
                return;
            }
            Out.WriteLine();
            OnStatement(context);
            Out.WriteIdentifier(variableName);
        }

        protected abstract void OnStatement(ParserRuleContext context);

        protected virtual bool OnComputationStatement(ParserRuleContext context)
        {
            return false;
        }

        public override void EnterFollowExpression(FollowExpressionContext context)
        {
            EnterState<Operand>().EnterFollowExpression(context);
        }

        public override void ExitFollowExpression(FollowExpressionContext context)
        {

        }

        public sealed class Operand : ExpressionState
        {
            public override void EnterFollowExpression(FollowExpressionContext context)
            {

            }

            public override void ExitFollowExpression(FollowExpressionContext context)
            {
                ExitState().ExitFollowExpression(context);
            }
        }
    }
}
