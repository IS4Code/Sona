using System;
using Antlr4.Runtime;

namespace Sona.Compiler.States
{
    internal interface IStatementContext
    {
        bool TrailAllowed { get; }
    }

    internal interface IBlockStatementContext
    {
        /// <summary>
        /// Writes the statement that terminates an open block.
        /// </summary>
        void WriteImplicitReturnStatement(ParserRuleContext context);
    }

    [Flags]
    internal enum ReturnFlags
    {
        None,

        /// <summary>
        /// Indicates that returning is performed through pre-existing variables.
        /// </summary>
        Indirect = 1
    }

    internal interface IReturnableStatementContext : IBlockStatementContext
    {
        ReturnFlags Flags { get; }

        /// <summary>
        /// Writes an <c>if</c> statement that checks if returning was requested.
        /// </summary>
        /// <remarks>
        /// This should be used only together with <see cref="ReturnFlags.Indirect"/>
        /// after a conditional block from which a value may be returned.
        /// </remarks>
        void WriteEarlyReturn(ParserRuleContext context);

        /// <summary>
        /// Writes the beginning of the final statement of the block that should return a value.
        /// This is paired with <see cref="WriteAfterReturnStatement(ParserRuleContext)"/> to terminate
        /// the statement.
        /// </summary>
        /// <remark>
        /// This call should be followed by an expression that produces the value. This could
        /// be a pre-existing variable storing the to-be-returned value, or
        /// a new expression wrapped in calls to <see cref="WriteReturnValue(ParserRuleContext)"/>
        /// and <see cref="WriteAfterReturnValue(ParserRuleContext)"/>.
        /// </remark>
        void WriteReturnStatement(ParserRuleContext context);

        /// <summary>
        /// Write the ending of the final statement to return a value,
        /// started by <see cref="WriteReturnStatement(ParserRuleContext)"/>.
        /// </summary>
        void WriteAfterReturnStatement(ParserRuleContext context);

        /// <summary>
        /// Writes the beginning of the expression producing a new value to be returned from the block.
        /// This is paired with <see cref="WriteAfterReturnValue(ParserRuleContext)"/>.
        /// </summary>
        /// <remarks>
        /// This should be used only to wrap the original expression after <c>return</c>.
        /// </remarks>
        void WriteReturnValue(bool isOption, ParserRuleContext context);

        /// <summary>
        /// Write the ending of the expression started
        /// by <see cref="WriteReturnValue(ParserRuleContext)"/>.
        /// </summary>
        void WriteAfterReturnValue(ParserRuleContext context);

        /// <summary>
        /// Writes the expression that corresponds to the value 
        /// return by a <c>return</c> statement with no argument.
        /// </summary>
        void WriteEmptyReturnValue(ParserRuleContext context);
    }

    internal interface IFunctionContext : IReturnableStatementContext, IInterruptibleStatementContext, IStatementContext, IComputationContext, IExpressionContext
    {

    }

    internal interface IDeclarationsBlockContext : IFunctionContext
    {
        bool Recursive { get; }
    }

    internal interface IComputationContext : IInterruptibleStatementContext, IReturnableStatementContext
    {
        bool IsCollection { get; }
        string? BuilderVariable { get; }
        void WriteBeginBlockExpression(ParserRuleContext context);
        void WriteEndBlockExpression(ParserRuleContext context);
    }

    internal interface IExpressionContext
    {
        ExpressionType Type { get; }
    }

    internal enum ExpressionType
    {
        Regular,
        Literal,
        Pattern
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

        void WriteBreak(bool hasExpression, ParserRuleContext context);
        void WriteAfterBreak(ParserRuleContext context);
        void WriteContinue(bool hasExpression, ParserRuleContext context);
        void WriteAfterContinue(ParserRuleContext context);
    }

    internal enum ImplementationType
    {
        Class,
        Struct
    }

    internal enum CollectionImplementationType
    {
        Array,
        List
    }
}
