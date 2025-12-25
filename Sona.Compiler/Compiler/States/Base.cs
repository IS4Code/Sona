using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Sona.Compiler.States;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler
{
    partial class ScriptState
    {
        public override void EnterName(NameContext context)
        {
            StartCaptureInput(context);
        }

        public override void ExitName(NameContext context)
        {
            Out.WriteIdentifier(StopCaptureInputIdentifier(context));
        }

        public override void EnterMemberName(MemberNameContext context)
        {
            StartCaptureInput(context);
        }

        public override void ExitMemberName(MemberNameContext context)
        {
            Out.WriteIdentifier(StopCaptureInputIdentifier(context));
        }

        public override void EnterDynamicMemberName(DynamicMemberNameContext context)
        {
            StartCaptureInput(context);
        }

        public override void ExitDynamicMemberName(DynamicMemberNameContext context)
        {
            Out.WriteIdentifier(StopCaptureInputIdentifier(context));
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

        public override void EnterTokenGTE(TokenGTEContext context)
        {
            EnterState<CompoundTerminal>().EnterTokenGTE(context);
        }

        public override void ExitTokenGTE(TokenGTEContext context)
        {

        }

        public override void EnterTokenRSHIFT(TokenRSHIFTContext context)
        {
            EnterState<CompoundTerminal>().EnterTokenRSHIFT(context);
        }

        public override void ExitTokenRSHIFT(TokenRSHIFTContext context)
        {

        }

        public override void EnterTokenDOUBLEQUESTION(TokenDOUBLEQUESTIONContext context)
        {
            EnterState<CompoundTerminal>().EnterTokenDOUBLEQUESTION(context);
        }

        public override void ExitTokenDOUBLEQUESTION(TokenDOUBLEQUESTIONContext context)
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
                    if(GetExpressionContext()?.HasFlag(ExpressionFlags.IsConstant) ?? false)
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
            StartCaptureInput(context);
        }

        public override void ExitNumber(NumberContext context)
        {
            Out.Write(StopCaptureInput(context));
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

        public override void EnterComputationSequenceConstructor(ComputationSequenceConstructorContext context)
        {
            EnterState<ComputationSequenceState>().EnterComputationSequenceConstructor(context);
        }

        public override void ExitComputationSequenceConstructor(ComputationSequenceConstructorContext context)
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

        private bool ContextIsConstant => (this as IExpressionContext ?? GetExpressionContext())?.HasFlag(ExpressionFlags.IsConstant) ?? false;

        public override void EnterPlainInterpolatedString(PlainInterpolatedStringContext context)
        {
            if(ContextIsConstant)
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
            if(ContextIsConstant)
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
            StartCaptureInput(context);
        }

        public override void ExitUnaryOperator(UnaryOperatorContext context)
        {
            var text = StopCaptureInput(context);
            switch(text)
            {
                case "~":
                    text = "~~~";
                    break;
            }
            Out.WriteOperator(text);
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

        sealed class CompoundTerminal : NodeState
        {
            readonly List<ITerminalNode> nodes = new();
            readonly StringBuilder text = new();

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                nodes.Clear();
                text.Clear();
            }

            private ScriptState ExitState(int type, RuleContext context)
            {
                var parent = base.ExitState();

                parent.VisitTerminal(new TerminalNode(text.ToString(), type, nodes[0], nodes[nodes.Count - 1], context.Parent));

                return parent;
            }

            public override void EnterTokenGTE(TokenGTEContext context)
            {

            }

            public override void ExitTokenGTE(TokenGTEContext context)
            {
                ExitState(SonaLexer.GTE, context).ExitTokenGTE(context);
            }

            public override void EnterTokenRSHIFT(TokenRSHIFTContext context)
            {

            }

            public override void ExitTokenRSHIFT(TokenRSHIFTContext context)
            {
                ExitState(SonaLexer.RSHIFT, context).ExitTokenRSHIFT(context);
            }

            public override void EnterTokenDOUBLEQUESTION(TokenDOUBLEQUESTIONContext context)
            {

            }

            public override void ExitTokenDOUBLEQUESTION(TokenDOUBLEQUESTIONContext context)
            {
                ExitState(SonaLexer.DOUBLE_QUESTION, context).ExitTokenDOUBLEQUESTION(context);
            }

            public override void VisitTerminal(ITerminalNode node)
            {
                // Do not call base because the virtual token will be visited as a whole

                nodes.Add(node);
                text.Append(node.Symbol.Text);
            }

            sealed record TerminalNode(string Text, int Type, ITerminalNode First, ITerminalNode Last, RuleContext Context) : ITerminalNode, IToken
            {
                IToken ITerminalNode.Symbol => this;

                IRuleNode ITerminalNode.Parent => Context.Parent;

                IParseTree IParseTree.Parent => Context.Parent;

                ITree ITree.Parent => Context.Parent;

                Interval ISyntaxTree.SourceInterval => new(First.SourceInterval.a, Last.SourceInterval.b);

                object ITree.Payload => Context.Payload;

                int ITree.ChildCount => 0;

                int IToken.Line => First.Symbol.Line;

                int IToken.Column => First.Symbol.Column;

                int IToken.Channel => First.Symbol.Channel;

                int IToken.TokenIndex => Last.Symbol.TokenIndex;

                int IToken.StartIndex => First.Symbol.StartIndex;

                int IToken.StopIndex => Last.Symbol.StopIndex;

                ITokenSource IToken.TokenSource => First.Symbol.TokenSource;

                ICharStream IToken.InputStream => First.Symbol.InputStream;

                T IParseTree.Accept<T>(IParseTreeVisitor<T> visitor)
                {
                    return visitor.VisitTerminal(this);
                }

                IParseTree IParseTree.GetChild(int i)
                {
                    throw new ArgumentOutOfRangeException(nameof(i));
                }

                ITree ITree.GetChild(int i)
                {
                    throw new ArgumentOutOfRangeException(nameof(i));
                }

                string IParseTree.GetText() => Text;
                string IParseTree.ToStringTree(Parser parser) => Text;
                string ITree.ToStringTree() => Text;
                public override string ToString() => Text;
            }
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
                Out.WriteNext('.', ref first);
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
