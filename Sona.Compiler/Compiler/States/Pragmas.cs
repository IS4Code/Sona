using Antlr4.Runtime;

namespace IS4.Sona.Compiler.States
{
    [LexerStateName("echo")]
    internal sealed class EchoPragma : LexerState
    {
        public string? Identifier { get; private set; }

        public EchoPragma() : base("echo")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new EchoPragma();
        }

        public override bool ReceiveToken(IToken token)
        {
            Identifier = GetIdentifier(token);
            return false;
        }
    }
}
