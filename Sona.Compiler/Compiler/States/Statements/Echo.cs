using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class EchoState : NodeState
    {
        readonly List<string> variables = new();
        bool hasPercent;
        ISourceCapture? stringCapture;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            variables.Clear();
            hasPercent = false;
            stringCapture = null;
        }

        public override void EnterEchoStatement(EchoStatementContext context)
        {
            var identifier = LexerContext.GetState<EchoPragma>()?.Identifier ?? "printfn";

            if(FindContext<IStatementContext>() is ChunkState)
            {
                // Top-level statement requires unit return
                Out.Write("do ");
            }
            Out.WriteCoreName("ExtraTopLevelOperators");
            Out.Write('.');
            Out.WriteIdentifier(identifier);
            Out.Write('(');
        }

        public override void ExitEchoStatement(EchoStatementContext context)
        {
            if(stringCapture != null)
            {
                // Has sole string argument
                if(hasPercent)
                {
                    // Needs to be "escaped" through a variable to be treated as literal string
                    EnterVariable();
                    stringCapture.Play(Out);
                    stringCapture = null;
                    ExitVariable();
                }
                else
                {
                    stringCapture.Play(Out);
                    stringCapture = null;
                }
            }
            if(variables.Count != 0)
            {
                Out.Write("$\"");
                foreach(var name in variables)
                {
                    Out.Write('{');
                    Out.WriteIdentifier(name);
                    Out.Write('}');
                }
                Out.Write('"');
            }
            Out.Write(')');
            ExitState().ExitEchoStatement(context);
        }

        public override void EnterString(StringContext context)
        {
            stringCapture = Out.StartCapture();
            EnterState<String>().EnterString(context);
        }

        public override void ExitString(StringContext context)
        {
            // Check next argument or end
            Out.StopCapture(stringCapture ?? ErrorCapture("COMPILER ERROR: Missing string capture.", context));
        }

        public override void EnterInterpolatedString(InterpolatedStringContext context)
        {
            stringCapture = Out.StartCapture();
        }

        public override void ExitInterpolatedString(InterpolatedStringContext context)
        {
            // Check next argument or end
            Out.StopCapture(stringCapture ?? ErrorCapture("COMPILER ERROR: Missing string capture.", context));
        }

        public override void EnterExpression(ExpressionContext context)
        {
            if(stringCapture != null)
            {
                // Always part of interpolated string since it is followed by an expression
                EnterVariable();
                stringCapture.Play(Out);
                stringCapture = null;
                ExitVariable();
            }
            EnterVariable();
            EnterState<ExpressionState>().EnterExpression(context);
        }

        void EnterVariable()
        {
            var name = Out.CreateTemporaryIdentifier();
            variables.Add(name);
            Out.EnterNestedScope(true);
            Out.Write("let ");
            Out.WriteIdentifier(name);
            Out.WriteOperator('=');
            Out.Write('(');
        }

        void ExitVariable()
        {
            Out.Write(") ");
            Out.ExitNestedScope();
            Out.Write("in ");
        }

        public override void ExitExpression(ExpressionContext context)
        {
            ExitVariable();
        }

        sealed class String : StringState
        {
            public override void VisitTerminal(ITerminalNode node)
            {
                base.VisitTerminal(node);

                var token = node.Symbol;
                switch(token.Type)
                {
                    case SonaLexer.STRING_PART:
                        if(token.Text.Contains('%') && FindContext<EchoState>() is { } context)
                        {
                            context.hasPercent = true;
                        }
                        break;
                }
            }
        }
    }
}
