using Sona.Compiler.States;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler
{
    partial class ScriptState
    {
        public override void EnterName(NameContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitName(NameContext context)
        {
            try
            {
                Out.WriteIdentifier(Tools.Syntax.GetIdentifierFromName(context.GetText()));
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterMemberName(MemberNameContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitMemberName(MemberNameContext context)
        {
            try
            {
                Out.WriteIdentifier(context.GetText().Substring(1));
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterDynamicMemberName(DynamicMemberNameContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitDynamicMemberName(DynamicMemberNameContext context)
        {
            try
            {
                Out.WriteIdentifier(context.GetText().Substring(1));
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterCompoundName(CompoundNameContext context)
        {
            EnterState<CompoundNameState>().EnterCompoundName(context);
        }

        public override void ExitCompoundName(CompoundNameContext context)
        {

        }

        public override void EnterCompoundNameGeneric(CompoundNameGenericContext context)
        {
            EnterState<CompoundNameState>().EnterCompoundNameGeneric(context);
        }

        public override void ExitCompoundNameGeneric(CompoundNameGenericContext context)
        {

        }

        public override void EnterGenericArguments(GenericArgumentsContext context)
        {
            EnterState<GenericArgumentsState>().EnterGenericArguments(context);
        }

        public override void ExitGenericArguments(GenericArgumentsContext context)
        {

        }

        public override void EnterNamedValue(NamedValueContext context)
        {
            var token = context.Start;
            switch(token.Type)
            {
                case SonaLexer.NONE:
                    Out.WriteOptionNone(OptionImplementationType);
                    break;
                case SonaLexer.DEFAULT:
                    if(GetExpressionContext()?.Type == ExpressionType.Literal)
                    {
                        Out.Write("(new _())");
                    }
                    else
                    {
                        Out.WriteCustomOperator("Default");
                    }
                    break;
                default:
                    Out.Write(token.Text);
                    break;
            }
        }

        public override void ExitNamedValue(NamedValueContext context)
        {

        }

        public override void EnterNumber(NumberContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitNumber(NumberContext context)
        {
            try
            {
                Out.Write(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterChar(CharContext context)
        {
            EnterState<StringState>().EnterChar(context);
        }

        public override void ExitChar(CharContext context)
        {

        }

        public override void EnterString(StringContext context)
        {
            EnterState<StringState>().EnterString(context);
        }

        public override void ExitString(StringContext context)
        {

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

        public override void EnterAnonymousRecordConstructor(AnonymousRecordConstructorContext context)
        {
            EnterState<RecordState>().EnterAnonymousRecordConstructor(context);
        }

        public override void ExitAnonymousRecordConstructor(AnonymousRecordConstructorContext context)
        {

        }

        public override void EnterAnonymousClassRecordConstructor(AnonymousClassRecordConstructorContext context)
        {
            EnterState<RecordState>().EnterAnonymousClassRecordConstructor(context);
        }

        public override void ExitAnonymousClassRecordConstructor(AnonymousClassRecordConstructorContext context)
        {

        }

        public override void EnterAnonymousStructRecordConstructor(AnonymousStructRecordConstructorContext context)
        {
            EnterState<RecordState>().EnterAnonymousStructRecordConstructor(context);
        }

        public override void ExitAnonymousStructRecordConstructor(AnonymousStructRecordConstructorContext context)
        {

        }

        public override void EnterTupleConstructor(TupleConstructorContext context)
        {
            EnterState<TupleState>().EnterTupleConstructor(context);
        }

        public override void ExitTupleConstructor(TupleConstructorContext context)
        {

        }

        public override void EnterExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {
            EnterState<TupleState>().EnterExplicitTupleConstructor(context);
        }

        public override void ExitExplicitTupleConstructor(ExplicitTupleConstructorContext context)
        {

        }

        public override void EnterClassTupleConstructor(ClassTupleConstructorContext context)
        {
            EnterState<TupleState>().EnterClassTupleConstructor(context);
        }

        public override void ExitClassTupleConstructor(ClassTupleConstructorContext context)
        {

        }

        public override void EnterStructTupleConstructor(StructTupleConstructorContext context)
        {
            EnterState<TupleState>().EnterStructTupleConstructor(context);
        }

        public override void ExitStructTupleConstructor(StructTupleConstructorContext context)
        {

        }

        public override void EnterSequenceConstructor(SequenceConstructorContext context)
        {
            EnterState<SequenceState>().EnterSequenceConstructor(context);
        }

        public override void ExitSequenceConstructor(SequenceConstructorContext context)
        {

        }

        public override void EnterType(TypeContext context)
        {
            EnterState<TypeState>().EnterType(context);
        }

        public override void ExitType(TypeContext context)
        {

        }

        public override void EnterPattern(PatternContext context)
        {
            EnterState<PatternState>().EnterPattern(context);
        }

        public override void ExitPattern(PatternContext context)
        {

        }

        protected virtual IExpressionContext? GetExpressionContext() => FindContext<IExpressionContext>();

        public override void EnterPlainInterpolatedString(PlainInterpolatedStringContext context)
        {
            if(GetExpressionContext()?.Type == ExpressionType.Literal)
            {
                EnterState<LiteralNormalInterpolatedString>().EnterPlainInterpolatedString(context);
            }
            else
            {
                EnterState<InterpolatedString>().EnterPlainInterpolatedString(context);
            }
        }

        public override void EnterVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            if(GetExpressionContext()?.Type == ExpressionType.Literal)
            {
                EnterState<LiteralVerbatimInterpolatedString>().EnterVerbatimInterpolatedString(context);
            }
            else
            {
                EnterState<InterpolatedString>().EnterVerbatimInterpolatedString(context);
            }
        }

        public override void ExitPlainInterpolatedString(PlainInterpolatedStringContext context)
        {

        }

        public override void ExitVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {

        }

        public override void EnterUnaryOperator(UnaryOperatorContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitUnaryOperator(UnaryOperatorContext context)
        {
            try
            {
                string text = context.GetText();
                switch(text)
                {
                    case "~":
                        text = "~~~";
                        break;
                }
                Out.WriteOperator(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterLocalAttribute(LocalAttributeContext context)
        {
            EnterState<AttributeState>().EnterLocalAttribute(context);
        }

        public override void ExitLocalAttribute(LocalAttributeContext context)
        {

        }

        public override void EnterGlobalAttribute(GlobalAttributeContext context)
        {
            EnterState<AttributeState>().EnterGlobalAttribute(context);
        }

        public override void ExitGlobalAttribute(GlobalAttributeContext context)
        {

        }
    }

    namespace States
    {
        internal sealed class CompoundNameState : NodeState
        {
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
            }

            public override void EnterCompoundName(CompoundNameContext context)
            {

            }

            public override void ExitCompoundName(CompoundNameContext context)
            {
                ExitState().ExitCompoundName(context);
            }

            public override void EnterCompoundNameGeneric(CompoundNameGenericContext context)
            {

            }

            public override void ExitCompoundNameGeneric(CompoundNameGenericContext context)
            {
                ExitState().ExitCompoundNameGeneric(context);
            }

            void OnName()
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.Write('.');
                }
            }

            public override void EnterName(NameContext context)
            {
                OnName();
                base.EnterName(context);
            }

            public override void EnterMemberName(MemberNameContext context)
            {
                OnName();
                base.EnterMemberName(context);
            }
        }
    }
}
