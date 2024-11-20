using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IS4.Sona.Tests
{
    [TestClass]
    public class Statements : CompilationTests
    {
        [DataRow("import a", "open a")]
        [DataRow("import a.n", "open a.n")]
        [DataRow("import(a.n)", "open a.n")]
        [DataRow("import((a.n))", "open a.n")]
        [DataRow("import a . n", "open a.n")]
        [DataRow("import let", null)]
        [DataRow("import @let", "open ``let``")]
        [DataRow("import a.let", "open a.``let``")]
        [DataRow("import a.*", "open type a")]
        [DataRow("import a.n.*", "open type a.n")]
        [DataRow("import(a.*)", "open type a")]
        [DataRow("import((a.*))", "open type a")]
        [DataRow("import a . *", "open type a")]
        [DataRow("import", null)]
        [DataRow("import 0", null)]
        [DataRow(@"import ""a/b.c""", @"#load ""a/b.c""
open B")]
        [DataRow(@"import(""a/b.c"")", @"#load ""a/b.c""
open B")]
        [DataRow(@"import((""a/b.c""))", @"#load ""a/b.c""
open B")]
        [DataRow(@"import @""a/b.c""", @"#load @""a/b.c""
open B")]
        [DataRow(@"include ""a""", @"#load ""a""")]
        [DataRow(@"include(""a"")", @"#load ""a""")]
        [DataRow(@"include((""a""))", @"#load ""a""")]
        [DataRow(@"include @""a""", @"#load @""a""")]
        [DataRow("include", null)]
        [DataRow("include a", null)]
        [DataRow("include 0", null)]
        [DataRow(@"require ""a""", @"#r ""a""")]
        [DataRow(@"require(""a"")", @"#r ""a""")]
        [DataRow(@"require((""a""))", @"#r ""a""")]
        [DataRow(@"require @""a""", @"#r @""a""")]
        [DataRow("require", null)]
        [DataRow("require a", null)]
        [DataRow("require 0", null)]
        [TestMethod]
        public void TopLevel(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }
    }
}
