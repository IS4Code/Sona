using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal class ExpressionState : NodeState
    {
        IExecutionScope? scope;

        protected bool IsLiteral => (scope ??= FindScope<IExecutionScope>())?.IsLiteral ?? false;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = null;
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

        public override void EnterFuncExpr(FuncExprContext context)
        {
            EnterState<FunctionExprState>().EnterFuncExpr(context);
        }

        public override void EnterNegatedExpr(NegatedExprContext context)
        {
            EnterState<NegatedExpression>().EnterNegatedExpr(context);
        }

        public override void EnterConjunctiveExpr(ConjunctiveExprContext context)
        {
            EnterState<LogicExpression>().EnterConjunctiveExpr(context);
        }

        public override void EnterDisjunctiveExpr(DisjunctiveExprContext context)
        {
            EnterState<LogicExpression>().EnterDisjunctiveExpr(context);
        }

        public override void EnterDnfExpr(DnfExprContext context)
        {
            EnterState<LogicFormExpression>().EnterDnfExpr(context);
        }

        public override void EnterCnfExpr(CnfExprContext context)
        {
            EnterState<LogicFormExpression>().EnterCnfExpr(context);
        }

        public override void EnterConcatExprArg(ConcatExprArgContext context)
        {
            Out.Write('(');
        }

        public override void ExitConcatExprArg(ConcatExprArgContext context)
        {
            Out.Write(')');
        }

        public override void EnterConcatExprNextArg(ConcatExprNextArgContext context)
        {
            if(IsLiteral)
            {
                Out.WriteOperator("+\"\"+");
            }
            else
            {
                Out.WriteSpecialMember("..");
            }
        }

        public override void EnterBitOrExprArg(BitOrExprArgContext context)
        {
            Out.Write('(');
        }

        public override void ExitBitOrExprArg(BitOrExprArgContext context)
        {
            Out.Write(')');
        }

        public override void EnterBitOrExprNextArg(BitOrExprNextArgContext context)
        {
            if(IsLiteral)
            {
                Out.WriteOperator("|||");
            }
            else
            {
                Out.WriteSpecialMember("|");
            }
        }

        public override void EnterBitXorExprArg(BitXorExprArgContext context)
        {
            Out.Write('(');
        }

        public override void ExitBitXorExprArg(BitXorExprArgContext context)
        {
            Out.Write(')');
        }

        public override void EnterBitXorExprNextArg(BitXorExprNextArgContext context)
        {
            if(IsLiteral)
            {
                Out.WriteOperator("^^^");
            }
            else
            {
                Out.WriteSpecialMember("^");
            }
        }

        public override void EnterBitAndExprArg(BitAndExprArgContext context)
        {
            Out.Write('(');
        }

        public override void ExitBitAndExprArg(BitAndExprArgContext context)
        {
            Out.Write(')');
        }

        public override void EnterBitAndExprNextArg(BitAndExprNextArgContext context)
        {
            if(IsLiteral)
            {
                Out.WriteOperator("&&&");
            }
            else
            {
                Out.WriteSpecialMember("&");
            }
        }

        public override void EnterBitShiftExprArg(BitShiftExprArgContext context)
        {
            Out.Write('(');
        }

        public override void ExitBitShiftExprArg(BitShiftExprArgContext context)
        {
            Out.Write(')');
        }

        public override void EnterBitShiftLeftExprNextArg(BitShiftLeftExprNextArgContext context)
        {
            if(IsLiteral)
            {
                Out.WriteOperator("<<<");
            }
            else
            {
                Out.WriteSpecialMember("<<");
            }
        }

        public override void EnterBitShiftRightExprNextArg(BitShiftRightExprNextArgContext context)
        {
            if(IsLiteral)
            {
                Out.WriteOperator(">>>");
            }
            else
            {
                Out.WriteSpecialMember(">>");
            }
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
                Out.WriteSpecialMember("#");
                Out.Write("()");
            }
        }

        public override void EnterNotExpr(NotExprContext context)
        {
            Out.WriteCoreOperator("not");
            Out.Write('(');
        }

        public override void ExitNotExpr(NotExprContext context)
        {
            Out.Write(')');
        }
    }

    internal sealed class ExpressionListState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
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

        public override void ExitExprList(ExprListContext context)
        {
            ExitState().ExitExprList(context);
        }
    }

    internal class MemberExprState : NodeState
    {
        public override void EnterMemberExpr(MemberExprContext context)
        {

        }

        public override void ExitMemberExpr(MemberExprContext context)
        {
            ExitState().ExitMemberExpr(context);
        }

        public override void EnterNestedExpr(NestedExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitNestedExpr(NestedExprContext context)
        {
            Out.Write(')');
        }

        public override void EnterSimpleExpr(SimpleExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitSimpleExpr(SimpleExprContext context)
        {
            Out.Write(')');
        }

        public override void EnterArrayConstructor(ArrayConstructorContext context)
        {
            EnterState<ArrayState>().EnterArrayConstructor(context);
        }

        public override void ExitArrayConstructor(ArrayConstructorContext context)
        {

        }

        public override void EnterRecordConstructor(RecordConstructorContext context)
        {
            EnterState<RecordState>().EnterRecordConstructor(context);
        }

        public override void ExitRecordConstructor(RecordConstructorContext context)
        {

        }

        public override void EnterSequenceConstructor(SequenceConstructorContext context)
        {
            Out.Write('(');
            EnterState<SequenceState>().EnterSequenceConstructor(context);
        }

        public override void ExitSequenceConstructor(SequenceConstructorContext context)
        {
            Out.Write(')');
        }

        public override void EnterExprList(ExprListContext context)
        {
            EnterState<ExpressionListState>().EnterExprList(context);
        }

        public override void ExitExprList(ExprListContext context)
        {

        }

        public override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        public override void EnterSimpleCallArgument(SimpleCallArgumentContext context)
        {
            Out.Write('(');
            EnterState<SimpleArgumentState>().EnterSimpleCallArgument(context);
        }

        public override void ExitSimpleCallArgument(SimpleCallArgumentContext context)
        {
            Out.Write(')');
        }

        public override void EnterNestedAssignment(NestedAssignmentContext context)
        {
            EnterState<AssignmentState>().EnterNestedAssignment(context);
        }

        public override void ExitNestedAssignment(NestedAssignmentContext context)
        {

        }

        public override void EnterIndexAccess(IndexAccessContext context)
        {
            Out.Write('[');
        }

        public override void ExitIndexAccess(IndexAccessContext context)
        {
            Out.Write(']');
        }

        public override void EnterMemberAccess(MemberAccessContext context)
        {
            Out.Write('.');
        }

        public override void EnterDynamicMemberAccess(DynamicMemberAccessContext context)
        {
            Out.Write('?');
        }

        public override void EnterCallArgTuple(CallArgTupleContext context)
        {
            Out.Write('(');
        }

        public override void ExitCallArgTuple(CallArgTupleContext context)
        {
            Out.Write(')');
        }

        sealed class AssignmentState : MemberExprState
        {
            public override void EnterNestedAssignment(NestedAssignmentContext context)
            {
                Out.Write('(');
            }

            public override void ExitNestedAssignment(NestedAssignmentContext context)
            {
                Out.Write(')');
                ExitState().ExitNestedAssignment(context);
            }

            public override void EnterMemberExpr(MemberExprContext context)
            {
                EnterState<MemberExprState>().EnterMemberExpr(context);
            }

            public override void ExitMemberExpr(MemberExprContext context)
            {

            }

            public override void EnterAssignment(AssignmentContext context)
            {
                Out.Write(')');
                Out.WriteSpecialMember("<-");
                Out.Write('(');
            }

            public override void ExitAssignment(AssignmentContext context)
            {

            }
        }

        sealed class SimpleArgumentState : MemberExprState
        {
            public override void EnterSimpleCallArgument(SimpleCallArgumentContext context)
            {

            }

            public override void ExitSimpleCallArgument(SimpleCallArgumentContext context)
            {
                ExitState().ExitSimpleCallArgument(context);
            }

            public override void EnterSequenceConstructor(SequenceConstructorContext context)
            {
                EnterState<SequenceState>().EnterSequenceConstructor(context);
            }

            public override void ExitSequenceConstructor(SequenceConstructorContext context)
            {

            }
        }
    }

    internal sealed class FunctionExprState : NodeState
    {
        string? name;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            name = null;
        }

        public override void EnterName(NameContext context)
        {
            Out.Write('(');
            Out.EnterScope();

            Out.Write("let rec ");

            base.EnterName(context);
        }

        public override void ExitName(NameContext context)
        {
            base.ExitName(context);

            name = context.GetText();
        }

        public override void EnterFuncExpr(FuncExprContext context)
        {

        }

        public override void ExitFuncExpr(FuncExprContext context)
        {
            ExitState().ExitFuncExpr(context);
        }

        public override void EnterParamList(ParamListContext context)
        {
            if(name == null)
            {
                // lambda expression
                Out.Write('(');
                Out.EnterScope();
                Out.Write("fun ");
            }

            EnterState<ParamListState>().EnterParamList(context);
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            if(name == null)
            {
                Out.WriteOperator("->");
            }
            else
            {
                Out.WriteOperator('=');
            }
            Out.WriteLine("begin");
            Out.EnterScope();
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitScope();
            if(name == null)
            {
                Out.Write("end)");
            }
            else
            {
                Out.Write("end in ");
                Out.WriteIdentifier(name);
                Out.Write(")");
            }
            Out.ExitScope();
        }
    }

    internal sealed class RecordState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
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

    internal sealed class ArrayState : NodeState
    {
        bool first;
        State state;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            state = State.Default;
        }

        public override void EnterArrayConstructor(ArrayConstructorContext context)
        {
            Out.Write("[| ");
        }

        public override void ExitArrayConstructor(ArrayConstructorContext context)
        {
            Out.Write(" |]");
            ExitState().ExitArrayConstructor(context);
        }

        void OnOperand()
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(';');
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            switch(state)
            {
                case State.Default:
                    OnOperand();
                    break;
            }
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            state = State.Default;
        }

        public override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            OnOperand();
            Out.Write("yield! ");
            state = State.Spread;
        }

        enum State
        {
            Default,
            Spread
        }
    }

    internal sealed class SequenceState : NodeState
    {
        bool first;
        State state;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            state = State.Default;
        }

        public override void EnterSequenceConstructor(SequenceConstructorContext context)
        {

        }

        public override void ExitSequenceConstructor(SequenceConstructorContext context)
        {
            if(first)
            {
                Out.WriteNamespacedName("Microsoft.FSharp.Collections", "Seq");
                Out.Write(".empty");
            }
            else
            {
                Out.Write(" }");
            }
            ExitState().ExitSequenceConstructor(context);
        }

        void OnOperand()
        {
            if(first)
            {
                first = false;
                Out.WriteCoreOperator("seq");
                Out.Write("{ ");
            }
            else
            {
                Out.Write(';');
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            switch(state)
            {
                case State.Key:
                    Out.Write(',');
                    break;
                case State.Default:
                    OnOperand();
                    break;
            }
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            switch(state)
            {
                case State.Key:
                    Out.Write(')');
                    break;
            }
            state = State.Default;
        }

        public override void EnterExprList(ExprListContext context)
        {
            OnOperand();
            Out.WriteNamespacedName("System.Collections.Generic", "KeyValuePair");
            Out.Write("<_,_>((");
            EnterState<ExpressionListState>().EnterExprList(context);
        }

        public override void ExitExprList(ExprListContext context)
        {
            Out.Write(')');
            state = State.Key;
        }

        public override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            OnOperand();
            Out.Write("yield! ");
            state = State.Spread;
        }

        enum State
        {
            Default,
            Key,
            Spread
        }
    }
}
