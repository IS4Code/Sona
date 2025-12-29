module Sona.Runtime.Computations

open System.Runtime.CompilerServices
open Sona.Runtime.ComputationBuilders
open Sona.Runtime.Collections

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
  
[<CompiledName("CoroutineBuilder")>]
let coroutine = { new CoroutineBuilder() with member _.ToString() = "coroutine" }
[<CompiledName("SequenceBuilder")>]
let sequence = { new SequenceBuilder() with member _.ToString() = "sequence" }
[<CompiledName("OptionBuilder")>]
let option = { new OptionBuilder() with member _.ToString() = "option" }
[<CompiledName("ValueOptionBuilder")>]
let voption = { new ValueOptionBuilder() with member _.ToString() = "voption" }
[<CompiledName("ResultBuilder")>]
let result<'TError> = ResultBuilderImpl<'TError>.Instance
[<CompiledName("ErrorResultBuilder")>]
let errorResult<'TSuccess> = ErrorResultBuilderImpl<'TSuccess>.Instance
[<CompiledName("DelayedBuilder")>]
let delayed = { new DelayedBuilder() with member _.ToString() = "delayed" }
[<CompiledName("ImmediateBuilder")>]
let immediate = { new ImmediateBuilder() with member _.ToString() = "immediate" }
[<CompiledName("ParallelBuilder")>]
let ``parallel`` = { new ParallelBuilder() with member _.ToString() = "parallel" }
[<CompiledName("GlobalBuilder")>]
let ``global`` = { new GlobalBuilder() with member _.ToString() = "global" }

[<CompiledName("ParallelOptionsBuilder")>]
let inline parallelOptions options : ParallelOptionsBuilder = { Options = options }

[<CompiledName("UniversalSequenceBuilder")>]
let inline sequenceVia builder : UniversalSequenceBuilder<_, _> = { BoolBuilder = builder; UnitBuilder = builder }

open System.Threading.Tasks

let inline private startTask a ct =
  Async.StartImmediateAsTask(a, cancellationToken = ct)

let inline private task (t : Task<_>) =
  t :> Task

type AsyncBuilder with
  member _.MergeSources(x : Async<_>, y : Async<_>) =
    async {
      let! ct = Async.CancellationToken
      let inline start t = startTask t ct
      let t1 = start x
      let t2 = start y

      do! Task.WhenAll(task t1, task t2) |> Async.AwaitTask

      return (t1.Result, t2.Result)
    }

  member _.Bind2(a1 : Async<_>, a2 : Async<_>, _func) =
    async {
      let! ct = Async.CancellationToken
      let inline start t = startTask t ct
      let t1 = start a1
      let t2 = start a2

      do! Task.WhenAll(task t1, task t2) |> Async.AwaitTask

      return! _func(t1.Result, t2.Result)
    }

  member _.Bind3(a1 : Async<_>, a2 : Async<_>, a3 : Async<_>, _func) =
    async {
      let! ct = Async.CancellationToken
      let inline start t = startTask t ct
      let t1 = start a1
      let t2 = start a2
      let t3 = start a3

      do! Task.WhenAll(task t1, task t2, task t3) |> Async.AwaitTask

      return! _func(t1.Result, t2.Result, t3.Result)
    }

  member _.Bind4(a1 : Async<_>, a2 : Async<_>, a3 : Async<_>, a4 : Async<_>, _func) =
    async {
      let! ct = Async.CancellationToken
      let inline start t = startTask t ct
      let t1 = start a1
      let t2 = start a2
      let t3 = start a3
      let t4 = start a4

      do! Task.WhenAll(task t1, task t2, task t3, task t4) |> Async.AwaitTask

      return! _func(t1.Result, t2.Result, t3.Result, t4.Result)
    }

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
      