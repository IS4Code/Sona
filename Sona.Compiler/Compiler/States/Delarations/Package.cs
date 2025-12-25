using Antlr4.Runtime;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal class PackageState : DeclarationsBlockState
    {
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
    }

    internal sealed class PackageSection : SectionState
    {
        public override void EnterPackageSection(PackageSectionContext context)
        {
            Recursive = LexerContext.GetState<ForwardRefPragma>() is { };
            Out.Write("module ");
            if(Recursive)
            {
                Out.Write("rec ");
            }
        }

        public override void ExitPackageSection(PackageSectionContext context)
        {
            ExitState()!.ExitPackageSection(context);
        }

        public override void EnterMainBlock(MainBlockContext context)
        {
            Out.WriteLine();

            base.EnterMainBlock(context);
        }
    }
}
