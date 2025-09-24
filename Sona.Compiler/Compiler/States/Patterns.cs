using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class PatternState : NodeState, IExpressionContext
    {
        int parenthesisLevel;

        ExpressionType IExpressionContext.Type => ExpressionType.Pattern;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            parenthesisLevel = 0;
        }

        public override void EnterPattern(PatternContext context)
        {

        }

        public override void ExitPattern(PatternContext context)
        {
            OnExit();
            ExitState().ExitPattern(context);
        }

        private void CloseParentheses()
        {
            while(parenthesisLevel > 0)
            {
                Out.Write(')');
                parenthesisLevel--;
            }
        }

        protected void OnExit()
        {
            CloseParentheses();
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
            var token = node.Symbol;
            switch(token.Type)
            {
                case SonaLexer.AND:
                    Out.Write(" as(");
                    parenthesisLevel++;
                    break;
                case SonaLexer.OR:
                    // Needs to close parentheses opened by `is`
                    CloseParentheses();
                    Out.WriteOperator('|');
                    break;
                case SonaLexer.AS:
                    if(this is not TuplePattern)
                    {
                        Out.WriteOperator(':');
                    }
                    break;
                case SonaLexer.IS:
                    Out.Write("(:? ");
                    parenthesisLevel++;
                    break;
            }
        }

        public override void EnterNestedPattern(NestedPatternContext context)
        {
            EnterState<NestedPattern>().EnterNestedPattern(context);
        }

        public override void ExitNestedPattern(NestedPatternContext context)
        {

        }

        public override void EnterPatternArgument(PatternArgumentContext context)
        {
            EnterState<Argument>().EnterPatternArgument(context);
        }

        public override void ExitPatternArgument(PatternArgumentContext context)
        {

        }

        public sealed override void EnterUnit(UnitContext context)
        {
            Out.Write("(()");
            Out.WriteOperator(':');
            Out.WriteCoreName("unit");
            Out.Write(')');
        }

        public sealed override void ExitUnit(UnitContext context)
        {

        }

        public sealed override void EnterInlineSourceFree(InlineSourceFreeContext context)
        {
            EnterState<InlineSource>().EnterInlineSourceFree(context);
        }

        public sealed override void ExitInlineSourceFree(InlineSourceFreeContext context)
        {

        }

        public override void EnterUnaryNumberConvertExpr(UnaryNumberConvertExprContext context)
        {
            EnterState<NumberConversionState>().EnterUnaryNumberConvertExpr(context);
        }

        public override void ExitUnaryNumberConvertExpr(UnaryNumberConvertExprContext context)
        {

        }

        public override void EnterUnaryCharConvertExpr(UnaryCharConvertExprContext context)
        {
            EnterState<CharConversionState>().EnterUnaryCharConvertExpr(context);
        }

        public override void ExitUnaryCharConvertExpr(UnaryCharConvertExprContext context)
        {

        }

        public override void EnterSomePattern(SomePatternContext context)
        {
            EnterState<SomePattern>().EnterSomePattern(context);
        }

        public override void ExitSomePattern(SomePatternContext context)
        {

        }

        public sealed override void EnterTypeArgument(TypeArgumentContext context)
        {
            EnterState<TypeState.Argument>().EnterTypeArgument(context);
        }

        public sealed override void ExitTypeArgument(TypeArgumentContext context)
        {

        }

        public sealed override void EnterTypePatternExplicit(TypePatternExplicitContext context)
        {
            parenthesisLevel++;
            Out.Write(" as(");
        }

        public sealed override void ExitTypePatternExplicit(TypePatternExplicitContext context)
        {

        }

        public sealed override void EnterTypePatternImplicit(TypePatternImplicitContext context)
        {
            parenthesisLevel++;
            Out.Write("_ as(");
        }

        public sealed override void ExitTypePatternImplicit(TypePatternImplicitContext context)
        {

        }

        public sealed override void EnterSimplePatternArgument(SimplePatternArgumentContext context)
        {
            Out.Write('(');
        }

        public sealed override void ExitSimplePatternArgument(SimplePatternArgumentContext context)
        {
            Out.Write(')');
        }

        public override void EnterPatternArguments(PatternArgumentsContext context)
        {
            EnterState<Arguments>().EnterPatternArguments(context);
        }

        public override void ExitPatternArguments(PatternArgumentsContext context)
        {

        }

        public override void EnterArrayConstructorPattern(ArrayConstructorPatternContext context)
        {
            EnterState<ArrayPattern>().EnterArrayConstructorPattern(context);
        }

        public override void ExitArrayConstructorPattern(ArrayConstructorPatternContext context)
        {

        }

        public override void EnterRecordConstructorPattern(RecordConstructorPatternContext context)
        {
            EnterState<RecordPattern>().EnterRecordConstructorPattern(context);
        }

        public override void ExitRecordConstructorPattern(RecordConstructorPatternContext context)
        {

        }

        public override void EnterTupleConstructorPattern(TupleConstructorPatternContext context)
        {
            EnterState<TuplePattern>().EnterTupleConstructorPattern(context);
        }

        public override void ExitTupleConstructorPattern(TupleConstructorPatternContext context)
        {

        }

        public override void EnterExplicitTupleConstructorPattern(ExplicitTupleConstructorPatternContext context)
        {
            EnterState<TuplePattern>().EnterExplicitTupleConstructorPattern(context);
        }

        public override void ExitExplicitTupleConstructorPattern(ExplicitTupleConstructorPatternContext context)
        {

        }

        public override void EnterClassTupleConstructorPattern(ClassTupleConstructorPatternContext context)
        {
            EnterState<TuplePattern>().EnterClassTupleConstructorPattern(context);
        }

        public override void ExitClassTupleConstructorPattern(ClassTupleConstructorPatternContext context)
        {

        }

        public override void EnterStructTupleConstructorPattern(StructTupleConstructorPatternContext context)
        {
            EnterState<TuplePattern>().EnterStructTupleConstructorPattern(context);
        }

        public override void ExitStructTupleConstructorPattern(StructTupleConstructorPatternContext context)
        {

        }

        public sealed class Argument : PatternState
        {
            bool hasPattern;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                hasPattern = false;
            }

            public override void EnterPattern(PatternContext context)
            {
                hasPattern = true;
            }

            public override void ExitPattern(PatternContext context)
            {
                OnExit();
            }

            public override void EnterPatternArgument(PatternArgumentContext context)
            {

            }

            public override void ExitPatternArgument(PatternArgumentContext context)
            {
                if(!hasPattern)
                {
                    Out.Write('_');
                }
                ExitState().ExitPatternArgument(context);
            }
        }

        sealed class NestedPattern : PatternState
        {
            public sealed override void EnterNestedPattern(NestedPatternContext context)
            {
                Out.Write('(');
            }

            public sealed override void ExitNestedPattern(NestedPatternContext context)
            {
                Out.Write(')');
                ExitState().ExitNestedPattern(context);
            }

            public override void EnterPattern(PatternContext context)
            {
                EnterState<PatternState>().EnterPattern(context);
            }

            public override void ExitPattern(PatternContext context)
            {
                OnExit();
            }
        }

        sealed class SomePattern : PatternState
        {
            public sealed override void EnterSomePattern(SomePatternContext context)
            {
                Out.WriteOptionSome(OptionImplementationType);
                Out.Write('(');
            }

            public sealed override void ExitSomePattern(SomePatternContext context)
            {
                Out.Write(')');
                ExitState().ExitSomePattern(context);
            }
        }

        sealed class Arguments : PatternState
        {
            bool namedArg, first, errorFieldMismatch;

            bool? usesFields;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                errorFieldMismatch = false;
                usesFields = null;
            }

            public override void EnterPatternArguments(PatternArgumentsContext context)
            {

            }

            public override void ExitPatternArguments(PatternArgumentsContext context)
            {
                if(usesFields == true)
                {
                    Out.Write(')');
                }
                ExitState().ExitPatternArguments(context);
            }

            public override void EnterPatternArgTuple(PatternArgTupleContext context)
            {
                if(usesFields == true)
                {
                    Error("Curried arguments are not supported together with named fields in a pattern.", context);
                    return;
                }
                namedArg = false;
                first = true;
                Out.Write('(');
            }

            public override void ExitPatternArgTuple(PatternArgTupleContext context)
            {
                if(usesFields != true)
                {
                    Out.Write(')');
                }
            }

            public override void EnterPatternArgument(PatternArgumentContext context)
            {
                if(namedArg)
                {
                    Out.WriteOperator('=');
                }
                else if(first)
                {
                    first = false;
                    usesFields = false;
                }
                else if(usesFields == true)
                {
                    FieldMismatchError(context);
                    Out.Write(';');
                    Out.WriteIdentifier(Environment.ErrorIdentifier);
                    Out.WriteOperator('=');
                }
                else
                {
                    Out.Write(',');
                }
                base.EnterPatternArgument(context);
            }

            public override void ExitPatternArgument(PatternArgumentContext context)
            {
                namedArg = false;
            }

            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                if(usesFields == false)
                {
                    if(!first)
                    {
                        Out.Write(',');
                    }
                    FieldMismatchError(context);
                    EnterState<IgnoredFieldAssignment>().EnterFieldAssignment(context);
                    return;
                }
                if(first)
                {
                    first = false;
                    usesFields = true;
                }
                else
                {
                    Out.Write(';');
                }
                namedArg = true;
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {

            }

            private void FieldMismatchError(ParserRuleContext context)
            {
                if(errorFieldMismatch)
                {
                    return;
                }
                errorFieldMismatch = true;
                Error("Using both named and positional fields in a single pattern discriminator is not supported.", context);
            }

            sealed class IgnoredFieldAssignment : NodeState
            {
                ISourceCapture? capture;

                public override void EnterFieldAssignment(FieldAssignmentContext context)
                {
                    capture = Out.StartCapture();
                }

                public override void ExitFieldAssignment(FieldAssignmentContext context)
                {
                    if(capture != null)
                    {
                        Out.StopCapture(capture);
                    }
                    ExitState().ExitFieldAssignment(context);
                }
            }
        }

        internal sealed class ArrayPattern : PatternState
        {
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
            }

            public override void EnterArrayConstructorPattern(ArrayConstructorPatternContext context)
            {
                Out.EnterNestedScope();
                Out.Write("[|");
            }

            public override void ExitArrayConstructorPattern(ArrayConstructorPatternContext context)
            {
                if(first)
                {
                    Out.Write(' ');
                }
                Out.ExitNestedScope();
                Out.Write("|]");
                ExitState().ExitArrayConstructorPattern(context);
            }

            public override void EnterPatternArgument(PatternArgumentContext context)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.Write(';');
                }
                base.EnterPatternArgument(context);
            }

            public override void EnterPattern(PatternContext context)
            {
                first = false;
                EnterState<PatternState>().EnterPattern(context);
            }

            public override void ExitPattern(PatternContext context)
            {

            }
        }

        sealed class RecordPattern : PatternState
        {
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
            }

            public override void EnterRecordConstructorPattern(RecordConstructorPatternContext context)
            {
                Out.Write("{ ");
            }

            public override void ExitRecordConstructorPattern(RecordConstructorPatternContext context)
            {
                Out.Write(" }");
                ExitState().ExitRecordConstructorPattern(context);
            }
            
            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                if(first)
                {
                    first = false;
                    return;
                }
                Out.Write(';');
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {
                Out.WriteOperator('=');
            }
        }

        sealed class TuplePattern : PatternState
        {
            bool first;
            ImplementationType tupleType;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
                tupleType = default;
            }

            public override void EnterTupleConstructorPattern(TupleConstructorPatternContext context)
            {
                tupleType = TupleImplementationType;
                OnEnter();
            }

            public override void ExitTupleConstructorPattern(TupleConstructorPatternContext context)
            {
                OnExit();
                ExitState().ExitTupleConstructorPattern(context);
            }

            public override void EnterExplicitTupleConstructorPattern(ExplicitTupleConstructorPatternContext context)
            {
                tupleType = TupleImplementationType;
                OnEnter();
            }

            public override void ExitExplicitTupleConstructorPattern(ExplicitTupleConstructorPatternContext context)
            {
                OnExit();
                ExitState().ExitExplicitTupleConstructorPattern(context);
            }

            public override void EnterClassTupleConstructorPattern(ClassTupleConstructorPatternContext context)
            {
                tupleType = ImplementationType.Class;
                OnEnter();
            }

            public override void ExitClassTupleConstructorPattern(ClassTupleConstructorPatternContext context)
            {
                OnExit();
                ExitState().ExitClassTupleConstructorPattern(context);
            }

            public override void EnterStructTupleConstructorPattern(StructTupleConstructorPatternContext context)
            {
                tupleType = ImplementationType.Struct;
                OnEnter();
            }

            public override void ExitStructTupleConstructorPattern(StructTupleConstructorPatternContext context)
            {
                OnExit();
                ExitState().ExitStructTupleConstructorPattern(context);
            }

            private void OnEnter()
            {
                Out.WriteTupleOpen(tupleType);
            }

            private new void OnExit()
            {
                Out.WriteTupleClose(tupleType);
            }

            public override void EnterPatternArgument(PatternArgumentContext context)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.Write(',');
                }
                base.EnterPatternArgument(context);
            }

            public override void ExitPatternArgument(PatternArgumentContext context)
            {

            }
        }
    }
}
