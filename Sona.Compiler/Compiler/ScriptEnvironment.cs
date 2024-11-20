using Antlr4.Runtime;

namespace IS4.Sona.Compiler
{
    internal record class ScriptEnvironment(Parser Parser, SourceWriter Output)
    {
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
