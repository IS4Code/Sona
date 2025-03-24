using System.Collections.Generic;
using Antlr4.Runtime;

namespace Sona.Compiler.States
{
    internal abstract class DocumentationCommentState : LexerState
    {
        public DocumentationCommentState(string name) : base(name)
        {

        }

        public abstract IReadOnlyCollection<IToken> ReadTokens();
    }
}
