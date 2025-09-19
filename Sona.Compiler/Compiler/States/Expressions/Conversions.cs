using System;
using System.Globalization;
using System.Numerics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
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
        ImplementationType optionType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            type = null;
            typePresent = false;
            typeCapture = null;
            asOption = false;
            optionType = default;
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
            optionType = LexerContext.GetState<OptionPragma>()?.Type ?? ImplementationType.Struct;
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
                        Out.WriteCustomOptionConversionOperator(optionType);
                        Out.Write('(');
                        return;
                }
                Error("This conversion does not support returning an option type.", context);
                Out.Write('(');
                return;
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
                        Out.WriteOptionType(optionType);
                        Out.Write('<');
                        typeCapture.Play(Out);
                        typeCapture = null;
                        Out.Write(">.");
                        Out.WriteOptionSomeIdentifier(optionType);
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
                        Out.WriteOptionSome(optionType);
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
                        Out.WriteOptionNone(optionType);
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
                        Out.WriteOptionSome(optionType);
                        Out.Write('(');
                        Out.WriteIdentifier(id);
                        Out.WriteOperator(":>");
                        Out.WriteCoreName("objnull");
                        Out.Write(')');
                        MatchNone();
                        Out.WriteOptionNone(optionType);
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
                    Out.WriteOptionSome(optionType);
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
                    Out.WriteOptionNone(optionType);
                    return;
                }
                case SonaLexer.SOME when asOption:
                {
                    var id = MatchSome();
                    if(typeCapture != null)
                    {
                        Out.WriteOptionType(optionType);
                        Out.Write('<');
                        typeCapture.Play(Out);
                        Out.Write(">.");
                        Out.WriteOptionSomeIdentifier(optionType);
                    }
                    else
                    {
                        Out.WriteOptionSome(optionType);
                    }
                    Out.Write(' ');
                    Out.WriteIdentifier(id);
                    MatchNone();
                    Out.WriteOptionNone(optionType);
                    return;
                }
                case SonaLexer.WIDEN:
                    if(asOption)
                    {
                        var id = MatchSome();
                        Out.WriteOptionSome(optionType);
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
                        Out.WriteOptionNone(optionType);
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
                        Out.WriteOptionSome(optionType);
                        Out.Write(' ');
                        Out.WriteIdentifier(id);
                        MatchNone();
                        Out.WriteOptionNone(optionType);
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
                Out.Write(")with|struct(true,(:? ");
                if(typeCapture != null)
                {
                    Out.Write('(');
                    typeCapture.Play(Out);
                    Out.Write(')');
                }
                else
                {
                    Out.Write('^');
                    Out.WriteIdentifier(Out.CreateTemporaryIdentifier());
                    Out.Write(' ');
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

    internal sealed class NumberConversionState : ExpressionState
    {
        string? suffix;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            suffix = null;
        }

        public override void EnterMemberNumberConvertExpr(MemberNumberConvertExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitMemberNumberConvertExpr(MemberNumberConvertExprContext context)
        {
            OnExit();
            ExitState().ExitMemberNumberConvertExpr(context);
        }

        public override void EnterAtomicNumberConvertExpr(AtomicNumberConvertExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitAtomicNumberConvertExpr(AtomicNumberConvertExprContext context)
        {
            OnExit();
            ExitState().ExitAtomicNumberConvertExpr(context);
        }

        void OnExit()
        {
            if(suffix == null)
            {
                Out.Write(')');
            }
            Out.Write(')');
        }

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            switch(context.Start.Type)
            {
                case SonaLexer.INT8:
                    suffix = "y";
                    break;
                case SonaLexer.UINT8:
                    suffix = "uy";
                    break;
                case SonaLexer.INT16:
                    suffix = "s";
                    break;
                case SonaLexer.UINT16:
                    suffix = "us";
                    break;
                case SonaLexer.INT:
                case SonaLexer.INT32:
                    suffix = "l";
                    break;
                case SonaLexer.UINT:
                case SonaLexer.UINT32:
                    suffix = "ul";
                    break;
                case SonaLexer.INT64:
                    suffix = "L";
                    break;
                case SonaLexer.UINT64:
                    suffix = "UL";
                    break;
                case SonaLexer.NATIVEINT:
                    suffix = "n";
                    break;
                case SonaLexer.UNATIVEINT:
                    suffix = "un";
                    break;
                case SonaLexer.BIGINT:
                    suffix = "I";
                    break;
                case SonaLexer.FLOAT32:
                    suffix = "f";
                    break;
                case SonaLexer.FLOAT:
                case SonaLexer.FLOAT64:
                    suffix = "";
                    break;
                case SonaLexer.DECIMAL:
                    suffix = "M";
                    break;
                default:
                    base.EnterPrimitiveType(context);
                    break;
            }
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {
            if(suffix == null)
            {
                base.ExitPrimitiveType(context);
                Out.Write('(');
            }
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
            var token = node.Symbol;
            var text = token.Text;
            if(token.Type is SonaLexer.PLUS or SonaLexer.MINUS)
            {
                Out.WriteOperator(text);
                return;
            }
            if(suffix == null)
            {
                if(token.Type is SonaLexer.INT_LITERAL or SonaLexer.FLOAT_LITERAL or SonaLexer.EXP_LITERAL or SonaLexer.HEX_LITERAL)
                {
                    Out.Write(text);
                }
                return;
            }
            switch(token.Type)
            {
                case SonaLexer.INT_LITERAL:
                    Out.Write(text);
                    if(suffix == "")
                    {
                        // No specific double suffix
                        Out.Write(".0");
                    }
                    else
                    {
                        Out.Write(suffix);
                    }
                    break;
                case SonaLexer.FLOAT_LITERAL:
                    Out.Write(text);
                    Out.Write(suffix);
                    break;
                case SonaLexer.EXP_LITERAL:
                    if(suffix is "" or "f" or "M")
                    {
                        goto case SonaLexer.FLOAT_LITERAL;
                    }
                    else if(Decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                    {
                        if(number.IsInteger())
                        {
                            text = number.ToString(CultureInfo.InvariantCulture);
                            goto case SonaLexer.INT_LITERAL;
                        }
                    }
                    var e = text.IndexOf("e", StringComparison.OrdinalIgnoreCase);
                    if(e != -1)
                    {
                        if(text.Length - e - 1 > 3)
                        {
                            // Exponent has over 3 digits
                            Error($"The literal value '{text}' is too big.", node);
                            goto case SonaLexer.FLOAT_LITERAL;
                        }
                        if(BigInteger.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var bigNumber))
                        {
                            text = bigNumber.ToString(CultureInfo.InvariantCulture);
                            goto case SonaLexer.INT_LITERAL;
                        }
                    }
                    Error($"The literal value '{text}' has a fractional part, which is not representable as an integer.", node);
                    goto case SonaLexer.FLOAT_LITERAL;
                case SonaLexer.HEX_LITERAL:
                    if(suffix is "" or "f" or "M")
                    {
                        // Use the initial 0 to keep the sign positive
                        if(!BigInteger.TryParse(text.Replace("x", "").Replace("X", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number))
                        {
                            Error($"The literal value '{text}' cannot be parsed.", node);
                            goto case SonaLexer.FLOAT_LITERAL;
                        }
                        text = number.ToString(CultureInfo.InvariantCulture);
                    }
                    goto case SonaLexer.INT_LITERAL;
            }
        }
    }

    internal sealed class CharConversionState : ExpressionState
    {
        int? type;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            type = null;
        }

        public override void EnterMemberCharConvertExpr(MemberCharConvertExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitMemberCharConvertExpr(MemberCharConvertExprContext context)
        {
            OnExit();
            ExitState().ExitMemberCharConvertExpr(context);
        }

        public override void EnterAtomicCharConvertExpr(AtomicCharConvertExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitAtomicCharConvertExpr(AtomicCharConvertExprContext context)
        {
            OnExit();
            ExitState().ExitAtomicCharConvertExpr(context);
        }

        void OnExit()
        {
            if(type == null)
            {
                Out.Write(')');
            }
            Out.Write(')');
        }

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            type = context.Start.Type;
            switch(type)
            {
                case SonaLexer.INT8:
                    Out.WriteCoreOperatorName("sbyte");
                    Out.Write(' ');
                    break;
                case SonaLexer.UINT8:
                    break;
                default:
                    type = null;
                    base.EnterPrimitiveType(context);
                    break;
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

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
            var token = node.Symbol;
            switch(token.Type)
            {
                case SonaLexer.BEGIN_CHAR:
                    Out.Write('\'');
                    break;
                case SonaLexer.END_CHAR:
                    Out.Write('\'');
                    if(type != null)
                    {
                        Out.Write('B');
                    }
                    break;
                case SonaLexer.CHAR_PART:
                    Out.Write(token.Text);
                    break;
            }
        }
    }
}
