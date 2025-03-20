﻿using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class RecordState : NodeState
    {
        bool first, isStruct;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            isStruct = false;
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
            isStruct = LexerContext.GetState<RecordPragma>()?.IsStruct ?? false;
            Out.Write(isStruct ? "(struct{| " : "{| ");
        }

        public override void ExitAnonymousRecordConstructor(AnonymousRecordConstructorContext context)
        {
            Out.Write(isStruct ? " |})" : " |}");
            ExitState().ExitAnonymousRecordConstructor(context);
        }

        public override void EnterAnonymousClassRecordConstructor(AnonymousClassRecordConstructorContext context)
        {
            Out.Write("{| ");
        }

        public override void ExitAnonymousClassRecordConstructor(AnonymousClassRecordConstructorContext context)
        {
            Out.Write(" |}");
            ExitState().ExitAnonymousClassRecordConstructor(context);
        }

        public override void EnterAnonymousStructRecordConstructor(AnonymousStructRecordConstructorContext context)
        {
            Out.Write("(struct{| ");
        }

        public override void ExitAnonymousStructRecordConstructor(AnonymousStructRecordConstructorContext context)
        {
            Out.Write(" |})");
            ExitState().ExitAnonymousStructRecordConstructor(context);
        }

        public override void EnterName(NameContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(';');
            }
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
    }

    internal sealed class TupleState : NodeState
    {
        bool first, isStruct;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            isStruct = false;
        }

        public override void EnterTupleConstructor(TupleConstructorContext context)
        {
            isStruct = LexerContext.GetState<TuplePragma>()?.IsStruct ?? true;
        }

        public override void ExitTupleConstructor(TupleConstructorContext context)
        {
            ExitState().ExitTupleConstructor(context);
        }

        public override void EnterExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            isStruct = LexerContext.GetState<TuplePragma>()?.IsStruct ?? true;
        }

        public override void ExitExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            ExitState().ExitExplicitTupleConstructor(context);
        }

        public override void EnterClassTupleConstructor(ClassTupleConstructorContext context)
        {

        }

        public override void ExitClassTupleConstructor(ClassTupleConstructorContext context)
        {
            ExitState().ExitClassTupleConstructor(context);
        }

        public override void EnterStructTupleConstructor(StructTupleConstructorContext context)
        {
            isStruct = true;
        }

        public override void ExitStructTupleConstructor(StructTupleConstructorContext context)
        {
            ExitState().ExitStructTupleConstructor(context);
        }

        public override void EnterSimpleTupleContents(SimpleTupleContentsContext context)
        {
            Out.Write(isStruct ? "(struct(" : "(");
        }

        public override void ExitSimpleTupleContents(SimpleTupleContentsContext context)
        {
            Out.Write(isStruct ? "))" : ")");
        }

        public override void EnterComplexTupleContents(ComplexTupleContentsContext context)
        {
            Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Tuples", isStruct ? "FromTreeValue" : "FromTree");
            Out.Write('(');
            EnterState<TupleAppendState>().EnterComplexTupleContents(context);
        }

        public override void ExitComplexTupleContents(ComplexTupleContentsContext context)
        {
            Out.Write(')');
        }

        public override void EnterExpression(ExpressionContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(',');
            }
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
            Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Tuples", "Append");
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
                Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Tuples", "ToTree");
                Out.Write('(');
            }

            public override void ExitSpreadExpression(SpreadExpressionContext context)
            {
                Out.Write(')');
                ExitState().ExitSpreadExpression(context);
            }
        }
    }

    internal sealed class TypeConstructionState : TypeState
    {
        bool namedArg;
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            namedArg = false;
            first = true;
        }

        public override void EnterMemberTypeConstructExpr(MemberTypeConstructExprContext context)
        {
            Out.Write("new ");
        }

        public override void ExitMemberTypeConstructExpr(MemberTypeConstructExprContext context)
        {
            Out.Write(')');
            ExitState().ExitMemberTypeConstructExpr(context);
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {
            base.ExitPrimitiveType(context);
            Out.Write('(');
        }

        public override void EnterExpression(ExpressionContext context)
        {
            if(namedArg)
            {
                Out.WriteOperator('=');
            }
            else if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(',');
            }
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            namedArg = false;
        }

        public override void EnterFieldAssignment(FieldAssignmentContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(',');
            }
            namedArg = true;
        }

        public override void ExitFieldAssignment(FieldAssignmentContext context)
        {

        }
    }

    internal sealed class TypeConversionState : ExpressionState
    {
        public override void EnterMemberTypeConvertExpr(MemberTypeConvertExprContext context)
        {

        }

        public override void ExitMemberTypeConvertExpr(MemberTypeConvertExprContext context)
        {
            Out.Write(')');
            ExitState().ExitMemberTypeConvertExpr(context);
        }

        public override void EnterAtomicTypeConvertExpr(AtomicTypeConvertExprContext context)
        {

        }

        public override void ExitAtomicTypeConvertExpr(AtomicTypeConvertExprContext context)
        {
            Out.Write(')');
            ExitState().ExitAtomicTypeConvertExpr(context);
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {
            base.ExitPrimitiveType(context);
            Out.Write('(');
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
