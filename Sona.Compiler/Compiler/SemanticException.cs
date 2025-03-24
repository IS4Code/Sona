using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Sona.Compiler
{
    internal class SemanticException : RecognitionException
    {
        public SemanticException(string message, ParserRuleContext ctx) : base(message, null, null, ctx)
        {
            OffendingState = ctx.invokingState;
            OffendingToken = ctx.Start;
        }

        public SemanticException(string message, ITerminalNode node) : this(message, GetNodeContext(node))
        {

        }

        static ParserRuleContext GetNodeContext(ITerminalNode node)
        {
            if(node is ParserRuleContext context)
            {
                return context;
            }

            ParserRuleContext newContext = node.Parent is ParserRuleContext parent
                ? new(parent, parent.invokingState)
                : new();

            newContext.Start = newContext.Stop = node.Symbol;
            return newContext;
        }
    }
}
