using System;
using Antlr4.Runtime;

namespace IS4.Sona.Compiler
{
    internal abstract class LexerState
    {
        protected string Name { get; }

        public LexerState(string name)
        {
            Name = name;
        }

        public abstract bool ReceiveToken(IToken token);

        public abstract LexerState ForkNew(IToken token);
    }

    internal sealed class LexerStateNameAttribute : Attribute
    {
        public string Name { get; }

        public LexerStateNameAttribute(string name)
        {
            Name = name;
        }
    }
}
