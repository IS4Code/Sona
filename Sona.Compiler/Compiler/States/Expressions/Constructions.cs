using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class RecordState : NodeState
    {
        bool first;
        ImplementationType recordType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            recordType = default;
        }

        public override void EnterRecordConstructor(RecordConstructorContext context)
        {
            Out.Write("{ ");
        }

        public override void ExitRecordConstructor(RecordConstructorContext context)
        {
            Out.Write(" }");
            ExitState().ExitRecordConstructor(context);
        }

        public override void EnterAnonymousRecordConstructor(AnonymousRecordConstructorContext context)
        {
            recordType = RecordImplementationType;
            Out.WriteAnonymousRecordOpen(recordType);
        }

        public override void ExitAnonymousRecordConstructor(AnonymousRecordConstructorContext context)
        {
            Out.WriteAnonymousRecordClose(recordType);
            ExitState().ExitAnonymousRecordConstructor(context);
        }

        public override void EnterAnonymousClassRecordConstructor(AnonymousClassRecordConstructorContext context)
        {
            Out.WriteAnonymousRecordOpen(ImplementationType.Class);
        }

        public override void ExitAnonymousClassRecordConstructor(AnonymousClassRecordConstructorContext context)
        {
            Out.WriteAnonymousRecordClose(ImplementationType.Class);
            ExitState().ExitAnonymousClassRecordConstructor(context);
        }

        public override void EnterAnonymousStructRecordConstructor(AnonymousStructRecordConstructorContext context)
        {
            Out.WriteAnonymousRecordOpen(ImplementationType.Struct);
        }

        public override void ExitAnonymousStructRecordConstructor(AnonymousStructRecordConstructorContext context)
        {
            Out.WriteAnonymousRecordClose(ImplementationType.Struct);
            ExitState().ExitAnonymousStructRecordConstructor(context);
        }

        public override void EnterName(NameContext context)
        {
            Out.WriteNext(';', ref first);
            base.EnterName(context);
        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.WriteOperator('=');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        public override void EnterEmptyFieldAssignment(EmptyFieldAssignmentContext context)
        {
            StartCaptureInput(context);
        }

        public override void ExitEmptyFieldAssignment(EmptyFieldAssignmentContext context)
        {
            var name = StopCaptureInputIdentifier(context);
            Out.WriteOperator('=');
            Out.WriteIdentifier(name);
        }
    }

    internal sealed class TupleState : NodeState
    {
        bool first;
        ImplementationType tupleType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            tupleType = default;
        }

        public override void EnterTupleConstructor(TupleConstructorContext context)
        {
            tupleType = TupleImplementationType;
        }

        public override void ExitTupleConstructor(TupleConstructorContext context)
        {
            ExitState().ExitTupleConstructor(context);
        }

        public override void EnterExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            tupleType = TupleImplementationType;
        }

        public override void ExitExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            ExitState().ExitExplicitTupleConstructor(context);
        }

        public override void EnterClassTupleConstructor(ClassTupleConstructorContext context)
        {
            tupleType = ImplementationType.Class;
        }

        public override void ExitClassTupleConstructor(ClassTupleConstructorContext context)
        {
            ExitState().ExitClassTupleConstructor(context);
        }

        public override void EnterStructTupleConstructor(StructTupleConstructorContext context)
        {
            tupleType = ImplementationType.Struct;
        }

        public override void ExitStructTupleConstructor(StructTupleConstructorContext context)
        {
            ExitState().ExitStructTupleConstructor(context);
        }

        public override void EnterSimpleTupleContents(SimpleTupleContentsContext context)
        {
            Out.WriteTupleOpen(tupleType);
        }

        public override void ExitSimpleTupleContents(SimpleTupleContentsContext context)
        {
            Out.WriteTupleClose(tupleType);
        }

        public override void EnterComplexTupleContents(ComplexTupleContentsContext context)
        {
            Out.WriteCustomTupleFromTreeOperator(tupleType);
            Out.Write('(');
            EnterState<TupleAppendState>().EnterComplexTupleContents(context);
        }

        public override void ExitComplexTupleContents(ComplexTupleContentsContext context)
        {
            Out.Write(')');
        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.WriteNext(',', ref first);
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }
    }

    internal sealed class TupleAppendState : NodeState
    {
        int tupleDepth;
        int appendDepth;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            tupleDepth = 0;
            appendDepth = 0;
        }

        public override void EnterComplexCallArgTuple(ComplexCallArgTupleContext context)
        {

        }

        public override void ExitComplexCallArgTuple(ComplexCallArgTupleContext context)
        {
            FinishAppends();
            ExitState().ExitComplexCallArgTuple(context);
        }

        public override void EnterComplexTupleContents(ComplexTupleContentsContext context)
        {

        }

        public override void ExitComplexTupleContents(ComplexTupleContentsContext context)
        {
            FinishAppends();
            ExitState().ExitComplexTupleContents(context);
        }

        private void FinishAppends()
        {
            FinishTuple();
            if(appendDepth != 0)
            {
                // Finish all appends (should be always non-zero when exiting)
                Out.Write("()");
                Out.Write(new string(')', appendDepth));
                appendDepth = 0;
            }
        }

        private void AppendTuples()
        {
            Out.WriteCustomTupleOperator("Append");
            appendDepth++;
            Out.Write('(');
        }

        private void FinishTuple()
        {
            if(tupleDepth != 0)
            {
                // Finish previous tuple
                Out.Write("()");
                Out.Write(new string(')', tupleDepth));
                Out.Write(", ");
                tupleDepth = 0;
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            if(tupleDepth == 0)
            {
                // First expression in a tuple
                AppendTuples();
            }
            Out.Write('(');
            tupleDepth++;
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            Out.Write(", ");
        }

        public override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            FinishTuple();
            AppendTuples();
            EnterState<SpreadExpression>().EnterSpreadExpression(context);
        }

        public override void ExitSpreadExpression(SpreadExpressionContext context)
        {
            Out.Write(", ");
        }

        public override void EnterFieldAssignment(FieldAssignmentContext context)
        {
            Error("Named arguments are not allowed alongside spread expressions.", context);
        }

        public override void ExitFieldAssignment(FieldAssignmentContext context)
        {

        }

        sealed class SpreadExpression : ExpressionState
        {
            public override void EnterSpreadExpression(SpreadExpressionContext context)
            {
                Out.WriteCustomTupleOperator("ToTree");
                Out.Write('(');
            }

            public override void ExitSpreadExpression(SpreadExpressionContext context)
            {
                Out.Write(')');
                ExitState().ExitSpreadExpression(context);
            }
        }
    }

    internal abstract class ConstructionState : TypeState
    {
        bool namedArg;
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            namedArg = false;
            first = true;
        }

        public sealed override void EnterComplexCallArgTuple(ComplexCallArgTupleContext context)
        {
            Out.WriteCustomTupleOperator("FromTree");
            Out.Write('(');
            EnterState<TupleAppendState>().EnterComplexCallArgTuple(context);
        }

        public sealed override void ExitComplexCallArgTuple(ComplexCallArgTupleContext context)
        {
            Out.Write(')');
        }

        private void OnEnterArgument(ParserRuleContext context)
        {
            Out.WriteNext(',', ref first);
        }

        private void OnEnterExpression(ParserRuleContext context)
        {
            if(namedArg)
            {
                Out.WriteOperator('=');
            }
            else
            {
                OnEnterArgument(context);
            }
        }

        private void OnExitExpression(ParserRuleContext context)
        {
            namedArg = false;
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

        public sealed override void EnterAtomicExpr(AtomicExprContext context)
        {
            OnEnterExpression(context);
            EnterState<AtomicExpressionState>().EnterAtomicExpr(context);
        }

        public sealed override void ExitAtomicExpr(AtomicExprContext context)
        {
            OnExitExpression(context);
        }

        public sealed override void EnterOptionalFieldAssignmentExpr(OptionalFieldAssignmentExprContext context)
        {
            OnEnterArgument(context);
            namedArg = true;
            Out.Write('?');
        }

        public sealed override void ExitOptionalFieldAssignmentExpr(OptionalFieldAssignmentExprContext context)
        {

        }

        public sealed override void EnterFieldAssignment(FieldAssignmentContext context)
        {
            OnEnterArgument(context);
            namedArg = true;
        }

        public sealed override void ExitFieldAssignment(FieldAssignmentContext context)
        {

        }

        public sealed override void EnterOptionSuffix(OptionSuffixContext context)
        {

        }

        public sealed override void ExitOptionSuffix(OptionSuffixContext context)
        {
            Error("Optional construction of instances from multiple arguments is not supported.", context);
        }
    }

    internal sealed class TypeConstructionState : ConstructionState
    {
        public override void EnterMemberTypeConstructExpr(MemberTypeConstructExprContext context)
        {
            Out.Write("(new ");
        }

        public override void ExitMemberTypeConstructExpr(MemberTypeConstructExprContext context)
        {
            Out.Write(')');
            ExitState().ExitMemberTypeConstructExpr(context);
        }

        public override void EnterConstructArguments(ConstructArgumentsContext context)
        {
            Out.Write('(');
        }

        public override void ExitConstructArguments(ConstructArgumentsContext context)
        {
            Out.Write(')');
        }
    }

    internal sealed class NewState : ConstructionState
    {
        bool hasType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasType = false;
        }

        public override void EnterMemberNewExpr(MemberNewExprContext context)
        {
            Out.Write("(new ");
        }

        public override void ExitMemberNewExpr(MemberNewExprContext context)
        {
            Out.Write(')');
            ExitState().ExitMemberNewExpr(context);
        }

        public override void EnterType(TypeContext context)
        {
            EnterState<TypeState>().EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {
            hasType = true;
        }

        public override void EnterConstructArguments(ConstructArgumentsContext context)
        {
            if(!hasType)
            {
                Out.Write('_');
            }
            Out.Write('(');
        }

        public override void ExitConstructArguments(ConstructArgumentsContext context)
        {
            Out.Write(')');
        }
    }
}
