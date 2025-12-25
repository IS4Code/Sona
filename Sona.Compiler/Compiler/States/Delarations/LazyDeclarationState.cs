using System.Collections.Generic;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class LazyDeclarationState : DeclarationState
    {
        readonly List<ISourceCapture> attributes = new();
        readonly List<(string identifier, ISourceCapture capture)> members = new();

        ISourceCapture? capture;
        bool capturingAttributes;

        string singletonTypeParam = "";

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            attributes.Clear();
            members.Clear();
            capture = null;
            capturingAttributes = true;
            singletonTypeParam = Out.CreateTemporaryIdentifier();
        }

        const string forceMethodIdentifier = "<Force>";

        public override void EnterLazyVariableDecl(LazyVariableDeclContext context)
        {
            if(LexerContext.GetState<RecursivePragma>()?.Value == true)
            {
                Error("A 'lazy' declaration is not permitted together with '#pragma recursive'.", context);
            }
        }

        public override void ExitLazyVariableDecl(LazyVariableDeclContext context)
        {
            // Each value is exposed as a singleton generic static type

            for(int i = 0; i < members.Count; i++)
            {
                var (identifier, capture) = members[i];
                
                Out.Write(i == 0 ? "type " : "and ");
                Out.WriteStaticTypeAttributes();
                Out.WriteIdentifier(TypeIdentifier(identifier));

                // Type parameters
                Out.WriteSingletonTypeParameters(singletonTypeParam);
                Out.Write(" private() = ");
                Out.WriteLine(_begin_);
                Out.EnterScope();

                // Apply the attributes
                foreach(var attributeCapture in attributes)
                {
                    attributeCapture.Play(Out);
                }

                Out.Write("static member val ");

                // Actual member declaration
                capture.Play(Out);

                if(i < members.Count - 1)
                {
                    // Has a following member
                    Out.WriteOperator("|>");

                    // Calling force on it
                    Out.WriteIdentifier(TypeIdentifier(members[i + 1].identifier));
                    Out.Write("<'");
                    Out.WriteIdentifier(singletonTypeParam);
                    Out.Write(">.");
                    Out.WriteIdentifier(forceMethodIdentifier);
                }

                Out.WriteLine();

                // Write the force method
                Out.Write("static member ");
                Out.WriteIdentifier(forceMethodIdentifier);
                Out.Write("<'x>(x : 'x) : 'x = x");

                Out.ExitScope();
                Out.WriteLine();
                Out.WriteLine(_end_);
            }

            foreach(var (identifier, _) in members)
            {
                // CLI get_ method for .NET code to see
                Out.Write("let [<");
                Out.WriteCoreName("CompiledNameAttribute");
                Out.Write("(\"");
                // Invisible outside F#
                Out.WriteStringPart("get_" + identifier);
                Out.Write("\")>]");
                Out.WriteIdentifier(Out.CreateTemporaryIdentifier());
                Out.Write("()");
                Out.WriteOperator('=');
                Out.WriteIdentifier(TypeIdentifier(identifier));
                Out.WriteSingletonTypeArguments();
                Out.Write('.');
                Out.WriteIdentifier(identifier);
                Out.WriteLine();

                // An inline type function to prevent overwriting the symbol, and for external F# code
                Out.Write("let [<");
                Out.WriteCoreName("CompiledNameAttribute");
                Out.Write("(\"");
                // Invisible outside F#
                Out.WriteStringPart("<get>" + identifier);
                Out.Write("\")>]inline ");
                Out.WriteIdentifier(identifier);
                Out.WriteSingletonTypeParameters(singletonTypeParam);
                Out.WriteOperator('=');
                Out.WriteIdentifier(TypeIdentifier(identifier));
                Out.Write("<'");
                Out.WriteIdentifier(singletonTypeParam);
                Out.Write(">.");
                Out.WriteIdentifier(identifier);
                Out.WriteLine();
            }

            for(int i = 0; i < members.Count; i++)
            {
                // Open each type to get to the property directly (unnecessary but better diagnostics)
                Out.Write("open type ");
                Out.WriteIdentifier(TypeIdentifier(members[i].identifier));
                Out.WriteSingletonTypeArguments();

                if(i < members.Count - 1)
                {
                    // Not last member
                    Out.WriteLine();
                }
            }

            ExitState().ExitLazyVariableDecl(context);
        }

        static string TypeIdentifier(string identifier)
        {
            return "lazy " + identifier;
        }

        public override void EnterLocalAttribute(LocalAttributeContext context)
        {
            if(capturingAttributes)
            {
                capture = Out.StartCapture();
                attributes.Add(capture);
            }
            base.EnterLocalAttribute(context);
        }

        public override void ExitLocalAttribute(LocalAttributeContext context)
        {
            base.ExitLocalAttribute(context);
            if(capturingAttributes && capture != null)
            {
                Out.StopCapture(capture);
                capture = null;
            }
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            capturingAttributes = false;

            // The name of the following declaration must be known to start processing
            capture = Out.StartCapture();
        }

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void EnterPattern(PatternContext context)
        {
            Error("`lazy` declarations cannot be used with arbitrary patterns.", context);
            base.EnterPattern(context);
        }

        public override void EnterName(NameContext context)
        {
            StartCaptureInput(context);

            base.EnterName(context);
        }

        public override void ExitName(NameContext context)
        {
            base.ExitName(context);

            var identifier = StopCaptureInputIdentifier(context);

            if(capture != null)
            {
                members.Add((identifier, capture));
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.WriteOperator('=');

            if(members.Count >= 2)
            {
                // Has a previous member
                var previousIdentifier = members[members.Count - 2].identifier;

                // Prefix with calling force on the previous member
                Out.WriteIdentifier(TypeIdentifier(previousIdentifier));
                Out.Write("<'");
                Out.WriteIdentifier(singletonTypeParam);
                Out.Write(">.");
                Out.WriteIdentifier(forceMethodIdentifier);
                Out.Write("();");
            }

            Out.Write('(');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            Out.Write(')');
            if(capture != null)
            {
                Out.StopCapture(capture);
                capture = null;
            }
        }
    }
}
