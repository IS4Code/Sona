﻿using System;

namespace IS4.Sona.Compiler.States
{
    internal interface IStatementContext
    {
        bool TrailAllowed { get; }
    }

    internal interface IReturnableStatementContext : IStatementContext
    {
        string? ReturnVariable { get; }
        string? ReturningVariable { get; }
    }

    internal interface IFunctionContext : IReturnableStatementContext, IInterruptibleStatementContext, IStatementContext, IComputationContext, IExpressionContext
    {
        bool IsOptionalReturn { get; }
        void WriteBegin();
        void WriteEnd();
    }

    internal interface IComputationContext : IInterruptibleStatementContext, IStatementContext
    {
        bool IsCollection { get; }
        string? BuilderVariable { get; }
    }

    internal interface IExpressionContext
    {
        bool IsLiteral { get; }
    }

    internal interface IBindingContext
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

    internal interface IInterruptibleStatementContext : IStatementContext
    {
        InterruptFlags Flags { get; }

        string? InterruptingVariable { get; }

        void WriteBreak(bool hasExpression);
        void WriteContinue(bool hasExpression);
    }
}
