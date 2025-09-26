using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Compiler.Tools;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal abstract class InlineSource
        : NodeState
    {
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
                if(
                    !text.Equals("F#", StringComparison.OrdinalIgnoreCase) &&
                    !text.Equals("FSharp", StringComparison.OrdinalIgnoreCase) &&
                    !text.Equals("JS", StringComparison.OrdinalIgnoreCase) &&
                    !text.Equals("JavaScript", StringComparison.OrdinalIgnoreCase))
                {
                    Error("Only F# and JavaScript inline source is supported.", context);
                }
            }
            finally
            {
                Environment.DisableParseTree();
            }
        }

        public override void EnterInlineSourceFSharp(InlineSourceFSharpContext context)
        {
            EnterState<InlineFSharpSource>().EnterInlineSourceFSharp(context);
        }

        public override void ExitInlineSourceFSharp(InlineSourceFSharpContext context)
        {

        }

        public override void EnterInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            EnterState<InlineJavaScriptSource>().EnterInlineSourceJavaScript(context);
        }

        public override void ExitInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {

        }
    }

    internal sealed class InlineSourceExpression : InlineSource
    {
        public override void EnterInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            Out.WriteNamespacedName("Fable.Core", "JsInterop");
            Out.Write(".emitJsExpr()(\"");
            base.EnterInlineSourceJavaScript(context);
        }

        public override void ExitInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            base.ExitInlineSourceJavaScript(context);
            Out.Write("\")");
        }
    }

    internal sealed class InlineSourceStatement : InlineSource
    {
        public override void EnterInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            Out.WriteNamespacedName("Fable.Core", "JsInterop");
            Out.Write(".emitJsStatement()(\"");
            base.EnterInlineSourceJavaScript(context);
        }

        public override void ExitInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            base.ExitInlineSourceJavaScript(context);
            Out.Write("\")");
        }
    }

    internal sealed class InlineSourceType : InlineSource
    {
        public override void EnterInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            Error("Inline JavaScript source is not supported as a type.", context);
            Out.WriteIdentifier("<error>");
            Out.Write("<const \"");
            base.EnterInlineSourceJavaScript(context);
        }

        public override void ExitInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            base.ExitInlineSourceJavaScript(context);
            Out.Write("\">");
        }
    }

    internal sealed class InlineSourcePattern : InlineSource
    {
        public override void EnterInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            Out.WriteNamespacedName("Fable.Core", "JsInterop");
            Out.Write(".emitJsExpr()(\"");
            base.EnterInlineSourceJavaScript(context);
        }

        public override void ExitInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            base.ExitInlineSourceJavaScript(context);
            Out.Write("\")");
        }
    }

    internal sealed class InlineFSharpSource : InlineSource
    {
        int? baseIndent;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            baseIndent = null;
        }

        public override void EnterInlineSourceFSharp(InlineSourceFSharpContext context)
        {

        }

        public override void ExitInlineSourceFSharp(InlineSourceFSharpContext context)
        {
            ExitState().ExitInlineSourceFSharp(context);
        }

        public override void EnterInlineSourceFSNewLine(InlineSourceFSNewLineContext context)
        {
            Environment.EnableParseTree();
        }

        public override void ExitInlineSourceFSNewLine(InlineSourceFSNewLineContext context)
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

        public override void EnterInlineSourceFSIndentation(InlineSourceFSIndentationContext context)
        {

        }

        public override void ExitInlineSourceFSIndentation(InlineSourceFSIndentationContext context)
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

        public override void EnterInlineSourceFSToken(InlineSourceFSTokenContext context)
        {
            EnterState<Write>().EnterInlineSourceFSToken(context);
        }

        public override void ExitInlineSourceFSToken(InlineSourceFSTokenContext context)
        {

        }

        public override void EnterInlineSourceFSPart(InlineSourceFSPartContext context)
        {
            EnterState<Write>().EnterInlineSourceFSPart(context);
        }

        public override void ExitInlineSourceFSPart(InlineSourceFSPartContext context)
        {

        }

        public override void EnterInlineSourceFSDirective(InlineSourceFSDirectiveContext context)
        {
            EnterState<Write>().EnterInlineSourceFSDirective(context);
        }

        public override void ExitInlineSourceFSDirective(InlineSourceFSDirectiveContext context)
        {

        }

        public override void EnterInlineSourceFSWhitespace(InlineSourceFSWhitespaceContext context)
        {
            EnterState<Write>().EnterInlineSourceFSWhitespace(context);
        }

        public override void ExitInlineSourceFSWhitespace(InlineSourceFSWhitespaceContext context)
        {

        }

        public override void EnterInlineSourceFSLineCutComment(InlineSourceFSLineCutCommentContext context)
        {
            EnterState<Ignore>().EnterInlineSourceFSLineCutComment(context);
        }

        public override void ExitInlineSourceFSLineCutComment(InlineSourceFSLineCutCommentContext context)
        {

        }

        sealed class Ignore : NodeState
        {
            protected override void OnEnterToken(IToken token)
            {

            }

            public override void EnterInlineSourceFSLineCutComment(InlineSourceFSLineCutCommentContext context)
            {

            }

            public override void ExitInlineSourceFSLineCutComment(InlineSourceFSLineCutCommentContext context)
            {
                ExitState().ExitInlineSourceFSLineCutComment(context);
            }

            public override void EnterInlineSourceFSIndentation(InlineSourceFSIndentationContext context)
            {

            }

            public override void ExitInlineSourceFSIndentation(InlineSourceFSIndentationContext context)
            {
                ExitState().ExitInlineSourceFSIndentation(context);
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

            public override void EnterInlineSourceFSToken(InlineSourceFSTokenContext context)
            {

            }

            public override void ExitInlineSourceFSToken(InlineSourceFSTokenContext context)
            {
                ExitState().ExitInlineSourceFSToken(context);
            }

            public override void EnterInlineSourceFSPart(InlineSourceFSPartContext context)
            {

            }

            public override void ExitInlineSourceFSPart(InlineSourceFSPartContext context)
            {
                ExitState().ExitInlineSourceFSPart(context);
            }

            public override void EnterInlineSourceFSDirective(InlineSourceFSDirectiveContext context)
            {

            }

            public override void ExitInlineSourceFSDirective(InlineSourceFSDirectiveContext context)
            {
                ExitState().ExitInlineSourceFSDirective(context);
            }

            public override void EnterInlineSourceFSWhitespace(InlineSourceFSWhitespaceContext context)
            {

            }

            public override void ExitInlineSourceFSWhitespace(InlineSourceFSWhitespaceContext context)
            {
                ExitState().ExitInlineSourceFSWhitespace(context);
            }
        }
    }

    internal sealed class InlineJavaScriptSource : InlineSource
    {
        public override void EnterInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {

        }

        public override void ExitInlineSourceJavaScript(InlineSourceJavaScriptContext context)
        {
            ExitState().ExitInlineSourceJavaScript(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            Out.WriteStringPart(node.Symbol.Text);
        }
    }
}
