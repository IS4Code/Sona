[<AutoOpen>]
module Sona.Runtime.Computations

open System.Runtime.CompilerServices
open Sona.Runtime.ComputationBuilders
open Sona.Runtime.Collections
open Sona.Runtime.Coroutines

[<Sealed>]
type private ResultBuilderImpl<'TError>() =
  inherit ResultBuilder<'TError>()

  static member val Instance : ResultBuilder<'TError> = ResultBuilderImpl<'TError>()

  override _.ToString() = "result"

[<Sealed>]
type private ErrorResultBuilderImpl<'TSuccess>() =
  inherit ErrorResultBuilder<'TSuccess>()

  static member val Instance : ErrorResultBuilder<'TSuccess> = ErrorResultBuilderImpl<'TSuccess>()

  override _.ToString() = "errorResult"
  
let coroutine = { new CoroutineBuilder() with member _.ToString() = "coroutine" }
let sequence = { new SequenceBuilder() with member _.ToString() = "sequence" }
let option = { new OptionBuilder() with member _.ToString() = "option" }
let voption = { new ValueOptionBuilder() with member _.ToString() = "voption" }
let result<'TError> = ResultBuilderImpl<'TError>.Instance
let errorResult<'TSuccess> = ErrorResultBuilderImpl<'TSuccess>.Instance
let delayed = { new DelayedBuilder() with member _.ToString() = "delayed" }
let immediate = { new ImmediateBuilder() with member _.ToString() = "immediate" }
let ``global`` = { new GlobalBuilder() with member _.ToString() = "global" }

[<CompiledName("SequenceVia")>]
let inline sequenceVia builder : UniversalSequenceBuilder<_, _> = { BoolBuilder = builder; UnitBuilder = builder }

[<AbstractClass; Extension>]
type UniversalSequenceBuilderExtensions1 internal() =
  [<Extension>]
  static member inline Zero(self : UniversalSequenceBuilder<_, _>) =
    UniversalSequence.zero
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
  
  [<Extension>]
  static member inline Yield(self : UniversalSequenceBuilder<_, _>, x) =
    UniversalSequence.``yield``
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      x
  
  [<Extension>]
  static member inline Bind(self : UniversalSequenceBuilder<_, _>, m, [<IIL>]_f) =
    UniversalSequence.bind
      self.BoolBuilder
      (fun f -> f())
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m _f
  
  [<Extension>]
  static member inline Combine(self : UniversalSequenceBuilder<_, _>, first, [<IIL>]_second) =
    UniversalSequence.combine
      self.BoolBuilder
      (fun f -> f())
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first _second
  
  [<Extension>]
  static member inline TryFinally(self : UniversalSequenceBuilder<_, _>, [<IIL>]_f, [<IIL>]_cleanup) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _f _cleanup
  
  [<Extension>]
  static member inline TryWith(self : UniversalSequenceBuilder<_, _>, [<IIL>]_f, [<IIL>]_fail) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _f _fail
  
  [<Extension>]
  static member inline Using(self : UniversalSequenceBuilder<_, _>, obj, [<IIL>]_f) =
    UniversalSequence.using
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      obj _f
  
  [<Extension>]
  static member inline While(self : UniversalSequenceBuilder<_, _>, [<IIL>]_cond, [<IIL>]_f) =
    UniversalSequence.``while``
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _cond _f
  
  [<Extension>]
  static member inline For(self : UniversalSequenceBuilder<_, _>, s, [<IIL>]_f) =
    UniversalSequence.``for``
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      s _f
    
[<AbstractClass; Extension>]
type UniversalSequenceBuilderExtensions2 internal() =
  inherit UniversalSequenceBuilderExtensions1()
  
  [<Extension>]
  static member inline Zero(self : UniversalSequenceBuilder<_, _>) =
    UniversalSequence.zero
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
  
  [<Extension>]
  static member inline Yield(self : UniversalSequenceBuilder<_, _>, x) =
    UniversalSequence.``yield``
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      x
  
  [<Extension>]
  static member inline Bind(self : UniversalSequenceBuilder<_, _>, m, [<IIL>]_f) =
    UniversalSequence.bind
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m _f
  
  [<Extension>]
  static member inline Combine(self : UniversalSequenceBuilder<_, _>, first, [<IIL>]_second) =
    UniversalSequence.combine
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first _second
  
  [<Extension>]
  static member inline TryFinally(self : UniversalSequenceBuilder<_, _>, [<IIL>]_f, [<IIL>]_cleanup) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _f _cleanup
  
  [<Extension>]
  static member inline TryWith(self : UniversalSequenceBuilder<_, _>, [<IIL>]_f, [<IIL>]_fail) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _f _fail
  
  [<Extension>]
  static member inline Using(self : UniversalSequenceBuilder<_, _>, obj, [<IIL>]_f) =
    UniversalSequence.using
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      obj _f
  
  [<Extension>]
  static member inline While(self : UniversalSequenceBuilder<_, _>, [<IIL>]_cond, [<IIL>]_f) =
    UniversalSequence.``while``
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _cond _f
  
  [<Extension>]
  static member inline For(self : UniversalSequenceBuilder<_, _>, s, [<IIL>]_f) =
    UniversalSequence.``for``
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      s _f

[<Sealed; AbstractClass; Extension>]
type UniversalSequenceBuilderExtensions3 private() =
  inherit UniversalSequenceBuilderExtensions2()
  
  [<Extension>]
  static member inline Zero(self : UniversalSequenceBuilder<_, _>) =
    UniversalSequence.zero
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
  
  [<Extension>]
  static member inline Yield(self : UniversalSequenceBuilder<_, _>, x) =
    UniversalSequence.``yield``
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      x
  
  [<Extension>]
  static member inline Bind(self : UniversalSequenceBuilder<_, _>, m, [<IIL>]_f) =
    UniversalSequence.bind
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m _f
  
  [<Extension>]
  static member inline Combine(self : UniversalSequenceBuilder<_, _>, first, [<IIL>]_second) =
    UniversalSequence.combine
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first _second
  
  [<Extension>]
  static member inline TryFinally(self : UniversalSequenceBuilder<_, _>, [<IIL>]_f, [<IIL>]_cleanup) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _f _cleanup
  
  [<Extension>]
  static member inline TryWith(self : UniversalSequenceBuilder<_, _>, [<IIL>]_f, [<IIL>]_fail) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _f _fail
  
  [<Extension>]
  static member inline Using(self : UniversalSequenceBuilder<_, _>, obj, [<IIL>]_f) =
    UniversalSequence.using
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      obj _f
  
  [<Extension>]
  static member inline While(self : UniversalSequenceBuilder<_, _>, [<IIL>]_cond, [<IIL>]_f) =
    UniversalSequence.``while``
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      _cond _f
  
  [<Extension>]
  static member inline For(self : UniversalSequenceBuilder<_, _>, s, [<IIL>]_f) =
    UniversalSequence.``for``
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      s _f
      