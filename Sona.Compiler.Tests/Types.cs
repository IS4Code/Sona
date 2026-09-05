using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    [TestClass]
    public class Types : CompilationTests
    {
        [DataRow("narrow a", "(downcast(a))")]
        [DataRow("widen a", "(upcast(a))")]
        [DataRow("enum a", "(global.Sona.Runtime.CompilerServices.Operators.ConvertEnum(a))")]
        [DataRow("unit<> a", "(global.Sona.Runtime.CompilerServices.Operators.ConvertUnit<_,_,_>(a))")]
        [DataRow("implicit a", "(global.Sona.Runtime.CompilerServices.Operators.Implicit(a))")]
        [DataRow("explicit a", "(global.Sona.Runtime.CompilerServices.Operators.Explicit(a))")]
        [TestMethod]
        public void Conversions(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("int(a)", "(global.Microsoft.FSharp.Core.Operators.int(a))")]
        [DataRow("int? a", "(global.Microsoft.FSharp.Core.Operators.int |> global.Sona.Runtime.CompilerServices.Operators.TryConversionValue(a))")]
        [TestMethod]
        public void NamedTypeConversions(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("new<T>()", "(new T())")]
        [DataRow("new<T>(a, b)", "(new T(a,b))")]
        [DataRow("T(a, b)", "T(a,b)")]
        [DataRow("T()", "T()")]
        [DataRow("T(x = a)", "T(x = a)")]
        [TestMethod]
        public void Constructions(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        const string ValueSome = "global.Microsoft.FSharp.Core.ValueSome";
        const string ValueNone = "global.Microsoft.FSharp.Core.ValueNone";

        [DataRow("some a", $"({ValueSome}(a))")]
        [DataRow("none", ValueNone)]
        [TestMethod]
        public void Options(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("(a, b)", "(struct(a,b))")]
        [DataRow("(as new; a, b)", "(struct(a,b))")]
        [DataRow("(as class; a, b)", "(a,b)")]
        [TestMethod]
        public void Tuples(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("{as new; x = a, y = b}", "{| x = a;y = b |}")]
        [DataRow("{as class; x = a}", "{| x = a |}")]
        [TestMethod]
        public void AnonymousRecordConstructions(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        [DataRow("a ?? b", "(let <$_1$> = global.Microsoft.FSharp.Core.Operators.Unchecked.defaultof<_> in match global.Sona.Runtime.CompilerServices.Operators.BindToLiftedResult(<$_1$>)(a)with|struct(true,<$_2$>)-><$_2$>|struct(false,_)->(if false then <$_1$> else(b)))")]
        [TestMethod]
        public void Coalesce(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }

        const string BindToResult = "global.Sona.Runtime.CompilerServices.Operators.BindToResult";

        [DataRow("a?.b",
            $"(a |> (fun <$_1$> -> match {BindToResult}(<$_1$>)with|struct(false,_)->{ValueNone}|struct(true,<$_2$>)->{ValueSome}(<$_2$> |> (fun <$_3$> -> <$_3$>.b))))")]
        [DataRow("a?.b.c",
            $"(a |> (fun <$_1$> -> match {BindToResult}(<$_1$>)with|struct(false,_)->{ValueNone}|struct(true,<$_2$>)->{ValueSome}(<$_2$> |> (fun <$_3$> -> <$_3$>.b.c))))")]
        [TestMethod]
        public void ConditionalMemberAccess(string source, string? expected)
        {
            AssertExpressionEquivalence(source, expected);
        }
    }
}
