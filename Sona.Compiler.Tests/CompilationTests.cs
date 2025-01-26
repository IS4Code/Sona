using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using IS4.Sona.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IS4.Sona.Tests
{
    public abstract class CompilationTests
    {
        protected virtual bool GenerateLineNumbers => false;

        static readonly char[] newlineChars = Environment.NewLine.ToCharArray();

        private protected const string not = "global.Microsoft.FSharp.Core.Operators.``not``";
        private protected const string cat = ".``..``";
        private protected const string each = ".``each()``";

        private string? CompileToSource(string source, bool exception)
        {
            var inputStream = new AntlrInputStream(source);
            var compiler = new SonaCompiler();
            compiler.AdjustLines = GenerateLineNumbers;

            var writer = new StringWriter();
            try
            {
                compiler.CompileToSource(inputStream, writer, throwOnError: true);

                return writer.ToString().TrimEnd(newlineChars);
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

        protected void AssertStatementEquivalence(string source, string? expected)
        {
            expected = ReplacePlaceholders(expected);
            if(expected != null)
            {
                expected = expected + Environment.NewLine + "()";
            }
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);
        }

        protected void AssertBlockEquivalence(string source, string? expected)
        {
            expected = ReplacePlaceholders(expected);
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);

            var source2 = $"function test() {source} end";
            string? expected2 = null;
            if(expected != null)
            {
                var indented = String.Join(Environment.NewLine, expected.Split(Environment.NewLine).Select(l => " " + l));
                expected = expected + Environment.NewLine + "()";
                expected2 = $@"let rec test() = begin
{indented}
end
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