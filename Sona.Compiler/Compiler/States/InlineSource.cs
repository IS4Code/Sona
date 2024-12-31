using Antlr4.Runtime;
using IS4.Sona.Compiler.Tools;
using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class InlineSource : NodeState
    {
        int? baseIndent;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            baseIndent = null;
        }

        protected override void UpdateOnToken(IToken token)
        {
            // Do not inform writer about new lines
        }

        public override void EnterInlineSourceFree(InlineSourceFreeContext context)
        {

        }

        public override void EnterInlineSourceReturning(InlineSourceReturningContext context)
        {

        }

        public override void EnterInlineSourceTerminating(InlineSourceTerminatingContext context)
        {

        }

        public override void ExitInlineSourceFree(InlineSourceFreeContext context)
        {
            ExitState().ExitInlineSourceFree(context);
        }

        public override void ExitInlineSourceReturning(InlineSourceReturningContext context)
        {
            ExitState().ExitInlineSourceReturning(context);
        }

        public override void ExitInlineSourceTerminating(InlineSourceTerminatingContext context)
        {
            ExitState().ExitInlineSourceTerminating(context);
        }

        public override void EnterString(StringContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitString(StringContext context)
        {
            try
            {
                var text = Syntax.GetStringLiteralValue(context.GetText());
                if(text != "F#")
                {
                    Error("Only F# inline source is supported.");
                }
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceIndentation(InlineSourceIndentationContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourceIndentation(InlineSourceIndentationContext context)
        {
            Out.WriteLine();
            try
            {
                var text = context.GetText();
                int indent = 0;
                int spaceAt = text.IndexOf(' ');
                if(spaceAt != -1)
                {
                    indent = text.Length - spaceAt;
                }
                if(baseIndent is { } previousIndent)
                {
                    indent -= previousIndent;
                    if(indent < 0)
                    {
                        indent = 0;
                    }
                }
                else
                {
                    baseIndent = indent;
                    indent = 0;
                }
                if(indent > 0)
                {
                    Out.Write(new string(' ', indent));
                }
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceToken(InlineSourceTokenContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourceToken(InlineSourceTokenContext context)
        {
            try
            {
                Out.Write(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourcePart(InlineSourcePartContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourcePart(InlineSourcePartContext context)
        {
            try
            {
                Out.Write(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceDirective(InlineSourceDirectiveContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourceDirective(InlineSourceDirectiveContext context)
        {
            try
            {
                Out.Write(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceEmptyLine(InlineSourceEmptyLineContext context)
        {

        }

        public override void ExitInlineSourceEmptyLine(InlineSourceEmptyLineContext context)
        {
            Out.WriteLine();
        }
    }
}
