using System;
using System.Linq;
using Antlr4.Runtime;

namespace IS4.Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, ISourceWriter Output, LexerContext LexerContext, string Begin, string End)
    {
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
    }
}
