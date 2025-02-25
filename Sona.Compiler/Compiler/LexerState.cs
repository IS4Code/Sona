﻿using System;
using Antlr4.Runtime;
using IS4.Sona.Grammar;

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

        protected string GetIdentifier(IToken token)
        {
            switch(token.Type)
            {
                case SonaLexer.NAME:
                    return token.Text;
                case SonaLexer.LITERAL_NAME:
                    return token.Text.Substring(1);
                default:
                    throw new ArgumentException($"Token {token} is not an identifier.", nameof(token));
            }
        }
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
