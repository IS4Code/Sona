using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    [TestClass]
    public class Statements : CompilationTests
    {
        new const string not = "global.Microsoft.FSharp.Core.Operators.``not``";

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
        [DataRow(@"import @""a/b.c""", @"#load ""a/b.c""
open B")]
        [DataRow(@"include ""a""", @"#load ""a""")]
        [DataRow(@"include(""a"")", @"#load ""a""")]
        [DataRow(@"include((""a""))", @"#load ""a""")]
        [DataRow(@"include @""a""", @"#load ""a""")]
        [DataRow(@"include @""\""", @"#load ""\\""")]
        [DataRow("include", null)]
        [DataRow("include a", null)]
        [DataRow("include 0", null)]
        [DataRow(@"require ""a""", @"#r ""a""")]
        [DataRow(@"require(""a"")", @"#r ""a""")]
        [DataRow(@"require((""a""))", @"#r ""a""")]
        [DataRow(@"require @""a""", @"#r ""a""")]
        [DataRow(@"require @""\""", @"#r ""\\""")]
        [DataRow("require", null)]
        [DataRow("require a", null)]
        [DataRow("require 0", null)]
        [TestMethod]
        public void TopLevel(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow("a[] = b", null)]
        [DataRow(@""""" = b", null)]
        [DataRow("1 = b", null)]
        [DataRow("a = b, c", null)]
        [TestMethod]
        public void Assignment(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("a = b", "do a <- b")]
        [DataRow("a() = b", "do a() <- b")]
        [DataRow("1() = b", "do (1)() <- b")]
        [DataRow("1.a = c", "do (1).a <- c")]
        [DataRow("a.b = c", "do a.b <- c")]
        [TestMethod]
        public void TopLevelAssignment(string source, string? expected)
        {
            AssertTopLevelStatementEquivalence(source, expected);
        }

        [DataRow("a = b", "a <- b")]
        [DataRow("a() = b", "a() <- b")]
        [DataRow("1() = b", "(1)() <- b")]
        [DataRow("1.a = c", "(1).a <- c")]
        [DataRow("a.b = c", "a.b <- c")]
        [TestMethod]
        public void InnerAssignment(string source, string? expected)
        {
            AssertInnerStatementEquivalence(source, expected);
        }

        const string ignore_begin = "global.Microsoft.FSharp.Core.Operators.ignore (";
        const string ignore_end   = "                                              )";
        const string @throw = " |> global.Sona.Runtime.CompilerServices.Operators.Throw";
        const string @default = "global.Microsoft.FSharp.Core.Operators.Unchecked.defaultof<_>";

        [DataRow("if a then end", @"if(a)then begin
 ()
end
()")]
        [DataRow("if a then end f(b)", @"if(a)then begin
 ()
end
<?do?>f(b)
()")]
        [DataRow("if a then f(b) end f(c)", @"if(a)then begin
 f(b)
 ()
end
<?do?>f(c)
()")]
        [DataRow("if a then f(b) else f(c) end f(d)", @"if(a)then begin
 f(b)
 ()
end
else begin
 f(c)
 ()
end
<?do?>f(d)
()")]
        [DataRow("if a then f(b) elseif c then f(d) end f(e)", @"if(a)then begin
 f(b)
 ()
end
elif(c)then begin
 f(d)
 ()
end
<?do?>f(e)
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
<?do?>f(f)
()")]
        [TestMethod]
        public void IfNonReturning(string source, string? expected)
        {
            AssertTopLevelBlockEquivalence(source, expected);
        }

        [DataRow("if a then throw b end f(c)", $@"if(a)then begin
 (b){@throw}
end
<?do?>f(c)
()")]
        [DataRow("if a then throw b else throw c end", $@"if true then begin
 if(a)then begin
  (b){@throw}
 end
 else begin
  (c){@throw}
 end
end
else <?do?>{@default}")]
        [DataRow("if a then throw b else throw c end f()", $@"if true then begin
 if(a)then begin
  (b){@throw}
 end
 else begin
  (c){@throw}
 end
end
else begin
 {ignore_begin}
  f()
  ()
 {ignore_end}
 <?do?>{@default}
end")]
        [TestMethod]
        public void IfThrowing(string source, string? expected)
        {
            AssertTopLevelBlockEquivalence(source, expected);
        }
        
        [DataRow("if a then throw b else throw c end return d", $@"if true then begin
 if(a)then begin
  (b){@throw}
 end
 else begin
  (c){@throw}
 end
end
else begin
 {ignore_begin}
  (d)
 {ignore_end}
 {@default}
end")]
        [DataRow("if a then return b else throw c end return d", $@"if true then begin
 if(a)then begin
  (b)
 end
 else begin
  (c){@throw}
 end
end
else begin
 {ignore_begin}
  (d)
 {ignore_end}
 {@default}
end")]
        [DataRow("if a then return b else throw c end return d", $@"if true then begin
 if(a)then begin
  (b)
 end
 else begin
  (c){@throw}
 end
end
else begin
 {ignore_begin}
  (d)
 {ignore_end}
 {@default}
end")]
        [TestMethod]
        public void IfReturningThrowing(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("if a then return b else end return c", $@"if(a)then begin
 (b)
end
else begin
 if true then begin
  ()
 end
 (c)
end")]
        [DataRow("if a then return b else f(c) end return d", $@"if(a)then begin
 (b)
end
else begin
 if true then begin
  f(c)
  ()
 end
 (d)
end")]
        [DataRow("if a then throw b elseif c then return d else end return e", $@"if(a)then begin
 (b){@throw}
end
elif(c)then begin
 (d)
end
else begin
 if true then begin
  ()
 end
 (e)
end")]
        [DataRow("if a then f(b) else return c end", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if(a)then begin
 f(b)
 ()
end
else begin
 <$result$> <- (c);<$returning$> <- true
end
if <$returning$> then <$result$>
else begin
 ()
end")]
        [DataRow("if a then f(b) else return c end return d", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if(a)then begin
 f(b)
 ()
end
else begin
 <$result$> <- (c);<$returning$> <- true
end
if <$returning$> then <$result$>
else begin
 (d)
end")]
        [DataRow("if a then return b elseif c then f(d) end", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if(a)then begin
 <$result$> <- (b);<$returning$> <- true
end
elif(c)then begin
 f(d)
 ()
end
if <$returning$> then <$result$>
else begin
 ()
end")]
        [DataRow("if a then return b elseif c then f(d) else return e end", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if(a)then begin
 <$result$> <- (b);<$returning$> <- true
end
elif(c)then begin
 f(d)
 ()
end
else begin
 <$result$> <- (e);<$returning$> <- true
end
if <$returning$> then <$result$>
else begin
 ()
end")]
        [DataRow("if x then if y then f(z) return a end g(z) end h(z) return b", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if(x)then begin
 if(y)then begin
  f(z)
  <$result$> <- (a);<$returning$> <- true
 end
 if <$returning$> then ()
 else begin
  g(z)
  ()
 end
end
if <$returning$> then <$result$>
else begin
 h(z)
 (b)
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
else {@default}")]
        [DataRow("do return a end f(b)", $@"if true then begin
 (a)
end
else begin
 {ignore_begin}
  f(b)
  ()
 {ignore_end}
 {@default}
end")]
        [DataRow("do throw a end", $@"if true then begin
 (a){@throw}
end
else {@default}")]
        [DataRow("do throw a end f(b)", $@"if true then begin
 (a){@throw}
end
else begin
 {ignore_begin}
  f(b)
  ()
 {ignore_end}
 {@default}
end")]
        [DataRow("do if x then f(y) else g(y) return a end h(y) end i(y)", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if true then begin
 if(x)then begin
  f(y)
  ()
 end
 else begin
  g(y)
  <$result$> <- (a);<$returning$> <- true
 end
 if <$returning$> then ()
 else begin
  h(y)
  ()
 end
end
if <$returning$> then <$result$>
else begin
 i(y)
 ()
end")]
        [DataRow("do if x then f(y) return a end g(y) end h(y) return b", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
if true then begin
 if(x)then begin
  f(y)
  <$result$> <- (a);<$returning$> <- true
 end
 if <$returning$> then ()
 else begin
  g(y)
  ()
 end
end
if <$returning$> then <$result$>
else begin
 h(y)
 (b)
end")]
        [TestMethod]
        public void Do(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("while x do f(a) end", $@"while(x)do begin
 f(a)
 ()
end
()")]
        [DataRow("while x do f(a) break end", $@"let mutable <$continuing$> = true
while <$continuing$> && (x)do begin
 let mutable <$interrupting$> = false
 f(a)
 <$continuing$> <- false
 <$interrupting$> <- true
end
()")]
        [DataRow("while x do throw a end", $@"while(x)do begin
 (a){@throw}
end
()")]
        [DataRow("while x do f(a) return b end g(a)", $@"let mutable <$continuing$> = true
let mutable <$returning$> = false
let mutable <$result$> = {@default}
while <$continuing$> && (x)do begin
 let mutable <$interrupting$> = false
 f(a)
 <$result$> <- (b);<$returning$> <- true
 <$continuing$> <- false
 <$interrupting$> <- true
end
if <$returning$> then <$result$>
else begin
 g(a)
 ()
end")]
        [TestMethod]
        public void While(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("while true do f(a) end", $@"if true then begin
 while true do begin
  f(a)
  ()
 end
 {@default}
end
else {@default}")]
        [DataRow("while true do f(a) end g(b)", $@"if true then begin
 while true do begin
  f(a)
  ()
 end
 {@default}
end
else begin
 {ignore_begin}
  g(b)
  ()
 {ignore_end}
 {@default}
end")]
        [DataRow("while ( ( true ) ) do f(a) end", $@"if true then begin
 while true do begin
  f(a)
  ()
 end
 {@default}
end
else {@default}")]
        [DataRow("while true do f(a) break end", $@"let mutable <$continuing$> = true
while <$continuing$> do begin
 let mutable <$interrupting$> = false
 f(a)
 <$continuing$> <- false
 <$interrupting$> <- true
end
()")]
        [DataRow("while true do throw a end", $@"if true then begin
 while true do begin
  (a){@throw}
 end
 {@default}
end
else {@default}")]
        [TestMethod]
        public void WhileTrue(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("repeat f(a) until x", $@"let mutable <$continuing$> = true
while <$continuing$> do begin
 f(a)
 ()
 <$continuing$> <- {not}(x)
end
()")]
        [DataRow("repeat f(a) break until x", $@"let mutable <$continuing$> = true
while <$continuing$> do begin
 let mutable <$interrupting$> = false
 f(a)
 <$continuing$> <- false
 <$interrupting$> <- true
 if <$continuing$> then <$continuing$> <- {not}(x)
end
()")]
        [DataRow("repeat throw a until x", $@"if true then begin
 let mutable <$continuing$> = true
 while <$continuing$> do begin
  (a){@throw}
  <$continuing$> <- {not}(x)
 end
 {@default}
end
else {@default}")]
        [DataRow("repeat f(a) return b until x g(a)", $@"let mutable <$returning$> = false
let mutable <$result$> = {@default}
let mutable <$continuing$> = true
while <$continuing$> do begin
 let mutable <$interrupting$> = false
 f(a)
 <$result$> <- (b);<$returning$> <- true
 <$continuing$> <- false
 <$interrupting$> <- true
 if <$continuing$> then <$continuing$> <- {not}(x)
end
if <$returning$> then <$result$>
else begin
 g(a)
 ()
end")]
        [TestMethod]
        public void Repeat(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        const string DisposeEnumerator_ = "global.Sona.Runtime.CompilerServices.SequenceHelpers.DisposeEnumerator(global.Sona.Runtime.CompilerServices.SequenceHelpers.Marker,&";

        [DataRow("for x in c do f(a) end", $@"for x in(c)do begin
 f(a)
 ()
end
()")]
        [DataRow("for x in c do f(a) break end", $@"let mutable <$continuing$> = true
let mutable <$enumerator$> = (c){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
()")]
        [DataRow("for x in c do throw a end", $@"for x in(c)do begin
 (a){@throw}
end
()")]
        [DataRow("for x in c do f(a) return b end g(a)", $@"let mutable <$continuing$> = true
let mutable <$returning$> = false
let mutable <$result$> = {@default}
let mutable <$enumerator$> = (c){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$result$> <- (b);<$returning$> <- true
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
if <$returning$> then <$result$>
else begin
 g(a)
 ()
end")]
        [TestMethod]
        public void ForSimple(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("for x in c by d do f(a) end", null)]
        [TestMethod]
        public void ForSimpleStep(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("for x in c..d do f(a) end", $@"for x in(c) .. (d)do begin
 f(a)
 ()
end
()")]
        [DataRow("for x in c..d do f(a) break end", $@"let mutable <$continuing$> = true
let mutable <$enumerator$> = ((..)(c)(d)){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
()")]
        [DataRow("for x in c..d do throw a end", $@"for x in(c) .. (d)do begin
 (a){@throw}
end
()")]
        [DataRow("for x in c..d do f(a) return b end g(a)", $@"let mutable <$continuing$> = true
let mutable <$returning$> = false
let mutable <$result$> = {@default}
let mutable <$enumerator$> = ((..)(c)(d)){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$result$> <- (b);<$returning$> <- true
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
if <$returning$> then <$result$>
else begin
 g(a)
 ()
end")]
        [TestMethod]
        public void ForRange(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("for x in c..d by e do f(a) end", $@"let <$start$> = c
let <$end$> = d
let <$step$> = e
for x in(<$start$>) .. <$step$> .. (<$end$>)do begin
 f(a)
 ()
end
()")]
        [DataRow("for x in c..d by e do f(a) break end", $@"let mutable <$continuing$> = true
let mutable <$enumerator$> = ((fun <$start$> <$end$> <$step$> -> (.. ..)<$start$> <$step$> <$end$>)(c)(d)(e)){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
()")]
        [DataRow("for x in c..d by e do throw a end", $@"let <$start$> = c
let <$end$> = d
let <$step$> = e
for x in(<$start$>) .. <$step$> .. (<$end$>)do begin
 (a){@throw}
end
()")]
        [DataRow("for x in c..d by e do f(a) return b end g(a)", $@"let mutable <$continuing$> = true
let mutable <$returning$> = false
let mutable <$result$> = {@default}
let mutable <$enumerator$> = ((fun <$start$> <$end$> <$step$> -> (.. ..)<$start$> <$step$> <$end$>)(c)(d)(e)){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$result$> <- (b);<$returning$> <- true
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
if <$returning$> then <$result$>
else begin
 g(a)
 ()
end")]
        [TestMethod]
        public void ForRangeStep(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }

        [DataRow("for x in c..d by 9 do f(a) end", $@"for x in(c) .. (9) .. (d)do begin
 f(a)
 ()
end
()")]
        [DataRow("for x in c..d by 9 do f(a) break end", $@"let mutable <$continuing$> = true
let mutable <$enumerator$> = ((fun <$start$> <$end$> <$step$> -> (.. ..)<$start$> <$step$> <$end$>)(c)(d)(9)){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
()")]
        [DataRow("for x in c..d by 9 do throw a end", $@"for x in(c) .. (9) .. (d)do begin
 (a){@throw}
end
()")]
        [DataRow("for x in c..d by 9 do f(a) return b end g(a)", $@"let mutable <$continuing$> = true
let mutable <$returning$> = false
let mutable <$result$> = {@default}
let mutable <$enumerator$> = ((fun <$start$> <$end$> <$step$> -> (.. ..)<$start$> <$step$> <$end$>)(c)(d)(9)){each}.GetEnumerator()
try
 while <$continuing$> && <$enumerator$>.MoveNext() do begin
  let mutable <$interrupting$> = false
  let x = <$enumerator$>.Current
  f(a)
  <$result$> <- (b);<$returning$> <- true
  <$continuing$> <- false
  <$interrupting$> <- true
 end
finally {DisposeEnumerator_}<$enumerator$>)
if <$returning$> then <$result$>
else begin
 g(a)
 ()
end")]
        [TestMethod]
        public void ForRangePrimitiveStep(string source, string? expected)
        {
            AssertBlockEquivalence(source, expected);
        }
    }
}
