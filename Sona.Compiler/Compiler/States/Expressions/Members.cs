using System.Collections.Generic;
using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class MemberExprState : TypeState
    {
        public override void EnterMemberExpr(MemberExprContext context)
        {

        }

        public override void ExitMemberExpr(MemberExprContext context)
        {
            ExitState().ExitMemberExpr(context);
        }

        public sealed override void EnterNestedExpr(NestedExprContext context)
        {
            Out.Write('(');
        }

        public sealed override void ExitNestedExpr(NestedExprContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterSimpleExpr(SimpleExprContext context)
        {
            Out.Write('(');
        }

        public sealed override void ExitSimpleExpr(SimpleExprContext context)
        {
            Out.Write(')');
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

        public override void EnterMemberTypeConstructExpr(MemberTypeConstructExprContext context)
        {
            EnterState<TypeConstructionState>().EnterMemberTypeConstructExpr(context);
        }

        public override void ExitMemberTypeConstructExpr(MemberTypeConstructExprContext context)
        {

        }

        public override void EnterMemberNewExpr(MemberNewExprContext context)
        {
            EnterState<NewState>().EnterMemberNewExpr(context);
        }

        public override void ExitMemberNewExpr(MemberNewExprContext context)
        {

        }

        public override void EnterMemberNumberConvertExpr(MemberNumberConvertExprContext context)
        {
            EnterState<NumberConversionState>().EnterMemberNumberConvertExpr(context);
        }

        public override void ExitMemberNumberConvertExpr(MemberNumberConvertExprContext context)
        {

        }

        public override void EnterMemberCharConvertExpr(MemberCharConvertExprContext context)
        {
            EnterState<CharConversionState>().EnterMemberCharConvertExpr(context);
        }

        public override void ExitMemberCharConvertExpr(MemberCharConvertExprContext context)
        {

        }

        public override void EnterMemberConvertExpr(MemberConvertExprContext context)
        {
            EnterState<ConversionState>().EnterMemberConvertExpr(context);
        }

        public override void ExitMemberConvertExpr(MemberConvertExprContext context)
        {

        }

        public override void EnterIndexAccess(IndexAccessContext context)
        {
            EnterState<MemberApplicationState>().EnterIndexAccess(context);
        }

        public override void ExitIndexAccess(IndexAccessContext context)
        {

        }

        public override void EnterMemberAccess(MemberAccessContext context)
        {
            Out.Write('.');
        }

        public sealed override void ExitMemberAccess(MemberAccessContext context)
        {

        }

        public override void EnterDynamicMemberAccess(DynamicMemberAccessContext context)
        {
            Out.Write('?');
        }

        public sealed override void ExitDynamicMemberAccess(DynamicMemberAccessContext context)
        {

        }

        public override void EnterDynamicExprMemberAccess(DynamicExprMemberAccessContext context)
        {
            Out.Write('?');
        }

        public sealed override void ExitDynamicExprMemberAccess(DynamicExprMemberAccessContext context)
        {

        }

        public override void EnterSimpleCallArgTuple(SimpleCallArgTupleContext context)
        {
            EnterState<MemberApplicationState>().EnterSimpleCallArgTuple(context);
        }

        public override void ExitSimpleCallArgTuple(SimpleCallArgTupleContext context)
        {

        }

        public override void EnterComplexCallArgTuple(ComplexCallArgTupleContext context)
        {
            Out.Write('(');
            Out.WriteCustomTupleOperator("FromTree");
            Out.Write('(');
            EnterState<TupleAppendState>().EnterComplexCallArgTuple(context);
        }

        public override void ExitComplexCallArgTuple(ComplexCallArgTupleContext context)
        {
            Out.Write("))");
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

            public override void EnterAltMemberExpr(AltMemberExprContext context)
            {
                EnterState<AltMemberExprState>().EnterAltMemberExpr(context);
            }

            public override void ExitAltMemberExpr(AltMemberExprContext context)
            {

            }

            public override void EnterAssignment(AssignmentContext context)
            {
                Out.Write(')');
                Out.WriteSpecialMember("operator Assign");
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
        }

        sealed class MemberApplicationState : MemberExprState
        {
            bool first;
            bool namedArg;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                namedArg = false;
                first = true;
            }

            public override void EnterSimpleCallArgTuple(SimpleCallArgTupleContext context)
            {
                Out.Write('(');
            }

            public override void ExitSimpleCallArgTuple(SimpleCallArgTupleContext context)
            {
                Out.Write(')');
                ExitState().ExitSimpleCallArgTuple(context);
            }

            public override void EnterIndexAccess(IndexAccessContext context)
            {
                Out.Write(".[");
            }

            public override void ExitIndexAccess(IndexAccessContext context)
            {
                Out.Write(']');
                ExitState().ExitIndexAccess(context);
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

            public override void EnterExpression(ExpressionContext context)
            {
                OnEnterExpression(context);
                base.EnterExpression(context);
            }

            public override void ExitExpression(ExpressionContext context)
            {
                OnExitExpression(context);
            }

            public override void EnterAtomicExpr(AtomicExprContext context)
            {
                OnEnterExpression(context);
                EnterState<AtomicExpressionState>().EnterAtomicExpr(context);
            }

            public override void ExitAtomicExpr(AtomicExprContext context)
            {
                OnExitExpression(context);
            }

            public override void EnterOptionalFieldAssignmentExpr(OptionalFieldAssignmentExprContext context)
            {
                OnEnterArgument(context);
                namedArg = true;
                Out.Write('?');
            }

            public override void ExitOptionalFieldAssignmentExpr(OptionalFieldAssignmentExprContext context)
            {

            }

            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                OnEnterArgument(context);
                namedArg = true;
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {

            }
        }
    }

    internal class AltMemberExprState : MemberExprState
    {
        bool lambdaOpen, compactMember;
        int level;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            lambdaOpen = false;
            compactMember = false;
            level = 0;
        }

        public override void EnterAltMemberExpr(AltMemberExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitAltMemberExpr(AltMemberExprContext context)
        {
            CloseLambda();
            while(level-- > 0)
            {
                // Clear all previous levels
                Out.Write("))");
            }
            Out.Write(')');
            ExitState().ExitAltMemberExpr(context);
        }

        private void OpenLambda()
        {
            if(lambdaOpen || compactMember)
            {
                return;
            }
            Out.WriteOperator("|>");
            var arg = Out.CreateTemporaryIdentifier();
            Out.Write("(fun ");
            Out.WriteIdentifier(arg);
            Out.WriteOperator("->");
            Out.WriteIdentifier(arg);
            lambdaOpen = true;
        }

        private void CloseLambda()
        {
            if(lambdaOpen)
            {
                Out.Write(')');
                lambdaOpen = false;
            }
        }

        public override void EnterPrimitiveTypeMemberAccess(PrimitiveTypeMemberAccessContext context)
        {
            compactMember = true;
        }

        public override void ExitPrimitiveTypeMemberAccess(PrimitiveTypeMemberAccessContext context)
        {
            compactMember = true;
        }

        public override void EnterIndexAccess(IndexAccessContext context)
        {
            OpenLambda();
            base.EnterIndexAccess(context);
        }

        public override void EnterMemberAccess(MemberAccessContext context)
        {
            OpenLambda();
            base.EnterMemberAccess(context);
        }

        public override void EnterDynamicMemberAccess(DynamicMemberAccessContext context)
        {
            OpenLambda();
            base.EnterDynamicMemberAccess(context);
        }

        public override void EnterDynamicExprMemberAccess(DynamicExprMemberAccessContext context)
        {
            OpenLambda();
            base.EnterDynamicExprMemberAccess(context);
        }

        public override void EnterSimpleCallArgTuple(SimpleCallArgTupleContext context)
        {
            OpenLambda();
            base.EnterSimpleCallArgTuple(context);
        }

        public override void EnterComplexCallArgTuple(ComplexCallArgTupleContext context)
        {
            OpenLambda();
            base.EnterComplexCallArgTuple(context);
        }

        public override void EnterConditionalMember(ConditionalMemberContext context)
        {

        }

        public override void ExitConditionalMember(ConditionalMemberContext context)
        {
            CloseLambda();
            Out.WriteOperator("|>");
            var arg = Out.CreateTemporaryIdentifier();
            Out.Write("(fun ");
            Out.WriteIdentifier(arg);
            Out.WriteOperator("->");
            Out.Write("match ");
            Out.WriteCustomOperator("BindToResult");
            Out.Write('(');
            Out.WriteIdentifier(arg);
            Out.Write(")with|struct(false,_)->");
            var optionType = OptionImplementationType;
            Out.WriteOptionNone(optionType);
            var value = Out.CreateTemporaryIdentifier();
            Out.Write("|struct(true,");
            Out.WriteIdentifier(value);
            Out.Write(")->");
            Out.WriteOptionSome(optionType);
            Out.Write('(');
            Out.WriteIdentifier(value);
            level++;
        }

        public override void EnterConstrainedMemberAccess(ConstrainedMemberAccessContext context)
        {
            CloseLambda();
            Out.WriteOperator("|>");
            EnterState<ImplicitConstrainedMemberState>().EnterConstrainedMemberAccess(context);
        }

        public override void ExitConstrainedMemberAccess(ConstrainedMemberAccessContext context)
        {

        }

        public override void EnterConstrainedFunctionAccess(ConstrainedFunctionAccessContext context)
        {
            CloseLambda();
            Out.WriteOperator("|>");
            EnterState<FunctionConstrainedMemberState>().EnterConstrainedFunctionAccess(context);
        }

        public override void ExitConstrainedFunctionAccess(ConstrainedFunctionAccessContext context)
        {

        }

        public override void EnterConstrainedPropertyAccess(ConstrainedPropertyAccessContext context)
        {
            CloseLambda();
            Out.WriteOperator("|>");
            EnterState<PropertyConstrainedMemberState>().EnterConstrainedPropertyAccess(context);
        }

        public override void ExitConstrainedPropertyAccess(ConstrainedPropertyAccessContext context)
        {

        }

        abstract class ConstrainedMemberState : AltMemberExprState
        {
            protected bool first;
            string? paramName;

            protected string ParamName(ParserRuleContext context) => paramName ?? Error("COMPILER ERROR: Missing constrained member access parameter name.", context);

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
                paramName = null;
            }

            protected void OnEnter()
            {
                paramName = Out.CreateTemporaryIdentifier();
                Out.Write("(fun ");
                Out.WriteIdentifier(paramName);
                Out.WriteOperator("->");
                Out.Write("(^");
                Out.WriteIdentifier(Out.CreateTemporaryIdentifier());
                Out.WriteOperator(':');
                Out.Write("(member ");
            }

            protected abstract void OpenArgTuple(ParserRuleContext context);
            protected abstract void CloseArgTuple(ParserRuleContext context);

            public sealed override void EnterSimpleCallArgTuple(SimpleCallArgTupleContext context)
            {
                OpenArgTuple(context);
            }

            public sealed override void ExitSimpleCallArgTuple(SimpleCallArgTupleContext context)
            {
                CloseArgTuple(context);
            }

            public sealed override void EnterComplexCallArgTuple(ComplexCallArgTupleContext context)
            {
                OpenArgTuple(context);
            }

            public sealed override void ExitComplexCallArgTuple(ComplexCallArgTupleContext context)
            {
                CloseArgTuple(context);
            }

            public sealed override void EnterSpreadExpression(SpreadExpressionContext context)
            {
                Error("A call to a constrained member expression cannot contain spread expressions.", context);
            }

            public sealed override void ExitSpreadExpression(SpreadExpressionContext context)
            {

            }

            public sealed override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                Error("A call to a constrained member expression cannot contain named arguments.", context);
            }

            public sealed override void ExitFieldAssignment(FieldAssignmentContext context)
            {

            }
        }

        sealed class ImplicitConstrainedMemberState : ConstrainedMemberState
        {
            bool property;
            ISourceCapture? argsCapture;
            readonly List<int> arities = new();

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                property = true;
                argsCapture = null;
                arities.Clear();
            }

            public override void EnterConstrainedMemberAccess(ConstrainedMemberAccessContext context)
            {
                OnEnter();
            }

            public override void ExitConstrainedMemberAccess(ConstrainedMemberAccessContext context)
            {
                if(property)
                {
                    Out.WriteOperator(':');
                    Out.Write("_)");
                    Out.WriteIdentifier(ParamName(context));
                }
                Out.Write("))");
                ExitState().ExitConstrainedMemberAccess(context);
            }

            public override void EnterCallArguments(CallArgumentsContext context)
            {
                property = false;
                Out.WriteOperator(':');
                argsCapture = Out.StartCapture();
            }

            public override void ExitCallArguments(CallArgumentsContext context)
            {
                var argsCapture = this.argsCapture ?? ErrorCapture("COMPILER ERROR: Missing constrained expression captured state.", context);
                Out.StopCapture(argsCapture);
                if(arities.Count == 0)
                {
                    Out.WriteCoreName("unit");
                }
                else
                {
                    bool firstTuple = true;
                    foreach(var arity in arities)
                    {
                        if(firstTuple)
                        {
                            firstTuple = false;
                        }
                        else
                        {
                            Out.WriteOperator("->");
                        }
                        if(arity == 0)
                        {
                            Out.WriteCoreName("unit");
                        }
                        else
                        {
                            Out.Write('_');
                            for(int i = 1; i < arity; i++)
                            {
                                Out.WriteOperator('*');
                                Out.Write('_');
                            }
                        }
                    }
                }
                Out.WriteOperator("->");
                Out.Write("_)(");
                argsCapture.Play(Out);
                Out.Write(')');
            }

            protected override void OpenArgTuple(ParserRuleContext context)
            {
                if(first)
                {
                    // May be empty yet
                    return;
                }
                if(arities.Count == 1 && arities[0] == 0)
                {
                    // Unit argument
                    Out.Write(",()");
                }
                // Prepare empty arity for the next tuple
                arities.Add(0);
            }

            protected override void CloseArgTuple(ParserRuleContext context)
            {
                if(first)
                {
                    // First tuple is empty
                    Out.WriteIdentifier(ParamName(context));
                    first = false;
                    // Add empty arity
                    arities.Add(0);
                }
                if(arities.Count > 1 && arities[arities.Count - 1] == 0)
                {
                    // Unit argument
                    Out.Write(",()");
                }
            }

            public override void EnterExpression(ExpressionContext context)
            {
                if(first)
                {
                    Out.WriteIdentifier(ParamName(context));

                    // Prepare first arity
                    first = false;
                    arities.Add(1);
                }
                else
                {
                    ++arities[arities.Count - 1];
                }

                // Always next expression
                Out.Write(',');

                base.EnterExpression(context);
            }
        }
        
        sealed class FunctionConstrainedMemberState : ConstrainedMemberState
        {
            bool emptyTuple;
            int tupleIndex;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                emptyTuple = true;
                tupleIndex = -1;
            }

            public override void EnterConstrainedFunctionAccess(ConstrainedFunctionAccessContext context)
            {
                OnEnter();
            }

            public override void ExitConstrainedFunctionAccess(ConstrainedFunctionAccessContext context)
            {
                try
                {
                    if(tupleIndex == -1)
                    {
                        Error("Function-constrained members must be immediately called.", context);
                    }
                    Out.Write("))");
                }
                finally
                {
                    ExitState().ExitConstrainedFunctionAccess(context);
                }
            }

            public override void EnterFunctionType(FunctionTypeContext context)
            {
                Out.WriteOperator(':');
                EnterState<FunctionType>().EnterFunctionType(context);
            }

            public override void ExitFunctionType(FunctionTypeContext context)
            {
                Out.Write(')');
            }

            public override void EnterCallArguments(CallArgumentsContext context)
            {
                Out.Write('(');
            }

            public override void ExitCallArguments(CallArgumentsContext context)
            {
                Out.Write(')');
            }

            protected override void OpenArgTuple(ParserRuleContext context)
            {
                tupleIndex++;
                if(first)
                {
                    // May be empty yet
                    emptyTuple = true;
                    return;
                }
                if(tupleIndex == 1 && emptyTuple)
                {
                    // Unit argument
                    Out.Write(",()");
                }
                emptyTuple = true;
            }

            protected override void CloseArgTuple(ParserRuleContext context)
            {
                if(first)
                {
                    // First tuple is empty
                    Out.WriteIdentifier(ParamName(context));
                    first = false;
                }
                if(tupleIndex >= 1 && emptyTuple)
                {
                    // Unit argument
                    Out.Write(",()");
                }
            }

            public override void EnterExpression(ExpressionContext context)
            {
                if(first)
                {
                    Out.WriteIdentifier(ParamName(context));
                    first = false;
                }

                // Always next expression
                Out.Write(',');

                emptyTuple = false;

                base.EnterExpression(context);
            }
        }
        
        
        sealed class PropertyConstrainedMemberState : ConstrainedMemberState
        {
            public override void EnterConstrainedPropertyAccess(ConstrainedPropertyAccessContext context)
            {
                OnEnter();
            }

            public override void ExitConstrainedPropertyAccess(ConstrainedPropertyAccessContext context)
            {
                Out.WriteIdentifier(ParamName(context));
                Out.Write("))");
                ExitState().ExitConstrainedPropertyAccess(context);
            }

            public override void EnterType(TypeContext context)
            {
                Out.WriteOperator(':');
                base.EnterType(context);
            }

            public override void ExitType(TypeContext context)
            {
                base.ExitType(context);
                Out.Write(')');
            }

            protected override void OpenArgTuple(ParserRuleContext context)
            {

            }

            protected override void CloseArgTuple(ParserRuleContext context)
            {

            }
        }
    }
}
