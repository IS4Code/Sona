using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class LogicExpression : BinaryState<LogicExprContext>
    {
        int notLevel;

        protected sealed override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            notLevel = 0;
        }

        protected override void OnEnterOperand(ParserRuleContext context)
        {
            if(notLevel == 0)
            {
                Out.Write('(');
            }
        }

        protected override void OnExitOperand(ParserRuleContext context)
        {
            if(notLevel == 0)
            {
                Out.Write(')');
            }
        }

        protected override void OnExit(LogicExprContext context)
        {
            while(notLevel-- > 0)
            {
                Out.Write(')');
            }
        }

        protected override void OnOperator(ITerminalNode node)
        {
            switch(node.Symbol.Type)
            {
                case SonaLexer.AND:
                    Out.WriteOperator("&&");
                    break;
                case SonaLexer.OR:
                    Out.WriteOperator("||");
                    break;
                case SonaLexer.NOT:
                    Out.WriteCoreOperatorName("not");
                    Out.Write('(');
                    notLevel++;
                    break;
            }
        }
    }

    internal sealed class AtomicLogicExpression : ExpressionState
    {
        int notLevel;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            notLevel = 0;
        }

        public override void EnterAtomicLogicExpr(AtomicLogicExprContext context)
        {

        }

        public override void ExitAtomicLogicExpr(AtomicLogicExprContext context)
        {
            while(notLevel-- > 0)
            {
                Out.Write(')');
            }
            ExitState().ExitAtomicLogicExpr(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
            if(node.Symbol.Type == SonaLexer.NOT)
            {
                Out.WriteCoreOperatorName("not");
                Out.Write('(');
                notLevel++;
            }
        }
    }

    internal sealed class InlineIfExpression : ExpressionState
    {
        public override void EnterInlineIfExpr(InlineIfExprContext context)
        {
            Out.Write("(if(");
        }

        public override void ExitInlineIfExpr(InlineIfExprContext context)
        {
            Out.Write("))");
            ExitState().ExitInlineIfExpr(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
            switch(node.Symbol.Type)
            {
                case SonaLexer.THEN:
                    Out.Write(")then(");
                    break;
                case SonaLexer.ELSE:
                    Out.Write(")else(");
                    break;
                case SonaLexer.ELSEIF:
                    Out.Write(")elif(");
                    break;
            }
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
