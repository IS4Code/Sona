using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class ExpressionState : NodeState
    {
        IExpressionContext? scope;

        protected bool IsLiteral => GetExpressionContext()?.IsLiteral ?? false;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = null;
        }

        protected sealed override IExpressionContext? GetExpressionContext()
        {
            return scope ??= FindContext<IExpressionContext>();
        }

        public override void EnterExpression(ExpressionContext context)
        {

        }

        public override void ExitExpression(ExpressionContext context)
        {
            ExitState().ExitExpression(context);
        }

        public override void EnterMemberExpr(MemberExprContext context)
        {
            EnterState<MemberExprState>().EnterMemberExpr(context);
        }

        public override void EnterAltMemberExpr(AltMemberExprContext context)
        {
            EnterState<AltMemberExprState>().EnterAltMemberExpr(context);
        }

        public override void EnterFuncExpr(FuncExprContext context)
        {
            EnterState<FunctionExprState>().EnterFuncExpr(context);
        }

        public override void EnterInlineExpr(InlineExprContext context)
        {
            EnterState<InlineStatementState>().EnterInlineExpr(context);
        }

        public override void EnterAtomicConvertExpr(AtomicConvertExprContext context)
        {
            EnterState<ConversionState>().EnterAtomicConvertExpr(context);
        }

        public override void ExitAtomicConvertExpr(AtomicConvertExprContext context)
        {

        }

        public override void EnterAtomicLogicExpr(AtomicLogicExprContext context)
        {
            EnterState<AtomicLogicExpression>().EnterAtomicLogicExpr(context);
        }

        public override void ExitAtomicLogicExpr(AtomicLogicExprContext context)
        {

        }

        public override void EnterLogicExpr(LogicExprContext context)
        {
            EnterState<LogicExpression>().EnterLogicExpr(context);
        }

        public override void ExitLogicExpr(LogicExprContext context)
        {

        }

        public override void EnterInlineIfExpr(InlineIfExprContext context)
        {
            EnterState<InlineIfExpression>().EnterInlineIfExpr(context);
        }

        public override void ExitInlineIfExpr(InlineIfExprContext context)
        {

        }

        public sealed override void EnterInlineSourceFree(InlineSourceFreeContext context)
        {
            Out.EnterNestedScope();
            Out.Write('(');
            EnterState<InlineSource>().EnterInlineSourceFree(context);
        }

        public sealed override void ExitInlineSourceFree(InlineSourceFreeContext context)
        {
            Out.WriteLine();
            Out.ExitNestedScope();
            Out.Write(')');
        }

        public override void EnterBooleanExpr(BooleanExprContext context)
        {
            EnterState<StandardBinaryState<BooleanExprContext>>().EnterBooleanExpr(context);
        }

        public override void ExitBooleanExpr(BooleanExprContext context)
        {

        }

        public override void EnterRelationalExpr(RelationalExprContext context)
        {
            EnterState<RelationalBinaryState<RelationalExprContext>>().EnterRelationalExpr(context);
        }

        public override void ExitRelationalExpr(RelationalExprContext context)
        {

        }

        public override void EnterCoalesceExpr(CoalesceExprContext context)
        {
            EnterState<CoalesceState>().EnterCoalesceExpr(context);
        }

        public override void ExitCoalesceExpr(CoalesceExprContext context)
        {

        }

        public override void EnterConcatExpr(ConcatExprContext context)
        {
            EnterState<SpecialBinaryState<ConcatExprContext>>().EnterConcatExpr(context);
        }

        public override void ExitConcatExpr(ConcatExprContext context)
        {

        }

        public override void EnterBitOrExpr(BitOrExprContext context)
        {
            EnterState<SpecialBinaryState<BitOrExprContext>>().EnterBitOrExpr(context);
        }

        public override void ExitBitOrExpr(BitOrExprContext context)
        {

        }

        public override void EnterBitXorExpr(BitXorExprContext context)
        {
            EnterState<SpecialBinaryState<BitXorExprContext>>().EnterBitXorExpr(context);
        }

        public override void ExitBitXorExpr(BitXorExprContext context)
        {

        }

        public override void EnterBitAndExpr(BitAndExprContext context)
        {
            EnterState<SpecialBinaryState<BitAndExprContext>>().EnterBitAndExpr(context);
        }

        public override void ExitBitAndExpr(BitAndExprContext context)
        {

        }

        public override void EnterBitShiftExpr(BitShiftExprContext context)
        {
            EnterState<SpecialBinaryState<BitShiftExprContext>>().EnterBitShiftExpr(context);
        }

        public override void ExitBitShiftExpr(BitShiftExprContext context)
        {

        }

        public override void EnterInnerExpr(InnerExprContext context)
        {
            EnterState<StandardBinaryState<InnerExprContext>>().EnterInnerExpr(context);
        }

        public override void ExitInnerExpr(InnerExprContext context)
        {

        }

        public override void EnterHashExpr(HashExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitHashExpr(HashExprContext context)
        {
            Out.Write(')');
            if(IsLiteral)
            {
                Out.Write(".Length");
            }
            else
            {
                Out.WriteSpecialUnaryOperator("Length");
            }
        }

        public override void EnterNotExpr(NotExprContext context)
        {
            Out.WriteCoreOperatorName("not");
            Out.Write('(');
        }

        public override void ExitNotExpr(NotExprContext context)
        {
            Out.Write(')');
        }

        public override void EnterUnit(UnitContext context)
        {
            Out.Write("(()");
            Out.WriteOperator(':');
            Out.WriteCoreName("unit");
            Out.Write(')');
        }

        public override void ExitUnit(UnitContext context)
        {

        }

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            var token = context.Start;
            string text;
            switch(token.Type)
            {
                case SonaLexer.OBJECT:
                    text = "box";
                    break;
                case SonaLexer.VOID:
                    text = "ignore";
                    break;
                case SonaLexer.BYTE:
                    // Might be uint8
                    text = "byte";
                    break;
                case SonaLexer.SBYTE:
                    // Might be int8
                    text = "sbyte";
                    break;
                case SonaLexer.BOOL:
                case SonaLexer.BIGINT:
                case SonaLexer.INT128:
                case SonaLexer.UINT128:
                case SonaLexer.FLOAT16:
                    if(token.Type == SonaLexer.BOOL)
                    {
                        Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Operators", "Convert");
                        Out.Write("(true)");
                        return;
                    }

                    Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Operators", "ConvertInvariant");
                    Out.Write('(');
                    Out.WriteCoreOperatorName("Unchecked");
                    Out.Write(".defaultof<");

                    switch(token.Type)
                    {
                        case SonaLexer.BIGINT:
                            Out.WriteCoreName("bigint");
                            break;
                        case SonaLexer.INT128:
                            Out.WriteSystemName("Int128");
                            break;
                        case SonaLexer.UINT128:
                            Out.WriteSystemName("UInt128");
                            break;
                        case SonaLexer.FLOAT16:
                            Out.WriteSystemName("Half");
                            break;
                    }

                    Out.Write(">)");
                    return;
                case SonaLexer.FLOAT64:
                    text = "float";
                    break;
                case SonaLexer.EXCEPTION:
                    Out.WriteSystemName("Exception");
                    return;
                default:
                    text = token.Text;
                    break;
            }
            Out.WriteCoreOperatorName(text);
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {

        }
    }

    internal sealed class ExpressionStatementState : BlockState, IStatementContext
    {
        bool IStatementContext.TrailAllowed => false;

        public override void EnterExpressionStatement(ExpressionStatementContext context)
        {

        }

        public override void ExitExpressionStatement(ExpressionStatementContext context)
        {
            ExitState()!.ExitExpressionStatement(context);
        }
    }

    internal sealed class InlineStatementState : IsolatedState
    {
        public override bool IsCollection => false;

        public override void EnterInlineExpr(InlineExprContext context)
        {
            Out.EnterNestedScope();
            Out.WriteLine("(");
        }

        public override void ExitInlineExpr(InlineExprContext context)
        {
            Out.WriteLine();
            Out.ExitNestedScope();
            Out.Write(")");
            ExitState().ExitInlineExpr(context);
        }

        public override void EnterExpressionStatement(ExpressionStatementContext context)
        {
            EnterState<ExpressionStatementState>().EnterExpressionStatement(context);
        }

        public override void ExitExpressionStatement(ExpressionStatementContext context)
        {

        }

        public override void WriteBeginBlockExpression()
        {
            Out.EnterNestedScope();
            Out.WriteLine("(");
        }

        public override void WriteEndBlockExpression()
        {
            Out.ExitNestedScope();
            Out.Write(')');
        }
    }
}
