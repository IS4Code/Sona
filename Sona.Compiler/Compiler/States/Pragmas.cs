using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Antlr4.Runtime;

namespace Sona.Compiler.States
{
    /// <summary>
    /// Represents a pragma without any arguments.
    /// </summary>
    internal abstract class EmptyPragma : LexerState
    {
        public EmptyPragma(string name) : base(name)
        {

        }

        public sealed override bool OnArgument(IToken token)
        {
            return false;
        }

        public sealed override bool OnEnd(IToken token)
        {
            return true;
        }
    }

    /// <summary>
    /// Represents a pragma with a single mandatory argument.
    /// </summary>
    internal abstract class SingleArgumentPragma : LexerState
    {
        public IToken? Token { get; private set; }

        public SingleArgumentPragma(string name) : base(name)
        {

        }

        // Token is never null when this finishes but it is not supposed to be used on false anyway.
        [MemberNotNullWhen(true, nameof(Token))]
        public override bool OnArgument(IToken token)
        {
            if(Token != null)
            {
                // Already given
                return false;
            }
            Token = token;
            return true;
        }

        [MemberNotNullWhen(true, nameof(Token))]
        public override bool OnEnd(IToken token)
        {
            return Token != null;
        }
    }

    /// <summary>
    /// Represents a pragma whose argument must be a valid identifier.
    /// </summary>
    internal abstract class IdentifierPragma : SingleArgumentPragma
    {
        public string? Identifier { get; private set; }

        public IdentifierPragma(string name) : base(name)
        {

        }

        [MemberNotNullWhen(true, nameof(Identifier))]
        public override bool OnArgument(IToken token)
        {
            if(!base.OnArgument(token))
            {
                return false;
            }
            Identifier = GetIdentifier(Token);
            return true;
        }
    }

    /// <summary>
    /// Represents a pragma taking a single enum argument.
    /// </summary>
    /// <typeparam name="TEnum">The type to provide valid values of the argument.</typeparam>
    internal abstract class EnumPragma<TEnum> : IdentifierPragma where TEnum : struct, Enum
    {
        public TEnum? Value { get; private set; }

        public EnumPragma(string name) : base(name)
        {

        }

        public sealed override bool OnArgument(IToken token)
        {
            if(!base.OnArgument(token))
            {
                return false;
            }
            if(!Enum.TryParse(Identifier, true, out TEnum value))
            {
                throw new ArgumentException($"Pragma argument must be one of {String.Join(", ", Enum.GetNames(typeof(TEnum)).Select(n => $"'{n.ToLowerInvariant()}'"))}.", nameof(token));
            }
            Value = value;
            return true;
        }
    }

    /// <summary>
    /// Represents a pragma with a <c>class</c>/<c>struct</c> argument.
    /// </summary>
    internal abstract class ClassStructPragma : EnumPragma<ImplementationType>
    {
        public ImplementationType? Type => Value;

        public ClassStructPragma(string name) : base(name)
        {
            
        }
    }

    /// <summary>
    /// Represents a pragma whose argument must be a string.
    /// </summary>
    internal abstract class StringPragma : SingleArgumentPragma
    {
        public string? Value { get; private set; }

        public StringPragma(string name) : base(name)
        {

        }

        [MemberNotNullWhen(true, nameof(Value))]
        public override bool OnArgument(IToken token)
        {
            if(!base.OnArgument(token))
            {
                return false;
            }
            Value = GetString(Token);
            return true;
        }
    }

    /// <summary>
    /// Represents a pragma taking a boolean value.
    /// </summary>
    internal abstract class BooleanPragma : EnumPragma<BooleanPragma.BooleanLabels>
    {
        public new bool? Value => base.Value switch
        {
            BooleanLabels.False => false,
            BooleanLabels.True => true,
            _ => null
        };

        public BooleanPragma(string name) : base(name)
        {

        }

        public enum BooleanLabels
        {
            False = 0,
            Off = 0,
            Disabled = 0,

            True = 1,
            On = 1,
            Enabled = 1
        }
    }

    /// <summary>
    /// Represents a pragma capturing its arguments for later retrieval.
    /// </summary>
    internal abstract class TokensPragma : LexerState
    {
        public TokensPragma(string name) : base(name)
        {

        }

        /// <summary>
        /// Reads out all tokens captured by the pragma.
        /// </summary>
        /// <returns>A newly formed collection of <see cref="IToken"/> instances for all the pragma's arguments.</returns>
        public abstract IReadOnlyCollection<IToken> ReadTokens();
    }

    [LexerStateName("echo")]
    internal sealed class EchoPragma : IdentifierPragma
    {
        public EchoPragma() : base("echo")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new EchoPragma();
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
    internal sealed class NewlinePragma : StringPragma
    {
        public string NewLineSequence { get; private set; } = ScriptEnvironment.DefaultNewLineSequence;

        public NewlinePragma() : base("newline")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new NewlinePragma();
        }

        public override bool OnArgument(IToken token)
        {
            if(!base.OnArgument(token))
            {
                return false;
            }
            NewLineSequence = String.Join("", Tools.Syntax.EscapeString(Value));
            return true;
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

    [LexerStateName("collection")]
    internal sealed class CollectionPragma : EnumPragma<CollectionImplementationType>
    {
        public CollectionImplementationType? Type => Value;

        public CollectionPragma() : base("collection")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new CollectionPragma();
        }
    }

    [LexerStateName("forwardref")]
    internal sealed class ForwardRefPragma : EmptyPragma
    {
        public ForwardRefPragma() : base("forwardref")
        {

        }

        public override LexerState ForkNew(IToken token)
        {
            return new ForwardRefPragma();
        }
    }
}
