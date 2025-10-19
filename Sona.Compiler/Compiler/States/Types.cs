using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class TypeState : NodeState
    {
        IToken? startToken;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            startToken = null;
        }

        public override void EnterAtomicType(AtomicTypeContext context)
        {
            startToken = context.Start;
        }

        public override void ExitAtomicType(AtomicTypeContext context)
        {

        }

        public override void EnterType(TypeContext context)
        {

        }

        public override void ExitType(TypeContext context)
        {
            ExitState().ExitType(context);
        }

        public sealed override void EnterUnit(UnitContext context)
        {
            Out.WriteCoreName("unit");
        }

        public sealed override void ExitUnit(UnitContext context)
        {

        }

        public override void EnterPrimitiveType(PrimitiveTypeContext context)
        {
            var token = context.Start;
            string text;
            switch(token.Type)
            {
                case SonaLexer.OBJECT:
                    text = "obj";
                    break;
                case SonaLexer.VOID:
                    Out.WriteSystemName("Void");
                    return;
                case SonaLexer.INT8:
                    text = "sbyte";
                    break;
                case SonaLexer.UINT8:
                    text = "byte";
                    break;
                case SonaLexer.INT16:
                    text = "int16";
                    break;
                case SonaLexer.UINT16:
                    text = "uint16";
                    break;
                case SonaLexer.INT64:
                    text = "int64";
                    break;
                case SonaLexer.UINT64:
                    text = "uint64";
                    break;
                case SonaLexer.INT128:
                    Out.WriteSystemName("Int128");
                    return;
                case SonaLexer.UINT128:
                    Out.WriteSystemName("UInt128");
                    return;
                case SonaLexer.FLOAT16:
                    Out.WriteSystemName("Half");
                    return;
                case SonaLexer.FLOAT32:
                    // Might be 'single'
                    text = "float32";
                    break;
                case SonaLexer.FLOAT64:
                    // Might be 'double'
                    text = "float";
                    break;
                case SonaLexer.EXCEPTION:
                    Out.WriteSystemName("Exception");
                    return;
                default:
                    text = token.Text;
                    break;
            }
            Out.WriteCoreName(text);
        }

        public override void ExitPrimitiveType(PrimitiveTypeContext context)
        {

        }

        public override void EnterTypeArgument(TypeArgumentContext context)
        {
            EnterState<Argument>().EnterTypeArgument(context);
        }

        public override void ExitTypeArgument(TypeArgumentContext context)
        {

        }

        public override void EnterNullableType(NullableTypeContext context)
        {
            EnterState<NullableType>().EnterNullableType(context);
        }

        public override void ExitNullableType(NullableTypeContext context)
        {

        }

        public override void EnterConjunctionType(ConjunctionTypeContext context)
        {
            EnterState<ConjunctionType>().EnterConjunctionType(context);
        }

        public override void ExitConjunctionType(ConjunctionTypeContext context)
        {

        }

        public override void EnterNestedType(NestedTypeContext context)
        {
            EnterState<NestedType>().EnterNestedType(context);
        }

        public override void ExitNestedType(NestedTypeContext context)
        {

        }

        public override void EnterFunctionType(FunctionTypeContext context)
        {
            Out.Write('(');
            EnterState<FunctionType>().EnterFunctionType(context);
        }

        public override void ExitFunctionType(FunctionTypeContext context)
        {
            Out.Write(')');
        }

        public override void EnterTupleType(TupleTypeContext context)
        {
            EnterState<TupleType>().EnterTupleType(context);
        }

        public override void ExitTupleType(TupleTypeContext context)
        {

        }

        public override void EnterClassTupleType(ClassTupleTypeContext context)
        {
            EnterState<TupleType>().EnterClassTupleType(context);
        }

        public override void ExitClassTupleType(ClassTupleTypeContext context)
        {

        }

        public override void EnterStructTupleType(StructTupleTypeContext context)
        {
            EnterState<TupleType>().EnterStructTupleType(context);
        }

        public override void ExitStructTupleType(StructTupleTypeContext context)
        {

        }

        public override void EnterAnonymousRecordType(AnonymousRecordTypeContext context)
        {
            EnterState<AnonymousRecordType>().EnterAnonymousRecordType(context);
        }

        public override void ExitAnonymousRecordType(AnonymousRecordTypeContext context)
        {

        }

        public override void EnterAnonymousClassRecordType(AnonymousClassRecordTypeContext context)
        {
            EnterState<AnonymousRecordType>().EnterAnonymousClassRecordType(context);
        }

        public override void ExitAnonymousClassRecordType(AnonymousClassRecordTypeContext context)
        {

        }

        public override void EnterAnonymousStructRecordType(AnonymousStructRecordTypeContext context)
        {
            EnterState<AnonymousRecordType>().EnterAnonymousStructRecordType(context);
        }

        public override void ExitAnonymousStructRecordType(AnonymousStructRecordTypeContext context)
        {

        }

        public sealed override void EnterInlineSourceFree(InlineSourceFreeContext context)
        {
            Out.EnterNestedScope();
            Out.Write('(');
            EnterState<InlineSourceType>().EnterInlineSourceFree(context);
        }

        public sealed override void ExitInlineSourceFree(InlineSourceFreeContext context)
        {
            Out.WriteLine();
            Out.ExitNestedScope();
            Out.Write(')');
        }

        public override void EnterTypeSuffix(TypeSuffixContext context)
        {
            if(startToken == context.Start)
            {
                // This is the first token of the type.
                Out.Write('_');
            }
        }

        public override void ExitTypeSuffix(TypeSuffixContext context)
        {

        }

        public sealed override void EnterArrayTypeSuffix(ArrayTypeSuffixContext context)
        {
            Out.WriteCollectionTypeSuffix(CollectionImplementationType);
        }

        public sealed override void ExitArrayTypeSuffix(ArrayTypeSuffixContext context)
        {

        }

        public sealed override void EnterMultiArrayTypeSuffix(MultiArrayTypeSuffixContext context)
        {
            EnterState<MultiArraySuffix>().EnterMultiArrayTypeSuffix(context);
        }

        public sealed override void ExitMultiArrayTypeSuffix(MultiArrayTypeSuffixContext context)
        {

        }

        public sealed override void EnterOptionTypeSuffix(OptionTypeSuffixContext context)
        {

        }

        public sealed override void ExitOptionTypeSuffix(OptionTypeSuffixContext context)
        {
            Out.Write(' ');
            Out.WriteOptionAbbreviation(OptionImplementationType);
        }

        public sealed override void EnterSequenceTypeSuffix(SequenceTypeSuffixContext context)
        {

        }

        public sealed override void ExitSequenceTypeSuffix(SequenceTypeSuffixContext context)
        {
            Out.Write(' ');
            Out.WriteCollectionName("seq");
        }

        public sealed class Argument : TypeState
        {
            bool hasType;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                hasType = false;
            }

            public override void EnterType(TypeContext context)
            {
                hasType = true;
            }

            public override void ExitType(TypeContext context)
            {

            }

            public override void EnterTypeArgument(TypeArgumentContext context)
            {

            }

            void OnNext()
            {
                if(!hasType)
                {
                    // No core type has been written yet.
                    Out.Write('_');
                    hasType = true;
                }
            }

            public override void ExitTypeArgument(TypeArgumentContext context)
            {
                OnNext();
                ExitState().ExitTypeArgument(context);
            }

            public override void EnterTypeSuffix(TypeSuffixContext context)
            {
                OnNext();
                base.EnterTypeSuffix(context);
            }
        }

        sealed class NullableType : TypeState
        {
            bool hasType;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                hasType = false;
            }

            public override void EnterNullableType(NullableTypeContext context)
            {
                Out.Write('(');
            }

            public override void ExitNullableType(NullableTypeContext context)
            {
                if(!hasType)
                {
                    Out.Write('_');
                }
                Out.WriteOperator('|');
                Out.Write("null");
                Out.Write(')');
                ExitState().ExitNullableType(context);
            }

            public override void EnterAtomicType(AtomicTypeContext context)
            {
                hasType = true;
                base.EnterAtomicType(context);
            }

            public override void ExitAtomicType(AtomicTypeContext context)
            {
                base.ExitAtomicType(context);
            }
        }

        sealed class ConjunctionType : TypeState
        {
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
            }

            public override void EnterConjunctionType(ConjunctionTypeContext context)
            {
                Out.Write('(');
            }

            public override void ExitConjunctionType(ConjunctionTypeContext context)
            {
                Out.Write(')');
                ExitState().ExitConjunctionType(context);
            }

            public override void EnterAtomicType(AtomicTypeContext context)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.WriteOperator('&');
                }
                Out.Write("#(");
                base.EnterAtomicType(context);
            }

            public override void ExitAtomicType(AtomicTypeContext context)
            {
                base.ExitAtomicType(context);
                Out.Write(')');
            }
        }

        sealed class NestedType : TypeState
        {
            public override void EnterType(TypeContext context)
            {

            }

            public override void ExitType(TypeContext context)
            {

            }

            public override void EnterNestedType(NestedTypeContext context)
            {
                Out.Write('(');
            }

            public override void ExitNestedType(NestedTypeContext context)
            {
                Out.Write(')');
                ExitState().ExitNestedType(context);
            }
        }

        sealed class MultiArraySuffix : NodeState
        {
            public override void EnterMultiArrayTypeSuffix(MultiArrayTypeSuffixContext context)
            {

            }

            public override void ExitMultiArrayTypeSuffix(MultiArrayTypeSuffixContext context)
            {
                ExitState().ExitMultiArrayTypeSuffix(context);
            }

            public override void VisitTerminal(ITerminalNode node)
            {
                base.VisitTerminal(node);

                Out.Write(node.Symbol.Text);
            }
        }
    }
    
    internal sealed class GenericArgumentsState : GenericArgumentState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
        }

        public override void EnterGenericArguments(GenericArgumentsContext context)
        {
            Out.Write('<');
        }

        public override void ExitGenericArguments(GenericArgumentsContext context)
        {
            Out.Write('>');
            ExitState().ExitGenericArguments(context);
        }

        public override void EnterGenericArgument(GenericArgumentContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(',');
            }
        }

        public override void ExitGenericArgument(GenericArgumentContext context)
        {

        }
    }

    internal class GenericArgumentState : TypeState, IExpressionContext
    {
        ExpressionFlags IExpressionContext.Flags => ExpressionFlags.IsValue | ExpressionFlags.IsConstant;

        public override void EnterGenericArgument(GenericArgumentContext context)
        {

        }

        public override void ExitGenericArgument(GenericArgumentContext context)
        {
            ExitState().ExitGenericArgument(context);
        }

        public override void EnterMeasureArgument(MeasureArgumentContext context)
        {
            Out.Write("1*(");
            EnterState<Measure>().EnterMeasureArgument(context);
        }

        public override void ExitMeasureArgument(MeasureArgumentContext context)
        {
            Out.Write(')');
        }

        public override void EnterLiteralArgument(LiteralArgumentContext context)
        {
            Out.Write("const(");
        }

        public override void ExitLiteralArgument(LiteralArgumentContext context)
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

        class Measure : NodeState
        {
            public override void EnterMeasureArgument(MeasureArgumentContext context)
            {

            }

            public override void ExitMeasureArgument(MeasureArgumentContext context)
            {
                ExitState().ExitMeasureArgument(context);
            }

            public override void VisitTerminal(ITerminalNode node)
            {
                base.VisitTerminal(node);

                var token = node.Symbol;
                switch(token.Type)
                {
                    case SonaLexer.OPENP:
                    case SonaLexer.CLOSEP:
                    case SonaLexer.ASTERISK:
                    case SonaLexer.SLASH:
                    case SonaLexer.SINGLE_XOR:
                    case SonaLexer.MINUS:
                        Out.Write(token.Text);
                        break;
                }
            }
        }
    }

    internal sealed class FunctionType : NodeState
    {
        bool hasParameters;
        bool hasReturn;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasParameters = false;
            hasReturn = false;
        }

        public override void EnterFunctionType(FunctionTypeContext context)
        {

        }

        public override void ExitFunctionType(FunctionTypeContext context)
        {
            if(!hasReturn)
            {
                OnReturn();
                Out.WriteOperator("->");
                Out.Write('_');
            }
            ExitState().ExitFunctionType(context);
        }

        public override void EnterParamTypesList(ParamTypesListContext context)
        {
            hasParameters = true;
            EnterState<Parameters>().EnterParamTypesList(context);
        }

        public override void ExitParamTypesList(ParamTypesListContext context)
        {

        }

        void OnReturn()
        {
            if(!hasParameters)
            {
                Out.Write('_');
            }
        }

        public override void EnterType(TypeContext context)
        {
            OnReturn();
            Out.WriteOperator("->");
            hasReturn = true;
            EnterState<TypeState>().EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {

        }

        sealed class Parameters : TypeState
        {
            bool firstTuple;
            bool firstParameter;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                firstTuple = true;
            }

            public override void EnterParamTypesList(ParamTypesListContext context)
            {

            }

            public override void ExitParamTypesList(ParamTypesListContext context)
            {
                ExitState().ExitParamTypesList(context);
            }

            public override void EnterParamTypesTuple(ParamTypesTupleContext context)
            {
                if(firstTuple)
                {
                    firstTuple = false;
                }
                else
                {
                    Out.WriteOperator("->");
                }
                firstParameter = true;
            }

            public override void ExitParamTypesTuple(ParamTypesTupleContext context)
            {
                if(firstParameter)
                {
                    // No parameters
                    Out.WriteCoreName("unit");
                }
                else
                {
                    Out.Write(')');
                }
            }

            void OnParameter()
            {
                if(firstParameter)
                {
                    firstParameter = false;
                    Out.Write('(');
                }
                else
                {
                    Out.WriteOperator('*');
                }
            }

            public override void EnterType(TypeContext context)
            {
                OnParameter();
            }

            public override void ExitType(TypeContext context)
            {

            }

            public override void EnterTypeArgument(TypeArgumentContext context)
            {
                OnParameter();
                base.EnterTypeArgument(context);
            }

            public override void ExitTypeArgument(TypeArgumentContext context)
            {
                base.ExitTypeArgument(context);
            }
        }
    }

    internal sealed class TupleType : NodeState
    {
        bool first;
        ImplementationType tupleType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            tupleType = default;
        }

        public override void EnterTupleType(TupleTypeContext context)
        {
            tupleType = TupleImplementationType;
            OnEnter();
        }

        public override void ExitTupleType(TupleTypeContext context)
        {
            OnExit();
            ExitState().ExitTupleType(context);
        }

        public override void EnterClassTupleType(ClassTupleTypeContext context)
        {
            tupleType = ImplementationType.Class;
            OnEnter();
        }

        public override void ExitClassTupleType(ClassTupleTypeContext context)
        {
            OnExit();
            ExitState().ExitClassTupleType(context);
        }

        public override void EnterStructTupleType(StructTupleTypeContext context)
        {
            tupleType = ImplementationType.Struct;
            OnEnter();
        }

        public override void ExitStructTupleType(StructTupleTypeContext context)
        {
            OnExit();
            ExitState().ExitStructTupleType(context);
        }

        void OnEnter()
        {
            Out.WriteTupleOpen(tupleType);
        }

        void OnExit()
        {
            Out.WriteTupleClose(tupleType);
        }

        public override void EnterTypeArgument(TypeArgumentContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.WriteOperator('*');
            }
            EnterState<TypeState.Argument>().EnterTypeArgument(context);
        }

        public override void ExitTypeArgument(TypeArgumentContext context)
        {

        }
    }

    internal sealed class AnonymousRecordType : NodeState
    {
        bool first, hasType;
        ImplementationType recordType;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
            recordType = default;
        }

        public override void EnterAnonymousRecordType(AnonymousRecordTypeContext context)
        {
            recordType = RecordImplementationType;
            OnEnter();
        }

        public override void ExitAnonymousRecordType(AnonymousRecordTypeContext context)
        {
            OnExit();
            ExitState().ExitAnonymousRecordType(context);
        }

        public override void EnterAnonymousClassRecordType(AnonymousClassRecordTypeContext context)
        {
            recordType = ImplementationType.Class;
            OnEnter();
        }

        public override void ExitAnonymousClassRecordType(AnonymousClassRecordTypeContext context)
        {
            OnExit();
            ExitState().ExitAnonymousClassRecordType(context);
        }

        public override void EnterAnonymousStructRecordType(AnonymousStructRecordTypeContext context)
        {
            recordType = ImplementationType.Struct;
            OnEnter();
        }

        public override void ExitAnonymousStructRecordType(AnonymousStructRecordTypeContext context)
        {
            OnExit();
            ExitState().ExitAnonymousStructRecordType(context);
        }

        void OnEnter()
        {
            Out.WriteAnonymousRecordOpen(recordType);
        }

        void OnExit()
        {
            Out.WriteAnonymousRecordClose(recordType);
        }

        public override void EnterAnonymousRecordMemberDeclaration(AnonymousRecordMemberDeclarationContext context)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                Out.Write(';');
            }
            hasType = false;
        }

        public override void ExitAnonymousRecordMemberDeclaration(AnonymousRecordMemberDeclarationContext context)
        {
            if(!hasType)
            {
                Out.WriteOperator(':');
                Out.Write('_');
            }
        }

        public override void EnterType(TypeContext context)
        {
            hasType = true;
            Out.WriteOperator(':');
            EnterState<TypeState>().EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {

        }
    }
}
