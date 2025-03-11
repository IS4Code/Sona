namespace Sona.Runtime.CompilerServices.Internal

[<AbstractClass>]
type Priority1 internal() = class end

[<AbstractClass>]
type Priority2 internal() =
  inherit Priority1()
 
[<AbstractClass>]
type Priority3 internal() =
  inherit Priority2()

[<Sealed>]
type Priority4 internal() =
  inherit Priority3()

namespace Sona.Runtime.CompilerServices
open System
open System.Runtime.CompilerServices
open Sona.Runtime.Traits
open Sona.Runtime.CompilerServices.Internal
open System.Runtime.ExceptionServices

[<Sealed; AbstractClass>]
type SequenceHelpers private() = 
  static member Marker = Priority4()

  static member inline DisposeEnumerator(_: Priority1, o: byref<^T>
    when ^T : not struct) =
    match box o with
    | :? IDisposable as d -> d.Dispose()
    | _ -> ()

  static member inline DisposeEnumerator(_: Priority2, o: byref<^T>
    when ^T : struct) = ()
    
  static member inline DisposeEnumerator(_: Priority3, o: byref<^T>
    when ^T : struct
    and ^T :> IDisposable) =
    o.Dispose()
  
[<Sealed; AbstractClass; Extension>]
type SequenceHelpersExtensions private() = 
  [<Extension>]
  static member inline GetEnumerable(o: _, _: Priority1) = o
  
  [<Extension>]
  static member inline GetEnumerable(o: inref<_>, _: Priority2) = &o
  
  [<Extension>]
  static member inline GetEnumerable(o: ^T array, _: Priority3): ^T seq = o
  
  [<Extension>]
  static member inline GetEnumerable(o: ^T list, _: Priority4): ^T seq = o

[<AbstractClass; AllowNullLiteral>]
type OperatorsBase internal() = 
  static member inline ``operator Throw``(_:OperatorsBase, x) =
    let _ = (^T : (member ``throw()``: unit -> _) x)
    raise (ArgumentException("The object's 'throw()' operator implementation did not throw an exception.", nameof x))
    x
    
  static member inline ``operator Rethrow``(_:OperatorsBase, x) =
    let _ = (^T : (member ``rethrow()``: unit -> _) x)
    raise (ArgumentException("The object's 'rethrow()' operator implementation did not throw an exception.", nameof x))
    x
  
  // Using string directly infers it in Throw
  static member inline ``operator Throw``(_:OperatorsBase, x : ^T when ^T :> IEquatable<string> and ^T :> char seq) =
    failwith (x.ToString())
    x

  static member inline ``operator Rethrow``(_:OperatorsBase, x : ExceptionDispatchInfo) =
    x.Throw()
    x

[<Sealed; AbstractClass; AllowNullLiteral>]
type Operators private() = 
  inherit OperatorsBase()
  static member inline ``operator Throw``(_:OperatorsBase, x : #Exception) =
    raise x
    x
    
  static member inline ``operator Rethrow``(_:OperatorsBase, x : #Exception) =
    ExceptionDispatchInfo.Capture(x).Throw()
    x
  
  // Including this prevents ExceptionDispatchInfo from being inferred in Rethrow
  static member inline ``operator Rethrow``(_:OperatorsBase, x : RuntimeWrappedException) = Operators.``operator Rethrow``(null, x :> Exception) :?> RuntimeWrappedException

module Operators =
  let inline Throw(x) =
    let _ = ((^self or ^x) : (static member ``operator Throw``: ^self * ^x -> ^x) (null : Operators), x)
    Unchecked.defaultof<_>
    
  let inline Rethrow(x) =
    let _ = ((^self or ^x) : (static member ``operator Rethrow``: ^self * ^x -> ^x) (null : Operators), x)
    Unchecked.defaultof<_>

  let inline ThrowObject(x : obj) =
    (# "throw" x #)
    Unchecked.defaultof<_>
  
module Patterns =
  [<return: Struct>]
  let inline (|RuntimeWrappedException|_|)(x : obj) =
    match x with
    | :? RuntimeWrappedException as wrapped -> ValueSome (wrapped.WrappedException)
    | :? Exception -> ValueNone
    | _ -> ValueSome x

[<AbstractClass; AllowNullLiteral>]
type InferenceBase internal() =
  static member inline ``operator date|timespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``operator date|datespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``operator date|datetimespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``operator time|timespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``operator time|datespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``operator time|datetimespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``operator datetime|timespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
  static member inline ``operator datetime|datespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
  static member inline ``operator datetime|datetimespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
 
[<Sealed; AbstractClass; AllowNullLiteral>]
type Inference private() =
  inherit InferenceBase()
  static member inline ``operator date|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``operator date|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``operator date|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x
  static member inline ``operator time|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``operator time|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``operator time|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x
  static member inline ``operator datetime|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``operator datetime|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``operator datetime|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x

  static member inline ``date|timespan`` x = ((^self or ^x) : (static member ``operator date|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``date|datespan`` x = ((^self or ^x) : (static member ``operator date|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``date|datetimespan`` x = ((^self or ^x) : (static member ``operator date|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|timespan`` x = ((^self or ^x) : (static member ``operator time|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|datespan`` x = ((^self or ^x) : (static member ``operator time|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|datetimespan`` x = ((^self or ^x) : (static member ``operator time|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|timespan`` x = ((^self or ^x) : (static member ``operator datetime|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|datespan`` x = ((^self or ^x) : (static member ``operator datetime|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|datetimespan`` x = ((^self or ^x) : (static member ``operator datetime|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)

[<Sealed; AbstractClass; AllowNullLiteral>]
type Tuples =
  static member inline ToTree(_ : unit) = ()
  
  static member inline ToTree(t : Tuple<_>) = (t.Item1, ())
  static member inline ToTree((a, b)) = (a, (b, ()))
  static member inline ToTree((a, b, c)) = (a, (b, (c, ())))
  static member inline ToTree((a, b, c, d)) = (a, (b, (c, (d, ()))))
  static member inline ToTree((a, b, c, d, e)) = (a, (b, (c, (d, (e, ())))))
  static member inline ToTree((a, b, c, d, e, f)) = (a, (b, (c, (d, (e, (f, ()))))))
  static member inline ToTree((a, b, c, d, e, f, g)) = (a, (b, (c, (d, (e, (f, (g, ())))))))
  static member inline ToTree((a, b, c, d, e, f, g, h)) = (a, (b, (c, (d, (e, (f, (g, (h, ()))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))
  
  static member inline ToTree(t : ValueTuple<_>) = (t.Item1, ())
  static member inline ToTree(struct(a, b)) = (a, (b, ()))
  static member inline ToTree(struct(a, b, c)) = (a, (b, (c, ())))
  static member inline ToTree(struct(a, b, c, d)) = (a, (b, (c, (d, ()))))
  static member inline ToTree(struct(a, b, c, d, e)) = (a, (b, (c, (d, (e, ())))))
  static member inline ToTree(struct(a, b, c, d, e, f)) = (a, (b, (c, (d, (e, (f, ()))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g)) = (a, (b, (c, (d, (e, (f, (g, ())))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h)) = (a, (b, (c, (d, (e, (f, (g, (h, ()))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))
  
  static member inline FromTree(_ : unit) = ()
  static member inline FromTree((a, ())) = a
  static member inline FromTree((a, (b, ()))) = (a, b)
  static member inline FromTree((a, (b, (c, ())))) = (a, b, c)
  static member inline FromTree((a, (b, (c, (d, ()))))) = (a, b, c, d)
  static member inline FromTree((a, (b, (c, (d, (e, ())))))) = (a, b, c, d, e)
  static member inline FromTree((a, (b, (c, (d, (e, (f, ()))))))) = (a, b, c, d, e, f)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, ())))))))) = (a, b, c, d, e, f, g)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, ()))))))))) = (a, b, c, d, e, f, g, h)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))) = (a, b, c, d, e, f, g, h, i)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))) = (a, b, c, d, e, f, g, h, i, j)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))) = (a, b, c, d, e, f, g, h, i, j, k)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
  
  static member inline FromTreeValue(_ : unit) = ()
  static member inline FromTreeValue((a, ())) = a
  static member inline FromTreeValue((a, (b, ()))) = struct(a, b)
  static member inline FromTreeValue((a, (b, (c, ())))) = struct(a, b, c)
  static member inline FromTreeValue((a, (b, (c, (d, ()))))) = struct(a, b, c, d)
  static member inline FromTreeValue((a, (b, (c, (d, (e, ())))))) = struct(a, b, c, d, e)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, ()))))))) = struct(a, b, c, d, e, f)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, ())))))))) = struct(a, b, c, d, e, f, g)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, ()))))))))) = struct(a, b, c, d, e, f, g, h)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))) = struct(a, b, c, d, e, f, g, h, i)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))) = struct(a, b, c, d, e, f, g, h, i, j)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
  
  static member inline ``operator Append``(_:Tuples, _ : unit, _ : unit) = ()
  static member inline ``operator Append``(_:Tuples, _ : unit, b : (_ * unit)) = b
  static member inline ``operator Append``(_:Tuples, _ : unit, b : (_ * (_ * _))) = b

  static member inline ``operator Append``(_:Tuples, a : (_ * unit), _ : unit) = a
  static member inline ``operator Append``(_:Tuples, (a1, ()), b : (_ * unit)) = (a1, b)
  static member inline ``operator Append``(_:Tuples, (a1, ()), b : (_ * (_ * _))) = (a1, b)

  static member inline ``operator Append``(_:Tuples, a : (_ * (_ * _)), _ : unit) = a
  static member inline ``operator Append``(_:Tuples, (a1, (a2, aRest : ^aRest)), b : (^b1 * unit)) =
    (a1, (a2, ((^self or ^aRest) : (static member ``operator Append`` : ^self * ^aRest * (^b1 * unit) -> _) (null : Tuples), aRest, b)))
  static member inline ``operator Append``(_:Tuples, (a1, (a2, aRest : ^aRest)), b : (^b1 * (^b2 * ^bRest))) =
    (a1, (a2, ((^self or ^aRest) : (static member ``operator Append`` : ^self * ^aRest * (^b1 * (^b2 * ^bRest)) -> _) (null : Tuples), aRest, b)))
  
  static member inline Append(a : unit, b : unit) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : unit, b : (_ * unit)) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : unit, b : (_ * (_ * _))) = Tuples.``operator Append``((null : Tuples), a, b)

  static member inline Append(a : (_ * unit), b : unit) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * unit), b : (_ * unit)) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * unit), b : (_ * (_ * _))) = Tuples.``operator Append``((null : Tuples), a, b)

  static member inline Append(a : (_ * (_ * _)), b : unit) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * (_ * _)), b : (_ * unit)) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * (_ * _)), b : (_ * (_ * _))) = Tuples.``operator Append``((null : Tuples), a, b)

namespace Microsoft.FSharp.Core
[<AutoOpen>]
module Operators =
  [<CompiledName("ToBoolean")>]
  let inline bool x = (^T : (static member op_Explicit : ^T -> bool) x)