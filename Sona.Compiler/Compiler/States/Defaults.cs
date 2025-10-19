using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Sona.Compiler.States
{
    internal sealed class Defaults : IComputationContext, IReturnableContext, IInterruptibleContext
    {
        readonly ScriptState state;

        ScriptEnvironment Environment => state.Environment;
        ISourceWriter GlobalOut => Environment.GlobalOutput;
        ISourceWriter Out => Environment.Output;

        ISourceWriter IScopeContext.GlobalWriter => GlobalOut;
        ISourceWriter IScopeContext.LocalWriter => Out;

        bool IStatementContext.TrailAllowed => false;
        BlockFlags IBlockContext.Flags => BlockFlags.None;
        ReturnFlags IReturnableContext.Flags => ReturnFlags.None;
        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;
        string? IInterruptibleContext.InterruptingVariable => null;
        ComputationFlags IComputationContext.Flags => ComputationFlags.None;

        public Defaults(ScriptState state)
        {
            this.state = state;
        }

        void Error(string message, ParserRuleContext context)
        {
            Environment.ReportError(message, context);
        }

        void Error(string message, ITerminalNode node)
        {
            Environment.ReportError(message, node);
        }

        void Error(string message, IToken token)
        {
            Environment.ReportError(message, token);
        }

        public void WriteEarlyReturn(ParserRuleContext context)
        {
            Error("COMPILER ERROR: Early return is not supported.", context);
        }

        public void WriteReturnStatement(ParserRuleContext context)
        {
            if(!String.IsNullOrEmpty(Environment.Return))
            {
                Out.Write('(');
                Out.Write(Environment.Return);
            }
        }

        public void WriteAfterReturnStatement(ParserRuleContext context)
        {
            if(!String.IsNullOrEmpty(Environment.Return))
            {
                Out.Write(')');
            }
        }

        public void WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            if(isOption)
            {
                Error("`return` with an option passthrough is permitted only within an optional function.", context);
            }
            Out.Write('(');
        }

        public void WriteAfterReturnValue(ParserRuleContext context)
        {
            Out.Write(')');
        }

        public void WriteEmptyReturnValue(ParserRuleContext context)
        {
            Out.Write("()");
        }

        public void WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if(!String.IsNullOrEmpty(Environment.Return))
            {
                Out.Write('(');
                Out.Write(Environment.Return);
                Out.Write("())");
            }
            else
            {
                Out.Write("()");
            }
        }

        public void WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Error("`break` must be used in a statement that supports it.", context);
        }

        public void WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Error("`continue` must be used in a statement that supports it.", context);
        }

        public void WriteAfterBreak(ParserRuleContext context)
        {

        }

        public void WriteAfterContinue(ParserRuleContext context)
        {

        }

        public void WriteBeginBlockExpression(ParserRuleContext context)
        {
            Out.EnterNestedScope();
            Out.WriteLine('(');
        }

        public void WriteEndBlockExpression(ParserRuleContext context)
        {
            Out.ExitNestedScope();
            Out.Write(')');
        }
    }
}
