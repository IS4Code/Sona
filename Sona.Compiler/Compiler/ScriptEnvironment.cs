using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, ISourceWriter Output, LexerContext LexerContext, string Begin, string End)
    {
        public string ErrorIdentifier => "<error>";

        readonly ConcurrentBag<RecognitionException> errors = new();
        public IReadOnlyCollection<RecognitionException> Errors => errors;

        public static string DefaultNewLineSequence { get; } = String.Join("", Environment.NewLine.Select(c => $"\\u{(ushort)c:X4}"));

        readonly bool buildParseTreeDefault = Parser.BuildParseTree;
        int buildParseTreeLevel = 0;

        public void EnableParseTree()
        {
            if(++buildParseTreeLevel >= 1)
            {
                Parser.BuildParseTree = true;
            }
        }

        public void DisableParseTree()
        {
            if(--buildParseTreeLevel <= 0)
            {
                Parser.BuildParseTree = buildParseTreeDefault;
            }
        }

        public void ReportError(string message, ParserRuleContext context)
        {
            try
            {
                throw new SemanticException(message, context);
            }
            catch(RecognitionException e)
            {
                errors.Add(e);
            }
        }

        public void ReportError(string message, ITerminalNode node)
        {
            try
            {
                throw new SemanticException(message, node);
            }
            catch(RecognitionException e)
            {
                errors.Add(e);
            }
        }

        class SemanticException : RecognitionException
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
}
