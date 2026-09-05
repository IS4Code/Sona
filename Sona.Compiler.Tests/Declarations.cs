using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
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

        const string emptyBody = @"(
 ()
)";

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
        [DataRow("function f() return 0 end", @"let rec f() = (
 (0)
)")]
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
            AssertTopLevelStatementEquivalence(source, expected);
        }

        const string @default = "global.Microsoft.FSharp.Core.Operators.Unchecked.defaultof<_>";

        const string AbstractSealed = "[<global.Microsoft.FSharp.Core.AbstractClassAttribute;global.Microsoft.FSharp.Core.SealedAttribute>]";
        const string enumConstraint = "when '<$_1$> :> global.System.Enum and '<$_1$> : not struct";

        [DataRow("lazy v=a", $@"type {AbstractSealed}``lazy v``<'<$_1$> {enumConstraint}> private() = begin
 static member val v = (a)
 static member ``<Force>``<'x>(x : 'x) : 'x = x
end
let [<global.Microsoft.FSharp.Core.CompiledNameAttribute(""get_v"")>]<$_2$>() = ``lazy v``<global.System.Enum>.v
let [<global.Microsoft.FSharp.Core.CompiledNameAttribute(""<get>v"")>]inline v<'<$_1$> {enumConstraint}> = ``lazy v``<'<$_1$>>.v
open type ``lazy v``<global.System.Enum>")]
        [TestMethod]
        public void Lazy(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow("use v=a", "use v = a")]
        [DataRow("use var v=a", "use mutable v = a")]
        [DataRow("use v=a, w=b", null)]
        [TestMethod]
        public void Use(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        const string literalAttr = "[<global.Microsoft.FSharp.Core.LiteralAttribute>]";

        [DataRow("package P const v=1 end", $@"module P = begin
 {literalAttr}
 let v = 1
 ()
end")]
        [DataRow("const v=1", $@"{literalAttr}
let v = 1")]
        [DataRow("package P const v=follow a end", null)]
        [TestMethod]
        public void Const(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow("function test() const v=1 end", null)]
        [TestMethod]
        public void ConstInsideFunction(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow("let v=a, w=b", "let (v,w) = (a,b)")]
        [DataRow("let (x, y) = a", "let ((struct(x,y))) = a")]
        [TestMethod]
        public void MultipleAndPatternDeclarations(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("inline function f(a) return a end", $@"let inline f(a) = (
 (a)
)")]
        [TestMethod]
        public void InlineFunction(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        const string voption = "global.Microsoft.FSharp.Core.voption";
        const string ValueSome = "global.Microsoft.FSharp.Core.ValueSome";
        const string ValueNone = "global.Microsoft.FSharp.Core.ValueNone";

        [DataRow("function f?() if a then return 1 end return end", $@"let rec f() : _ {voption} = (
 let mutable <$returning$> = false
 let mutable <$result$> = {@default}
 if(a)then begin
  <$result$> <- {ValueSome}(1);<$returning$> <- true
 end
 if <$returning$> then <$result$>
 else begin
  {ValueNone}
 end
)")]
        [TestMethod]
        public void OptionalFunction(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("case function C(x) return x end", $@"let rec (|C|)(x) = (
 (x)
)")]
        [DataRow("case function (A or B)(x) if x then return A end return B end", $@"let rec (|A|B|)(x) = (
 let mutable <$returning$> = false
 let mutable <$result$> = {@default}
 if(x)then begin
  <$result$> <- (A);<$returning$> <- true
 end
 if <$returning$> then <$result$>
 else begin
  (B)
 end
)")]
        [DataRow("case function C?(x) if x then return x end end", $@"let rec (|C|_|)(x) = (
 let mutable <$returning$> = false
 let mutable <$result$> = {@default}
 if(x)then begin
  <$result$> <- {ValueSome}(x);<$returning$> <- true
 end
 if <$returning$> then <$result$>
 else begin
  {ValueNone}
 end
)")]
        [TestMethod]
        public void CaseFunction(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("let f = case C", "let f = (|C|)")]
        [TestMethod]
        public void CaseFunctionReference(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("package P let v=1 end", @"module P = begin
 let v = 1
 ()
end")]
        [DataRow("package P end", @"module P = begin
 ()
end")]
        [TestMethod]
        public void Package(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }
    }
}
