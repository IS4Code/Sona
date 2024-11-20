using System;
using System.IO;
using Antlr4.Runtime;
using IS4.Sona.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IS4.Sona.Tests
{
    public abstract class CompilationTests
    {
        protected virtual bool GenerateLineNumbers => false;

        static readonly char[] newlineChars = Environment.NewLine.ToCharArray();

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

        protected void AssertStatementEquivalence(string source, string? expected)
        {
            if(expected != null)
            {
                expected = expected + Environment.NewLine + "()";
            }
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);
        }

        protected void AssertExpressionEquivalence(string source, string? expected)
        {
            source = "return " + source + ";";
            var actual = CompileToSource(source, expected == null);
            Assert.AreEqual(expected, actual);
        }
    }
}