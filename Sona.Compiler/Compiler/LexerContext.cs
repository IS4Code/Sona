using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Antlr4.Runtime;
using Sona.Compiler.States;
using Sona.Grammar;

namespace Sona.Compiler
{
    // Progressively updated collection of stacks for each type of lexer context
    using ContextStacks = ImmutableDictionary<string, ImmutableStack<LexerState>>;

    /// <summary>
    /// Maintains a space of entities produced by the lexer which may
    /// be retrieved from the parser, such as documentation comments or pragmas.
    /// </summary>
    internal sealed class LexerContext
    {
        public ScriptEnvironment? Environment { get; internal set; }

        record struct ContextRecord(int TokenIndex, ContextStacks Context);

        readonly Queue<ContextRecord> contexts = new();

        record LexerStateRecord(IToken CreatedAt)
        {
            public IToken? RemovedAt { get; set; }
        }

        readonly Dictionary<LexerState, LexerStateRecord> activeStates = new(Tools.ReferenceEqualityComparer<LexerState>.Instance);

        // As observed by the parser
        ContextStacks parserContext;

        // As observed by the lexer
        ContextStacks lexerContext;

        // Pragma parsing
        ParseState state;
        PragmaOperation pragmaOperation;
        LexerState? currentlyLexedPragma;

        const string docCommentStateName = "#C";

        public LexerContext(SonaLexer lexer)
        {
            parserContext = lexerContext = ContextStacks.Empty;
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
            if(currentlyLexedPragma != null)
            {
                // Inside a #pragma when a name was provided
                if(type == SonaLexer.END_PRAGMA)
                {
                    if(!currentlyLexedPragma.OnEnd(token))
                    {
                        // End signal rejected
                        Error($"Pragma '{currentlyLexedPragma.Name}' is missing arguments.", token);
                    }
                    state = ParseState.None;
                    currentlyLexedPragma = null;
                    return;
                }
                if(!currentlyLexedPragma.OnArgument(token))
                {
                    Error($"Unexpected token '{token.Text}' for pragma '{currentlyLexedPragma.Name}'.", token);
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
                    // Beginning of pragma - name expected
                    if(type != SonaLexer.NAME)
                    {
                        // Not a name
                        break;
                    }
                    var name = token.Text;
                    if(Enum.TryParse(name, true, out PragmaOperation operation) && operation != PragmaOperation.Normal)
                    {
                        if(pragmaOperation != PragmaOperation.Normal)
                        {
                            // Operation already specified
                            break;
                        }
                        pragmaOperation = operation;
                        return;
                    }
                    // Initialize the pragma
                    OnPragma(name, token);
                    return;
            }
            // Fallback
            Error($"Unexpected token '{token.Text}'.", token);
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

        private void Error(string message, IToken token)
        {
            Environment?.ReportError(message, token);
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
            var stateName = NameCache<TState>.Name;
            if(!parserContext.TryGetValue(stateName, out var stack) || stack.IsEmpty || stack.Peek() is not TState state)
            {
                // Nothing in effect
                return null;
            }
            while(state.Lifetime == LexerStateLifetime.Temporary)
            {
                // Pop locally
                stack = stack.Pop();

                // This is only an optimization for the current parser context,
                // the lexer context could be well ahead and contain the old stack,
                // which is why another collection is necessary to track
                // if the state was actually used or not.
                parserContext = parserContext.SetItem(stateName, stack);

                if(activeStates.Remove(state))
                {
                    // Not used before
                    return state;
                }

                if(stack.IsEmpty)
                {
                    return null;
                }

                if(stack.Peek() is not TState nextState)
                {
                    // Nothing in effect
                    return null;
                }
                state = nextState;
            }
            return state;
        }

        enum ParseState
        {
            None,
            Pragma
        }

        enum PragmaOperation
        {
            Normal,
            Push,
            Pop,
            Once
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
                    // End of the pragma is handled normally
                    currentlyLexedPragma = PopPragma.Instance;

                    // Remove from the top
                    if(stack.IsEmpty)
                    {
                        Error($"'#pragma pop {name}' used without a corresponding 'push'.", token);
                        break;
                    }

                    stack = stack.Pop(out var currentState);
                    while(currentState.Lifetime > LexerStateLifetime.Permanent)
                    {
                        // This pragma is parser-controlled, so either it will have been already removed, or the program is invalid
                        MarkShadowedPragma(currentState, token);
                        if(stack.IsEmpty)
                        {
                            break;
                        }
                        stack = stack.Pop(out currentState);
                    }
                    if(currentState.Lifetime == LexerStateLifetime.Permanent)
                    {
                        // Lexer-controlled pragma
                        activeStates.Remove(currentState);
                    }
                    break;
                case PragmaOperation.Push:
                    currentlyLexedPragma = NewPragma(LexerStateLifetime.Permanent);
                    stack = stack.Push(currentlyLexedPragma);
                    break;
                case PragmaOperation.Once:
                    currentlyLexedPragma = NewPragma(LexerStateLifetime.Temporary);
                    stack = stack.Push(currentlyLexedPragma);
                    break;
                default:
                    currentlyLexedPragma = NewPragma(LexerStateLifetime.Permanent);
                    if(!stack.IsEmpty)
                    {
                        // Replace the top
                        stack = stack.Pop();
                    }
                    stack = stack.Push(currentlyLexedPragma);
                    break;
            }

            // Update the context at this token
            AddContext(token, name, stack);

            LexerState NewPragma(LexerStateLifetime lifetime)
            {
                if(lifetime != LexerStateLifetime.Permanent)
                {
                    // Remove any pragmas shadowed by the new one
                    while(!stack.IsEmpty)
                    {
                        // A copy of the stack
                        var poppedStack = stack.Pop(out var topState);
                        if(topState.Lifetime < lifetime)
                        {
                            break;
                        }
                        // Assume any parser-controlled pragma with the same or shorter lifetime will have been removed
                        stack = poppedStack;
                        MarkShadowedPragma(topState, token);
                    }
                }

                LexerState newPragma;
                if(stack.IsEmpty)
                {
                    // Create a brand new one
                    newPragma = CreatePragma(name, token);
                }
                else
                {
                    // Duplicate the top one
                    var topState = stack.Peek();
                    newPragma = topState.ForkNew(token);
                }
                newPragma.Lifetime = lifetime;
                activeStates[newPragma] = new(token);
                return newPragma;
            }
        }

        private void MarkShadowedPragma(LexerState state, IToken token)
        {
            if(activeStates.TryGetValue(state, out var pragmaRecord))
            {
                pragmaRecord.RemovedAt ??= token;
            }
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
                case "recursive":
                    return new RecursivePragma();
                case "collection":
                    return new CollectionPragma();
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

            public override bool OnArgument(IToken token)
            {
                return false;
            }

            public override bool OnEnd(IToken token)
            {
                return false;
            }

            public override IReadOnlyCollection<IToken> ReadTokens()
            {
                var result = value.ToImmutable();
                value.Clear();
                return result;
            }
        }

        /// <summary>
        /// A singleton pragma for terminating <code>#pragma pop</code>.
        /// </summary>
        sealed class PopPragma : EmptyPragma
        {
            public static readonly PopPragma Instance = new();

            private PopPragma() : base("pop")
            {

            }

            public override LexerState ForkNew(IToken token)
            {
                return Instance;
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
