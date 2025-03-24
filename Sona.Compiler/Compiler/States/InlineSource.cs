using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Compiler.Tools;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class InlineSource : NodeState
    {
        int? baseIndent;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            baseIndent = null;
        }

        protected override void OnEnterToken(IToken token)
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

        public override void EnterInlineSourceLanguage(InlineSourceLanguageContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourceLanguage(InlineSourceLanguageContext context)
        {
            try
            {
                var text = Syntax.GetStringLiteralValue(context.GetText());
                if(text != "F#")
                {
                    Error("Only F# inline source is supported.", context);
                }
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceNewLine(InlineSourceNewLineContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourceNewLine(InlineSourceNewLineContext context)
        {
            try
            {
                Out.WriteLineEnd(context.GetText());
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceIndentation(InlineSourceIndentationContext context)
        {

        }

        public override void ExitInlineSourceIndentation(InlineSourceIndentationContext context)
        {
            var indent = context.Stop.StopIndex - context.Start.StartIndex + 1;
            if(baseIndent is { } previousIndent)
            {
                indent -= previousIndent;
                if(indent < 0)
                {
                    Error($"Line token is placed {-indent} characters before indent level.", context);
                    indent = 0;
                }
            }
            else
            {
                baseIndent = indent;
                indent = 0;
            }
            Out.WriteSpace(indent);
        }

        public override void EnterInlineSourceToken(InlineSourceTokenContext context)
        {
            EnterState<Write>().EnterInlineSourceToken(context);
        }

        public override void ExitInlineSourceToken(InlineSourceTokenContext context)
        {

        }

        public override void EnterInlineSourcePart(InlineSourcePartContext context)
        {
            EnterState<Write>().EnterInlineSourcePart(context);
        }

        public override void ExitInlineSourcePart(InlineSourcePartContext context)
        {

        }

        public override void EnterInlineSourceDirective(InlineSourceDirectiveContext context)
        {
            EnterState<Write>().EnterInlineSourceDirective(context);
        }

        public override void ExitInlineSourceDirective(InlineSourceDirectiveContext context)
        {

        }

        public override void EnterInlineSourceWhitespace(InlineSourceWhitespaceContext context)
        {
            EnterState<Write>().EnterInlineSourceWhitespace(context);
        }

        public override void ExitInlineSourceWhitespace(InlineSourceWhitespaceContext context)
        {

        }

        public override void EnterInlineSourceLineCutComment(InlineSourceLineCutCommentContext context)
        {
            EnterState<Ignore>().EnterInlineSourceLineCutComment(context);
        }

        public override void ExitInlineSourceLineCutComment(InlineSourceLineCutCommentContext context)
        {

        }

        sealed class Ignore : NodeState
        {
            protected override void OnEnterToken(IToken token)
            {

            }

            public override void EnterInlineSourceLineCutComment(InlineSourceLineCutCommentContext context)
            {

            }

            public override void ExitInlineSourceLineCutComment(InlineSourceLineCutCommentContext context)
            {
                ExitState().ExitInlineSourceLineCutComment(context);
            }

            public override void EnterInlineSourceIndentation(InlineSourceIndentationContext context)
            {

            }

            public override void ExitInlineSourceIndentation(InlineSourceIndentationContext context)
            {
                ExitState().ExitInlineSourceIndentation(context);
            }
        }

        sealed class Write : NodeState
        {
            protected override void OnEnterToken(IToken token)
            {

            }

            public override void VisitTerminal(ITerminalNode node)
            {
                Out.Write(node.Symbol.Text);
            }

            public override void EnterInlineSourceToken(InlineSourceTokenContext context)
            {

            }

            public override void ExitInlineSourceToken(InlineSourceTokenContext context)
            {
                ExitState().ExitInlineSourceToken(context);
            }

            public override void EnterInlineSourcePart(InlineSourcePartContext context)
            {

            }

            public override void ExitInlineSourcePart(InlineSourcePartContext context)
            {
                ExitState().ExitInlineSourcePart(context);
            }

            public override void EnterInlineSourceDirective(InlineSourceDirectiveContext context)
            {

            }

            public override void ExitInlineSourceDirective(InlineSourceDirectiveContext context)
            {
                ExitState().ExitInlineSourceDirective(context);
            }

            public override void EnterInlineSourceWhitespace(InlineSourceWhitespaceContext context)
            {

            }

            public override void ExitInlineSourceWhitespace(InlineSourceWhitespaceContext context)
            {
                ExitState().ExitInlineSourceWhitespace(context);
            }
        }
    }
}
