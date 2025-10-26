using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class IsolatedState : NodeState, IFunctionContext
    {
        IExpressionContext? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = null;
        }

        public abstract ComputationFlags Flags { get; }

        public virtual bool TrailAllowed => false;

        BlockFlags IBlockContext.Flags => BlockFlags.None;

        ReturnFlags IReturnableContext.Flags => ReturnFlags.None;

        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;

        string? IInterruptibleContext.InterruptingVariable => null;

        ExpressionFlags IExpressionContext.Flags => GetExpressionContext()?.Flags ?? ExpressionFlags.IsValue;

        protected sealed override IExpressionContext? GetExpressionContext()
        {
            return scope ??= FindContext<IExpressionContext>();
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
            Defaults.WriteReturnValue(isOption, context);
        }

        void IReturnableContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnValue(context);
        }

        void IReturnableContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            Defaults.WriteEmptyReturnValue(context);
        }

        void IBlockContext.WriteImplicitReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteImplicitReturnStatement(context);
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

        public abstract void WriteBeginBlockExpression(ParserRuleContext context);
        public abstract void WriteEndBlockExpression(ParserRuleContext context);
    }

    internal abstract class CollectionState : IsolatedState
    {
        protected bool IsEmpty { get; private set; }
        bool hasYielded;

        public override ComputationFlags Flags => ComputationFlags.IsCollection;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            IsEmpty = true;
            hasYielded = false;
        }

        public override void EnterCollectionElement(CollectionElementContext context)
        {
            IsEmpty = false;
            Out.WriteLine();
        }

        public override void ExitCollectionElement(CollectionElementContext context)
        {

        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            Out.Write("yield ");
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            hasYielded = true;
        }

        public sealed override void EnterCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
            Out.Write("yield ");
            Out.WriteNamespacedName("System.Collections.Generic", "KeyValuePair");
            Out.Write("<_,_>(");
        }

        public sealed override void EnterAssignment(AssignmentContext context)
        {
            Out.Write(',');
        }

        public sealed override void ExitAssignment(AssignmentContext context)
        {

        }

        public sealed override void ExitCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
            Out.Write(')');
            hasYielded = true;
        }

        public sealed override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            Out.Write("yield! ");
            EnterState<ExpressionState.Spread>().EnterSpreadExpression(context);
        }

        public sealed override void ExitSpreadExpression(SpreadExpressionContext context)
        {
            hasYielded = true;
        }

        public sealed override void EnterExpressionStatement(ExpressionStatementContext context)
        {
            if(!hasYielded)
            {
                // Yield at least once to disambiguate from value construction
                Out.Write("if false then yield ");
                Out.WriteDefaultValue();
                Out.WriteLine();
                hasYielded = true;
            }
            EnterState<ExpressionStatementState>().EnterExpressionStatement(context);
        }

        public sealed override void ExitExpressionStatement(ExpressionStatementContext context)
        {

        }
    }

    internal sealed class ArrayState : CollectionState
    {
        CollectionImplementationType collectionType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            collectionType = default;
        }

        public override void EnterArrayConstructor(ArrayConstructorContext context)
        {
            collectionType = CollectionImplementationType;

            Out.EnterNestedScope();
            Out.WriteCollectionOpen(collectionType);
        }

        public override void ExitArrayConstructor(ArrayConstructorContext context)
        {
            if(IsEmpty)
            {
                Out.Write(' ');
            }
            else
            {
                Out.WriteLine();
            }
            Out.ExitNestedScope();
            Out.WriteCollectionClose(collectionType);
            ExitState().ExitArrayConstructor(context);
        }

        public override void WriteBeginBlockExpression(ParserRuleContext context)
        {
            Out.EnterNestedScope();
            Out.WriteCollectionOpen(collectionType);
        }

        public override void WriteEndBlockExpression(ParserRuleContext context)
        {
            Out.ExitNestedScope();
            Out.WriteCollectionClose(collectionType);
        }
    }

    internal sealed class SequenceState : CollectionState
    {
        public override void EnterSequenceConstructor(SequenceConstructorContext context)
        {

        }

        public override void ExitSequenceConstructor(SequenceConstructorContext context)
        {
            if(IsEmpty)
            {
                Out.WriteNamespacedName("Microsoft.FSharp.Collections", "Seq", "empty");
            }
            else
            {
                Out.WriteLine();
                Out.ExitNestedScope();
                Out.Write("})");
            }
            ExitState().ExitSequenceConstructor(context);
        }

        public override void EnterCollectionElement(CollectionElementContext context)
        {
            if(IsEmpty)
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteCoreOperatorName("seq");
                Out.Write('{');
            }

            base.EnterCollectionElement(context);
        }

        public override void WriteBeginBlockExpression(ParserRuleContext context)
        {
            Out.EnterNestedScope();
            Out.Write('(');
            Out.WriteCoreOperatorName("seq");
            Out.WriteLine('{');
        }

        public override void WriteEndBlockExpression(ParserRuleContext context)
        {
            Out.ExitNestedScope();
            Out.Write("})");
        }
    }
}
