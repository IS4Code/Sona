using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Sona.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    public abstract class CompilationTests
    {
        protected virtual bool GenerateLineNumbers => false;

        static readonly char[] newlineChars = Environment.NewLine.ToCharArray();

        private protected const string not = "global.Microsoft.FSharp.Core.Operators.``not``";
        private protected const string cat = " |> global.Sona.Runtime.CompilerServices.BinaryOperators.Concat";
        private protected const string each = ".``operator AsEnumerable``(global.Sona.Runtime.CompilerServices.SequenceHelpers.Marker)";
        private protected const string set = ".``operator Assign``";

        static readonly Regex beginEndRegex = new Regex(@"\(\*(begin|end)\*\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private string? CompileToSource(string source, bool exception)
        {
            var inputStream = new AntlrInputStream(source);
            var compiler = new SonaCompiler();

            var options = new CompilerOptions
            (
                Target: BinaryTarget.Exe,
                Flags: CompilerFlags.Privileged | CompilerFlags.DebuggingComments |
                    (GenerateLineNumbers ? 0 : CompilerFlags.IgnoreLineNumbers),
                AssemblyLoadContext: AssemblyLoadContext.Default
            );

            var writer = new StringWriter();
            try
            {
                var result = compiler.CompileToSource(inputStream, writer, options);

                if(!result.Success)
                {
                    return null;
                }

                return beginEndRegex.Replace(writer.ToString().TrimEnd(newlineChars), "$1");
            }
            catch when(exception)
            {
                return null;
            }
        }

        static readonly Regex placeholderRegex = new(@"<\$([^<>$]+)\$>", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        [return: NotNullIfNotNull(nameof(str))]
        private string? ReplacePlaceholders(string? str)
        {
            if(str is null)
            {
                return null;
            }
            var dict = new Dictionary<string, string>();
            int count = 0;
            return placeholderRegex.Replace(str, m => {
                var name = m.Groups[1].Value;
                if(dict.TryGetValue(name, out var id))
                {
                    return id;
                }
                dict[name] = id = $"``_ {++count}``";
                return id;
            });
        }

        protected void AssertTopLevelStatementEquivalence(string source, string? expected)
        {
            expected = ReplacePlaceholders(expected);
            if(expected != null)
            {
                expected = expected + Environment.NewLine + "()";
            }
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);
        }

        protected void AssertInnerStatementEquivalence(string source, string? expected)
        {
            expected = ReplacePlaceholders(expected);
            if(expected != null)
            {
                var indented = String.Join(Environment.NewLine, expected.Split(Environment.NewLine).Select(l => " " + l));
                expected = $@"let rec test() = (
{indented}
 ()
)
()";
            }
            source = $"function test() {source} end";
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);
        }

        protected void AssertStatementEquivalence(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
            AssertInnerStatementEquivalence(source, expected);
        }

        protected void AssertBlockEquivalence(string source, string? expected)
        {
            const string optionalDo = "<?do?>";

            expected = ReplacePlaceholders(expected);
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected?.Replace(optionalDo, "do "), actual);

            var source2 = $"function test() {source} end";
            string? expected2 = null;
            if(expected != null)
            {
                var indented = String.Join(Environment.NewLine, expected.Split(Environment.NewLine).Select(l => " " + l.Replace(optionalDo, "")));
                expected = expected + Environment.NewLine + "()";
                expected2 = $@"let rec test() = (
{indented}
)
()";
            }
            var actual2 = CompileToSource(source2, expected2 == null);
            Assert.AreEqual(expected2, actual2);
        }

        protected void AssertExpressionEquivalence(string source, string? expected)
        {
            expected = ReplacePlaceholders(expected);
            source = "return " + source + ";";
            if(expected != null)
            {
                expected = $"({expected})";
            }
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);
        }
    }
}