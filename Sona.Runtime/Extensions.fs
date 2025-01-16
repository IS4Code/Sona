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

namespace Sona.Runtime
open System
open Sona.Runtime.Internal
open System.Runtime.CompilerServices

[<Sealed>]
[<AbstractClass>]
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
  
[<Sealed>]
[<AbstractClass>]
[<Extension>]
type SequenceHelpersExtensions private() = 
  [<Extension>]
  static member inline GetEnumerable(o: _, _: Priority1) = o
  
  [<Extension>]
  static member inline GetEnumerable(o: inref<_>, _: Priority2) = &o
  
  [<Extension>]
  static member inline GetEnumerable(o: ^T array, _: Priority3): ^T seq = o
  
  [<Extension>]
  static member inline GetEnumerable(o: ^T list, _: Priority4): ^T seq = o