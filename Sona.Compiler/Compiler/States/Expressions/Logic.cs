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
                    if(IsConstant)
                    {
                        Out.WriteCoreOperatorName("not");
                    }
                    else
                    {
                        Out.WriteCustomUnaryOperator("Not");
                    }
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
                if(IsConstant)
                {
                    Out.WriteCoreOperatorName("not");
                }
                else
                {
                    Out.WriteCustomUnaryOperator("Not");
                }
                Out.Write('(');
                notLevel++;
            }
        }
    }

    internal sealed class InlineIfExpression : ExpressionState
    {
        bool hasMatch;
        int level;
        ISourceCapture? patternCapture;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasMatch = false;
            level = 0;
            patternCapture = null;
        }

        public override void EnterInlineIfExpr(InlineIfExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitInlineIfExpr(InlineIfExprContext context)
        {
            while(level > 0)
            {
                Out.Write(')');
                level--;
            }
            // One more due to `else` not starting a normal expression
            Out.Write("))");
            ExitState().ExitInlineIfExpr(context);
        }

        public sealed override void EnterIf(IfContext context)
        {
            Out.Write("if");
        }

        public sealed override void ExitIf(IfContext context)
        {
            Out.Write("then");
            hasMatch = false;
        }

        public sealed override void EnterCaseIf(CaseIfContext context)
        {
            Out.Write("match");
        }

        public sealed override void ExitCaseIf(CaseIfContext context)
        {
            Out.WriteOperator("->");
            hasMatch = true;
        }

        public sealed override void EnterElseif(ElseifContext context)
        {
            if(hasMatch)
            {
                level++;
                Out.Write("|_");
                Out.WriteOperator("->");
                Out.Write("(if");
            }
            else
            {
                Out.Write("elif");
            }
        }

        public sealed override void ExitElseif(ElseifContext context)
        {
            Out.Write("then");
            hasMatch = false;
        }

        public sealed override void EnterCaseElseif(CaseElseifContext context)
        {
            if(hasMatch)
            {
                Out.Write("|_");
                Out.WriteOperator("->");
            }
            else
            {
                Out.Write("else");
            }
            level++;
            Out.Write("(match");
        }

        public sealed override void ExitCaseElseif(CaseElseifContext context)
        {
            Out.WriteOperator("->");
            hasMatch = true;
        }

        public sealed override void EnterElse(ElseContext context)
        {
            if(hasMatch)
            {
                Out.Write("|_");
                Out.WriteOperator("->");
            }
            else
            {
                Out.Write("else");
            }
            // Due to not starting a normal expression
            Out.Write('(');
        }

        public sealed override void ExitElse(ElseContext context)
        {

        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.Write('(');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            Out.Write(')');
            if(patternCapture != null)
            {
                patternCapture.Play(Out);
                patternCapture = null;
            }
        }

        public override void EnterPattern(PatternContext context)
        {
            if(patternCapture != null)
            {
                Error("COMPILER ERROR: Entered pattern while another is being captured.", context);
            }
            patternCapture = Out.StartCapture();
            Out.Write("with|(");
            base.EnterPattern(context);
        }

        public sealed override void ExitPattern(PatternContext context)
        {
            base.ExitPattern(context);
            Out.Write(')');
            if(patternCapture != null)
            {
                Out.StopCapture(patternCapture);
            }
        }

        public sealed override void EnterWhenClause(WhenClauseContext context)
        {
            Out.Write("when");
        }

        public sealed override void ExitWhenClause(WhenClauseContext context)
        {

        }
    }
}
