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
        [TestMethod]
        public void If(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }
    }
}
