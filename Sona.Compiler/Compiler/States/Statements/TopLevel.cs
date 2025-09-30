using System;
using System.IO;
using Sona.Compiler.Tools;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class TopLevelStatement : NodeState
    {
        string? argument;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            argument = null;
        }

        public override void EnterTopLevelStatement(TopLevelStatementContext context)
        {

        }

        public override void ExitTopLevelStatement(TopLevelStatementContext context)
        {
            ExitState().ExitTopLevelStatement(context);
        }

        public override void EnterImportStatement(ImportStatementContext context)
        {
            Out.Write("open ");
        }

        public override void EnterImportTypeStatement(ImportTypeStatementContext context)
        {
            Out.Write("open type ");
        }

        public override void EnterImportFileStatement(ImportFileStatementContext context)
        {
            Out.Write("#load ");
        }

        public override void EnterIncludeStatement(IncludeStatementContext context)
        {
            Out.Write("#load ");
        }

        public override void EnterRequireStatement(RequireStatementContext context)
        {
            Out.Write("#r ");
        }

        public override void EnterPackageStatement(PackageStatementContext context)
        {
            EnterState<PackageState>().EnterPackageStatement(context);
        }

        public override void ExitImportStatement(ImportStatementContext context)
        {

        }

        public override void ExitImportTypeStatement(ImportTypeStatementContext context)
        {

        }

        public override void ExitImportFileStatement(ImportFileStatementContext context)
        {
            Out.WriteLine();
            Out.Write("open ");

            var path = Syntax.GetStringLiteralValue(argument!);
            var name = Path.GetFileNameWithoutExtension(path);
            if(name.Length > 0)
            {
                char c = name[0];
                char uc = Char.ToUpperInvariant(c);
                if(uc != c)
                {
                    name = uc + name.Substring(1);
                }
            }

            Out.WriteIdentifier(name);
        }

        public override void ExitIncludeStatement(IncludeStatementContext context)
        {

        }

        public override void ExitRequireStatement(RequireStatementContext context)
        {

        }

        public override void ExitPackageStatement(PackageStatementContext context)
        {

        }

        public override void EnterString(StringContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitString(StringContext context)
        {
            try
            {
                Out.Write(argument = context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }
    }
}
