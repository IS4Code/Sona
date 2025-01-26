﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IS4.Sona.Tests
{
    [TestClass]
    public class Expressions : CompilationTests
    {
        [DataRow("0", "0")]
        [DataRow("-0", " - 0")]
        [DataRow("+0", " + 0")]
        [DataRow("9999999999999999999999999999999999", "9999999999999999999999999999999999")]
        [DataRow("0.1", "0.1")]
        [DataRow("0.", null)]
        [DataRow(".1", null)]
        [DataRow("1e", null)]
        [DataRow("1e10", "1e10")]
        [DataRow("1.1e10", "1.1e10")]
        [DataRow("1E10", "1E10")]
        [DataRow("1E+10", "1E+10")]
        [DataRow("1E-10", "1E-10")]
        [DataRow("1e10.1", null)]
        [DataRow("0x", null)]
        [DataRow("12F", null)]
        [DataRow("0x12F", "0x12F")]
        [DataRow("0x12G", null)]
        [DataRow("true", "true")]
        [DataRow("false", "false")]
        [DataRow("null", "null")]
        [DataRow(@"""abc""", @"""abc""")]
        [DataRow(@"""a\""bc""", @"""a\""bc""")]
        [DataRow(@"""\*""", @"""\*""")]
        [DataRow(@"""abc", null)]
        [DataRow("\"a\rc\"", null)]
        [DataRow("\"a\nc\"", null)]
        [DataRow("'abc'", "'abc'")]
        [DataRow("'abc", null)]
        [DataRow("'a\rc'", null)]
        [DataRow("'a\nc'", null)]
        [DataRow(@"'\*'", @"'\*'")]
        [DataRow(@"@""abc""", @"@""abc""")]
        [DataRow(@"@""a""""bc""", @"@""a""""bc""")]
        [DataRow(@"@""a\bc""", @"@""a\bc""")]
        [DataRow(@"@""a\""bc""", null)]
        [DataRow("@\"a\rc\"", "@\"a\rc\"")]
        [DataRow("@\"a\nc\"", "@\"a\nc\"")]
        [TestMethod]
        public void Primitive(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("a and b", "a && b")]
        [DataRow("a and b and c", "a && b && c")]
        [DataRow("a or b", "a || b")]
        [DataRow("a or b or c", "a || b || c")]
        [DataRow("not a", $"{not}(a)")]
        [DataRow("not not a", $"{not}({not}(a))")]
        [DataRow("not a and b", null)]
        [DataRow("not a or b", null)]
        [DataRow("not a and not b", null)]
        [DataRow("not a or not b", null)]
        [DataRow("a and not b", $"a && {not}(b)")]
        [DataRow("a or not b", $"a || {not}(b)")]
        [DataRow("a and not b and c", null)]
        [DataRow("a or not b or c", null)]
        [DataRow("a and not not b", $"a && {not}({not}(b))")]
        [DataRow("a or not not b", $"a || {not}({not}(b))")]
        [DataRow("a and b or c", null)]
        [DataRow("a or b and c", null)]
        [DataRow("a and b || c", "a && (b || c)")]
        [DataRow("a && b or c", "(a && b) || c")]
        [DataRow("a or b && c", "a || (b && c)")]
        [DataRow("a || b and c", "(a || b) && c")]
        [DataRow("not a && b", $"{not}(a && b)")]
        [DataRow("not a || b", $"{not}(a || b)")]
        [DataRow("a1 and b1 || b2 && b3 || b4 and c1", "a1 && (b1 || b2 && b3 || b4) && c1")]
        [TestMethod]
        public void LogicOperators(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("a and b < c", "a && (b < c)")]
        [DataRow("a or b < c", "a || (b < c)")]
        [DataRow("a && b < c", "a && b < c")]
        [DataRow("a || b < c", "a || b < c")]
        [DataRow("a < b <= c > d >= e == f != g ~= h", "a < b <= c > d >= e = f <> g <> h")]
        [DataRow("a ~= b != c == d >= e > f <= g < h", "a <> b <> c = d >= e > f <= g < h")]
        [TestMethod]
        public void RelationalOperators(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        const string and = ".``&``";
        const string or = ".``|``";
        const string xor = ".``^``";
        const string shl = ".``<<``";
        const string shr = ".``>>``";

        [DataRow("a and b + c", "a && b + c")]
        [DataRow("a or b + c", "a || b + c")]
        [DataRow("a..b", $"(a){cat}(b)")]
        [DataRow("a..b..c", $"(a){cat}(b){cat}(c)")]
        [DataRow("a .. b ==1..2", $"(a){cat}(b) = (1){cat}(2)")]
        [DataRow("a&b", $"(a){and}(b)")]
        [DataRow("a^b", $"(a){xor}(b)")]
        [DataRow("a|b", $"(a){or}(b)")]
        [DataRow("a>>b", $"(a){shr}(b)")]
        [DataRow("a<<b", $"(a){shl}(b)")]
        [DataRow("a&b&c", $"(a){and}(b){and}(c)")]
        [DataRow("a^b^c", $"(a){xor}(b){xor}(c)")]
        [DataRow("a|b|c", $"(a){or}(b){or}(c)")]
        [DataRow("a>>b<<c", $"(a){shr}(b){shl}(c)")]
        [DataRow("a<<b>>c", $"(a){shl}(b){shr}(c)")]
        [DataRow("a&b^c|d..e", $"((((a){and}(b)){xor}(c)){or}(d)){cat}(e)")]
        [DataRow("a..b|c^d&e", $"(a){cat}((b){or}((c){xor}((d){and}(e))))")]
        [DataRow("a<<b&c", $"((a){shl}(b)){and}(c)")]
        [DataRow("a<<b+c", $"(a){shl}(b + c)")]
        [DataRow("a+b-c*d/e%f", "a + b - c * d / e % f")]
        [DataRow("1+1", "1 + 1")]
        [DataRow("1-1", "1 - 1")]
        [TestMethod]
        public void BinaryOperators(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        const string hash = ".``#``()";

        [DataRow("+a", " + a")]
        [DataRow("-a", " - a")]
        [DataRow("!a", $"{not}(a)")]
        [DataRow("#a", $"(a){hash}")]
        [DataRow("++a", " +  + a")]
        [DataRow("--a", " -  - a")]
        [DataRow("!!a", $"{not}({not}(a))")]
        [DataRow("##a", $"((a){hash}){hash}")]
        [DataRow("+-!#a", $" +  - {not}((a){hash})")]
        [DataRow("+a+1", " + a + 1")]
        [DataRow("-a+1", " - a + 1")]
        [DataRow("!a+1", $"{not}(a) + 1")]
        [DataRow("#a+1", $"(a){hash} + 1")]
        [TestMethod]
        public void UnaryOperators(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("a.b", "a.b")]
        [DataRow("a[b]", "a[b]")]
        [DataRow("a[b,]", null)]
        [DataRow("a[]", null)]
        [DataRow("a[b, c]", "a[b,c]")]
        [DataRow("a[b].c", "a[b].c")]
        [DataRow("()[a]", null)]
        [DataRow("(a).b", "(a).b")]
        [DataRow("a:b", "a?b")]
        [DataRow("f(a).b", "f(a).b")]
        [DataRow("f(a)[b, c]", "f(a)[b,c]")]
        [DataRow("a.f(b).c", "a.f(b).c")]
        [DataRow("!(a).f(b)[c, d].e", $"{not}((a).f(b)[c,d].e)")]
        [TestMethod]
        public void MemberAccess(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        const string seq = "global.Microsoft.FSharp.Core.Operators.seq";
        const string Seq = "global.Microsoft.FSharp.Collections.Seq";
        const string KeyValuePair = "global.System.Collections.Generic.KeyValuePair<_,_>";

        [DataRow("f(a)", "f(a)")]
        [DataRow("f(a, b)", "f(a,b)")]
        [DataRow("f(a,)", null)]
        [DataRow("f(a; b)", "f(a)(b)")]
        [DataRow("f(a;)", "f(a)()")]
        [DataRow("f(;a)", "f()(a)")]
        [DataRow("f(;a;)", "f()(a)()")]
        [DataRow(@"f""x""", @"f(""x"")")]
        [DataRow("f{a=1}", "f({ a = 1 })")]
        [DataRow("f{a}", $"f({seq}{{ a }})")]
        [DataRow("f{}", $"f({Seq}.empty)")]
        [TestMethod]
        public void FunctionCall(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("[]", "[|  |]")]
        [DataRow("[a]", "[| a |]")]
        [DataRow("[a, b]", "[| a;b |]")]
        [DataRow("[a, b,]", null)]
        [DataRow("[a][0]", "[| a |][0]")]
        [DataRow("[a].b", "[| a |].b")]
        [DataRow("[a](b)", "[| a |](b)")]
        [TestMethod]
        public void Array(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("{a=1}", "{ a = 1 }")]
        [DataRow("{a=1, b=2}", "{ a = 1;b = 2 }")]
        [DataRow("{a=1,}", null)]
        [DataRow("{a=1}[0]", "{ a = 1 }[0]")]
        [DataRow("{a=1}.b", "{ a = 1 }.b")]
        [DataRow("{a=1}(b)", "{ a = 1 }(b)")]
        [TestMethod]
        public void Record(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("{}", $"({Seq}.empty)")]
        [DataRow("{a}", $"({seq}{{ a }})")]
        [DataRow("{a, b}", $"({seq}{{ a;b }})")]
        [DataRow("{a, b,}", null)]
        [DataRow("{a}[0]", $"({seq}{{ a }})[0]")]
        [DataRow("{a}.b", $"({seq}{{ a }}).b")]
        [DataRow("{a}(b)", $"({seq}{{ a }})(b)")]
        [DataRow("{[a]=b}", $"({seq}{{ {KeyValuePair}((a),b) }})")]
        [DataRow("{[a]=b,[c]=d}", $"({seq}{{ {KeyValuePair}((a),b);{KeyValuePair}((c),d) }})")]
        [DataRow("{[a,b]=c}", $"({seq}{{ {KeyValuePair}((a,b),c) }})")]
        [DataRow("{[a]=b,c}", $"({seq}{{ {KeyValuePair}((a),b);c }})")]
        [TestMethod]
        public void Sequences(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }
    }
}
