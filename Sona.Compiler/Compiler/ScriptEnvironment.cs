using Antlr4.Runtime;

namespace IS4.Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, ISourceWriter Output, LexerContext ChannelContext)
    {
        public string Begin => "begin";
        public string End => "end";

        readonly bool buildParseTreeDefault = Parser.BuildParseTree;

        public void EnableParseTree()
        {
            Parser.BuildParseTree = true;
        }

        public void DisableParseTree()
        {
            Parser.BuildParseTree = buildParseTreeDefault;
        }
    }
}
