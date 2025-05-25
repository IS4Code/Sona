using Antlr4.Runtime;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class ConversionState : ExpressionState
    {
        int? type;
        bool typePresent;
        ISourceCapture? typeCapture;
        bool asOption;
        bool optionIsStruct;

        string Option => optionIsStruct ? "ValueOption" : "Option";
        string Some => optionIsStruct ? "ValueSome" : "Some";
        string None => optionIsStruct ? "ValueNone" : "None";

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            type = null;
            typePresent = false;
            typeCapture = null;
            asOption = false;
            optionIsStruct = false;
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

        public override void EnterGenericArgument(GenericArgumentContext context)
        {
            typePresent = true;
            switch(type)
            {
                case SonaLexer.ENUM:
                    Out.WriteCustomOperator("ConvertEnum");
                    Out.Write("<_,");
                    break;
                case SonaLexer.IMPLICIT:
                    Out.WriteCustomOperator("Implicit");
                    Out.Write("<_,");
                    break;
                case SonaLexer.EXPLICIT:
                    Out.WriteCustomOperator("Explicit");
                    Out.Write("<_,");
                    break;
                case SonaLexer.UNIT:
                    Out.WriteCustomOperator("ConvertUnit");
                    Out.Write("<_,_,");
                    break;
                case SonaLexer.SOME:
                case SonaLexer.NEW:
                case SonaLexer.WIDEN:
                case SonaLexer.NARROW:
                    typeCapture = Out.StartCapture();
                    break;
            }
            EnterState<GenericArgumentState>().EnterGenericArgument(context);
        }

        public override void ExitGenericArgument(GenericArgumentContext context)
        {
            switch(type)
            {
                case SonaLexer.ENUM:
                case SonaLexer.IMPLICIT:
                case SonaLexer.EXPLICIT:
                case SonaLexer.UNIT:
                    Out.Write('>');
                    break;
                case SonaLexer.SOME:
                case SonaLexer.NEW:
                case SonaLexer.WIDEN:
                case SonaLexer.NARROW:
                    Out.StopCapture(typeCapture ?? ErrorCapture("COMPILER ERROR: Missing type capture.", context));
                    break;
            }
        }

        public override void EnterOptionSuffix(OptionSuffixContext context)
        {
            asOption = true;
        }

        public override void ExitOptionSuffix(OptionSuffixContext context)
        {

        }

        void OnEnter(ParserRuleContext context)
        {
            var token = context.Start;
            type = token.Type;
            switch(type)
            {
                case SonaLexer.VOID:
                    Out.EnterNestedScope(true);
                    Out.Write("let _");
                    Out.WriteOperator('=');
                    break;
                case SonaLexer.OBJECT:
                case SonaLexer.NEW:
                case SonaLexer.SOME:
                case SonaLexer.ENUM:
                case SonaLexer.IMPLICIT:
                case SonaLexer.EXPLICIT:
                case SonaLexer.UNIT:
                case SonaLexer.WIDEN:
                case SonaLexer.NARROW:
                    // Wait for type/operand
                    break;
                default:
                    // Normal primitive type
                    type = null;
                    break;
            }
            optionIsStruct = LexerContext.GetState<OptionPragma>()?.IsStruct ?? true;
        }

        void OnOperand(ParserRuleContext context)
        {
            if(!typePresent)
            {
                // Same as with a type, just without arguments
                switch(type)
                {
                    case SonaLexer.ENUM:
                        Out.WriteCustomOperator("ConvertEnum");
                        break;
                    case SonaLexer.IMPLICIT:
                        Out.WriteCustomOperator("Implicit");
                        break;
                    case SonaLexer.EXPLICIT:
                        Out.WriteCustomOperator("Explicit");
                        break;
                    case SonaLexer.UNIT:
                        Out.WriteCustomOperator("ConvertUnit");
                        break;
                }
                // Continue next
            }
            if(asOption)
            {
                // An option type is requested
                switch(type)
                {
                    case SonaLexer.VOID:
                        // Doesn't affect the operation
                        return;
                    case SonaLexer.OBJECT:
                    case SonaLexer.NEW:
                    case SonaLexer.SOME:
                    case SonaLexer.WIDEN:
                        // Bind to result first
                        Out.Write("match ");
                        Out.WriteCustomOperator("BindToResult");
                        Out.Write('(');
                        return;
                    case SonaLexer.NARROW:
                        // Optionally bind to result first
                        Out.Write("match ");
                        Out.WriteCustomOperator("OptionalBindToResult");
                        Out.Write('(');
                        return;
                    case SonaLexer.ENUM:
                    case SonaLexer.IMPLICIT:
                    case SonaLexer.EXPLICIT:
                    case SonaLexer.UNIT:
                    case null:
                        // Conversion through function
                        Out.WriteOperator("|>");
                        Out.WriteCustomOperator(optionIsStruct ? "TryConversionValue" : "TryConversion");
                        Out.Write('(');
                        return;
                }
                Error("This conversion does not support returning an option type.", context);
            }
            if(typeCapture != null)
            {
                // A type was captured
                switch(type)
                {
                    case SonaLexer.NEW:
                        Out.Write("new ");
                        typeCapture.Play(Out);
                        typeCapture = null;
                        break;
                    case SonaLexer.SOME:
                        Out.WriteCoreName(Option);
                        Out.Write('<');
                        typeCapture.Play(Out);
                        typeCapture = null;
                        Out.Write(">.");
                        Out.WriteIdentifier(Some);
                        break;
                    case SonaLexer.WIDEN:
                    case SonaLexer.NARROW:
                        // Different operator path
                        Out.Write('(');
                        return;
                }
                // Continue next
            }
            switch(type)
            {
                case SonaLexer.SOME:
                    if(!typePresent)
                    {
                        Out.WriteCoreName(Some);
                    }
                    Out.Write('(');
                    return;
                case SonaLexer.WIDEN:
                    Out.Write("upcast(");
                    return;
                case SonaLexer.NARROW:
                    Out.Write("downcast(");
                    return;
                case SonaLexer.NEW:
                    if(!typePresent)
                    {
                        Out.Write("new _");
                    }
                    Out.Write('(');
                    return;
                case SonaLexer.ENUM:
                case SonaLexer.IMPLICIT:
                case SonaLexer.EXPLICIT:
                case SonaLexer.UNIT:
                case null:
                    Out.Write('(');
                    return;
            }
        }

        void OnExit()
        {
            switch(type)
            {
                case SonaLexer.VOID:
                    Out.ExitNestedScope();
                    Out.Write(" in ");
                    if(asOption)
                    {
                        Out.WriteCoreName(None);
                    }
                    else
                    {
                        Out.Write("()");
                    }
                    return;
                case SonaLexer.OBJECT:
                    if(asOption)
                    {
                        var id = MatchSome();
                        Out.WriteCoreName(Some);
                        Out.Write('(');
                        Out.WriteIdentifier(id);
                        Out.WriteOperator(":>");
                        Out.WriteCoreName("objnull");
                        Out.Write(')');
                        MatchNone();
                        Out.WriteCoreName(None);
                    }
                    else
                    {
                        Out.WriteOperator(":>");
                        Out.WriteCoreName("objnull");
                    }
                    return;
                case SonaLexer.NEW when asOption:
                {
                    var id = MatchSome();
                    Out.WriteCoreName(Some);
                    Out.Write('(');
                    Out.Write("new ");
                    if(typeCapture != null)
                    {
                        typeCapture.Play(Out);
                    }
                    else
                    {
                        Out.Write('_');
                    }
                    Out.Write('(');
                    Out.WriteIdentifier(id);
                    Out.Write("))");
                    MatchNone();
                    Out.WriteCoreName(None);
                    return;
                }
                case SonaLexer.SOME when asOption:
                {
                    var id = MatchSome();
                    if(typeCapture != null)
                    {
                        Out.WriteCoreName(Option);
                        Out.Write('<');
                        typeCapture.Play(Out);
                        Out.Write(">.");
                        Out.WriteIdentifier(Some);
                    }
                    else
                    {
                        Out.WriteCoreName(Some);
                    }
                    Out.Write(' ');
                    Out.WriteIdentifier(id);
                    MatchNone();
                    Out.WriteCoreName(None);
                    return;
                }
                case SonaLexer.WIDEN:
                    if(asOption)
                    {
                        var id = MatchSome();
                        Out.WriteCoreName(Some);
                        Out.Write('(');
                        if(typeCapture != null)
                        {
                            Out.WriteIdentifier(id);
                            Out.WriteOperator(":>");
                            typeCapture.Play(Out);
                        }
                        else
                        {
                            Out.Write("upcast ");
                            Out.WriteIdentifier(id);
                        }
                        Out.Write(')');
                        MatchNone();
                        Out.WriteCoreName(None);
                        return;
                    }
                    else if(typeCapture != null)
                    {
                        Out.WriteOperator(":>");
                        typeCapture.Play(Out);
                        return;
                    }
                    goto default;
                case SonaLexer.NARROW:
                    if(asOption)
                    {
                        var id = MatchDerivedSome();
                        Out.WriteCoreName(Some);
                        Out.Write(' ');
                        Out.WriteIdentifier(id);
                        MatchNone();
                        Out.WriteCoreName(None);
                        return;
                    }
                    else if(typeCapture != null)
                    {
                        Out.WriteOperator(":?>");
                        typeCapture.Play(Out);
                        return;
                    }
                    goto default;
                default:
                    Out.Write(')');
                    return;
            }

            string MatchSome()
            {
                Out.Write(")with|struct(true,");
                var id = Out.CreateTemporaryIdentifier();
                Out.WriteIdentifier(id);
                Out.Write(')');
                Out.WriteOperator("->");
                return id;
            }

            string MatchDerivedSome()
            {
                Out.Write(")with|struct(true,(");
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
                Out.Write("))");
                Out.WriteOperator("->");
                return id;
            }

            void MatchNone()
            {
                Out.Write("|_");
                Out.WriteOperator("->");
            }
        }
    }
}
