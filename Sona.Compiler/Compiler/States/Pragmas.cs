using System;
using System.Linq;
using Antlr4.Runtime;

namespace Sona.Compiler.States
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

    internal abstract class ClassStructPragma : LexerState
    {
        public ImplementationType? Type { get; private set; }

        public ClassStructPragma(string name) : base(name)
        {
            
        }

        public sealed override bool ReceiveToken(IToken token)
        {
            var type = GetIdentifier(token);
            switch(type)
            {
                case "class":
                    Type = ImplementationType.Class;
                    break;
                case "struct":
                    Type = ImplementationType.Struct;
                    break;
                default:
                    throw new ArgumentException("Only 'class' or 'struct' is a valid pragma argument.");
            }
            return false;
        }
    }

    [LexerStateName("tuple")]
    internal sealed class TuplePragma : ClassStructPragma
    {
        public TuplePragma() : base("tuple")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new TuplePragma();
        }
    }

    [LexerStateName("record")]
    internal sealed class RecordPragma : ClassStructPragma
    {
        public RecordPragma() : base("record")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new RecordPragma();
        }
    }

    [LexerStateName("option")]
    internal sealed class OptionPragma : ClassStructPragma
    {
        public OptionPragma() : base("option")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new OptionPragma();
        }
    }

    [LexerStateName("newline")]
    internal sealed class NewlinePragma : LexerState
    {
        public string NewLineSequence { get; private set; } = ScriptEnvironment.DefaultNewLineSequence;

        public NewlinePragma() : base("newline")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new NewlinePragma();
        }

        public override bool ReceiveToken(IToken token)
        {
            NewLineSequence = String.Join("", GetString(token).Select(c => $"\\u{(ushort)c:X4}"));
            return false;
        }
    }

    internal abstract class BooleanPragma : LexerState
    {
        public bool? Value { get; private set; }

        public BooleanPragma(string name) : base(name)
        {

        }

        public sealed override bool ReceiveToken(IToken token)
        {
            var type = GetIdentifier(token);
            switch(type)
            {
                case "true":
                case "on":
                case "enabled":
                    Value = true;
                    break;
                case "false":
                case "off":
                case "disabled":
                    Value = false;
                    break;
                default:
                    throw new ArgumentException("Only a boolean value is a valid pragma argument.");
            }
            return false;
        }
    }

    [LexerStateName("recursive")]
    internal sealed class RecursivePragma : BooleanPragma
    {
        public RecursivePragma() : base("recursive")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new RecursivePragma();
        }
    }
}
