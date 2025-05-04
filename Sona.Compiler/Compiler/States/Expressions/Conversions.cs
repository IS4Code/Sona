using Antlr4.Runtime;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class ConversionState : ExpressionState
    {
        int? type;
        ISourceCapture? typeCapture;
        bool testConversion;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            type = null;
            typeCapture = null;
            testConversion = false;
        }

        public override void EnterMemberConvertExpr(MemberConvertExprContext context)
        {
            Out.Write('(');
            OnEnter(context);
        }

        public override void ExitMemberConvertExpr(MemberConvertExprContext context)
        {
            OnExit();
            Out.Write(')');
            ExitState().ExitMemberConvertExpr(context);
        }

        public override void EnterAtomicConvertExpr(AtomicConvertExprContext context)
        {
            Out.Write('(');
            OnEnter(context);
        }

        public override void ExitAtomicConvertExpr(AtomicConvertExprContext context)
        {
            OnExit();
            Out.Write(')');
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
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            OnOperand(context);
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        public override void EnterAtomicExpr(AtomicExprContext context)
        {
            OnOperand(context);
            EnterState<Operand>().EnterAtomicExpr(context);
        }

        public override void ExitAtomicExpr(AtomicExprContext context)
        {

        }

        class Operand : ExpressionState
        {
            public override void EnterAtomicExpr(AtomicExprContext context)
            {

            }

            public override void ExitAtomicExpr(AtomicExprContext context)
            {
                ExitState().ExitAtomicExpr(context);
            }
        }

        public override void EnterType(TypeContext context)
        {
            typeCapture = Out.StartCapture();
            base.EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {
            base.ExitType(context);
            Out.StopCapture(typeCapture ?? ErrorCapture("COMPILER ERROR: Missing type capture.", context));
        }

        public override void EnterTestConversion(TestConversionContext context)
        {
            switch(type)
            {
                case null or SonaLexer.NARROW:
                    break;
                default:
                    Error("This conversion always succeeds; use `some` to wrap the value in an option type.", context);
                    return;
            }

            testConversion = true;
        }

        public override void ExitTestConversion(TestConversionContext context)
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
                case SonaLexer.WIDEN:
                case SonaLexer.NARROW:
                    // Wait for the rest
                    return;
            }
            // Normal primitive type
            type = null;
        }

        void OnOperand(ParserRuleContext context)
        {
            if(testConversion)
            {
                // An option type is requested
                switch(type)
                {
                    case SonaLexer.NARROW:
                        Out.Write("(match(");
                        return;
                    case null:
                        // An arbitrary type constructor
                        Out.WriteOperator("|>");
                        bool isStruct = LexerContext.GetState<OptionPragma>()?.IsStruct ?? true;
                        Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Operators", isStruct ? "TryConversionValue" : "TryConversion");
                        Out.Write('(');
                        return;
                }
            }
            else if(typeCapture != null)
            {
                // A type was captured
                switch(type)
                {
                    case SonaLexer.WIDEN:
                    case SonaLexer.NARROW:
                        Out.Write('(');
                        return;
                }
            }
            else
            {
                switch(type)
                {
                    case SonaLexer.WIDEN:
                        Out.Write("upcast(");
                        return;
                    case SonaLexer.NARROW:
                        Out.Write("downcast(");
                        return;
                    case null:
                        Out.Write('(');
                        return;
                }
            }
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
                case SonaLexer.WIDEN when typeCapture != null:
                    Out.WriteOperator(":>");
                    typeCapture.Play(Out);
                    Out.Write(')');
                    return;
                case SonaLexer.NARROW:
                    if(testConversion)
                    {
                        Out.Write(")with|");
                        Out.WriteOperator(":?");
                        if(typeCapture != null)
                        {
                            Out.Write('(');
                            typeCapture.Play(Out);
                            Out.Write(')');
                        }
                        else
                        {
                            Out.Write("_ ");
                        }
                        Out.Write("as ");
                        var id = Out.CreateTemporaryIdentifier();
                        Out.WriteIdentifier(id);
                        Out.WriteOperator("->");
                        bool isStruct = LexerContext.GetState<OptionPragma>()?.IsStruct ?? true;
                        Out.WriteCoreName(isStruct ? "ValueSome" : "Some");
                        Out.Write(' ');
                        Out.WriteIdentifier(id);
                        Out.Write("|_");
                        Out.WriteOperator("->");
                        Out.WriteCoreName(isStruct ? "ValueNone" : "None");
                        Out.Write(')');
                        return;
                    }
                    else if(typeCapture != null)
                    {
                        Out.WriteOperator(":?>");
                        typeCapture.Play(Out);
                        Out.Write(')');
                        return;
                    }
                    goto default;
                default:
                    Out.Write(')');
                    return;
            }
        }
    }
}
