﻿namespace IS4.Sona.Compiler.States
{
    internal interface IReturnScope
    {
        string? ReturnVariable { get; }
        string? SuccessVariable { get; }
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
}
