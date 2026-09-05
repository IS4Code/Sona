using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    [TestClass]
    public class Patterns : CompilationTests
    {
        static string Switch(string pattern) => $"switch v case {pattern} do f(a) else f(b) end";

        const string matchHeader = "match(v)with";
        const string matchFooter = @"| _ -> begin
 f(b)
 ()
end
()";

        static string Expect(string casePattern) => $@"{matchHeader}
| ({casePattern}) -> begin
 f(a)
 ()
end
{matchFooter}";

        [DataRow("1")]
        [TestMethod]
        public void ConstantPattern(string pattern)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(pattern));
        }

        [DataRow("x as int", "x : global.Microsoft.FSharp.Core.int")]
        [TestMethod]
        public void VariablePattern(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow("C(x)", "C(x)")]
        [TestMethod]
        public void NamedPattern(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow("(1 or 2)", "(1 | 2)")]
        [TestMethod]
        public void LogicPattern(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow("is<int> x", "(:? global.Microsoft.FSharp.Core.int as(x))")]
        [TestMethod]
        public void TypeTestingPattern(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow("(x, y)", "(struct(x,y))")]
        [TestMethod]
        public void ConstructionPatternTuple(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow("[, x]", "[|_;x|]")]
        [TestMethod]
        public void ConstructionPatternArray(string pattern, string expectedInner)
        {
            var expected = $@"{matchHeader}
| ({expectedInner}) -> begin
 f(a)
 ()
end
{matchFooter}";
            AssertTopLevelBlockEquivalence(Switch(pattern), expected);
        }

        [DataRow("with { Length = l }", "(``<Sona>``.``Get Length``(l))")]
        [TestMethod]
        public void MemberTestingPattern(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow("> 0", "global.Sona.Runtime.CompilerServices.Patterns.GreaterThan(0)")]
        [TestMethod]
        public void RelationalPattern(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow(@"let /abc/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex YWJj``())([|  |])) = v")]
        // Inline option override after the closing slash is prepended as (?i).
        [DataRow(@"let /abc/i = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KD9pKWFiYw``())([|  |])) = v")]
        // A bare group captures nothing under the always-on explicit-capture option.
        [DataRow(@"let /(\d+)/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KFxkKyk``())([|  |])) = v")]
        [TestMethod]
        public void RegexPattern(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow(@"let /a(?{x}\d+)b/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex YSg_PDE-XGQrKWI``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x) |])) = v")]
        // Two capture groups are numbered (?<1>, (?<2> in source order and played back as two array elements.
        [DataRow(@"let /a(?{x}\d+)(?{y}[a-z]+)b/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex YSg_PDE-XGQrKSg_PDI-W2Etel0rKWI``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x);global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(y) |])) = v")]
        // Option overrides still work with a capture present.
        [DataRow(@"let /(?{x}\d+)/i = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KD9pKSg_PDE-XGQrKQ``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x) |])) = v")]
        // The capture sub-pattern carries its own type annotation through to the playback.
        [DataRow(@"let /(?{x as string}\d+)/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KD88MT5cZCsp``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x : global.Microsoft.FSharp.Core.string) |])) = v")]
        // An option sub-pattern round-trips as a ValueSome playback.
        [DataRow(@"let /(?{some x}\d+)?/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KD88MT5cZCspPw``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(global.Microsoft.FSharp.Core.ValueSome(x)) |])) = v")]
        // A back-reference to a captured group is allowed.
        [DataRow(@"let /(?{x}\d+)\k<1>/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KD88MT5cZCspXGs8MT4``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x) |])) = v")]
        // Writing an extra (?<1> capture for an already-declared group is permitted and adds no playback.
        [DataRow(@"let /(?{x}\d+)(?<1>[a-z]+)/ = v", @"let (global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex KD88MT5cZCspKD88MT5bYS16XSsp``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x) |])) = v")]
        [TestMethod]
        public void RegexPatternWithCaptureGroup(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow(@"/a(?{x}\d+)/", @"global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex YSg_PDE-XGQrKQ``())([| global.Sona.Runtime.CompilerServices.Patterns.UnpackRegexGroup(x) |])")]
        [TestMethod]
        public void RegexPatternInSwitch(string pattern, string expectedInner)
        {
            AssertTopLevelBlockEquivalence(Switch(pattern), Expect(expectedInner));
        }

        [DataRow(@"let (/a/, /b/) = pair", @"let ((struct(global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex YQ``())([|  |]),global.Sona.Runtime.CompilerServices.Patterns.MatchRegex(``<Sona>``.``Regex Yg``())([|  |])))) = pair")]
        [TestMethod]
        public void RegexPatternInTuple(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        // Every one of these is rejected before any F# is produced.
        [DataRow(@"let /(?<1>\d+)/ = v")]        // capturing group not introduced by (?{
        [DataRow(@"let /(?<name>\d+)/ = v")]     // non-numeric capture name
        [DataRow(@"let /abc/z = v")]             // unrecognised inline option
        [DataRow(@"let /a[b/ = v")]              // invalid regex - unterminated set
        [DataRow(@"let /a(b/ = v")]              // invalid regex - unbalanced parenthesis
        [DataRow(@"let /a#b/ = v")]              // '#' must be escaped
        [DataRow(@"let /(?{_}\d+)/ = v")]        // '_' is not a usable sub-pattern binding
        [DataRow("let /a\nb/ = v")]              // a pattern may not span lines
        [TestMethod]
        public void RegexPatternInvalid(string source)
        {
            AssertStatementEquivalence(source, null);
        }
    }
}
