using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class DeclarationsBlockState : BlockState, IFunctionContext, IDeclarationsBlockContext
    {
        ExpressionFlags IExpressionContext.Flags => ExpressionFlags.IsValue;

        BlockFlags IBlockContext.Flags => BlockFlags.None;

        ReturnFlags IReturnableContext.Flags => ReturnFlags.None;

        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;

        string? IInterruptibleContext.InterruptingVariable => null;

        ComputationFlags IComputationContext.Flags => ComputationFlags.None;

        public bool Recursive { get; protected set; }

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

    internal abstract class SectionState : DeclarationsBlockState
    {
        public override void EnterMainBlock(MainBlockContext context)
        {
            Out.WriteLine("open global.System");
            Out.WriteLine("open global.Sona.Runtime");
            Out.WriteLine("open global.Sona.Runtime.Computations");
            Out.WriteLine("open global.Sona.Runtime.Traits");
        }

        public override void ExitMainBlock(MainBlockContext context)
        {

        }
    }

    internal class ChunkState : SectionState
    {
        public ChunkState(ScriptEnvironment environment)
        {
            Initialize(environment, null);
        }

        public override void EnterNamespaceSection(NamespaceSectionContext context)
        {
            EnterState<NamespaceSection>().EnterNamespaceSection(context);
        }

        public override void ExitNamespaceSection(NamespaceSectionContext context)
        {

        }

        public override void EnterPackageSection(PackageSectionContext context)
        {
            EnterState<PackageSection>().EnterPackageSection(context);
        }

        public override void ExitPackageSection(PackageSectionContext context)
        {

        }
    }
}
