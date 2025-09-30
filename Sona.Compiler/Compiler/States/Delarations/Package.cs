using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class PackageState : BlockState, IDeclarationsBlockContext
    {
        // Package return is currently ignored
        string? IReturnableStatementContext.ReturnVariable => null;
        string? IReturnableStatementContext.ReturningVariable => null;
        ExpressionType IExpressionContext.Type => ExpressionType.Regular;

        InterruptFlags IInterruptibleStatementContext.Flags => InterruptFlags.None;

        string? IInterruptibleStatementContext.InterruptingVariable => null;

        ImplementationType? IStatementContext.ReturnOptionType => null;

        bool IComputationContext.IsCollection => false;

        string? IComputationContext.BuilderVariable => null;

        public bool Recursive { get; private set; }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            Recursive = false;
        }

        public override void EnterPackageStatement(PackageStatementContext context)
        {
            Recursive = LexerContext.GetState<ForwardRefPragma>() is { };
            Out.Write("module ");
        }

        public override void ExitPackageStatement(PackageStatementContext context)
        {
            ExitState()?.ExitPackageStatement(context);
        }

        public override void EnterName(NameContext context)
        {
            // Newly recursive
            if(Recursive && FindContext<IDeclarationsBlockContext>() is not { Recursive: true })
            {
                // Attributes must be before
                Out.Write("rec ");
            }
            base.EnterName(context);
        }

        public override void EnterMainBlock(MainBlockContext context)
        {
            Out.WriteOperator('=');
            Out.WriteLine(_begin_);
            Out.EnterScope();
        }

        public override void ExitMainBlock(MainBlockContext context)
        {
            Out.ExitScope();
            Out.Write(_end_);
        }

        void IComputationContext.WriteBeginBlockExpression()
        {
            Out.EnterNestedScope();
            Out.WriteLine('(');
        }

        void IComputationContext.WriteEndBlockExpression()
        {
            Out.ExitNestedScope();
            Out.Write(')');
        }

        void IInterruptibleStatementContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Error("`break` must be used in a statement that supports it.", context);
        }

        void IInterruptibleStatementContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Error("`continue` must be used in a statement that supports it.", context);
        }

        void IInterruptibleStatementContext.WriteAfterBreak(ParserRuleContext context)
        {

        }

        void IInterruptibleStatementContext.WriteAfterContinue(ParserRuleContext context)
        {

        }
    }
}
