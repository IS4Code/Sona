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
module Inference =
  [<Literal>]
  let Instance : Inference = null
  
  let inline ``date|timespan`` x = ((^self or ^x) : (static member ``date|timespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``date|datespan`` x = ((^self or ^x) : (static member ``date|datespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``date|datetimespan`` x = ((^self or ^x) : (static member ``date|datetimespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``time|timespan`` x = ((^self or ^x) : (static member ``time|timespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``time|datespan`` x = ((^self or ^x) : (static member ``time|datespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``time|datetimespan`` x = ((^self or ^x) : (static member ``time|datetimespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``datetime|timespan`` x = ((^self or ^x) : (static member ``datetime|timespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``datetime|datespan`` x = ((^self or ^x) : (static member ``datetime|datespan``: ^self * ^x -> ^x) Instance, x)
  let inline ``datetime|datetimespan`` x = ((^self or ^x) : (static member ``datetime|datetimespan``: ^self * ^x -> ^x) Instance, x)
#warnon "64"
