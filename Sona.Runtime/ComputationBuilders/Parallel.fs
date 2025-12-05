namespace Sona.Runtime.ComputationBuilders

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Sona.Runtime.Reflection

#nowarn "64" // This construct causes code to be less generic than indicated by the type annotations.

module Parallel =
  let inline internal finish(result : ParallelLoopResult) =
    if not(result.IsCompleted) then
      raise(OperationCanceledException "The parallel operation was canceled.")
  
  [<return: Struct>]
  let inline (|Sequence|_|) (body : objnull) (s : objnull) =
    match struct(s, body) with
    | ((:? ('T seq) as s), (:? ('T -> unit) as body)) ->
    ValueSome(s, body)
    | _ -> ValueNone
  
  [<NoCompilerInlining>]
  let forBoxed<'TElement> (s : objnull) (opt : ParallelOptions voption) (body : 'TElement -> unit) =
    let inline parallelForInt (start : int) (stop : int) (body : int -> unit) =
      match opt with
      | ValueSome opt -> Parallel.For(start, stop, opt, body)
      | ValueNone -> Parallel.For(start, stop, body)
      |> finish
      true

    let inline parallelForLong (start : int64) (stop : int64) (body : int64 -> unit) =
      match opt with
      | ValueSome opt -> Parallel.For(start, stop, opt, body)
      | ValueNone -> Parallel.For(start, stop, body)
      |> finish
      true

    let inline forInt s body fromInt =
      match Type<_>.TryGetSequenceBounds s with
      | ValueSome(start, step, stop) when step = LanguagePrimitives.GenericOne ->
        parallelForInt (int start) (int(stop) + 1) (fun i -> body(fromInt i))
      | _ -> false

    let inline forLong s body fromLong =
      match Type<_>.TryGetSequenceBounds s with
      | ValueSome(start, step, stop) when step = LanguagePrimitives.GenericOne ->
        parallelForLong (int64 start) (int64(stop) + 1L) (fun i -> body(fromLong i))
      | _ -> false

    let boxed = box body
    match s with
    | :? ('TElement[]) as arr ->
      // TODO: Use LongLength but that needs indexing with long
      parallelForInt 0 arr.Length (fun i -> body(arr[i]))
    | :? IList<'TElement> as list ->
      parallelForInt 0 list.Count (fun i -> body(list[i]))
    | Sequence boxed (s, body) -> forInt s body int8
    | Sequence boxed (s, body) -> forInt s body uint8
    | Sequence boxed (s, body) -> forInt s body int16
    | Sequence boxed (s, body) -> forInt s body uint16
    | Sequence boxed (s, body) -> forInt s body int32
    | Sequence boxed (s, body) -> forInt s body uint32
    | Sequence boxed (s, body) -> forLong s body int64
    | Sequence boxed (s, body) -> forLong s body uint64
    | Sequence boxed (s, body) when IntPtr.Size <= 4 -> forInt s body nativeint
    | Sequence boxed (s, body) when IntPtr.Size <= 4 -> forInt s body unativeint
    | Sequence boxed (s, body) -> forLong s body nativeint
    | Sequence boxed (s, body) -> forLong s body unativeint
    | _ -> false

  let inline forEach (s : 'TCollection) (opt : ParallelOptions voption) ([<IIL>]_body : 'TElement -> unit) =
    if not(forBoxed (box s) opt _body) then
      let action = Action<'TElement>(_body)

      // Force TParallel to be Parallel for the constrained member lookup
      let _ : ^TParallel = Unchecked.defaultof<Parallel>
      match opt with
      | ValueSome opt -> ((^TParallel or ^TCollection) : (static member ForEach : ^TCollection * ParallelOptions * Action<'TElement> -> ParallelLoopResult) s, opt, action)
      | ValueNone -> ((^TParallel or ^TCollection) : (static member ForEach : ^TCollection * Action<'TElement> -> ParallelLoopResult) s, action)
      |> finish

  [<return: Struct>]
  let inline private (|AggregateCompatible|_|) (fail : 'T -> unit when 'T :> exn) (e : exn) =
    match e with
    | :? AggregateException as ae ->
      let rest = ResizeArray<exn>()
      let tasks : Action array = [|
        for inner in ae.InnerExceptions do
          match inner with
          | :? 'T as e ->
            // Compatible exception
            yield Action(fun() -> fail e)
          | e -> rest.Add e
      |]
      if tasks.Length > 0 then
        // Some exceptions are compatible with the handler
        ValueSome(struct(tasks, rest))
      else
        ValueNone
    | _ -> ValueNone

  let catch (e : exn) (opt : ParallelOptions voption) (fail : 'T -> unit when 'T :> exn) =
    match e with
    | (:? AggregateException) & (:? 'T as ae) when typeof<AggregateException>.IsAssignableFrom typeof<'T> ->
      // Handler expects AggregateException
      fail ae
    | AggregateCompatible fail (tasks, rest) ->
      try
        match opt with
        | ValueSome opt -> Parallel.Invoke(opt, tasks)
        | ValueNone -> Parallel.Invoke(tasks)
      with
      | :? AggregateException as ae when rest.Count > 0 ->
        // Exception handlers ended with a failure and there are previous unhandled exceptions
        raise(AggregateException(System.Linq.Enumerable.Concat(rest, ae.InnerExceptions)))
      if rest.Count > 0 then
        // There are unhandled exceptions
        raise(AggregateException(rest))
    | :? 'T as e ->
      // Not an aggregate (other issue)
      fail e
    | e ->
      // Incompatible exception
      Sona.Runtime.CompilerServices.Operators.Rethrow e

[<Struct; IsReadOnly; NoEquality; NoComparison>]
type Parallelized<'T> = { Value : 'T }

[<AbstractClass>]
type ParallelBuilderBase() =
  member inline _.Run([<IIL>]_f : _ -> Parallelized<_>) = _f().Value
  member inline _.Delay([<IIL>]_f : unit -> Immediate<_>) = _f
  member inline _.Delay([<IIL>]_f : unit -> Parallelized<_>) = _f
  
  member inline _.ReturnFrom(x : Immediate<_>) : Immediate<_> = x
  member inline _.ReturnFrom(x : Parallelized<_>) : Parallelized<_> = x
  member inline _.Return value : Immediate<_> = { Value = value }
  member inline this.Zero() = this.Return(())

  member inline _.Bind(x : Immediate<_>, [<IIL>]_f : _ -> Immediate<_>) = _f x.Value
  member inline _.Bind(x : Immediate<_>, [<IIL>]_f : _ -> Parallelized<_>) = _f x.Value
  member inline _.Bind(x : Parallelized<_>, [<IIL>]_f : _ -> Immediate<_>) : Parallelized<_> = { Value = (_f x.Value).Value }
  member inline _.Bind(x : Parallelized<_>, [<IIL>]_f : _ -> Parallelized<_>) = _f x.Value
  member inline this.Combine(x : Immediate<unit>, [<IIL>]_f : unit -> Immediate<_>) = this.Bind(x, _f)
  member inline this.Combine(x : Immediate<unit>, [<IIL>]_f : unit -> Parallelized<_>) = this.Bind(x, _f)
  member inline this.Combine(x : Parallelized<unit>, [<IIL>]_f : unit -> Immediate<_>) = this.Bind(x, _f)
  member inline this.Combine(x : Parallelized<unit>, [<IIL>]_f : unit -> Parallelized<_>) = this.Bind(x, _f)
  
[<AbstractClass>]
type ParallelBuilder() =
  inherit ParallelBuilderBase()

  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try _f()
    finally _cleanup()

  member inline _.Using(x : #IDisposable, [<IIL>]_f : _ -> _) =
    try _f x
    finally x.Dispose()

  member inline _.TryWith([<IIL>]_f : unit -> Immediate<_>, [<IIL>]_fail : exn -> Immediate<_>) =
    try _f()
    with | e -> _fail e

  member inline _.TryWith([<IIL>]_f : unit -> Immediate<_>, [<IIL>]_fail : exn -> Parallelized<_>) =
    try { Value = _f().Value }
    with | e -> _fail e

  member inline _.TryWith([<IIL>]_f : unit -> Parallelized<_>, [<IIL>]_fail : #exn -> Immediate<_>) =
    try _f()
    with | e -> { Value = Parallel.catch e ValueNone (fun e -> (_fail e).Value) }

  member inline _.For(s, [<IIL>]_f : 'TElement -> Immediate<unit>) : Parallelized<_> =
    Parallel.forEach s ValueNone (fun e -> (_f e).Value)
    { Value = () }

[<Struct>]
type ParallelOptionsBuilder = { Options : ParallelOptions } with
  member inline _.Run([<IIL>]_f : _ -> Parallelized<_>) = _f().Value
  member inline _.Delay([<IIL>]_f : unit -> Immediate<_>) = _f
  member inline _.Delay([<IIL>]_f : unit -> Parallelized<_>) = _f
  
  member inline _.ReturnFrom(x : Immediate<_>) : Immediate<_> = x
  member inline _.ReturnFrom(x : Parallelized<_>) : Parallelized<_> = x
  member inline _.Return value : Immediate<_> = { Value = value }
  member inline this.Zero() = this.Return(())

  member inline _.Bind(x : Immediate<_>, [<IIL>]_f : _ -> Immediate<_>) = _f x.Value
  member inline _.Bind(x : Immediate<_>, [<IIL>]_f : _ -> Parallelized<_>) = _f x.Value
  member inline _.Bind(x : Parallelized<_>, [<IIL>]_f : _ -> Immediate<_>) : Parallelized<_> = { Value = (_f x.Value).Value }
  member inline _.Bind(x : Parallelized<_>, [<IIL>]_f : _ -> Parallelized<_>) = _f x.Value
  member inline this.Combine(x : Immediate<unit>, [<IIL>]_f : unit -> Immediate<_>) = this.Bind(x, _f)
  member inline this.Combine(x : Immediate<unit>, [<IIL>]_f : unit -> Parallelized<_>) = this.Bind(x, _f)
  member inline this.Combine(x : Parallelized<unit>, [<IIL>]_f : unit -> Immediate<_>) = this.Bind(x, _f)
  member inline this.Combine(x : Parallelized<unit>, [<IIL>]_f : unit -> Parallelized<_>) = this.Bind(x, _f)
  
  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try _f()
    finally _cleanup()

  member inline _.Using(x : #IDisposable, [<IIL>]_f : _ -> _) =
    try _f x
    finally x.Dispose()

  member inline _.TryWith([<IIL>]_f : unit -> Immediate<_>, [<IIL>]_fail : exn -> Immediate<_>) =
    try _f()
    with | e -> _fail e

  member inline _.TryWith([<IIL>]_f : unit -> Immediate<_>, [<IIL>]_fail : exn -> Parallelized<_>) =
    try { Value = _f().Value }
    with | e -> _fail e

  member inline this.TryWith([<IIL>]_f : unit -> Parallelized<_>, [<IIL>]_fail : #exn -> Immediate<_>) =
    try _f()
    with | e -> { Value = Parallel.catch e (ValueSome this.Options) (fun e -> (_fail e).Value) }

  member inline this.For(s, [<IIL>]_f : 'TElement -> Immediate<unit>) : Parallelized<_> =
    Parallel.forEach s (ValueSome this.Options) (fun e -> (_f e).Value)
    { Value = () }
