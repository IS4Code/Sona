using Antlr4.Runtime;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal abstract class IsolatedState : NodeState, IFunctionContext
    {
        IExpressionContext? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = null;
        }

        public abstract bool IsCollection { get; }

        public virtual string? BuilderVariable => null;

        public virtual bool TrailAllowed => false;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;

        string? IInterruptibleStatementContext.InterruptingVariable => null;

        bool IFunctionContext.IsOptionalReturn => false;

        string? IReturnableStatementContext.ReturnVariable => null;

        string? IReturnableStatementContext.ReturningVariable => null;

        bool IExpressionContext.IsLiteral => GetExpressionContext()?.IsLiteral ?? false;

        protected sealed override IExpressionContext? GetExpressionContext()
        {
            return scope ??= FindContext<IExpressionContext>();
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

        public abstract void WriteBeginBlockExpression();
        public abstract void WriteEndBlockExpression();
    }

    internal abstract class CollectionState : IsolatedState
    {
        protected bool IsSimple { get; private set; }
        protected bool IsEmpty { get; private set; }

        public sealed override bool IsCollection => true;

        protected sealed override void Initialize(ScriptEnvironment environment, ScriptState? parent)
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
        public override void EnterArrayConstructor(ArrayConstructorContext context)
        {
            Out.EnterNestedScope();
            Out.Write("[|");
        }

        public override void ExitArrayConstructor(ArrayConstructorContext context)
        {
            if(IsEmpty)
            {
                Out.Write(' ');
            }
            Out.ExitNestedScope();
            Out.Write("|]");
            ExitState().ExitArrayConstructor(context);
        }

        public override void WriteBeginBlockExpression()
        {
            Out.EnterNestedScope();
            Out.WriteLine("[|");
        }

        public override void WriteEndBlockExpression()
        {
            Out.ExitNestedScope();
            Out.Write("|]");
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

        public override void WriteBeginBlockExpression()
        {
            Out.EnterNestedScope();
            Out.Write('(');
            Out.WriteCoreOperatorName("seq");
            Out.WriteLine("{");
        }

        public override void WriteEndBlockExpression()
        {
            Out.ExitNestedScope();
            Out.Write("})");
        }
    }
}
