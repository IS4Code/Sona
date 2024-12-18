using Antlr4.Runtime;

namespace IS4.Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, SourceWriter Output)
    {
        public string Begin => "begin";
        public string End => "end";

        public void EnableParseTree()
        {
            Parser.BuildParseTree = true;
        }

        public void DisableParseTree()
        {
            Parser.BuildParseTree = false;
        }
    }
}
