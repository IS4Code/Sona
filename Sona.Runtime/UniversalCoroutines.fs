namespace Sona.Runtime.Coroutines

open Sona.Runtime.Reflection

open CoroutineHelpers

module UniversalCoroutine =
  [<AbstractClass>]
  type RunningUniversalCoroutineBase<'TInput, 'TElement, 'TUnitMonad> internal() =
    inherit AtomicUniversalCoroutineBase<'TInput, 'TElement, IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad>, 'TUnitMonad>(Paused)

    interface IAutoResumeUniversalCoroutine

  [<CompiledName("Running")>]
  let inline running([<IIL>]unitFactory : unit -> 'TUnitMonad) : IUniversalCoroutine<'TInput, 'TElement, IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad>, 'TUnitMonad> = {
    new RunningUniversalCoroutineBase<'TInput, 'TElement, 'TUnitMonad>() with

    member this.TryResume(context : CoroutineContext) =
      match context with
      | CoroutineContext.Running value ->
        if this.TryFinish(Finished value) then ValueSome(unitFactory())
        else ValueNone
      | _ -> ValueNone

    member this.TryResume(value : 'TInput, context : CoroutineContext) =
      if Type<'TInput>.IsEmptyDefaultValue value then
        this.TryResume(context)
      else
        ValueNone

    member this.TryResumeThrow(``exception`` : exn, _ : CoroutineContext) =
      if this.TryFinish(Faulted ``exception``) then ValueSome(unitFactory())
      else ValueNone
    
    member _.DisposeUniversal() = unitFactory()
  }
    
  [<CompiledName("Yield")>]
  let inline ``yield`` ([<IIL>]unitFactory : unit -> 'TUnitMonad) (element : 'TElement) : IUniversalCoroutine<'TResumed, 'TElement, 'TResumed, 'TUnitMonad> = {
    new AtomicUniversalCoroutineBase<'TResumed, 'TElement, 'TResumed, 'TUnitMonad>(Yielded element) with

    member this.TryResume(context : CoroutineContext) =
      if Type<'TResumed>.HasEmptyDefaultValue then
        this.TryResume(Unchecked.defaultof<'TResumed>, context)
      else
        ValueNone

    member this.TryResume(value : 'TResumed, context : CoroutineContext) =
      if this.TryFinish(Finished value) then ValueSome(unitFactory())
      else ValueNone

    member this.TryResumeThrow(``exception`` : exn, _ : CoroutineContext) =
      if this.TryFinish(Faulted ``exception``) then ValueSome(unitFactory())
      else ValueNone
    
    member _.DisposeUniversal() = unitFactory()
  }
