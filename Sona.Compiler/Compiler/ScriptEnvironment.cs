using Antlr4.Runtime;

namespace IS4.Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, ISourceWriter Output, LexerContext ChannelContext)
    {
        public string Begin => "begin";
        public string End => "end";

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
