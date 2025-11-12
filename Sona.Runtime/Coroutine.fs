namespace Sona.Runtime.Coroutines

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open Sona.Runtime
open Sona.Runtime.Reflection

[<Struct; IsReadOnly>]
type CoroutineState<'TElement, 'TResult> =
| Paused
| Yielded of element : 'TElement
| Finished of result : 'TResult
| Faulted of reason : Exception

type CoroutineStatus =
| Paused = 0
| Yielded = 1
| Finished = 2
| Faulted = 3

[<Interface>]
type IUniversalCoroutine =
  abstract member Status : CoroutineStatus with get
  abstract member State : CoroutineState<objnull, objnull> with get
  abstract member Exception : Exception with get
  abstract member Current : objnull with get
  abstract member Result : objnull with get

[<Struct; NoEquality; NoComparison>]
type CoroutineContext internal =
  val mutable internal Coroutine : objnull

module CoroutineContext =
  [<return: Struct>]
  let (|Running|_|) (ctx : CoroutineContext) : 'T voption when 'T :> IUniversalCoroutine =
    match ctx.Coroutine with
    | :? 'T as result -> ValueSome result
    | _ -> ValueNone

  let derive (ctx : CoroutineContext) (coroutine : 'T when 'T :> IUniversalCoroutine) =
    match ctx with
    | Running(_ : 'T) -> ctx
    | _ -> CoroutineContext(Coroutine = coroutine)

[<Interface>]
type IUniversalCoroutine<'TUnitMonad> =
  inherit IUniversalCoroutine
  inherit IUniversalDisposable<'TUnitMonad>
  abstract member Resume : [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> 'TUnitMonad
  abstract member TryResume : [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> 'TUnitMonad voption
  abstract member TryResume : input : objnull * [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> 'TUnitMonad voption
  
[<Interface>]
type IUniversalResumableCoroutine<'TInput, 'TUnitMonad> =
  inherit IUniversalCoroutine<'TUnitMonad>
  abstract member Resume : input : 'TInput * [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> 'TUnitMonad
  abstract member TryResume : input : 'TInput * [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> 'TUnitMonad voption

[<Interface>]
type IUniversalYieldableCoroutine<'TElement> =
  inherit IUniversalCoroutine
  abstract member Current : 'TElement with get

[<Interface>]
type IUniversalYieldableCoroutine<'TElement, 'TUnitMonad> =
  inherit IUniversalYieldableCoroutine<'TElement>
  inherit IUniversalCoroutine<'TUnitMonad>

[<Interface>]
type IUniversalFinishableCoroutine<'TResult> =
  inherit IUniversalCoroutine
  abstract member Result : 'TResult with get

[<Interface>]
type IUniversalFinishableCoroutine<'TResult, 'TUnitMonad> =
  inherit IUniversalFinishableCoroutine<'TResult>
  inherit IUniversalCoroutine<'TUnitMonad>

[<Interface>]
type IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad> =
  inherit IUniversalResumableCoroutine<'TInput, 'TUnitMonad>
  inherit IUniversalYieldableCoroutine<'TElement, 'TUnitMonad>

[<Interface>]
type IUniversalReadOnlyCoroutine<'TElement, 'TResult> =
  inherit IUniversalYieldableCoroutine<'TElement>
  inherit IUniversalFinishableCoroutine<'TResult>
  abstract member State : CoroutineState<'TElement, 'TResult> with get

[<Interface>]
type IUniversalReadOnlyCoroutine<'TElement, 'TResult, 'TUnitMonad> =
  inherit IUniversalReadOnlyCoroutine<'TElement, 'TResult>
  inherit IUniversalYieldableCoroutine<'TElement, 'TUnitMonad>
  inherit IUniversalFinishableCoroutine<'TResult, 'TUnitMonad>

[<Interface>]
type IUniversalCoroutine<'TInput, 'TElement, 'TResult, 'TUnitMonad> =
  inherit IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad>
  inherit IUniversalReadOnlyCoroutine<'TElement, 'TResult, 'TUnitMonad>

[<Interface>]
type ICoroutine =
  inherit IUniversalCoroutine<unit>
  inherit IDisposable
  abstract member Resume : [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> unit
  abstract member TryResume : [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> bool
  abstract member TryResume : input : objnull * [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> bool
  
[<Interface>]
type IResumableCoroutine<'TInput> =
  inherit ICoroutine
  inherit IUniversalResumableCoroutine<'TInput, unit>
  abstract member Resume : input : 'TInput * [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> unit
  abstract member TryResume : input : 'TInput * [<Optional; DefaultParameterValue(CoroutineContext())>]context : CoroutineContext -> bool

[<Interface>]
type IYieldableCoroutine<'TElement> =
  inherit ICoroutine
  inherit IUniversalYieldableCoroutine<'TElement, unit>
  
[<Interface>]
type IFinishableCoroutine<'TResult> =
  inherit ICoroutine
  inherit IUniversalFinishableCoroutine<'TResult, unit>

[<Interface>]
type IIterableCoroutine<'TInput, 'TElement> =
  inherit IResumableCoroutine<'TInput>
  inherit IYieldableCoroutine<'TElement>
  inherit IUniversalIterableCoroutine<'TInput, 'TElement, unit>

[<Interface>]
type IReadOnlyCoroutine<'TElement, 'TResult> =
  inherit IYieldableCoroutine<'TElement>
  inherit IFinishableCoroutine<'TResult>
  inherit IUniversalReadOnlyCoroutine<'TElement, 'TResult, unit>

[<Interface>]
type ICoroutine<'TInput, 'TElement, 'TResult> =
  inherit IIterableCoroutine<'TInput, 'TElement>
  inherit IReadOnlyCoroutine<'TElement, 'TResult>
  inherit IUniversalCoroutine<'TInput, 'TElement, 'TResult, unit>

module internal CoroutineHelpers =
  [<Literal>]
  let notYieldedException = "The coroutine has not currently yielded any element."
  
  [<Literal>]
  let notFinishedException = "The coroutine has not finished yet."
  
  [<Literal>]
  let notFaultedException = "The coroutine has not faulted."
  
  [<Literal>]
  let notResumableException = "The coroutine cannot be resumed with an argument."
  
  [<Literal>]
  let notStartableException = "The coroutine cannot be resumed without an argument."
  
  [<Literal>]
  let invalidException = "The coroutine transitioned into an invalid state."
  
  [<return: Struct>]
  let inline (|NonNullRef|_|) (location : byref<'T>) : 'T voption when 'T : not struct =
    let value = Volatile.Read(&location)
    if isNull(box value) then ValueNone
    else ValueSome value
  
  let inline initOnce (location : byref<'T>) (value : 'T) : bool when 'T : not struct =
    isNull(box(Interlocked.CompareExchange(&location, value, Unchecked.defaultof<_>)))
    
  type IIL = InlineIfLambdaAttribute

open CoroutineHelpers

[<AbstractClass>]
type UniversalReadOnlyCoroutineBase<'TElement, 'TResult, 'TUnitMonad>() =
  abstract member State : CoroutineState<'TElement, 'TResult> with get
  abstract member TryResume : context : CoroutineContext -> 'TUnitMonad voption
  abstract member TryResume : input : objnull * context : CoroutineContext -> 'TUnitMonad voption
  abstract member DisposeUniversal: unit -> 'TUnitMonad

  interface IUniversalReadOnlyCoroutine<'TElement, 'TResult, 'TUnitMonad> with
    member this.State : CoroutineState<'TElement,'TResult> = this.State
    
    member this.State : CoroutineState<objnull, objnull> =
      match this.State with
      | Paused -> Paused
      | Yielded element -> Yielded (box element)
      | Finished result -> Finished (box result)
      | Faulted reason -> Faulted reason
    
    member this.Status : CoroutineStatus =
      match this.State with
      | Paused -> CoroutineStatus.Paused
      | Yielded _ -> CoroutineStatus.Yielded
      | Finished _ -> CoroutineStatus.Finished
      | Faulted _ -> CoroutineStatus.Faulted
    
    member this.Result : 'TResult =
      match this.State with
      | Finished result -> result
      | _ -> raise(InvalidOperationException notFinishedException)
    
    member this.Result : objnull =
      match this.State with
      | Finished result -> box result
      | _ -> raise(InvalidOperationException notFinishedException)
    
    member this.Current : 'TElement =
      match this.State with
      | Yielded element -> element
      | Paused when Type<'TElement>.HasEmptyDefaultValue -> Unchecked.defaultof<'TElement>
      | _ -> raise(InvalidOperationException notYieldedException)
    
    member this.Current : objnull =
      match this.State with
      | Yielded element -> box element
      | Paused when Type<'TElement>.HasEmptyDefaultValue -> box Unchecked.defaultof<'TElement>
      | _ -> raise(InvalidOperationException notYieldedException)
    
    member this.Exception : Exception =
      match this.State with
      | Faulted reason -> reason
      | _ -> raise(InvalidOperationException notFaultedException)
    
    member this.TryResume(context : CoroutineContext) : 'TUnitMonad voption =
      this.TryResume(context)
    
    member this.Resume(context : CoroutineContext) : 'TUnitMonad =
      match this.TryResume(context) with
      | ValueSome result -> result
      | ValueNone -> raise(InvalidOperationException notStartableException)
    
    member this.TryResume(input : objnull, context : CoroutineContext) : 'TUnitMonad voption =
      this.TryResume(input, context)
    
    member this.DisposeUniversal() : 'TUnitMonad = this.DisposeUniversal()

[<AbstractClass>]
type UniversalCoroutineBase<'TInput, 'TElement, 'TResult, 'TUnitMonad>() =
  inherit UniversalReadOnlyCoroutineBase<'TElement, 'TResult, 'TUnitMonad>()
  
  abstract member TryResume : input : 'TInput * context : CoroutineContext -> 'TUnitMonad voption

  override this.TryResume(input : objnull, context : CoroutineContext) =
    match input with
    | :? 'TInput as input' -> this.TryResume(input', context)
    | Null when Type<'TInput>.HasNullDefaultValue -> this.TryResume(Unchecked.defaultof<'TInput>, context)
    | _ -> ValueNone

  interface IUniversalCoroutine<'TInput, 'TElement, 'TResult, 'TUnitMonad> with
    member this.TryResume(input : 'TInput, context : CoroutineContext) : 'TUnitMonad voption = this.TryResume(input, context)
    member this.Resume(input : 'TInput, context : CoroutineContext) : 'TUnitMonad =
      match this.TryResume(input, context) with
      | ValueSome result -> result
      | ValueNone -> raise(InvalidOperationException notResumableException)

[<AbstractClass>]
type CoroutineBase<'TInput, 'TElement, 'TResult, 'TUnit>() =
  inherit UniversalCoroutineBase<'TInput, 'TElement, 'TResult, 'TUnit>()

  static do
    if not Type<'TUnit>.IsUnit then
      raise(ArgumentException("The type parameter must be unit.", nameof<'TUnit>))

  abstract member Dispose : unit -> unit

  override this.DisposeUniversal() = this.Dispose(); Unchecked.defaultof<_>

[<AbstractClass>]
type CoroutineBase<'TInput, 'TElement, 'TResult>() =
  inherit CoroutineBase<'TInput, 'TElement, 'TResult, unit>()
  
  abstract member TryResume : context : CoroutineContext -> bool
  abstract member TryResume : input : 'TInput * context : CoroutineContext -> bool

  override this.TryResume(context : CoroutineContext) : unit voption =
    if this.TryResume(context) then ValueSome()
    else ValueNone

  override this.TryResume(input : 'TInput, context : CoroutineContext) : unit voption =
    if this.TryResume(input, context) then ValueSome()
    else ValueNone
  
  interface ICoroutine<'TInput, 'TElement, 'TResult> with
    member this.Dispose() : unit = this.Dispose()

    member this.Resume(context : CoroutineContext) : unit =
      if this.TryResume(context) then ()
      else raise(InvalidOperationException notStartableException)

    member this.TryResume(context : CoroutineContext) : bool =
      this.TryResume(context)

    member this.Resume(input: 'TInput, context : CoroutineContext) : unit =
      if this.TryResume(input, context) then ()
      else raise(InvalidOperationException notResumableException)

    member this.TryResume(input : 'TInput, context : CoroutineContext) : bool =
      this.TryResume(input, context)

    member this.TryResume(input : objnull, context : CoroutineContext) : bool =
      match this.TryResume(input, context) with
      | ValueSome _ -> true
      | ValueNone -> false
