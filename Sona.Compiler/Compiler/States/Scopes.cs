using System;

namespace IS4.Sona.Compiler.States
{
    internal interface IReturnScope
    {
        string? ReturnVariable { get; }
        string? ReturningVariable { get; }
    }

    internal interface IFunctionScope
    {
        void WriteBegin();
        void WriteEnd();
    }

    internal interface IExecutionScope
    {
        bool IsLiteral { get; }
        bool IsInline { get; }
    }

    internal interface IBindingScope
    {
        void Add(string name);
        bool Contains(string name);
    }

    [Flags]
    internal enum InterruptFlags
    {
        None,
        CanBreak = 1,
        CanContinue = 2
    }

    internal interface IInterruptibleScope
    {
        InterruptFlags Flags { get; }

        string? InterruptingVariable { get; }

        void WriteBreak(bool hasExpression);
        void WriteContinue(bool hasExpression);
    }
}
