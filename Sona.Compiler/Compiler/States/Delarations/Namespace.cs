using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class NamespaceSection : SectionState
    {
        bool hasName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasName = false;
        }

        public override void EnterNamespaceSection(NamespaceSectionContext context)
        {
            Recursive = LexerContext.GetState<ForwardRefPragma>() is { };
            Out.Write("namespace ");
            if(Recursive)
            {
                Out.Write("rec ");
            }
        }

        public override void ExitNamespaceSection(NamespaceSectionContext context)
        {
            ExitState()!.ExitNamespaceSection(context);
        }

        public override void EnterCompoundName(CompoundNameContext context)
        {
            hasName = true;
            base.EnterCompoundName(context);
        }

        public override void EnterMainBlock(MainBlockContext context)
        {
            if(!hasName)
            {
                Out.Write("global");
            }
            Out.WriteLine();

            base.EnterMainBlock(context);
        }
    }
}
