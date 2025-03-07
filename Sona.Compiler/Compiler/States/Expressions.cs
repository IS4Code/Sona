using System.Collections.Generic;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
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

        public override void EnterAtomicTypeConvertExpr(AtomicTypeConvertExprContext context)
        {
            EnterState<TypeConversionState>().EnterAtomicTypeConvertExpr(context);
        }

        public override void ExitAtomicTypeConvertExpr(AtomicTypeConvertExprContext context)
        {

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

        public override void EnterAtomicVoidExpr(AtomicVoidExprContext context)
        {
            Out.Write("(let _");
            Out.WriteOperator('=');
        }

        public override void ExitAtomicVoidExpr(AtomicVoidExprContext context)
        {
            Out.Write(" in ())");
        }

        public override void EnterAtomicObjectExpr(AtomicObjectExprContext context)
        {
            Out.Write('(');
        }

        public override void ExitAtomicObjectExpr(AtomicObjectExprContext context)
        {
            Out.WriteOperator(":>");
            Out.WriteNamespacedName("Microsoft.FSharp.Core", "objnull");
            Out.Write(')');
        }

        public override void EnterUnit(UnitContext context)
        {
            Out.Write("(()");
            Out.WriteOperator(':');
            Out.WriteNamespacedName("Microsoft.FSharp.Core", "unit");
            Out.Write(')');
        }

        public override void ExitUnit(UnitContext context)
        {

        }

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {
            try
            {
                var text = context.GetText();
                switch(text)
                {
                    case "object":
                        text = "box";
                        break;
                    case "void":
                        text = "ignore";
                        break;
                }
                Out.WriteCoreOperator(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
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
            Out.WriteNamespacedName("Microsoft.FSharp.Core", "objnull");
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

        public override void EnterCallArgTuple(CallArgTupleContext context)
        {
            EnterState<MemberApplicationState>().EnterCallArgTuple(context);
        }

        public override void ExitCallArgTuple(CallArgTupleContext context)
        {

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

            public override void EnterCallArgTuple(CallArgTupleContext context)
            {
                Out.Write('(');
            }

            public override void ExitCallArgTuple(CallArgTupleContext context)
            {
                Out.Write(')');
                ExitState().ExitCallArgTuple(context);
            }

            public override void EnterIndexAccess(IndexAccessContext context)
            {
                Out.Write('[');
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

        public override void EnterCallArgTuple(CallArgTupleContext context)
        {
            OpenLambda();
            base.EnterCallArgTuple(context);
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

            string ParamName => paramName ?? Error("COMPILER ERROR: Missing constrained member access parameter name.");

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
                    Out.WriteIdentifier(ParamName);
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
                var argsCapture = this.argsCapture ?? ErrorCapture("COMPILER ERROR: Missing constraint expression captured state.");
                Out.StopCapture(argsCapture);
                if(arities.Count == 0)
                {
                    Out.WriteNamespacedName("Microsoft.FSharp.Core", "unit");
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
                            Out.WriteNamespacedName("Microsoft.FSharp.Core", "unit");
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

            public override void EnterCallArgTuple(CallArgTupleContext context)
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

            public override void ExitCallArgTuple(CallArgTupleContext context)
            {
                if(first)
                {
                    // First tuple is empty
                    Out.WriteIdentifier(ParamName);
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
                    Out.WriteIdentifier(ParamName);

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

            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                Error("A call to a constraint member expression cannot contain named arguments.");
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {

            }
        }
    }

    internal sealed class FunctionExprState : NodeState, IFunctionContext
    {
        // Establish a scope to return from
        string? IReturnableStatementContext.ReturnVariable => null;
        string? IReturnableStatementContext.ReturningVariable => null;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;

        string? IInterruptibleStatementContext.InterruptingVariable => null;

        bool IExpressionContext.IsLiteral => false;

        bool IFunctionContext.IsOptionalReturn => false;

        string? IComputationContext.BuilderVariable => null;

        bool IComputationContext.IsCollection => false;

        bool IStatementContext.TrailAllowed => true;

        string? name;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            name = null;
        }

        public override void EnterName(NameContext context)
        {
            Out.EnterNestedScope(true);
            Out.Write("(let rec ");

            base.EnterName(context);
        }

        public override void ExitName(NameContext context)
        {
            base.ExitName(context);

            name = context.GetText();
            Out.WriteOperator('=');
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
                // Scope not entered before
                Out.EnterNestedScope();
                Out.Write('(');
            }
            Out.Write("fun ");

            EnterState<ParamListState>().EnterParamList(context);
        }

        public override void EnterValueBlock(ValueBlockContext context)
        {
            Out.WriteOperator("->");
            Out.EnterNestedScope();
            Out.WriteLine("(");
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitNestedScope();
            if(name != null)
            {
                Out.Write(')');
                Out.WriteLine();
                Out.ExitNestedScope();
                Out.Write(" in ");
                Out.WriteIdentifier(name);
            }
            else
            {
                Out.ExitNestedScope();
                Out.Write(')');
            }
            Out.Write(')');
        }

        void IFunctionContext.WriteBegin()
        {
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        void IFunctionContext.WriteEnd()
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression)
        {
            Error("`break` must be used in a statement that supports it.");
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression)
        {
            Error("`continue` must be used in a statement that supports it.");
        }

        void IInterruptibleStatementContext.WriteAfterBreak()
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue()
        {

        }
    }

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
            Out.Write(isStruct ? "(struct(" : "(");
        }

        public override void ExitTupleConstructor(TupleConstructorContext context)
        {
            Out.Write(isStruct ? "))" : ")");
            ExitState().ExitTupleConstructor(context);
        }

        public override void EnterExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            isStruct = LexerContext.GetState<TuplePragma>()?.IsStruct ?? true;
            Out.Write(isStruct ? "(struct(" : "(");
        }

        public override void ExitExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            Out.Write(isStruct ? "))" : ")");
            ExitState().ExitExplicitTupleConstructor(context);
        }

        public override void EnterClassTupleConstructor(ClassTupleConstructorContext context)
        {
            Out.Write("(");
        }

        public override void ExitClassTupleConstructor(ClassTupleConstructorContext context)
        {
            Out.Write(")");
            ExitState().ExitClassTupleConstructor(context);
        }

        public override void EnterStructTupleConstructor(StructTupleConstructorContext context)
        {
            Out.Write("(struct(");
        }

        public override void ExitStructTupleConstructor(StructTupleConstructorContext context)
        {
            Out.Write("))");
            ExitState().ExitStructTupleConstructor(context);
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

    internal abstract class IsolatedState : NodeState, IFunctionContext
    {
        IExpressionContext? scope;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            scope = null;
        }

        public abstract bool IsCollection { get; }

        public virtual string? BuilderVariable => null;

        public virtual bool TrailAllowed => false;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;

        string? IInterruptibleStatementContext.InterruptingVariable => null;

        bool IFunctionContext.IsOptionalReturn => false;

        string? IReturnableStatementContext.ReturnVariable => null;

        string? IReturnableStatementContext.ReturningVariable => null;

        bool IExpressionContext.IsLiteral => GetExpressionContext()?.IsLiteral ?? false;

        protected sealed override IExpressionContext? GetExpressionContext()
        {
            return scope ??= FindContext<IExpressionContext>();
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression)
        {
            Error("`break` must be used in a statement that supports it.");
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression)
        {
            Error("`continue` must be used in a statement that supports it.");
        }

        void IInterruptibleStatementContext.WriteAfterBreak()
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue()
        {

        }

        void IFunctionContext.WriteBegin()
        {
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        void IFunctionContext.WriteEnd()
        {
            Out.ExitScope();
            Out.Write(_end_);
        }
    }

    internal abstract class CollectionState : IsolatedState
    {
        protected bool IsSimple { get; private set; }
        protected bool IsEmpty { get; private set; }

        public sealed override bool IsCollection => true;

        protected sealed override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            IsEmpty = true;
        }

        public sealed override void EnterSimpleCollectionContents(SimpleCollectionContentsContext context)
        {
            IsSimple = true;
        }

        public sealed override void EnterComplexCollectionContents(ComplexCollectionContentsContext context)
        {
            IsSimple = false;
        }

        public sealed override void ExitSimpleCollectionContents(SimpleCollectionContentsContext context)
        {
            OnLeave();
        }

        public sealed override void ExitComplexCollectionContents(ComplexCollectionContentsContext context)
        {
            OnLeave();
        }

        protected virtual void OnOperand(bool simple)
        {
            if(IsEmpty)
            {
                IsEmpty = false;
                if(IsSimple)
                {
                    Out.Write(' ');
                }
                else
                {
                    Out.WriteLine();
                }
            }
            else
            {
                if(IsSimple)
                {
                    Out.Write(';');
                }
                else
                {
                    Out.WriteLine();
                }
            }
            if(simple && !IsSimple)
            {
                // Yield explicitly (there are complex elements)
                Out.Write("yield ");
            }
        }

        void OnLeave()
        {
            if(IsSimple)
            {
                Out.Write(' ');
            }
            else
            {
                Out.WriteLine();
            }
        }

        public sealed override void EnterSimpleCollectionElement(SimpleCollectionElementContext context)
        {
            OnOperand(true);
        }

        public sealed override void ExitSimpleCollectionElement(SimpleCollectionElementContext context)
        {

        }

        public sealed override void EnterComplexCollectionElement(ComplexCollectionElementContext context)
        {
            OnOperand(false);
        }

        public sealed override void ExitComplexCollectionElement(ComplexCollectionElementContext context)
        {

        }

        public sealed override void EnterExpression(ExpressionContext context)
        {
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public sealed override void ExitExpression(ExpressionContext context)
        {

        }

        public sealed override void EnterCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
            Out.WriteNamespacedName("System.Collections.Generic", "KeyValuePair");
            Out.Write("<_,_>(");
        }

        public sealed override void EnterAssignment(AssignmentContext context)
        {
            Out.Write(',');
        }

        public sealed override void ExitAssignment(AssignmentContext context)
        {
            
        }

        public sealed override void ExitCollectionFieldExpression(CollectionFieldExpressionContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterSpreadExpression(SpreadExpressionContext context)
        {
            Out.Write("yield! ");
            EnterState<SpreadExpression>().EnterSpreadExpression(context);
        }

        sealed class SpreadExpression : ExpressionState
        {
            public override void EnterSpreadExpression(SpreadExpressionContext context)
            {

            }

            public override void ExitSpreadExpression(SpreadExpressionContext context)
            {
                ExitState().ExitSpreadExpression(context);
            }
        }

        public sealed override void EnterExpressionStatement(ExpressionStatementContext context)
        {
            EnterState<ExpressionStatementState>().EnterExpressionStatement(context);
        }

        public sealed override void ExitExpressionStatement(ExpressionStatementContext context)
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

    internal sealed class ArrayState : CollectionState
    {
        public override void EnterArrayConstructor(ArrayConstructorContext context)
        {
            Out.EnterNestedScope();
            Out.Write("[|");
        }

        public override void ExitArrayConstructor(ArrayConstructorContext context)
        {
            if(IsEmpty)
            {
                Out.Write(' ');
            }
            Out.ExitNestedScope();
            Out.Write("|]");
            ExitState().ExitArrayConstructor(context);
        }
    }

    internal sealed class SequenceState : CollectionState
    {
        public override void EnterSequenceConstructor(SequenceConstructorContext context)
        {

        }

        public override void ExitSequenceConstructor(SequenceConstructorContext context)
        {
            if(IsEmpty)
            {
                Out.WriteNamespacedName("Microsoft.FSharp.Collections", "Seq");
                Out.Write(".empty");
            }
            else
            {
                Out.ExitNestedScope();
                Out.Write('}');
                Out.Write(')');
            }
            ExitState().ExitSequenceConstructor(context);
        }

        protected override void OnOperand(bool simple)
        {
            if(IsEmpty)
            {
                Out.EnterNestedScope();
                Out.Write('(');
                Out.WriteCoreOperator("seq");
                Out.Write('{');
            }

            base.OnOperand(simple);
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

        public sealed override void EnterExpressionStatement(ExpressionStatementContext context)
        {
            EnterState<ExpressionStatementState>().EnterExpressionStatement(context);
        }

        public sealed override void ExitExpressionStatement(ExpressionStatementContext context)
        {

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
