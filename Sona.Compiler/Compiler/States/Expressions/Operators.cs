using static IS4.Sona.Grammar.SonaParser;

namespace IS4.Sona.Compiler.States
{
    internal sealed class CoalesceState : ExpressionState
    {
        string? alternativeName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            alternativeName = null;
        }

        public override void EnterCoalesceExpr(CoalesceExprContext context)
        {
            // Expose the type of the alternative through a variable
            Out.Write("(let mutable ");
            alternativeName = Out.CreateTemporaryIdentifier();
            Out.WriteIdentifier(alternativeName);
            Out.WriteOperator('=');
            Out.WriteCoreOperatorName("Unchecked");
            Out.Write(".defaultof<_> in match ");
            Out.WriteNamespacedName("Sona.Runtime.CompilerServices", "Operators", "BindToResult");
            Out.Write('(');
            Out.WriteIdentifier(alternativeName);
            Out.Write(')');
        }

        public override void EnterCoalesceExprArg(CoalesceExprArgContext context)
        {
            Out.Write('(');
        }

        public override void ExitCoalesceExprArg(CoalesceExprArgContext context)
        {
            Out.Write(")with|struct(true,");
            var arg = Out.CreateTemporaryIdentifier();
            Out.WriteIdentifier(arg);
            Out.Write(")->");
            Out.WriteIdentifier(arg);
            Out.Write("|struct(false,_)->(");
            Out.WriteIdentifier(alternativeName ?? Error("COMPILER ERROR: Temporary variable missing.", context));
            Out.WriteOperator("<-");
            Out.Write('(');
        }

        public override void ExitCoalesceExpr(CoalesceExprContext context)
        {
            Out.Write(");");
            Out.WriteIdentifier(alternativeName ?? Error("COMPILER ERROR: Temporary variable missing.", context));
            Out.Write("))");
            ExitState().ExitCoalesceExpr(context);
        }

        public override void EnterCoalesceExprOuter(CoalesceExprOuterContext context)
        {
            EnterState<Outer>().EnterCoalesceExprOuter(context);
        }

        public override void ExitCoalesceExprOuter(CoalesceExprOuterContext context)
        {

        }

        sealed class Outer : ExpressionState
        {
            public override void EnterCoalesceExprOuter(CoalesceExprOuterContext context)
            {

            }

            public override void ExitCoalesceExprOuter(CoalesceExprOuterContext context)
            {
                ExitState().ExitCoalesceExprOuter(context);
            }
        }
    }
}
