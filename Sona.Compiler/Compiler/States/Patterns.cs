using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class PatternState : NodeState, IExpressionContext
    {
        int parenthesisLevel;
        bool patternExpected;

        ExpressionType IExpressionContext.Type => ExpressionType.Pattern;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            parenthesisLevel = 0;
            patternExpected = false;
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
            if(patternExpected)
            {
                Out.Write('_');
                patternExpected = false;
            }
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
                    if(patternExpected)
                    {
                        patternExpected = false;
                    }
                    else
                    {
                        Out.Write(" as(");
                        parenthesisLevel++;
                    }
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
            EnterState<InlineSourcePattern>().EnterInlineSourceFree(context);
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

        public override void EnterRelationalPattern(RelationalPatternContext context)
        {
            EnterState<RelationalPattern>().EnterRelationalPattern(context);
        }

        public override void ExitRelationalPattern(RelationalPatternContext context)
        {

        }

        public override void EnterRegexPattern(RegexPatternContext context)
        {
            EnterState<RegexPatternState>().EnterRegexPattern(context);
        }

        public override void ExitRegexPattern(RegexPatternContext context)
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

        public sealed override void EnterNotNullPattern(NotNullPatternContext context)
        {
            Out.WriteCustomPattern("NonNull");
            parenthesisLevel++;
            Out.Write('(');
            patternExpected = true;
        }

        public sealed override void ExitNotNullPattern(NotNullPatternContext context)
        {

        }

        public sealed override void EnterAtomicNotNullPattern(AtomicNotNullPatternContext context)
        {
            Out.WriteCustomPattern("NonNull");
            parenthesisLevel++;
            Out.Write('(');
            patternExpected = true;
        }

        public sealed override void ExitAtomicNotNullPattern(AtomicNotNullPatternContext context)
        {

        }

        public sealed override void EnterNullPattern(NullPatternContext context)
        {
            Out.WriteCustomPattern("Null");
        }

        public sealed override void ExitNullPattern(NullPatternContext context)
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

        public override void EnterMemberTestPattern(MemberTestPatternContext context)
        {
            EnterState<MembersPattern>().EnterMemberTestPattern(context);
        }

        public override void ExitMemberTestPattern(MemberTestPatternContext context)
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

        abstract class OperatorPattern : PatternState
        {
            public sealed override void EnterUnaryPattern(UnaryPatternContext context)
            {
                EnterState<Argument>().EnterUnaryPattern(context);
            }

            public sealed override void ExitUnaryPattern(UnaryPatternContext context)
            {

            }

            new sealed class Argument : PatternState
            {
                protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
                {
                    base.Initialize(environment, parent);
                }

                public override void EnterUnaryPattern(UnaryPatternContext context)
                {

                }

                public override void ExitUnaryPattern(UnaryPatternContext context)
                {
                    OnExit();
                    ExitState().ExitUnaryPattern(context);
                }
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

        sealed class SomePattern : OperatorPattern
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

            private void OnEnterArgTuple(ParserRuleContext context)
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

            private void OnExitArgTuple(ParserRuleContext context)
            {
                if(usesFields != true)
                {
                    Out.Write(')');
                }
            }

            public override void EnterPatternArgTuple(PatternArgTupleContext context)
            {
                OnEnterArgTuple(context);
            }

            public override void ExitPatternArgTuple(PatternArgTupleContext context)
            {
                OnExitArgTuple(context);
            }

            public override void EnterEmptyPatternArgTuple(EmptyPatternArgTupleContext context)
            {
                OnEnterArgTuple(context);
                if(usesFields == true)
                {
                    Out.Write(';');
                    Out.WriteIdentifier(Environment.ErrorIdentifier);
                    Out.WriteOperator('=');
                    Out.Write('_');
                    return;
                }
                usesFields = false;
                Out.WriteCustomPattern("Empty");
                Out.Write("()");
            }

            public override void ExitEmptyPatternArgTuple(EmptyPatternArgTupleContext context)
            {
                OnExitArgTuple(context);
            }

            private void OnEnterArgument(ParserRuleContext context)
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
            }

            private void OnExitArgument(ParserRuleContext context)
            {
                namedArg = false;
            }

            public override void EnterPatternArgument(PatternArgumentContext context)
            {
                OnEnterArgument(context);
                EnterState<Argument>().EnterPatternArgument(context);
            }

            public override void ExitPatternArgument(PatternArgumentContext context)
            {
                OnExitArgument(context);
            }

            public override void EnterPattern(PatternContext context)
            {
                OnEnterArgument(context);
                EnterState<PatternState>().EnterPattern(context);
            }

            public override void ExitPattern(PatternContext context)
            {
                OnExitArgument(context);
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
            CollectionImplementationType collectionType;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
                collectionType = default;
            }

            public override void EnterArrayConstructorPattern(ArrayConstructorPatternContext context)
            {
                collectionType = CollectionImplementationType;

                Out.EnterNestedScope();
                Out.WriteCollectionOpen(collectionType);
            }

            public override void ExitArrayConstructorPattern(ArrayConstructorPatternContext context)
            {
                if(first)
                {
                    Out.Write(' ');
                }
                Out.ExitNestedScope();
                Out.WriteCollectionClose(collectionType);
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

            private void OnEnterField()
            {
                if(first)
                {
                    first = false;
                    return;
                }
                Out.Write(';');
            }
            
            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                OnEnterField();
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {
                Out.WriteOperator('=');
            }

            public override void EnterEmptyFieldAssignment(EmptyFieldAssignmentContext context)
            {
                OnEnterField();
                Environment.EnableParseTree();
            }

            public override void ExitEmptyFieldAssignment(EmptyFieldAssignmentContext context)
            {
                string name;
                try
                {
                    name = Tools.Syntax.GetIdentifierFromName(context.GetText());
                }
                finally
                {
                    Environment.DisableParseTree();
                }
                Out.WriteOperator('=');
                Out.WriteIdentifier(name);
            }

            public override void EnterFieldRelation(FieldRelationContext context)
            {
                OnEnterField();
            }

            public override void ExitFieldRelation(FieldRelationContext context)
            {

            }

            public override void EnterPattern(PatternContext context)
            {
                EnterState<PatternState>().EnterPattern(context);
            }

            public override void ExitPattern(PatternContext context)
            {

            }

            public override void EnterRelationalPattern(RelationalPatternContext context)
            {
                Out.WriteOperator('=');
                base.EnterRelationalPattern(context);
            }

            public override void ExitRelationalPattern(RelationalPatternContext context)
            {
                base.ExitRelationalPattern(context);
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

        sealed class MembersPattern : PatternState
        {
            bool first;

            protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
            {
                base.Initialize(environment, parent);

                first = true;
            }

            public override void EnterMemberTestPattern(MemberTestPatternContext context)
            {
                Out.Write('(');
            }

            public override void ExitMemberTestPattern(MemberTestPatternContext context)
            {
                Out.Write(')');
                ExitState().ExitMemberTestPattern(context);
            }

            public override void EnterRecordConstructorPattern(RecordConstructorPatternContext context)
            {

            }

            public override void ExitRecordConstructorPattern(RecordConstructorPatternContext context)
            {

            }

            private void OnEnterField()
            {
                if(first)
                {
                    first = false;
                    return;
                }
                Out.WriteOperator('&');
            }

            public override void EnterFieldAssignment(FieldAssignmentContext context)
            {
                OnEnterField();
            }

            public override void ExitFieldAssignment(FieldAssignmentContext context)
            {

            }

            public override void EnterEmptyFieldAssignment(EmptyFieldAssignmentContext context)
            {
                OnEnterField();
                Environment.EnableParseTree();
            }

            public override void ExitEmptyFieldAssignment(EmptyFieldAssignmentContext context)
            {
                string name;
                try
                {
                    name = Tools.Syntax.GetIdentifierFromName(context.GetText());
                }
                finally
                {
                    Environment.DisableParseTree();
                }
                Out.Write('(');
                Out.WriteIdentifier(name);
                Out.Write(')');
            }

            public override void EnterFieldRelation(FieldRelationContext context)
            {
                OnEnterField();
            }

            public override void ExitFieldRelation(FieldRelationContext context)
            {
                
            }

            public override void EnterName(NameContext context)
            {
                Environment.EnableParseTree();
            }

            public override void ExitName(NameContext context)
            {
                string identifier;
                try
                {
                    identifier = Tools.Syntax.GetIdentifierFromName(context.GetText());
                }
                finally
                {
                    Environment.DisableParseTree();
                }

                var patternIdentifier = "Get " + identifier;

                if(Environment.DefineGlobalSymbol($"|{patternIdentifier}|"))
                {
                    GlobalOut.Write("let inline (|");
                    GlobalOut.WriteIdentifier(patternIdentifier);
                    GlobalOut.Write("|) (x : ^T) = (^T : (member ");
                    GlobalOut.WriteIdentifier(identifier);
                    GlobalOut.WriteLine(" : _) x)");
                }

                Out.WriteIdentifier(Environment.GlobalModuleIdentifier);
                Out.Write('.');
                Out.WriteIdentifier(patternIdentifier);
            }

            public override void EnterPattern(PatternContext context)
            {
                Out.Write('(');
                EnterState<PatternState>().EnterPattern(context);
            }

            public override void ExitPattern(PatternContext context)
            {
                Out.Write(')');
            }

            public override void EnterRelationalPattern(RelationalPatternContext context)
            {
                Out.Write('(');
                base.EnterRelationalPattern(context);
            }

            public override void ExitRelationalPattern(RelationalPatternContext context)
            {
                base.ExitRelationalPattern(context);
                Out.Write(')');
            }
        }

        sealed class RelationalPattern : OperatorPattern
        {
            public sealed override void EnterRelationalPattern(RelationalPatternContext context)
            {

            }

            public sealed override void ExitRelationalPattern(RelationalPatternContext context)
            {
                Out.Write(')');
                ExitState().ExitRelationalPattern(context);
            }

            public override void VisitTerminal(ITerminalNode node)
            {
                base.VisitTerminal(node);

                switch(node.Symbol.Type)
                {
                    case SonaLexer.EQ:
                        Out.WriteCustomPattern("Equality");
                        break;
                    case SonaLexer.NEQ:
                        Out.WriteCustomPattern("Inequality");
                        break;
                    case SonaLexer.GT:
                        Out.WriteCustomPattern("GreaterThan");
                        break;
                    case SonaLexer.LT:
                        Out.WriteCustomPattern("LessThan");
                        break;
                    case SonaLexer.GTE:
                        Out.WriteCustomPattern("GreaterThanOrEqual");
                        break;
                    case SonaLexer.LTE:
                        Out.WriteCustomPattern("LessThanOrEqual");
                        break;
                    default:
                        return;
                }
                Out.Write('(');
            }
        }
    }
}
