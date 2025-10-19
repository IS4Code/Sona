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
        protected bool IsSimple { get; private set; }
        protected bool IsEmpty { get; private set; }

        public override ComputationFlags Flags => ComputationFlags.IsCollection;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            IsEmpty = true;
        }

        public sealed override void EnterSimpleCollectionContents(SimpleCollectionContentsContext context)
        {
            IsSimple = true;
        }

        public sealed override void EnterComplexCollectionContents(ComplexCollectionContentsContext context)
        {
            IsSimple = false;
        }

        public sealed override void ExitSimpleCollectionContents(SimpleCollectionContentsContext context)
        {
            OnLeave();
        }

        public sealed override void ExitComplexCollectionContents(ComplexCollectionContentsContext context)
        {
            OnLeave();
        }

        protected virtual void OnOperand(bool simple)
        {
            if(IsEmpty)
            {
                IsEmpty = false;
                if(IsSimple)
                {
                    Out.Write(' ');
                }
                else
                {
                    Out.WriteLine();
                }
            }
            else
            {
                if(IsSimple)
                {
                    Out.Write(';');
                }
                else
                {
                    Out.WriteLine();
                }
            }
            if(simple && !IsSimple)
            {
                // Yield explicitly (there are complex elements)
                Out.Write("yield ");
            }
        }

        void OnLeave()
        {
            if(IsSimple)
            {
                Out.Write(' ');
            }
            else
            {
                Out.WriteLine();
            }
        }

        public sealed override void EnterSimpleCollectionElement(SimpleCollectionElementContext context)
        {
            OnOperand(true);
        }

        public sealed override void ExitSimpleCollectionElement(SimpleCollectionElementContext context)
        {

        }

        public sealed override void EnterComplexCollectionElement(ComplexCollectionElementContext context)
        {
            OnOperand(false);
        }

        public sealed override void ExitComplexCollectionElement(ComplexCollectionElementContext context)
        {

        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {

        }

        public sealed override void EnterCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
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
        }

        public sealed override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            Out.Write("yield! ");
            EnterState<SpreadExpression>().EnterSpreadExpression(context);
        }

        public sealed override void ExitSpreadExpression(SpreadExpressionContext context)
        {

        }

        sealed class SpreadExpression : ExpressionState
        {
            public override void EnterSpreadExpression(SpreadExpressionContext context)
            {

            }

            public override void ExitSpreadExpression(SpreadExpressionContext context)
            {
                ExitState().ExitSpreadExpression(context);
            }
        }

        public sealed override void EnterExpressionStatement(ExpressionStatementContext context)
        {
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
                Out.ExitNestedScope();
                Out.Write("})");
            }
            ExitState().ExitSequenceConstructor(context);
        }

        protected override void OnOperand(bool simple)
        {
            if(IsEmpty)
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteCoreOperatorName("seq");
                Out.Write('{');
            }

            base.OnOperand(simple);
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
