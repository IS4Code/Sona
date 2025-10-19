using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class PackageState : BlockState, IDeclarationsBlockContext
    {
        ExpressionFlags IExpressionContext.Flags => ExpressionFlags.IsValue;

        BlockFlags IBlockContext.Flags => BlockFlags.None;

        ReturnFlags IReturnableContext.Flags => ReturnFlags.None;

        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;

        string? IInterruptibleContext.InterruptingVariable => null;

        ComputationFlags IComputationContext.Flags => ComputationFlags.None;

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

        void IComputationContext.WriteBeginBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteBeginBlockExpression(context);
        }

        void IComputationContext.WriteEndBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteEndBlockExpression(context);
        }

        void IReturnableContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            Out.Write("do ");
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {

        }

        void IBlockContext.WriteImplicitReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteImplicitReturnStatement(context);
        }

        void IReturnableContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            Defaults.WriteReturnValue(isOption, context);
        }

        void IReturnableContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnValue(context);
        }

        void IReturnableContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            Defaults.WriteEmptyReturnValue(context);
        }

        void IInterruptibleContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteBreak(hasExpression, context);
        }

        void IInterruptibleContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteContinue(hasExpression, context);
        }

        void IInterruptibleContext.WriteAfterBreak(ParserRuleContext context)
        {
            Defaults.WriteAfterBreak(context);
        }

        void IInterruptibleContext.WriteAfterContinue(ParserRuleContext context)
        {
            Defaults.WriteAfterContinue(context);
        }
    }
}
