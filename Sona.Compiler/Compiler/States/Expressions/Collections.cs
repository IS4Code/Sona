using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime;
using Sona.Compiler.Tools;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class IsolatedState : NodeState, IFunctionContext
    {
        IExpressionContext? scope;
        BindingSet bindings;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = null;
            bindings = new(FindContext<IBindingContext>());
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

        void IBindingContext.Set(string name, BindingKind kind)
        {
            bindings.Set(name, kind);
        }

        BindingKind IBindingContext.Get(string name)
        {
            return bindings.Get(name);
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

        public override void EnterExpression(ExpressionContext context)
        {
            Out.Write("yield ");
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            hasYielded = true;
        }

        public sealed override void EnterCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
            Out.Write("yield ");
            EnterState<Field>().EnterCollectionFieldExpression(context);
        }

        public sealed override void ExitCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
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

        public sealed override void EnterFollowExpression(FollowExpressionContext context)
        {
            EnterState<YieldFollowOperand>().EnterFollowExpression(context);
        }

        public sealed override void ExitFollowExpression(FollowExpressionContext context)
        {
            hasYielded = true;
        }

        public sealed override void EnterSpreadFollowExpression(SpreadFollowExpressionContext context)
        {
            EnterState<YieldSpreadFollowOperand>().EnterSpreadFollowExpression(context);
        }

        public sealed override void ExitSpreadFollowExpression(SpreadFollowExpressionContext context)
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

        sealed class Field : NodeState
        {
            public sealed override void EnterCollectionFieldExpression(CollectionFieldExpressionContext context)
            {
                Out.WriteNamespacedName("System.Collections.Generic", "KeyValuePair");
                Out.Write("<_,_>(");
            }

            public sealed override void ExitCollectionFieldExpression(CollectionFieldExpressionContext context)
            {
                Out.Write(')');
                ExitState().ExitCollectionFieldExpression(context);
            }

            public sealed override void EnterAssignment(AssignmentContext context)
            {
                Out.Write(',');
            }

            public sealed override void ExitAssignment(AssignmentContext context)
            {

            }

            public override void EnterExpression(ExpressionContext context)
            {
                EnterState<ExpressionState>().EnterExpression(context);
            }

            public override void ExitExpression(ExpressionContext context)
            {

            }
        }

        abstract class FollowOperand : ExpressionState
        {
            string? variableName;

            protected sealed override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                variableName = null;
            }

            protected void OnEnter(ParserRuleContext context)
            {
                var computationScope = FindContext<IComputationContext>();
                if(computationScope?.HasFlag(ComputationFlags.IsComputation) ?? false)
                {
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
        }

        sealed class YieldFollowOperand : FollowOperand
        {
            public override void EnterFollowExpression(FollowExpressionContext context)
            {
                OnEnter(context);
            }

            public override void ExitFollowExpression(FollowExpressionContext context)
            {
                OnExit(context);
                ExitState().ExitFollowExpression(context);
            }

            protected override void OnStatement(ParserRuleContext context)
            {
                Out.Write("yield ");
            }
        }

        sealed class YieldSpreadFollowOperand : FollowOperand
        {
            public override void EnterSpreadFollowExpression(SpreadFollowExpressionContext context)
            {
                OnEnter(context);
            }

            public override void ExitSpreadFollowExpression(SpreadFollowExpressionContext context)
            {
                OnExit(context);
                ExitState().ExitSpreadFollowExpression(context);
            }

            protected override void OnStatement(ParserRuleContext context)
            {
                Out.Write("yield! ");
            }
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
                Defaults.WriteImplicitReturnStatement(context);
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
                Defaults.WriteImplicitReturnStatement(context);
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

    internal sealed class ComputationSequenceState : CollectionState
    {
        public string? BuilderVariable { get; private set; }
        bool initialized;

        [MemberNotNullWhen(true, nameof(BuilderVariable))]
        bool IsComputation => BuilderVariable != null;

        public override ComputationFlags Flags => base.Flags | ComputationFlags.IsComputation;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            BuilderVariable = null;
            initialized = false;
        }

        public override void EnterComputationSequenceConstructor(ComputationSequenceConstructorContext context)
        {

        }

        public override void ExitComputationSequenceConstructor(ComputationSequenceConstructorContext context)
        {
            if(IsEmpty)
            {
                Out.WriteLine();
                // Yield at least once to require computation support
                Out.Write("if false then yield ");
                Out.WriteDefaultValue();
            }
            Out.WriteLine();
            Defaults.WriteImplicitReturnStatement(context);
            Out.WriteLine();
            Out.ExitNestedScope();
            Out.Write("})");
            ExitState().ExitComputationSequenceConstructor(context);
        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            if(initialized)
            {
                base.EnterExpression(context);
                return;
            }

            BuilderVariable = Out.CreateTemporaryIdentifier();

            Out.EnterNestedScope();
            Out.Write("(let ");
            Out.WriteIdentifier(BuilderVariable);
            Out.WriteOperator('=');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {
            if(initialized)
            {
                base.ExitExpression(context);
                return;
            }

            initialized = true;
            Out.Write(" in ");
            Out.WriteIdentifier(BuilderVariable!);
            Out.Write(" { ");
        }

        public override void WriteBeginBlockExpression(ParserRuleContext context)
        {
            if(IsComputation)
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteIdentifier(BuilderVariable);
                Out.WriteLine('{');
            }
            else
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteCoreOperatorName("seq");
                Out.WriteLine('{');
            }
        }

        public override void WriteEndBlockExpression(ParserRuleContext context)
        {
            Out.ExitNestedScope();
            Out.Write("})");
        }
    }
}
