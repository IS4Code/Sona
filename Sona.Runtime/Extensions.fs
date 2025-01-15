namespace Sona.Runtime.Internal

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
  
[<Struct>]
type ``array enumerator``<'T> =
  val mutable Array: 'T[]
  val mutable Position: int
  
  static member inline make(arr: 'T[]) =
    let mutable r = Unchecked.defaultof<``array enumerator``<'T>>
    r.Array <- arr
    r.Position <- -1
    r

  member inline this.MoveNext() =
    this.Position <- this.Position + 1
    this.Position < this.Array.Length

  member inline this.Current = this.Array[this.Position];

  interface System.Collections.Generic.IEnumerator<'T> with
    member this.MoveNext() = this.MoveNext()
    member this.Reset() = this.Position <- -1
    member this.Current: 'T = this.Current
    member this.Current: obj = this.Current
    member _.Dispose() = ()

namespace Sona.Runtime
open System
open Sona.Runtime.Internal

[<Sealed>]
[<AbstractClass>]
type SequenceHelpers private() = 
  static member Marker = Priority4()

  static member inline DisposeEnumerator(_: Priority1, obj: byref<^T>
    when ^T : not struct) =
    match box obj with
    | :? IDisposable as d -> d.Dispose()
    | _ -> ()

  static member inline DisposeEnumerator(_: Priority2, obj: byref<^T>
    when ^T : struct) = ()
    
  static member inline DisposeEnumerator(_: Priority3, obj: byref<^T>
    when ^T : struct
    and ^T :> IDisposable) =
    obj.Dispose()

  static member inline GetEnumerator(_: Priority1, obj: ^T :> Collections.IEnumerable) = obj.GetEnumerator()
    
  static member inline GetEnumerator(_: Priority2, obj: ^T :> _ seq) = obj.GetEnumerator()

  static member inline GetEnumerator(_: Priority3, obj) =
    (^T: (member GetEnumerator: unit -> _) obj)
  
  static member inline GetEnumerator(_: Priority4, obj: ^T array) = ``array enumerator``<^T>.make(obj)
