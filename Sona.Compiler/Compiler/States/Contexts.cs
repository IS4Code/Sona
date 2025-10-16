using System;
using Antlr4.Runtime;

namespace Sona.Compiler.States
{
    internal interface IScopeContext
    {
        ISourceWriter GlobalWriter { get; }
        ISourceWriter LocalWriter { get; }
    }

    internal interface IStatementContext : IScopeContext
    {
        bool TrailAllowed { get; }
    }

    [Flags]
    internal enum BlockFlags
    {
        None,

        /// <summary>
        /// Indicates that execution may flow elsewhere from within this block.
        /// This flag is inherited in nested blocks.
        /// </summary>
        HasTrySemantics = 1
    }

    internal interface IBlockStatementContext : IStatementContext
    {
        BlockFlags Flags { get; }

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

    internal interface IExpressionContext : IScopeContext
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

    internal static class ContextExtensions
    {
        public static bool HasFlag(this IBlockStatementContext scope, BlockFlags flags)
        {
            return (scope.Flags & flags) == flags;
        }

        public static bool HasFlag(this IReturnableStatementContext scope, ReturnFlags flags)
        {
            return (scope.Flags & flags) == flags;
        }

        public static bool HasFlag(this IInterruptibleStatementContext scope, InterruptFlags flags)
        {
            return (scope.Flags & flags) == flags;
        }

        /// <summary>
        /// Writes the beginning of a <c>return</c> statement with a new value.
        /// </summary>
        public static void WriteDirectReturnStatement(this IReturnableStatementContext returnScope, bool isOption, ParserRuleContext context)
        {
            returnScope.WriteReturnStatement(context);
            returnScope.WriteReturnValue(isOption, context);
        }

        /// <summary>
        /// Writes the ending of a <c>return</c> statement with a new value.
        /// </summary>
        public static void WriteAfterDirectReturnStatement(this IReturnableStatementContext returnScope, ParserRuleContext context)
        {
            returnScope.WriteAfterReturnValue(context);
            returnScope.WriteAfterReturnStatement(context);
        }

        /// <summary>
        /// Writes a direct <c>return</c> statement with no value.
        /// </summary>
        public static void WriteEmptyReturnStatement(this IReturnableStatementContext returnScope, ParserRuleContext context)
        {
            returnScope.WriteReturnStatement(context);
            returnScope.WriteEmptyReturnValue(context);
            returnScope.WriteAfterReturnStatement(context);
        }

        /// <summary>
        /// Writes a synthesized <c>return</c> statement using a pre-existing variable.
        /// </summary>
        public static void WriteIndirectReturnStatement(this IReturnableStatementContext returnScope, string identifier, ParserRuleContext context)
        {
            returnScope.WriteReturnStatement(context);
            returnScope.LocalWriter.WriteIdentifier(identifier);
            returnScope.WriteAfterReturnStatement(context);
        }

        /// <summary>
        /// Writes a synthesized <c>return</c> statement using the default value.
        /// </summary>
        public static void WriteDefaultReturnStatement(this IReturnableStatementContext returnScope, ParserRuleContext context)
        {
            returnScope.WriteReturnStatement(context);
            returnScope.LocalWriter.WriteDefaultValue();
            returnScope.WriteAfterReturnStatement(context);
        }
    }
}
