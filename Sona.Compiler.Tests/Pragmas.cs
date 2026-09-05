using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    [TestClass]
    public class Pragmas : CompilationTests
    {
        [DataRow(@"#pragma tuple class
function test() return (a, b) end", $@"let rec test() = (
 ((a,b))
)")]
        [DataRow("function test() return (a, b) end", $@"let rec test() = (
 ((struct(a,b)))
)")]
        [TestMethod]
        public void TuplePragma(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow(@"#pragma record struct
function test() return {as new; x = a} end", @"let rec test() = (
 ((struct{| x = a |}))
)")]
        [TestMethod]
        public void RecordPragma(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow(@"#pragma option class
function test() return some a end", @"let rec test() = (
 ((global.Microsoft.FSharp.Core.Some(a)))
)")]
        [TestMethod]
        public void OptionPragma(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow(@"#pragma collection list
function test() return [a, b] end", @"let rec test() = (
 ([
  yield a
  yield b
  ()
  ])
)")]
        [TestMethod]
        public void CollectionPragma(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        const string printfn = "global.Microsoft.FSharp.Core.ExtraTopLevelOperators.printfn";
        const string eprintfn = "global.Microsoft.FSharp.Core.ExtraTopLevelOperators.eprintfn";

        [DataRow(@"#pragma echo eprintfn
echo ""x""", $"do {eprintfn}(\"x\")")]
        [DataRow(@"#pragma push echo eprintfn
echo ""x""
#pragma pop echo
echo ""y""",
            $@"do {eprintfn}(""x"")
do {printfn}(""y"")")]
        [TestMethod]
        public void EchoPragma(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }
    }
}
