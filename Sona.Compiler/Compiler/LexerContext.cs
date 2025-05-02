using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Antlr4.Runtime;
using Sona.Compiler.States;
using Sona.Grammar;

namespace Sona.Compiler
{
    /// <summary>
    /// Maintains a space of entities produced by the lexer which may
    /// be retrieved from the parser, such as documentation comments or pragmas.
    /// </summary>
    internal sealed class LexerContext
    {
        record struct ContextRecord(int TokenIndex, ImmutableDictionary<string, ImmutableStack<LexerState>> Context);

        readonly Queue<ContextRecord> contexts = new();

        ImmutableDictionary<string, ImmutableStack<LexerState>> parserContext;
        ImmutableDictionary<string, ImmutableStack<LexerState>> lexerContext;

        // Pragma parsing
        ParseState state;
        PragmaOperation pragmaOperation;
        LexerState? currentPragma;

        const string docCommentStateName = "#C";

        public LexerContext(SonaLexer lexer)
        {
            parserContext = lexerContext = ImmutableDictionary<string, ImmutableStack<LexerState>>.Empty;
        }

        /// <summary>
        /// Called when the lexer reads a new token in a special channel.
        /// </summary>
        /// <param name="token">The read token.</param>
        public void OnLexerToken(IToken token)
        {
            int type = token.Type;
            if(type == SonaLexer.DOC_COMMENT)
            {
                // Comments are recognized everywhere
                AddComment(token);
                return;
            }
            if((state == ParseState.PragmaEnd || pragmaOperation == PragmaOperation.Pop) && type == SonaLexer.END_PRAGMA)
            {
                // End of pragma requested
                state = ParseState.None;
                return;
            }
            if(currentPragma != null)
            {
                // Inside a pragma
                if(!currentPragma.ReceiveToken(token))
                {
                    // Last token for this pragma
                    state = ParseState.PragmaEnd;
                    currentPragma = null;
                }
                if(type == SonaLexer.END_PRAGMA)
                {
                    state = ParseState.None;
                    currentPragma = null;
                }
                return;
            }
            switch(state)
            {
                case ParseState.None:
                    // Pragma directive
                    if(type != SonaLexer.BEGIN_PRAGMA)
                    {
                        break;
                    }
                    state = ParseState.Pragma;
                    pragmaOperation = PragmaOperation.Normal;
                    return;
                case ParseState.Pragma:
                    // Beginning of pragma - name needed
                    if(type != SonaLexer.NAME)
                    {
                        break;
                    }
                    var name = token.Text;
                    switch(name)
                    {
                        case "push":
                            if(pragmaOperation != PragmaOperation.Normal)
                            {
                                break;
                            }
                            pragmaOperation = PragmaOperation.Push;
                            return;
                        case "pop":
                            if(pragmaOperation != PragmaOperation.Normal)
                            {
                                break;
                            }
                            pragmaOperation = PragmaOperation.Pop;
                            return;
                        default:
                            OnPragma(name, token);
                            return;
                    }
                    break;
            }
            // Fallback
            throw new Exception($"Unexpected token '{token}' in a channel.");
        }

        /// <summary>
        /// Called when the parser finishes processing a token.
        /// </summary>
        /// <param name="token">The processed token.</param>
        public void OnParserToken(IToken token)
        {
            int index = token.TokenIndex;
            while(contexts.TryPeek(out var first))
            {
                // Check first element
                if(first.TokenIndex > index)
                {
                    // Future token
                    return;
                }
                // Set the context for this token
                parserContext = contexts.Dequeue().Context;
            }
        }

        /// <summary>
        /// Retrieves a lexer state in the context of the parser's current position.
        /// </summary>
        /// <typeparam name="TState">The type of the lexer state.</typeparam>
        /// <returns>
        /// The particular <typeparamref name="TState"/> instance that is applicable
        /// in the current context, or <see langword="null"/>.
        /// </returns>
        public TState? GetState<TState>() where TState : LexerState
        {
            if(!parserContext.TryGetValue(NameCache<TState>.Name, out var stack) || stack.IsEmpty)
            {
                return null;
            }
            return stack.Peek() as TState;
        }

        enum ParseState
        {
            None,
            Pragma,
            PragmaEnd
        }

        enum PragmaOperation
        {
            Normal,
            Push,
            Pop
        }

        private void OnPragma(string name, IToken token)
        {
            // A pragma name is parsed
            if(!lexerContext.TryGetValue(name, out var stack))
            {
                // Start with an empty stack
                stack = ImmutableStack<LexerState>.Empty;
            }
            switch(pragmaOperation)
            {
                case PragmaOperation.Pop:
                    if(stack.IsEmpty)
                    {
                        throw new Exception($"'#pragma pop {name}' used without a corresponding 'push'.");
                    }
                    stack = stack.Pop();
                    currentPragma = null;
                    break;
                case PragmaOperation.Push:
                    if(stack.IsEmpty)
                    {
                        currentPragma = CreatePragma(name, token);
                        stack = stack.Push(currentPragma);
                    }
                    else
                    {
                        currentPragma = stack.Peek().ForkNew(token);
                        stack = stack.Push(currentPragma);
                    }
                    break;
                default:
                    if(stack.IsEmpty)
                    {
                        currentPragma = CreatePragma(name, token);
                        stack = stack.Push(currentPragma);
                    }
                    else
                    {
                        stack = stack.Pop(out currentPragma);
                        currentPragma = currentPragma.ForkNew(token);
                        stack = stack.Push(currentPragma);
                    }
                    break;
            }
            AddContext(token, name, stack);
        }

        private LexerState CreatePragma(string name, IToken token)
        {
            switch(name)
            {
                case "echo":
                    return new EchoPragma();
                case "record":
                    return new RecordPragma();
                case "tuple":
                    return new TuplePragma();
                case "option":
                    return new OptionPragma();
                case "newline":
                    return new NewlinePragma();
            }
            throw new Exception($"'{name}' is not recognized as a valid pragma name.");
        }

        private void AddComment(IToken token)
        {
            if(!lexerContext.TryGetValue(docCommentStateName, out var stack))
            {
                // Start with an empty stack
                stack = ImmutableStack<LexerState>.Empty;
            }
            if(stack.IsEmpty)
            {
                stack = stack.Push(new DocComment(token));
            }
            else
            {
                var comment = stack.Peek();
                var newComment = comment.ForkNew(token);
                if(newComment == comment)
                {
                    // Optimization for consecutive comments
                    return;
                }
                // This stack should always have at most one item
                stack = ImmutableStack.Create(newComment);
            }
            AddContext(token, docCommentStateName, stack);
        }

        private void AddContext(IToken token, string name, ImmutableStack<LexerState> stack)
        {
            var index = token.TokenIndex;
            lexerContext = lexerContext.SetItem(name, stack);
            contexts.Enqueue(new(index, lexerContext));
        }

        sealed class DocComment : DocumentationCommentState
        {
            int index;

            readonly ImmutableList<IToken>.Builder value;

            public DocComment(IToken token) : base(docCommentStateName)
            {
                index = token.TokenIndex;

                value = ImmutableList.CreateBuilder<IToken>();
                value.Add(token);
            }

            public override LexerState ForkNew(IToken token)
            {
                int newIndex = token.TokenIndex;
                if(newIndex == index + 1)
                {
                    // Consecutive comment
                    index = newIndex;
                    value.Add(token);
                    return this;
                }
                // There is a gap, so this is likely a block of comments for a new element
                return new DocComment(token);
            }

            public override bool ReceiveToken(IToken token)
            {
                throw new NotSupportedException();
            }

            public override IReadOnlyCollection<IToken> ReadTokens()
            {
                var result = value.ToImmutable();
                value.Clear();
                return result;
            }
        }

        static class NameCache<TState> where TState : LexerState
        {
            public static readonly string Name = GetName();

            static string GetName()
            {
                var type = typeof(TState);

                if(type.IsAssignableFrom(typeof(DocComment)))
                {
                    // Documentation comment implementation
                    return docCommentStateName;
                }
                var attr = type.GetCustomAttribute<LexerStateNameAttribute>();
                if(attr == null)
                {
                    throw new InvalidOperationException($"The lexer state {type} must have an {nameof(LexerStateNameAttribute)} attribute with a name to be retrieved.");
                }
                return attr.Name;
            }
        }
    }
}
