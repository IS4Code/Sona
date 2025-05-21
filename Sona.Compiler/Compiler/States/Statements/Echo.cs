using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class EchoState : NodeState
    {
        readonly List<string> variables = new();
        bool hasFormat, first;
        ISourceCapture? stringCapture;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            variables.Clear();
            hasFormat = false;
            first = true;
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
        }

        public override void ExitEchoStatement(EchoStatementContext context)
        {
            if(stringCapture != null)
            {
                // Has sole string argument - treat as format
                Out.Write('(');
                stringCapture.Play(Out);
                stringCapture = null;
                Out.Write(')');
            }
            else if(!hasFormat)
            {
                Out.Write("$\"");
                foreach(var name in variables)
                {
                    Out.Write('{');
                    Out.WriteIdentifier(name);
                    Out.Write('}');
                }
                Out.Write("\")");
            }
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
                // Has string argument
                if(hasFormat)
                {
                    // Does formatting - first argument
                    Out.Write('(');
                    stringCapture.Play(Out);
                    stringCapture = null;
                    Out.Write(')');
                }
                else
                {
                    // Part of interpolated string
                    Out.Write("(");
                    AddVariable();
                    stringCapture.Play(Out);
                    stringCapture = null;
                    Out.Write(") ");
                    Out.ExitNestedScope();
                    Out.Write("in ");
                    first = false;
                }
            }
            if(hasFormat)
            {
                Out.Write('(');
            }
            else
            {
                if(first)
                {
                    Out.Write("(");
                    first = false;
                }
                AddVariable();
            }
            EnterState<ExpressionState>().EnterExpression(context);

            void AddVariable()
            {
                var name = Out.CreateTemporaryIdentifier();
                variables.Add(name);
                Out.EnterNestedScope(true);
                Out.Write("let ");
                Out.WriteIdentifier(name);
                Out.WriteOperator('=');
                Out.Write('(');
            }
        }

        public override void ExitExpression(ExpressionContext context)
        {
            if(hasFormat)
            {
                Out.Write(')');
            }
            else
            {
                Out.Write(") ");
                Out.ExitNestedScope();
                Out.Write("in ");
            }
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
                            context.hasFormat = true;
                        }
                        break;
                }
            }
        }
    }
}
