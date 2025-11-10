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

        bool closeReturnStatement, closeVariableStatement;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            closeReturnStatement = false;
            closeVariableStatement = false;
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
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                    closeReturnStatement = true;
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
                }
            }
        }

        protected override void OnComputationExit(StatementFlags flags, ParserRuleContext context)
        {
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

    internal sealed class NewFollowVariableState : NewVariableState
    {
        readonly List<ISourceCapture> captures = new();
        readonly List<string> variables = new();

        ISourceCapture? capture;
        bool first;
        bool isUse;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            captures.Clear();
            variables.Clear();
            capture = null;
            first = true;
        }

        public override void EnterFollowVariableDecl(FollowVariableDeclContext context)
        {
            // Capture any attributes
            capture = Out.StartCapture();
        }

        public override void ExitFollowVariableDecl(FollowVariableDeclContext context)
        {
            if(captures.Count > 0)
            {
                if(IsUse)
                {
                    Error("A `use` declaration initializing multiple variables with `follow` is not supported.", context);
                }

                Out.WriteLine();

                // Play the prolog
                captures[0].Play(Out);

                if(captures.Count == 2)
                {
                    // Exactly one variable, just play out
                    captures[1].Play(Out);
                    Out.WriteOperator('=');
                    Out.WriteIdentifier(variables[0]);
                }
                else
                {
                    // Assign via a tuple
                    Out.Write('(');
                    bool first = true;
                    foreach(var capture in captures.Skip(1))
                    {
                        Out.WriteNext(',', ref first);
                        capture.Play(Out);
                    }
                    Out.Write(')');
                    Out.WriteOperator('=');
                    Out.Write('(');
                    first = true;
                    foreach(var variable in variables)
                    {
                        Out.WriteNext(',', ref first);
                        Out.WriteIdentifier(variable);
                    }
                    Out.Write(')');
                }
            }

            ExitState().ExitFollowVariableDecl(context);
        }

        protected override void WriteRec(ParserRuleContext context)
        {
            if(LexerContext.GetState<RecursivePragma>()?.Value ?? false)
            {
                Error("The use of `#pragma recursive` is not supported together with a `follow`-initialized variable.", context);
            }
        }

        private void FlushCapture()
        {
            if(capture != null)
            {
                // Buffering was not needed because this can be expressed without a temporary variable
                Out.StopCapture(capture);
                capture.Play(Out);
                capture = null;
            }
        }

        public override void EnterLet(LetContext context)
        {
            FlushCapture();
            // ( needed for syntax
            Out.Write("let! (");
            WriteRec(context);
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            if(first)
            {
                if(capture != null)
                {
                    // Recording prolog before the first declaration, needs to be stored separately
                    Out.StopCapture(capture);
                    captures.Add(capture);

                    // Another one for the variable
                    capture = Out.StartCapture();
                }
            }
            else
            {
                Out.WriteLine();
                Out.Write("and! ");

                if(capture != null)
                {
                    // This one was already stopped before the expression
                    capture = Out.StartCapture();
                }
                else
                {
                    // Direct pattern
                    Out.Write('(');
                }
            }
            base.EnterDeclaration(context);
        }

        public sealed override void EnterUnaryExpr(UnaryExprContext context)
        {
            if(capture != null)
            {
                // Store capture for the pattern
                Out.StopCapture(capture);
                captures.Add(capture);
                // Do not set to null (will be checked on next declaration)

                if(first)
                {
                    // Real destination
                    Out.Write("let! ");
                }
                else
                {
                    // `and!` is already prepared
                }

                // We need a temporary variable to store the result
                var name = Out.CreateTemporaryIdentifier();
                Out.WriteIdentifier(name);
                variables.Add(name);
            }
            else
            {
                // End ( from let! or and!
                Out.Write(')');
            }
            Out.WriteOperator('=');
            EnterState<ExpressionState.Unary>().EnterUnaryExpr(context);
        }

        public sealed override void ExitUnaryExpr(UnaryExprContext context)
        {
            first = false;
        }
    }
}
