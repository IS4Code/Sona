using Antlr4.Runtime;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class ConversionState : ExpressionState
    {
        int? type;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            type = null;
        }

        public override void EnterMemberConvertExpr(MemberConvertExprContext context)
        {
            OnEnter(context);
        }

        public override void ExitMemberConvertExpr(MemberConvertExprContext context)
        {
            OnExit();
            ExitState().ExitMemberConvertExpr(context);
        }

        public override void EnterAtomicConvertExpr(AtomicConvertExprContext context)
        {
            OnEnter(context);
        }

        public override void ExitAtomicConvertExpr(AtomicConvertExprContext context)
        {
            OnExit();
            ExitState().ExitAtomicConvertExpr(context);
        }

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            if(type == null)
            {
                base.EnterPrimitiveType(context);
            }
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {
            if(type == null)
            {
                base.ExitPrimitiveType(context);
                Out.Write('(');
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        void OnEnter(ParserRuleContext context)
        {
            var token = context.Start;
            type = token.Type;
            switch(type)
            {
                case SonaLexer.SOME:
                    Out.WriteCoreName(LexerContext.GetState<OptionPragma>()?.IsStruct ?? true ? "ValueSome" : "Some");
                    Out.Write('(');
                    return;
                case SonaLexer.VOID:
                    Out.EnterNestedScope(true);
                    Out.Write("(let _");
                    Out.WriteOperator('=');
                    return;
                case SonaLexer.OBJECT:
                    Out.Write('(');
                    return;
            }
            // Normal primitive type
            type = null;
        }

        void OnExit()
        {
            switch(type)
            {
                case SonaLexer.VOID:
                    Out.ExitNestedScope();
                    Out.Write(" in ())");
                    return;
                case SonaLexer.OBJECT:
                    Out.WriteOperator(":>");
                    Out.WriteCoreName("objnull");
                    Out.Write(')');
                    return;
                default:
                    Out.Write(')');
                    return;
            }
        }
    }
}
