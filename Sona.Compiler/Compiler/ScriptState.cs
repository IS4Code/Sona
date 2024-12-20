using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using IS4.Sona.Grammar;

namespace IS4.Sona.Compiler
{
    internal abstract partial class ScriptState : SonaParserBaseListener
    {
        public ScriptEnvironment Environment { get; private set; } = null!;
        public ScriptState? Parent { get; private set; }

        protected SourceWriter Out => Environment.Output;

        public int StateLevel { get; private set; }

        private protected string _begin_ => Environment.Begin;
        private protected string _end_ => Environment.End;

        protected virtual void UpdateOnToken(IToken token)
        {
            Environment.Output.UpdateLine(token);
        }

        public sealed override void EnterEveryRule(ParserRuleContext context)
        {
            if(context.Start is { } start)
            {
                UpdateOnToken(start);
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
            var state = new TState();
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

        protected T? FindScope<T>() where T : class
        {
            switch(Parent)
            {
                case null:
                    return null;
                case T result:
                    return result;
                default:
                    return Parent.FindScope<T>();
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
