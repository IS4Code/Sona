using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    [TestClass]
    public class InterpolatedStrings : CompilationTests
    {
        [DataRow(@"$""a{b}c""", @"(let <$_1$> = (b) in $""a{<$_1$>}c"")")]
        [DataRow(@"$""{{a}}""", @"($""{{a}}"")")]
        [DataRow(@"$@""a{b}""", @"(let <$_1$> = (b) in $@""a{<$_1$>}"")")]
        // Several holes each get their own binding, in order.
        [DataRow(@"$""{a} and {b}""", @"(let <$_1$> = (a) in let <$_2$> = (b) in $""{<$_1$>} and {<$_2$>}"")")]
        // Literal braces around a hole survive.
        [DataRow(@"$""{{{x}}}""", @"(let <$_1$> = (x) in $""{{{<$_1$>}}}"")")]
        // Verbatim interpolation with a backslash and two holes.
        [DataRow(@"$@""path {dir}\{file}""", @"(let <$_1$> = (dir) in let <$_2$> = (file) in $@""path {<$_1$>}\{<$_2$>}"")")]
        [TestMethod]
        public void Basic(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow(@"$""{x:d}""", @"(let <$_1$> = (x) in $""%d{<$_1$>}"")")]
        [DataRow(@"$""{x,10}""", @"(let <$_1$> = (x) in $""{<$_1$>,10}"")")]
        [TestMethod]
        public void FormatCharAndAlignment(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow(@"$""{x:0.00}""",
            @"(let <$_1$> = (x) : global.Sona.Runtime.Traits.``trait number``<_> in $""{<$_1$>:``0.00``}"")")]
        // Alignment and a numeric format specifier together, with a negative (left) alignment.
        [DataRow(@"$""{x,-8:F1}""",
            @"(let <$_1$> = (x) : global.Sona.Runtime.Traits.``trait number``<_> in $""{<$_1$>,-8:F1}"")")]
        [TestMethod]
        public void NumericFormat(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }
    }
}
