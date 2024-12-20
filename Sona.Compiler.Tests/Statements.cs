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

        const string ignore = "global.Microsoft.FSharp.Core.Operators.ignore";
        const string @throw = "``throw()``()";
        const string @default = "global.Microsoft.FSharp.Core.Operators.Unchecked.defaultof<_>";
        const string _1 = "``_ 1``";
        const string _2 = "``_ 2``";

        [DataRow("if a then end", @"if(a)then begin
 ()
end
()")]
        [DataRow("if a then end f(b)", @"if(a)then begin
 ()
end
f(b)
()")]
        [DataRow("if a then f(b) end f(c)", @"if(a)then begin
 f(b)
 ()
end
f(c)
()")]
        [DataRow("if a then f(b) else f(c) end f(d)", @"if(a)then begin
 f(b)
 ()
end
else begin
 f(c)
 ()
end
f(d)
()")]
        [DataRow("if a then f(b) elseif c then f(d) end f(e)", @"if(a)then begin
 f(b)
 ()
end
elif(c)then begin
 f(d)
 ()
end
f(e)
()")]
        [DataRow("if a then f(b) elseif c then f(d) else f(e) end f(f)", @"if(a)then begin
 f(b)
 ()
end
elif(c)then begin
 f(d)
 ()
end
else begin
 f(e)
 ()
end
f(f)
()")]
        [TestMethod]
        public void IfNonReturning(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("if a then throw b end f(c)", $@"if(a)then begin
 (b).{@throw}
end
f(c)
()")]
        [DataRow("if a then throw b else throw c end", $@"if true then begin
 if(a)then begin
  (b).{@throw}
 end
 else begin
  (c).{@throw}
 end
end
else begin
 {ignore} begin
  ()
 end
 {@default}
end")]
        [DataRow("if a then throw b else throw c end f()", $@"if true then begin
 if(a)then begin
  (b).{@throw}
 end
 else begin
  (c).{@throw}
 end
end
else begin
 {ignore} begin
  f()
  ()
 end
 {@default}
end")]
        [DataRow("if a then throw b else throw c end return d", $@"if true then begin
 if(a)then begin
  (b).{@throw}
 end
 else begin
  (c).{@throw}
 end
end
else begin
 {ignore} begin
  (d)
 end
 {@default}
end")]
        [DataRow("if a then return b else throw c end return d", $@"if true then begin
 if(a)then begin
  (b)
 end
 else begin
  (c).{@throw}
 end
end
else begin
 {ignore} begin
  (d)
 end
 {@default}
end")]
        [DataRow("if a then return b else throw c end return d", $@"if true then begin
 if(a)then begin
  (b)
 end
 else begin
  (c).{@throw}
 end
end
else begin
 {ignore} begin
  (d)
 end
 {@default}
end")]
        [TestMethod]
        public void IfThrowing(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("if a then return b else end return c", $@"if(a)then begin
 (b)
end
else begin
 do begin
  ()
 end
 (c)
end")]
        [DataRow("if a then return b else f(c) end return d", $@"if(a)then begin
 (b)
end
else begin
 do begin
  f(c)
  ()
 end
 (d)
end")]
        [DataRow("if a then throw b elseif c then return d else end return e", $@"if(a)then begin
 (b).{@throw}
end
elif(c)then begin
 (d)
end
else begin
 do begin
  ()
 end
 (e)
end")]
        [DataRow("if a then f(b) else return c end", $@"let mutable {_1} = false
let mutable {_2} = {@default}
if(a)then begin
 f(b)
 ()
end
else begin
 {_2} <- (c)
 {_1} <- true
end
if {_1} then {_2}
else begin
 ()
end")]
        [DataRow("if a then f(b) else return c end return d", $@"let mutable {_1} = false
let mutable {_2} = {@default}
if(a)then begin
 f(b)
 ()
end
else begin
 {_2} <- (c)
 {_1} <- true
end
if {_1} then {_2}
else begin
 (d)
end")]
        [DataRow("if a then return b elseif c then f(d) end", $@"let mutable {_1} = false
let mutable {_2} = {@default}
if(a)then begin
 {_2} <- (b)
 {_1} <- true
end
elif(c)then begin
 f(d)
 ()
end
if {_1} then {_2}
else begin
 ()
end")]
        [DataRow("if a then return b elseif c then f(d) else return e end", $@"let mutable {_1} = false
let mutable {_2} = {@default}
if(a)then begin
 {_2} <- (b)
 {_1} <- true
end
elif(c)then begin
 f(d)
 ()
end
else begin
 {_2} <- (e)
 {_1} <- true
end
if {_1} then {_2}
else begin
 ()
end")]
        [TestMethod]
        public void IfReturning(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        
        [DataRow("do f(a) end", $@"if true then begin
 f(a)
 ()
end
()")]
        [DataRow("do return a end", $@"if true then begin
 (a)
end
else begin
 {ignore} begin
  ()
 end
 {@default}
end")]
        [DataRow("do throw a end", $@"if true then begin
 (a).{@throw}
end
else begin
 {ignore} begin
  ()
 end
 {@default}
end")]
        [TestMethod]
        public void Do(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }
    }
}
