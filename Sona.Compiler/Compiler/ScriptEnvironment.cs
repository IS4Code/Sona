using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, ISourceWriter Output, ISourceWriter GlobalOutput, LexerContext LexerContext, string Begin, string End, string Return)
    {
        public string ErrorIdentifier => "<error>";
        public string GlobalModuleIdentifier => "<" + nameof(Sona) + ">";

        readonly ConcurrentBag<RecognitionException> errors = new();
        public IReadOnlyCollection<RecognitionException> Errors => errors;

        ConcurrentDictionary<string, Microsoft.FSharp.Core.Unit?>? globalSymbols;
        public ICollection<string> DefinedGlobalSymbols => globalSymbols?.Keys ?? Array.Empty<string>();

        public static string DefaultNewLineSequence { get; } = String.Join("", Tools.Syntax.EscapeString(Environment.NewLine));

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

        public bool DefineGlobalSymbol(string name)
        {
            if(Interlocked.CompareExchange(ref globalSymbols, new(), null) == null)
            {
                // Initialize the module
                GlobalOutput.Write("module internal ");
                GlobalOutput.WriteIdentifier(GlobalModuleIdentifier);
                GlobalOutput.WriteOperator('=');
                GlobalOutput.WriteLine();
                GlobalOutput.EnterScope();
            }
            return globalSymbols.TryAdd(name, null);
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

        public void ReportError(string message, IToken token)
        {
            try
            {
                throw new SemanticException(message, token);
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

            public SemanticException(string message, IToken token) : this(message, GetTokenContext(token))
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

            static ParserRuleContext GetTokenContext(IToken token)
            {
                var newContext = new ParserRuleContext();
                newContext.Start = newContext.Stop = token;
                return newContext;
            }
        }
    }
}
