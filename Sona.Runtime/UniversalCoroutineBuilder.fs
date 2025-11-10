namespace Sona.Runtime.Coroutines

open System.Threading
open Sona.Runtime.Reflection

open CoroutineHelpers

module UniversalCoroutine =
  [<CompiledName("Running")>]
  let inline running([<IIL>]unitFactory : unit -> 'TUnitMonad) : IUniversalCoroutine<'TInput, 'TElement, IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad>, 'TUnitMonad> =
   let mutable result = Unchecked.defaultof<IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad>> in {
    new UniversalCoroutineBase<'TInput, 'TElement, IUniversalIterableCoroutine<'TInput, 'TElement, 'TUnitMonad>, 'TUnitMonad>() with

    member _.State =
      match &result with
      | NonNullRef result -> Finished result
      | _ -> Paused
    
    member _.TryResume(context : CoroutineContext) =
      match context with
      | CoroutineContext.Running value ->
        if initOnce &result value then ValueSome(unitFactory())
        else ValueNone
      | _ -> ValueNone

    member this.TryResume(value : 'TInput, context : CoroutineContext) : 'TUnitMonad voption =
      if Type<'TInput>.IsEmptyDefaultValue value then
        this.TryResume(context)
      else
        ValueNone
    
    member _.DisposeUniversal() = unitFactory()
   }
    
  [<CompiledName("Yield")>]
  let inline ``yield`` ([<IIL>]unitFactory : unit -> 'TUnitMonad) (element : 'TElement) : IUniversalCoroutine<'TResumed, 'TElement, 'TResumed, 'TUnitMonad> =
   let none = None : 'TResumed option
   let mutable result = none in {
    new UniversalCoroutineBase<'TResumed, 'TElement, 'TResumed, 'TUnitMonad>() with

    member _.State =
      match Volatile.Read(&result) with
      | Some value -> Finished value
      | None -> Yielded element
    
    member this.TryResume(context : CoroutineContext) =
      if Type<'TResumed>.HasEmptyDefaultValue then
        this.TryResume(Unchecked.defaultof<'TResumed>, context)
      else
        ValueNone

    member _.TryResume(input : 'TResumed, _ : CoroutineContext) =
      match Interlocked.CompareExchange(&result, Some input, none) with
      | Some _ -> ValueNone
      | None -> ValueSome(unitFactory())
    
    member _.DisposeUniversal() = unitFactory()
   }
