using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sona.Tests
{
    [TestClass]
    public class ComputationBlocks : CompilationTests
    {
        [DataRow("function f() with async follow g() end",
            @"let rec f() = (
 (let <$_1$> = async in <$_1$> { begin
   let! () = (g())
   return ()
  end
 })
)")]
        [DataRow("function f() with async echo \"a\" follow g() end",
            @"let rec f() = (
 (let <$_1$> = async in <$_1$> { begin
   global.Microsoft.FSharp.Core.ExtraTopLevelOperators.printfn(""a"")
   let! () = (g())
   return ()
  end
 })
)")]
        [DataRow("function f() with async follow g()! end",
            @"let rec f() = (
 (let <$_1$> = async in <$_1$> { begin
   let! _ = (g())
   return ()
  end
 })
)")]
        // `return follow` is the return-bind (return!) form.
        [DataRow("function f() with async return follow g() end",
            @"let rec f() = (
 (let <$_1$> = async in <$_1$> { begin
   return! g()
  end
 })
)")]
        // `follow` as the initialiser of a `let`, then a plain return.
        [DataRow("function f() with async let x = follow g() return x end",
            @"let rec f() = (
 (let <$_1$> = async in <$_1$> { begin
   let! (x) = g()
   return (x)
  end
 })
)")]
        [TestMethod]
        public void WithAndFollow(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        const string ifFalseYieldDefault = "if false then yield global.Microsoft.FSharp.Core.Operators.Unchecked.defaultof<_>";

        [DataRow("function f() with.. taskSeq yield a end",
            $@"let rec f() = (
 (let <$_1$> = taskSeq in <$_1$> {{ begin
   {ifFalseYieldDefault}
   yield a
   ()
  end
 }})
)")]
        [DataRow("function f() with.. taskSeq yield break end",
            $@"let rec f() = (
 (let <$_1$> = taskSeq in <$_1$> {{ begin
   {ifFalseYieldDefault}
   ()
  end
 }})
)")]
        [DataRow("function f() with.. taskSeq yield return a end",
            $@"let rec f() = (
 (let <$_1$> = taskSeq in <$_1$> {{ begin
   {ifFalseYieldDefault}
   return a
   ()
  end
 }})
)")]
        [TestMethod]
        public void Yield(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("function f() with() follow g() end",
            @"let rec f() = (
 (begin
   do (global.Sona.Runtime.Computations.``global``.ReturnFrom(g()) : global.Sona.Runtime.ComputationBuilders.Immediate<_>).Value
   ()
  end
 )
)")]
        [TestMethod]
        public void DefaultBuilder(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }

        [DataRow("function f() if a then with async follow g() end echo \"x\" end", null)]
        [TestMethod]
        public void WithCannotEscapeItsBlock(string source, string? expected)
        {
            AssertStatementEquivalence(source, expected);
        }
    }
}
