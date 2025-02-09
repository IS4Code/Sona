using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IS4.Sona.Tests
{
    [TestClass]
    public class Declarations : CompilationTests
    {
        [DataRow("let v=0", "let v = 0")]
        [DataRow("var v=0", "let mutable v = 0")]
        [DataRow("let v", null)]
        [DataRow("let val=0", "let ``val`` = 0")]
        [DataRow("let @var=0", "let var = 0")]
        [DataRow("let @function=0", "let ``function`` = 0")]
        [TestMethod]
        public void Variables(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        const string emptyBody = @"begin
 ()
end";

        [DataRow("function f() end", $"let rec f() = {emptyBody}")]
        [DataRow("function val() end", $"let rec ``val``() = {emptyBody}")]
        [DataRow("function @var() end", $"let rec var() = {emptyBody}")]
        [DataRow("function f end", null)]
        [DataRow("function f()", null)]
        [DataRow("function f() end", $"let rec f() = {emptyBody}")]
        [DataRow("function f() end function g() end", $@"let rec f() = {emptyBody}
and g() = {emptyBody}")]
        [DataRow("function f() end function g() end function h() end", $@"let rec f() = {emptyBody}
and g() = {emptyBody}
and h() = {emptyBody}")]
        [DataRow("function f() end; function g() end", $@"let rec f() = {emptyBody}
let rec g() = {emptyBody}")]
        [DataRow("function f(a) end", $"let rec f(a) = {emptyBody}")]
        [DataRow("function f(a,b) end", $"let rec f(a,b) = {emptyBody}")]
        [DataRow("function f(a,) end", null)]
        [DataRow("function f(a;b) end", $"let rec f(a)(b) = {emptyBody}")]
        [DataRow("function f(a;) end", $"let rec f(a)() = {emptyBody}")]
        [DataRow("function f() return end", $"let rec f() = {emptyBody}")]
        [DataRow("function f() return 0 end", @"let rec f() = begin
 (0)
end")]
        [TestMethod]
        public void Functions(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        const string funcSource = "function f() end";
        const string funcExpected = $"f() = {emptyBody}";

        [DataRow("#:X#let v=0", @"[<X()>]
let v = 0")]
        [DataRow("let #:X#v=0", "let [<X()>]v = 0")]
        [DataRow($"#:X#{funcSource}", $"let rec [<X()>]{funcExpected}")]
        [DataRow($"#:X   #{funcSource}", $"let rec [<X()>]{funcExpected}")]
        [DataRow($"#: X:#{funcSource}", $"let rec [<X()>]{funcExpected}")]
        [DataRow($@"#:X
{funcSource}", $"let rec [<X()>]{funcExpected}")]
        [DataRow($"#method X#{funcSource}", $"let rec [<method:X()>]{funcExpected}")]
        [DataRow($"#method:X#{funcSource}", $"let rec [<method:X()>]{funcExpected}")]
        [DataRow($@"#method X
{funcSource}", $"let rec [<method:X()>]{funcExpected}")]
        [DataRow("#method X", null)]
        [DataRow($"#item X#{funcSource}", $"let rec [<X()>]{funcExpected}")]
        [DataRow($"#param X#{funcSource}", $"let rec [<param:X()>]{funcExpected}")]
        [DataRow($"#:X 1#{funcSource}", $"let rec [<X(1)>]{funcExpected}")]
        [DataRow($"#:X(1)#{funcSource}", $"let rec [<X((1))>]{funcExpected}")]
        [DataRow($"#:X 1 2#{funcSource}", $"let rec [<X(1,2)>]{funcExpected}")]
        [DataRow($"#:X 1,2#{funcSource}", null)]
        [DataRow($"#:X (1)2#{funcSource}", null)]
        [DataRow($"#:X 1+2#{funcSource}", null)]
        [DataRow($"#:X 1 + 2#{funcSource}", null)]
        [DataRow($"#:X 1 x#{funcSource}", $"let rec [<X(1,x)>]{funcExpected}")]
        [DataRow($"#:X 1 (x)#{funcSource}", $"let rec [<X(1,(x))>]{funcExpected}")]
        [DataRow($"#:X 1 x=2#{funcSource}", $"let rec [<X(1,x = 2)>]{funcExpected}")]
        [DataRow($"#:X 1 x = 2#{funcSource}", $"let rec [<X(1,x = 2)>]{funcExpected}")]
        [DataRow($"#:X 1 (x = 2)#{funcSource}", $"let rec [<X(1,(x){set}(2))>]{funcExpected}")]
        [DataRow($@"#:X (1
) 2#{funcSource}", $"let rec [<X((1),2)>]{funcExpected}")]
        [DataRow($@"#:X (1
)
{funcSource}", $"let rec [<X((1))>]{funcExpected}")]
        [DataRow($@"#:X (1
) {funcSource}", null)]
        [DataRow($"#:X,Y#{funcSource}", $"let rec [<X();Y()>]{funcExpected}")]
        [DataRow($@"#:X,
Y#{funcSource}", $"let rec [<X();Y()>]{funcExpected}")]
        [DataRow($"#:X 1,Y 2#{funcSource}", $"let rec [<X(1);Y(2)>]{funcExpected}")]
        [DataRow("function f(#:X#a) end", $"let rec f([<X()>]a) = {emptyBody}")]
        [DataRow("function f(#:X:#a) end", $"let rec f([<X()>]a) = {emptyBody}")]
        [DataRow("function f(#param X#a) end", $"let rec f([<param:X()>]a) = {emptyBody}")]
        [DataRow(@"function f(#param X
a) end", $"let rec f([<param:X()>]a) = {emptyBody}")]
        [DataRow("#assembly X", $"[<assembly:X()>]do()")]
        [DataRow("#module X", $"[<``module``:X()>]do()")]
        [DataRow("#entry X", $"[<method:X()>]do()")]
        [DataRow("#program X", $"[<X()>]do()")]
        [TestMethod]
        public void Attributes(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }
    }
}
