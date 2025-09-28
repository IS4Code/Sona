using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class StringBaseState : NodeState
    {
        protected string NewLineSequence { get; private set; } = "";

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            NewLineSequence = LexerContext.GetState<NewlinePragma>()?.NewLineSequence ?? ScriptEnvironment.DefaultNewLineSequence;
        }
    }

    internal class StringState : StringBaseState
    {
        public override void EnterChar(CharContext context)
        {

        }

        public override void ExitChar(CharContext context)
        {
            ExitState().ExitChar(context);
        }

        public override void EnterString(StringContext context)
        {

        }

        public override void ExitString(StringContext context)
        {
            ExitState().ExitString(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            var token = node.Symbol;
            switch(token.Type)
            {
                case SonaLexer.BEGIN_STRING:
                case SonaLexer.END_STRING:
                case SonaLexer.END_STRING_SUFFIX:
                    Out.Write('"');
                    break;
                case SonaLexer.BEGIN_VERBATIM_STRING:
                    Out.Write("@\"");
                    break;
                case SonaLexer.BEGIN_CHAR:
                case SonaLexer.END_CHAR:
                    Out.Write('\'');
                    break;
                case SonaLexer.CHAR_PART:
                case SonaLexer.STRING_PART:
                    Out.Write(token.Text);
                    break;
                case SonaLexer.LITERAL_NEWLINE:
                    Out.Write(NewLineSequence);
                    break;
                case SonaLexer.LITERAL_ESCAPE_NEWLINE:
                    Out.Write(token.Text.Substring(1));
                    break;
            }
        }
    }
}
