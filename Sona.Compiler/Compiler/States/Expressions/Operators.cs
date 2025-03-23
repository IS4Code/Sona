using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using IS4.Sona.Grammar;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal abstract class BinaryState<TContext> : ExpressionState where TContext : ParserRuleContext
    {
        ISourceCapture? capture;
        TContext? operationContext;
        ParserRuleContext? operandContext;
        int atomicLevel;

        protected ITerminalNode? FirstOperator { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            capture = null;
            operationContext = null;
            operandContext = null;
            FirstOperator = null;
            atomicLevel = 0;
        }

        private void EnterOperation(TContext context)
        {
            capture = Out.StartCapture();
            operationContext = context;
        }

        private void ExitOperation(TContext context)
        {
            if(capture != null)
            {
                // Exiting without an operator, just replay
                Out.StopCapture(capture);
                capture.Play(Out);
                capture = null;
                return;
            }
            OnExit(context);
        }

        private void Operator(ITerminalNode node)
        {
            if(capture != null)
            {
                // Store operator to be checked in derived classes
                FirstOperator = node;

                // Inner expression is already captured
                Out.StopCapture(capture);

                // Simulate entering operand
                OnEnter(operationContext!);
                OnEnterOperand(operandContext!);

                // Replay the expression
                capture.Play(Out);
                capture = null;

                // Simulate exiting operand
                OnExitOperand(operandContext!);
            }
            OnOperator(node);
        }

        private void EnterOperand(ParserRuleContext context)
        {
            if(capture == null)
            {
                // Direct
                OnEnterOperand(context);
            }
            else
            {
                // Wait until operator
                operandContext = context;
            }
        }

        private void ExitOperand(ParserRuleContext context)
        {
            if(capture == null)
            {
                // Direct
                OnExitOperand(context);
            }
            else
            {
                // Wait until operator
                operandContext = context;
            }
        }

        protected virtual void OnEnter(TContext context)
        {

        }

        protected virtual void OnExit(TContext context)
        {

        }

        protected virtual void OnEnterOperand(ParserRuleContext context)
        {

        }

        protected virtual void OnExitOperand(ParserRuleContext context)
        {

        }

        protected abstract void OnOperator(ITerminalNode node);

        public sealed override void EnterLogicExpr(LogicExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterLogicExpr(context);
            }
        }

        public sealed override void EnterOuterExpr(OuterExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterOuterExpr(context);
            }
        }

        public sealed override void EnterCoalesceExpr(CoalesceExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterCoalesceExpr(context);
            }
        }

        public sealed override void EnterConcatExpr(ConcatExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterConcatExpr(context);
            }
        }

        public sealed override void EnterBitOrExpr(BitOrExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterBitOrExpr(context);
            }
        }

        public sealed override void EnterBitXorExpr(BitXorExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterBitXorExpr(context);
            }
        }

        public sealed override void EnterBitAndExpr(BitAndExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterBitAndExpr(context);
            }
        }

        public sealed override void EnterBitShiftExpr(BitShiftExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterBitShiftExpr(context);
            }
        }

        public sealed override void EnterInnerExpr(InnerExprContext context)
        {
            if(context is TContext ctx)
            {
                EnterOperation(ctx);
            }
            else
            {
                EnterOperand(context);
                base.EnterInnerExpr(context);
            }
        }

        public override void ExitLogicExpr(LogicExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitLogicExpr(context);
            }
            else
            {
                base.ExitLogicExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitOuterExpr(OuterExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitOuterExpr(context);
            }
            else
            {
                base.ExitOuterExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitCoalesceExpr(CoalesceExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitCoalesceExpr(context);
            }
            else
            {
                base.ExitCoalesceExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitConcatExpr(ConcatExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitConcatExpr(context);
            }
            else
            {
                base.ExitConcatExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitBitOrExpr(BitOrExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitBitOrExpr(context);
            }
            else
            {
                base.ExitBitOrExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitBitXorExpr(BitXorExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitBitXorExpr(context);
            }
            else
            {
                base.ExitBitXorExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitBitAndExpr(BitAndExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitBitAndExpr(context);
            }
            else
            {
                base.ExitBitAndExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitBitShiftExpr(BitShiftExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitBitShiftExpr(context);
            }
            else
            {
                base.ExitBitShiftExpr(context);
                ExitOperand(context);
            }
        }

        public sealed override void ExitInnerExpr(InnerExprContext context)
        {
            if(context is TContext ctx)
            {
                ExitOperation(ctx);
                ExitState().ExitInnerExpr(context);
            }
            else
            {
                base.ExitInnerExpr(context);
                ExitOperand(context);
            }
        }

        // Ignore terminals in atomic expressions
        public sealed override void EnterAtomicExpr(AtomicExprContext context)
        {
            atomicLevel++;
        }

        public sealed override void ExitAtomicExpr(AtomicExprContext context)
        {
            atomicLevel--;
        }

        public sealed override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            if(atomicLevel > 0)
            {
                return;
            }

            switch(node.Symbol.Type)
            {
                case SonaLexer.EQ:
                case SonaLexer.NEQ:
                case SonaLexer.NEQ_ALT:
                case SonaLexer.PLUS:
                case SonaLexer.MINUS:
                case SonaLexer.ASTERISK:
                case SonaLexer.SLASH:
                case SonaLexer.PERCENT:
                case SonaLexer.CONCAT:
                case SonaLexer.LT:
                case SonaLexer.LTE:
                case SonaLexer.GT:
                case SonaLexer.GTE:
                case SonaLexer.DOUBLE_QUESTION:
                case SonaLexer.DOUBLE_AND:
                case SonaLexer.DOUBLE_OR:
                case SonaLexer.SINGLE_AND:
                case SonaLexer.SINGLE_OR:
                case SonaLexer.SINGLE_XOR:
                case SonaLexer.RSHIFT:
                case SonaLexer.LSHIFT:
                case SonaLexer.AND:
                case SonaLexer.OR:
                case SonaLexer.NOT:
                    Operator(node);
                    break;
            }
        }
    }

    internal sealed class StandardBinaryState<TContext> : BinaryState<TContext> where TContext : ParserRuleContext
    {
        // Wrap the whole expression in (...) if first operand is generated as =
        bool WrapInParentheses => FirstOperator is null or { Symbol: { Type: SonaLexer.EQ } };

        protected override void OnEnter(TContext context)
        {
            if(WrapInParentheses)
            {
                Out.Write('(');
            }
        }

        protected override void OnExit(TContext context)
        {
            if(WrapInParentheses)
            {
                Out.Write(')');
            }
        }

        protected override void OnOperator(ITerminalNode node)
        {
            var token = node.Symbol;
            string text = token.Text;
            switch(token.Type)
            {
                case SonaLexer.EQ:
                    text = "=";
                    break;
                case SonaLexer.NEQ:
                case SonaLexer.NEQ_ALT:
                    text = "<>";
                    break;
            }
            Out.WriteOperator(text);
        }
    }

    internal sealed class SpecialBinaryState<TContext> : BinaryState<TContext> where TContext : ParserRuleContext
    {
        protected override void OnEnterOperand(ParserRuleContext context)
        {
            Out.Write('(');
        }

        protected override void OnExitOperand(ParserRuleContext context)
        {
            Out.Write(')');
        }

        protected override void OnOperator(ITerminalNode node)
        {
            switch(node.Symbol.Type)
            {
                case SonaLexer.CONCAT:
                    if(IsLiteral)
                    {
                        Out.WriteOperator("+\"\"+");
                    }
                    else
                    {
                        Out.WriteSpecialBinaryOperator("Concat");
                    }
                    break;
                case SonaLexer.SINGLE_OR:
                    if(IsLiteral)
                    {
                        Out.WriteOperator("|||");
                    }
                    else
                    {
                        Out.WriteSpecialBinaryOperator("Pipe");
                    }
                    break;
                case SonaLexer.SINGLE_XOR:
                    if(IsLiteral)
                    {
                        Out.WriteOperator("^^^");
                    }
                    else
                    {
                        Out.WriteSpecialBinaryOperator("Hat");
                    }
                    break;
                case SonaLexer.SINGLE_AND:
                    if(IsLiteral)
                    {
                        Out.WriteOperator("&&&");
                    }
                    else
                    {
                        Out.WriteSpecialBinaryOperator("And");
                    }
                    break;
                case SonaLexer.LSHIFT:
                    if(IsLiteral)
                    {
                        Out.WriteOperator("<<<");
                    }
                    else
                    {
                        Out.WriteSpecialBinaryOperator("LeftShift");
                    }
                    break;
                case SonaLexer.RSHIFT:
                    if(IsLiteral)
                    {
                        Out.WriteOperator(">>>");
                    }
                    else
                    {
                        Out.WriteSpecialBinaryOperator("RightShift");
                    }
                    break;
            }
        }
    }

    internal abstract class RightAssociativeBinaryState<TContext> : BinaryState<TContext> where TContext : ParserRuleContext
    {
        TContext? operationContext;
        ParserRuleContext? operandContext;
        ISourceCapture? capture;
        int level;

        protected sealed override void OnEnter(TContext context)
        {
            capture = null;
            level = 0;
            operationContext = context;
            OnNestedEnter(context);
        }

        protected sealed override void OnExit(TContext context)
        {
            if(capture != null)
            {
                // Final operand is captured
                Out.StopCapture(capture);

                // Simulate entering final operand
                OnNestedEnterOperand(operandContext!);

                // Replay the expression
                capture.Play(Out);
                capture = null;

                // Simulate exiting final operand
                OnNestedExitOperand(operandContext!);
            }
            while(level-- > 0)
            {
                // Clear all previous levels
                OnNestedExit(context);
                OnNestedExitOperand(operandContext!);
            }
            OnNestedExit(context);
        }

        protected sealed override void OnEnterOperand(ParserRuleContext context)
        {
            if(capture == null)
            {
                // Direct
                OnNestedEnterOperand(context);
            }
            else
            {
                // Wait until operator
                operandContext = context;
            }
        }

        protected sealed override void OnExitOperand(ParserRuleContext context)
        {
            if(capture == null)
            {
                // Direct
                OnNestedExitOperand(context);
            }
            else
            {
                // Wait until operator
                operandContext = context;
            }
        }

        protected sealed override void OnOperator(ITerminalNode node)
        {
            if(capture != null)
            {
                // Non-final operand is captured
                Out.StopCapture(capture);

                // Simulate entering new operation
                level++;
                OnNestedEnterOperand(operandContext!);
                OnNestedEnter(operationContext!);
                OnNestedEnterOperand(operandContext!);

                // Replay the expression
                capture.Play(Out);
                capture = null;

                // Simulate exiting the operand
                OnNestedExitOperand(operandContext!);
            }
            OnNestedOperator(node);

            // Capture next operand until end or another operator
            capture = Out.StartCapture();
        }

        protected virtual void OnNestedEnter(TContext context)
        {

        }

        protected virtual void OnNestedExit(TContext context)
        {

        }

        protected virtual void OnNestedEnterOperand(ParserRuleContext context)
        {

        }

        protected virtual void OnNestedExitOperand(ParserRuleContext context)
        {

        }

        protected abstract void OnNestedOperator(ITerminalNode node);
    }

    internal sealed class CoalesceState : RightAssociativeBinaryState<CoalesceExprContext>
    {
        string? alternativeName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            alternativeName = null;
        }

        protected override void OnNestedEnter(CoalesceExprContext context)
        {
            // Expose the type of the alternative through a variable
            Out.Write("(let ");
            alternativeName = Out.CreateTemporaryIdentifier();
            Out.WriteIdentifier(alternativeName);
            Out.WriteOperator('=');
            Out.WriteCoreOperatorName("Unchecked");
            Out.Write(".defaultof<_> in match ");
            Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Operators", "BindToLiftedResult");
            Out.Write('(');
            Out.WriteIdentifier(alternativeName);
            Out.Write(")(");
        }

        protected override void OnNestedOperator(ITerminalNode node)
        {
            // After first operand
            Out.Write(")with|struct(true,");
            var arg = Out.CreateTemporaryIdentifier();
            Out.WriteIdentifier(arg);
            Out.Write(")->");
            Out.WriteIdentifier(arg);
            Out.Write("|struct(false,_)->(if false then ");
            Out.WriteIdentifier(alternativeName ?? Error("COMPILER ERROR: Temporary variable missing.", node));
            Out.Write(" else(");
            alternativeName = null;
        }

        protected override void OnNestedExit(CoalesceExprContext context)
        {
            Out.Write(")))");
        }
    }
}
