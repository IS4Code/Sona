using IS4.Sona.Compiler.States;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler
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
                Out.WriteIdentifier(context.GetText().TrimStart('@'));
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

        public override void EnterNamedValue(NamedValueContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitNamedValue(NamedValueContext context)
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

        public override void EnterString(StringContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitString(StringContext context)
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
            EnterState<SequenceState>().EnterSequenceConstructor(context);
        }

        public override void ExitSequenceConstructor(SequenceConstructorContext context)
        {

        }

        protected virtual IExpressionContext? GetExpressionContext() => FindContext<IExpressionContext>();

        public override void EnterInterpolatedString(InterpolatedStringContext context)
        {
            if(GetExpressionContext()?.IsLiteral ?? false)
            {
                EnterState<LiteralNormalInterpolatedString>().EnterInterpolatedString(context);
            }
            else
            {
                EnterState<InterpolatedString>().EnterInterpolatedString(context);
            }
        }

        public override void EnterVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {
            if(GetExpressionContext()?.IsLiteral ?? false)
            {
                EnterState<LiteralVerbatimInterpolatedString>().EnterVerbatimInterpolatedString(context);
            }
            else
            {
                EnterState<InterpolatedString>().EnterVerbatimInterpolatedString(context);
            }
        }

        public override void ExitInterpolatedString(InterpolatedStringContext context)
        {

        }

        public override void ExitVerbatimInterpolatedString(VerbatimInterpolatedStringContext context)
        {

        }

        public override void EnterOuterBinaryOperator(OuterBinaryOperatorContext context)
        {
            Environment.EnableParseTree();
        }

        public override void EnterInnerBinaryOperator(InnerBinaryOperatorContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitOuterBinaryOperator(OuterBinaryOperatorContext context)
        {
            try
            {
                string text = context.GetText();
                switch(text)
                {
                    case "==":
                        text = "=";
                        break;
                    case "!=" or "~=":
                        text = "<>";
                        break;
                }
                Out.WriteOperator(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void ExitInnerBinaryOperator(InnerBinaryOperatorContext context)
        {
            try
            {
                string text = context.GetText();
                Out.WriteOperator(text);
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterUnaryOperator(UnaryOperatorContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitUnaryOperator(UnaryOperatorContext context)
        {
            try
            {
                base.ExitUnaryOperator(context);

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
