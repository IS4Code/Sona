using System.Collections.Generic;
using Antlr4.Runtime;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal class MemberExprState : NodeState
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

        public override void EnterMemberTypeConvertExpr(MemberTypeConvertExprContext context)
        {
            EnterState<TypeConversionState>().EnterMemberTypeConvertExpr(context);
        }

        public override void ExitMemberTypeConvertExpr(MemberTypeConvertExprContext context)
        {

        }

        public override void EnterMemberVoidExpr(MemberVoidExprContext context)
        {
            Out.Write("(let _");
            Out.WriteOperator('=');
        }

        public override void ExitMemberVoidExpr(MemberVoidExprContext context)
        {
            Out.Write(" in ())");
        }

        public override void EnterMemberObjectExpr(MemberObjectExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitMemberObjectExpr(MemberObjectExprContext context)
        {
            Out.WriteOperator(":>");
            Out.WriteCoreName("objnull");
            Out.Write(')');
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
            Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Tuples", "FromTree");
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
                base.EnterExpression(context);
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
    }

    internal class AltMemberExprState : MemberExprState
    {
        bool lambdaOpen;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            lambdaOpen = false;
        }

        public override void EnterAltMemberExpr(AltMemberExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitAltMemberExpr(AltMemberExprContext context)
        {
            CloseLambda();
            Out.Write(')');
            ExitState().ExitAltMemberExpr(context);
        }

        private void OpenLambda()
        {
            if(lambdaOpen)
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

        public override void EnterConstrainedMemberAccess(ConstrainedMemberAccessContext context)
        {
            CloseLambda();
            Out.WriteOperator("|>");
            EnterState<ConstrainedMemberState>().EnterConstrainedMemberAccess(context);
        }

        public override void ExitConstrainedMemberAccess(ConstrainedMemberAccessContext context)
        {

        }

        class ConstrainedMemberState : AltMemberExprState
        {
            bool property, first;
            string? paramName;
            ISourceCapture? argsCapture;
            readonly List<int> arities = new();

            string ParamName(ParserRuleContext context) => paramName ?? Error("COMPILER ERROR: Missing constrained member access parameter name.", context);

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                property = true;
                first = true;
                paramName = null;
                argsCapture = null;
                arities.Clear();
            }

            public override void EnterConstrainedMemberAccess(ConstrainedMemberAccessContext context)
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
                var argsCapture = this.argsCapture ?? ErrorCapture("COMPILER ERROR: Missing constraint expression captured state.", context);
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

            private void OpenArgTuple(ParserRuleContext context)
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

            private void CloseArgTuple(ParserRuleContext context)
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

            public override void EnterSimpleCallArgTuple(SimpleCallArgTupleContext context)
            {
                OpenArgTuple(context);
            }

            public override void ExitSimpleCallArgTuple(SimpleCallArgTupleContext context)
            {
                CloseArgTuple(context);
            }

            public override void EnterComplexCallArgTuple(ComplexCallArgTupleContext context)
            {
                OpenArgTuple(context);
            }

            public override void ExitComplexCallArgTuple(ComplexCallArgTupleContext context)
            {
                CloseArgTuple(context);
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

            public override void EnterSpreadExpression(SpreadExpressionContext context)
            {
                Error("A call to a constraint member expression cannot contain spread expressions.", context);
            }

            public override void ExitSpreadExpression(SpreadExpressionContext context)
            {

            }

            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                Error("A call to a constraint member expression cannot contain named arguments.", context);
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {

            }
        }
    }
}
