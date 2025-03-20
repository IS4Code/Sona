using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using IS4.Sona.Grammar;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
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
                Out.Write(")");
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

    internal sealed class NegatedExpression : ExpressionState
    {
        int notLevel;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            notLevel = 0;
        }

        public override void EnterNegatedExpr(NegatedExprContext context)
        {

        }

        public override void ExitNegatedExpr(NegatedExprContext context)
        {
            while(notLevel-- > 0)
            {
                Out.Write(")");
            }
            ExitState().ExitNegatedExpr(context);
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
}
