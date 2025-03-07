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
type InferenceBase internal() =
  static member inline ``date|timespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``date|datespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``date|datetimespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``time|timespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``time|datespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``time|datetimespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``datetime|timespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
  static member inline ``datetime|datespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
  static member inline ``datetime|datetimespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
 
[<Sealed; AbstractClass; AllowNullLiteral>]
type Inference private() =
  inherit InferenceBase()
  static member inline ``date|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``date|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``date|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x
  static member inline ``time|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``time|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``time|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x
  static member inline ``datetime|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``datetime|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``datetime|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x

#nowarn "64"
type Inference with
  static member inline ``date|timespan`` x = ((^self or ^x) : (static member ``date|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``date|datespan`` x = ((^self or ^x) : (static member ``date|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``date|datetimespan`` x = ((^self or ^x) : (static member ``date|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|timespan`` x = ((^self or ^x) : (static member ``time|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|datespan`` x = ((^self or ^x) : (static member ``time|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|datetimespan`` x = ((^self or ^x) : (static member ``time|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|timespan`` x = ((^self or ^x) : (static member ``datetime|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|datespan`` x = ((^self or ^x) : (static member ``datetime|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|datetimespan`` x = ((^self or ^x) : (static member ``datetime|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
#warnon "64"

[<Sealed; AbstractClass; AllowNullLiteral>]
type Tuples =
  static member inline ToTree(_ : unit) = ()
  
  static member inline ToTree(t :Tuple<_>) = struct(t.Item1, ())
  static member inline ToTree((a , b)) = struct(a, struct(b, ()))
  static member inline ToTree((a, b, c)) = struct(a, struct(b, struct(c, ())))
  static member inline ToTree((a, b, c, d)) = struct(a, struct(b, struct(c, struct(d, ()))))
  static member inline ToTree((a, b, c, d, e)) = struct(a, struct(b, struct(c, struct(d, struct(e, ())))))
  static member inline ToTree((a, b, c, d, e, f)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, ()))))))
  static member inline ToTree((a, b, c, d, e, f, g)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, ())))))))
  static member inline ToTree((a, b, c, d, e, f, g, h)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, ()))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, ())))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, ()))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, ())))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, ()))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, ())))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, ()))))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, ())))))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, struct(p, ()))))))))))))))))
  
  static member inline ToTree(t : ValueTuple<_>) = struct(t.Item1, ())
  static member inline ToTree(struct(a, b)) = struct(a, struct(b, ()))
  static member inline ToTree(struct(a, b, c)) = struct(a, struct(b, struct(c, ())))
  static member inline ToTree(struct(a, b, c, d)) = struct(a, struct(b, struct(c, struct(d, ()))))
  static member inline ToTree(struct(a, b, c, d, e)) = struct(a, struct(b, struct(c, struct(d, struct(e, ())))))
  static member inline ToTree(struct(a, b, c, d, e, f)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, ()))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, ())))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, ()))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, ())))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, ()))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, ())))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, ()))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, ())))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, ()))))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, ())))))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)) = struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, struct(p, ()))))))))))))))))
  
  static member inline FromTree(_ : unit) = ()
  static member inline FromTree(struct(a, ())) = a
  static member inline FromTree(struct(a, struct(b, ()))) = (a, b)
  static member inline FromTree(struct(a, struct(b, struct(c, ())))) = (a, b, c)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, ()))))) = (a, b, c, d)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, ())))))) = (a, b, c, d, e)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, ()))))))) = (a, b, c, d, e, f)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, ())))))))) = (a, b, c, d, e, f, g)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, ()))))))))) = (a, b, c, d, e, f, g, h)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, ())))))))))) = (a, b, c, d, e, f, g, h, i)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, ()))))))))))) = (a, b, c, d, e, f, g, h, i, j)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, ())))))))))))) = (a, b, c, d, e, f, g, h, i, j, k)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, ()))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, ())))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, ()))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, ())))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  static member inline FromTree(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, struct(p, ()))))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
  
  static member inline FromTreeValue(_ : unit) = ()
  static member inline FromTreeValue(struct(a, ())) = a
  static member inline FromTreeValue(struct(a, struct(b, ()))) = struct(a, b)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, ())))) = struct(a, b, c)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, ()))))) = struct(a, b, c, d)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, ())))))) = struct(a, b, c, d, e)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, ()))))))) = struct(a, b, c, d, e, f)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, ())))))))) = struct(a, b, c, d, e, f, g)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, ()))))))))) = struct(a, b, c, d, e, f, g, h)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, ())))))))))) = struct(a, b, c, d, e, f, g, h, i)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, ()))))))))))) = struct(a, b, c, d, e, f, g, h, i, j)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, ())))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, ()))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, ())))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, ()))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, ())))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  static member inline FromTreeValue(struct(a, struct(b, struct(c, struct(d, struct(e, struct(f, struct(g, struct(h, struct(i, struct(j, struct(k, struct(l, struct(m, struct(n, struct(o, struct(p, ()))))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
  
#nowarn "64"
type Tuples with
  static member inline AppendCore(_ : Tuples, _ : unit, _ : unit) = ()
  static member inline AppendCore(_ : Tuples, _ : unit, b : struct(_ * unit)) = b
  static member inline AppendCore(_ : Tuples, _ : unit, b : struct(_ * struct(_ * _))) = b

  static member inline AppendCore(_ : Tuples, a : struct(_ * unit), _ : unit) = a
  static member inline AppendCore(_ : Tuples, struct(a1, ()), b : struct(_ * unit)) = struct(a1, b)
  static member inline AppendCore(_ : Tuples, struct(a1, ()), b : struct(_ * struct(_ * _))) = struct(a1, b)

  static member inline AppendCore(_ : Tuples, a : struct(_ * struct(_ * _)), _ : unit) = a
  static member inline AppendCore(_ : Tuples, struct(a1, struct(a2, aRest : ^aRest)), b : struct(^b1 * unit)) =
    struct(a1, struct(a2, ((^self or ^aRest) : (static member AppendCore : ^self * ^aRest * struct(^b1 * unit) -> _) (null : Tuples), aRest, b)))
  static member inline AppendCore(_ : Tuples, struct(a1, struct(a2, aRest : ^aRest)), b : struct(^b1 * struct(^b2 * ^bRest))) =
    struct(a1, struct(a2, ((^self or ^aRest) : (static member AppendCore : ^self * ^aRest * struct(^b1 * struct(^b2 * ^bRest)) -> _) (null : Tuples), aRest, b)))
#warnon "64"

type Tuples with
  static member inline Append(a : unit, b : unit) = Tuples.AppendCore((null : Tuples), a, b)
  static member inline Append(a : unit, b : struct(_ * unit)) = Tuples.AppendCore((null : Tuples), a, b)
  static member inline Append(a : unit, b : struct(_ * struct(_ * _))) = Tuples.AppendCore((null : Tuples), a, b)

  static member inline Append(a : struct(_ * unit), b : unit) = Tuples.AppendCore((null : Tuples), a, b)
  static member inline Append(a : struct(_ * unit), b : struct(_ * unit)) = Tuples.AppendCore((null : Tuples), a, b)
  static member inline Append(a : struct(_ * unit), b : struct(_ * struct(_ * _))) = Tuples.AppendCore((null : Tuples), a, b)

  static member inline Append(a : struct(_ * struct(_ * _)), b : unit) = Tuples.AppendCore((null : Tuples), a, b)
  static member inline Append(a : struct(_ * struct(_ * _)), b : struct(_ * unit)) = Tuples.AppendCore((null : Tuples), a, b)
  static member inline Append(a : struct(_ * struct(_ * _)), b : struct(_ * struct(_ * _))) = Tuples.AppendCore((null : Tuples), a, b)

namespace Microsoft.FSharp.Core
[<AutoOpen>]
module Operators =
  [<CompiledName("ToBoolean")>]
  let inline bool x = (^T : (static member op_Explicit : ^T -> bool) x)