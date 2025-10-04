using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Compiler.States;
using Sona.Grammar;

namespace Sona.Compiler
{
    internal abstract partial class ScriptState : ParserListener
    {
        public ScriptEnvironment Environment { get; private set; } = null!;
        public ScriptState? Parent { get; private set; }

        protected ISourceWriter Out => Environment.Output;
        protected ISourceWriter GlobalOut => Environment.GlobalOutput;
        protected LexerContext LexerContext => Environment.LexerContext;

        protected ImplementationType OptionImplementationType => LexerContext.GetState<OptionPragma>()?.Type ?? ImplementationType.Struct;
        protected ImplementationType TupleImplementationType => LexerContext.GetState<TuplePragma>()?.Type ?? ImplementationType.Struct;
        protected ImplementationType RecordImplementationType => LexerContext.GetState<RecordPragma>()?.Type ?? ImplementationType.Class;
        protected CollectionImplementationType CollectionImplementationType => LexerContext.GetState<CollectionPragma>()?.Type ?? CollectionImplementationType.Array;

        public int StateLevel { get; private set; }

        protected virtual bool IgnoreContext => false;

        private protected string _begin_ => Environment.Begin;
        private protected string _end_ => Environment.End;

        protected virtual void OnEnterToken(IToken token)
        {
            Environment.Output.UpdateLine(token);
            Environment.LexerContext.OnParserToken(token);
        }

        public sealed override void EnterEveryRule(ParserRuleContext context)
        {
            if(context.Start is { } start)
            {
                OnEnterToken(start);
            }
            base.EnterEveryRule(context);
            EnterLevel();
        }

        public sealed override void ExitEveryRule(ParserRuleContext context)
        {
            ExitLevel();
            base.ExitEveryRule(context);
        }

        protected TState EnterState<TState>() where TState : ScriptState, new()
        {
            var state = Tools.StatePool.Get<TState>();
            state.Initialize(Environment, this);

            var list = (List<IParseTreeListener>)Environment.Parser.ParseListeners;
            CollectionsMarshal.AsSpan(list)[0] = state;
            return state;
        }

        protected ScriptState? ExitState()
        {
            if(StateLevel != 0)
            {
                throw new InvalidOperationException("Attempting to exit parser state without the full node being processed.");
            }

            var list = (List<IParseTreeListener>)Environment.Parser.ParseListeners;
            CollectionsMarshal.AsSpan(list)[0] = Parent ?? Empty.Instance;
            Parent?.ExitLevel();

            Tools.StatePool.Return(this);

            return Parent;
        }

        private void EnterLevel()
        {
            StateLevel++;
        }

        private void ExitLevel()
        {
            if(StateLevel == -1)
            {
                throw new InvalidOperationException("Parser state level underflow.");
            }

            StateLevel--;
        }

        protected virtual void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            Environment = environment;
            Parent = parent;
            StateLevel = 0;
        }

        protected internal T? FindContext<T>() where T : class
        {
            switch(Parent)
            {
                case null:
                    return null;
                case { IgnoreContext: true }:
                    goto default;
                case T result:
                    return result;
                default:
                    return Parent.FindContext<T>();
            }
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            var token = node.Symbol;
            OnEnterToken(token);

            switch(token.Type)
            {
                case SonaLexer.NAME:
                    ValidateName(token.Text.AsSpan(), node);
                    break;
                case SonaLexer.LITERAL_NAME:
                case SonaLexer.MEMBER_NAME:
                case SonaLexer.DYNAMIC_MEMBER_NAME:
                    ValidateName(token.Text.AsSpan(1), node);
                    break;
            }
        }

        private void ValidateName(ReadOnlySpan<char> span, ITerminalNode node)
        {
            if(!IsValidName(span, out var c))
            {
                if(c == span[0] || Char.IsWhiteSpace(c))
                {
                    Error($"Character '{c}' (\\u{(ushort)c:X4}) is not recognized as valid syntax.", node);
                }
                else
                {
                    Error($"Character '{c}' (\\u{(ushort)c:X4}) is not accepted as a part of an identifier.", node);
                }
            }
        }

        private bool IsValidName(ReadOnlySpan<char> span, out char errorChar)
        {
            // Quick scan for non-ASCII characters
            var nonAsciiCharacter = span.IndexOfAnyInRange('\u0080', '\uFFFF');
            if(nonAsciiCharacter == -1)
            {
                // Already validated by lexer
                errorChar = default;
                return true;
            }
            char c;
            if(nonAsciiCharacter == 0)
            {
                c = span[0];
                if(Char.GetUnicodeCategory(c) is not (
                    UnicodeCategory.UppercaseLetter or
                    UnicodeCategory.LowercaseLetter or
                    UnicodeCategory.TitlecaseLetter or
                    UnicodeCategory.ModifierLetter or
                    UnicodeCategory.OtherLetter or
                    UnicodeCategory.LetterNumber
                ))
                {
                    errorChar = c;
                    return false;
                }
                span = span.Slice(1);
                nonAsciiCharacter = span.IndexOfAnyInRange('\u0080', '\uFFFF');
            }
            while(nonAsciiCharacter != -1)
            {
                c = span[nonAsciiCharacter];
                if(Char.GetUnicodeCategory(c) is not (
                    UnicodeCategory.UppercaseLetter or
                    UnicodeCategory.LowercaseLetter or
                    UnicodeCategory.TitlecaseLetter or
                    UnicodeCategory.ModifierLetter or
                    UnicodeCategory.OtherLetter or
                    UnicodeCategory.LetterNumber or
                    UnicodeCategory.ConnectorPunctuation or
                    UnicodeCategory.NonSpacingMark or
                    UnicodeCategory.SpacingCombiningMark or
                    UnicodeCategory.Format
                ))
                {
                    errorChar = c;
                    return false;
                }
                span = span.Slice(nonAsciiCharacter + 1);
                nonAsciiCharacter = span.IndexOfAnyInRange('\u0080', '\uFFFF');
            }
            errorChar = default;
            return true;
        }

        protected string Error(string message, ParserRuleContext context)
        {
            Environment.ReportError(message, context);
            return Environment.ErrorIdentifier;
        }

        protected ISourceCapture ErrorCapture(string message, ParserRuleContext context)
        {
            Environment.ReportError(message, context);
            return ErrorIdentifierCapture;
        }

        protected string Error(string message, ITerminalNode node)
        {
            Environment.ReportError(message, node);
            return Environment.ErrorIdentifier;
        }

        protected ISourceCapture ErrorCapture(string message, ITerminalNode node)
        {
            Environment.ReportError(message, node);
            return ErrorIdentifierCapture;
        }

        protected string Error(string message, IToken token)
        {
            Environment.ReportError(message, token);
            return Environment.ErrorIdentifier;
        }

        protected ISourceCapture ErrorCapture(string message, IToken token)
        {
            Environment.ReportError(message, token);
            return ErrorIdentifierCapture;
        }

        ISourceCapture ErrorIdentifierCapture {
            get {
                var capture = Out.StartCapture();
                Out.WriteIdentifier(Environment.ErrorIdentifier);
                Out.StopCapture(capture);
                return capture;
            }
        }

        class Empty : ScriptState
        {
            public static readonly Empty Instance = new();
        }
    }

    internal abstract class NodeState : ScriptState
    {
        public new ScriptState Parent => base.Parent!;

        protected new ScriptState ExitState()
        {
            return base.ExitState()!;
        }
    }
}
