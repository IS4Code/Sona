using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sona.Compiler.SourceGenerator
{
    [Generator]
    public sealed class ListenerGenerator : IIncrementalGenerator
    {
        const string baseNs = nameof(Sona);

        const string interfaceNs = baseNs + ".Grammar";
        const string interfaceName = "ISonaParserListener";
        const string interfaceType = interfaceNs + "." + interfaceName;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is InterfaceDeclarationSyntax listener && listener.Identifier.Text == interfaceName && GetFullName(listener) == interfaceType,
                (context, _) => (TypeDeclarationSyntax)context.Node
            );

            context.RegisterSourceOutput(provider, Execute);
        }

        private void Execute(SourceProductionContext context, TypeDeclarationSyntax type)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("#pragma warning disable 0809"); // Obsolete member '%s' overrides non-obsolete member '%s'
            sb.AppendLine($"namespace {baseNs}.Compiler");
            sb.AppendLine("{");
            {
                sb.AppendLine("partial class ParserListener");
                sb.AppendLine("{");
                {
                    sb.AppendLine("const string obsoleteMsg = \"This listener method is not implemented and should not be called.\";");
                    foreach(var method in type.Members.OfType<MethodDeclarationSyntax>())
                    {
                        var methodName = method.Identifier.Text;
                        string ruleName;

                        if(methodName.StartsWith("Enter", StringComparison.Ordinal))
                        {
                            ruleName = methodName.Substring(5);
                        }
                        else if(methodName.StartsWith("Exit", StringComparison.Ordinal))
                        {
                            ruleName = methodName.Substring(4);
                        }
                        else
                        {
                            continue;
                        }

                        // Listener method

                        var signature = $"void {methodName}({baseNs}.Grammar.SonaParser.{ruleName}Context context)";

                        if(ruleName.IndexOf('_') != -1)
                        {
                            // Helper rule - override as sealed, prevent calling
                            sb.AppendLine("[Obsolete(obsoleteMsg, true)]");
                            sb.Append($"public sealed override {signature} {{ }}");
                        }
                        else if(ruleName.StartsWith("Error", StringComparison.Ordinal))
                        {
                            // Error rule - must be implemented
                            sb.Append($"public abstract override {signature};");
                        }
                        else
                        {
                            // Unimplemented, prevent calling
                            sb.AppendLine("[Obsolete(obsoleteMsg, true)]");
                            sb.Append($"public override {signature} {{ }}");
                        }
                    }
                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");

            context.AddSource("ParserListener.Generated.cs", sb.ToString());
        }

        private static string GetFullName(TypeDeclarationSyntax cls)
        {
            var parts = new List<string>();
            parts.Add(cls.Identifier.ToString());

            var ns = cls.Parent;
            while(ns != null && ns is not NamespaceDeclarationSyntax)
            {
                ns = ns.Parent;
            }

            if(ns != null)
            {
                parts.Add(((NamespaceDeclarationSyntax)ns).Name.ToString());
            }

            parts.Reverse();

            return String.Join(".", parts);
        }
    }
}
