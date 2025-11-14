namespace Sona.Runtime.Coroutines

open System
open System.Threading
open Sona.Runtime.Reflection

open CoroutineHelpers

module Coroutine =
  [<CompiledName("Running")>]
  let inline running<'TInput, 'TElement>() : ICoroutine<'TInput, 'TElement, IIterableCoroutine<'TInput, 'TElement>> =
   let mutable result = Unchecked.defaultof<IIterableCoroutine<'TInput, 'TElement>> in {
    new CoroutineBase<'TInput, 'TElement, IIterableCoroutine<'TInput, 'TElement>>() with

    member _.State =
      match &result with
      | NonNullRef result -> Finished result
      | _ -> Paused
    
    member _.TryResume(context : CoroutineContext) =
      match context with
      | CoroutineContext.Running value ->
        initOnce &result value
      | _ -> false

    member this.TryResume(value : 'TInput, context : CoroutineContext) =
      if Type<'TInput>.IsEmptyDefaultValue value then
        this.TryResume(context)
      else
        false
    
    member _.Dispose() = ()
   }

  [<AbstractClass; Sealed>]
  type internal ZeroCoroutine<'TInput, 'TElement> private() =
    static member val Instance = {
      new CoroutineBase<'TInput, 'TElement, unit>() with
    
      member _.State = Finished()
      member _.TryResume(_ : CoroutineContext) = false
      member _.TryResume(_ : 'TInput, _ : CoroutineContext) = false
      member _.Dispose() = ()
    }
    
  [<CompiledName("Zero")>]
  let zero() : ICoroutine<'TResumed, 'TElement, unit> = ZeroCoroutine<_, _>.Instance
  
  [<CompiledName("Pause")>]
  let inline ``pause``() : ICoroutine<'TResumed, 'TElement, 'TResumed> =
   let none = None : 'TResumed option
   let mutable result = none in {
    new CoroutineBase<'TResumed, 'TElement, 'TResumed>() with

    member _.State =
      match Volatile.Read(&result) with
      | Some value -> Finished value
      | None -> Paused
    
    member this.TryResume(context : CoroutineContext) =
      if Type<'TResumed>.HasEmptyDefaultValue then
        this.TryResume(Unchecked.defaultof<'TResumed>, context)
      else
        false

    member _.TryResume(input : 'TResumed, _ : CoroutineContext) =
      match Interlocked.CompareExchange(&result, Some input, none) with
      | Some _ -> false
      | None -> true
    
    member _.Dispose() = ()
   }
  
  [<CompiledName("Yield")>]
  let inline ``yield``(element : 'TElement) : ICoroutine<'TResumed, 'TElement, 'TResumed> =
   let none = None : 'TResumed option
   let mutable result = none in {
    new CoroutineBase<'TResumed, 'TElement, 'TResumed>() with

    member _.State =
      match Volatile.Read(&result) with
      | Some value -> Finished value
      | None -> Yielded element
    
    member this.TryResume(context : CoroutineContext) =
      if Type<'TResumed>.HasEmptyDefaultValue then
        this.TryResume(Unchecked.defaultof<'TResumed>, context)
      else
        false

    member _.TryResume(input : 'TResumed, _ : CoroutineContext) =
      match Interlocked.CompareExchange(&result, Some input, none) with
      | Some _ -> false
      | None -> true
    
    member _.Dispose() = ()
   }

  [<CompiledName("YieldFrom")>]
  let inline yieldFrom(coroutine : ICoroutine<_, _, unit>) = coroutine
  
  [<CompiledName("Return")>]
  let inline ``return``(result : 'TResult) : ICoroutine<'TInput, _, 'TResult> = {
    new CoroutineBase<'TInput, _, 'TResult>() with

    member _.State = Finished result
    member _.TryResume(_ : CoroutineContext) = false
    member _.TryResume(_ : 'TInput, _ : CoroutineContext) = false
    member _.Dispose() = ()
  }

  [<CompiledName("ReturnFrom")>]
  let inline returnFrom(coroutine : ICoroutine<_, _, _>) = coroutine
  
  [<CompiledName("Fault")>]
  let inline fault(reason : Exception) : ICoroutine<'TInput, _, _> = {
    new CoroutineBase<'TInput, _, _>() with

    member _.State = Faulted reason
    member _.TryResume(_ : CoroutineContext) = false
    member _.TryResume(_ : 'TInput, _ : CoroutineContext) = false
    member _.Dispose() = ()
  }
  
  [<CompiledName("FromFaulted")>]
  let inline fromFaulted (coroutine : ICoroutine<_, _, _>) (reason : Exception) =
    try
      coroutine.Dispose()
      fault reason
    with
    | e -> fault e
  
  [<CompiledName("Start")>]
  let inline start([<IIL>]_f : unit -> ICoroutine<'TInput, 'TElement, 'TResult>) : ICoroutine<'TInput, 'TElement, 'TResult> =
    try
      let inner = _f()
      {
        new CoroutineBase<'TInput, 'TElement, 'TResult>() with
    
        member _.State = inner.State
    
        member this.TryResume(context : CoroutineContext) =
          let context = CoroutineContext.derive context this
          inner.TryResume(context)
    
        member this.TryResume(input : 'TInput, context : CoroutineContext) =
          let context = CoroutineContext.derive context this
          inner.TryResume(input, context)
    
        member _.Dispose() =
          inner.Dispose()
       }
    with
    | e -> fault e
    
  let inline bindState (c : ICoroutine<'TInput, 'TElement, 'TIntermediate>) ([<IIL>]finishSelector : _ -> 'TArgument voption) ([<IIL>]stateSelector : CoroutineState<'TElement, 'TIntermediate> -> CoroutineState<'TElement, 'TResult>) ([<IIL>]finishHandler : 'TArgument -> ICoroutine<'TInput, 'TElement, 'TResult>) ([<IIL>]disposeHandler : unit -> unit) : ICoroutine<'TInput, 'TElement, 'TResult> =
    let inline doFinish result =
      try
        finishHandler result
      with
      // If follow-up code ends with an exception
      | e -> fault e

    match finishSelector c.State with
    | ValueSome result ->
      // Final result state
      doFinish result
    | _ ->
      match c.State with
      | Faulted reason ->
        // Ended in an exception - wrap
        fromFaulted c reason
      | _ ->
        let mutable next = Unchecked.defaultof<ICoroutine<'TInput, 'TElement, 'TResult>> 
        let inline tryFinish() =
          match finishSelector c.State with
          | ValueSome result ->
            let inner = doFinish result
            Interlocked.CompareExchange(&next, inner, Unchecked.defaultof<_>) |> ignore
            // New coroutine initialized
            true
          | _ -> false
        {
          new CoroutineBase<'TInput, 'TElement, 'TResult>() with
      
          member _.State =
            match &next with
            | NonNullRef value -> value.State
            | _ ->
              let state = c.State
              match finishSelector state with
              | ValueSome _ -> Paused
              | _ -> stateSelector state
      
          member _.TryResume(context : CoroutineContext) =
            match &next with
            | NonNullRef value -> value.TryResume(context)
            | _ ->
              if tryFinish() then
                // Moved to a new state
                true
              elif not(c.TryResume(context)) then
                // Can't resume original coroutine
                false
              else
                // Original moved, check if it can be replaced
                tryFinish() |> ignore
                true
      
          member _.TryResume(input : 'TInput, context : CoroutineContext) =
            match &next with
            | NonNullRef value -> value.TryResume(input, context)
            | _ ->
              if Type<'TInput>.IsEmptyDefaultValue input && tryFinish() then
                // Moved to a new state
                true
              elif not(c.TryResume(input, context)) then
                // Can't resume original coroutine
                false
              else
                // Original moved, check if it can be replaced
                tryFinish() |> ignore
                true
      
          member _.Dispose() =
            match &next with
            | NonNullRef value -> value.Dispose()
            | _ -> disposeHandler()
        }
  
  [<CompiledName("Bind")>]
  let inline bind (c : ICoroutine<'TInput, 'TElement, 'TArgument>) ([<IIL>]_f : 'TArgument -> ICoroutine<'TInput, 'TElement, 'TResult>) : ICoroutine<'TInput, 'TElement, 'TResult> =
    bindState
      c
      (
        // Finish selector
        function
        | Finished r -> ValueSome r
        | _ -> ValueNone
      )
      (
        // State selector
        function
        | Paused -> Paused
        | Yielded element -> Yielded element
        | Finished _ -> raise(InvalidOperationException "Invalid coroutine state.")
        | Faulted reason -> Faulted reason
      )
      (
        // Finish handler
        fun result ->
          c.Dispose()
          _f result
      )
      (
        // Dispose handler
        fun() -> c.Dispose()
      )

  [<CompiledName("Combine")>]
  let inline combine (first : ICoroutine<'TInput, 'TElement, unit>) ([<IIL>]_second : unit -> ICoroutine<'TInput, 'TElement, 'TResult>) : ICoroutine<'TInput, 'TElement, 'TResult> =
    bind first _second
  
  [<CompiledName("TryFinally")>]
  let inline tryFinally ([<IIL>]_f : unit -> ICoroutine<'TInput, 'TElement, 'TResult>) ([<IIL>]_cleanup : unit -> unit) : ICoroutine<'TInput, 'TElement, 'TResult> =
    let mutable c =
      let mutable received = false
      try
        let c = _f()
        received <- true
        c
      finally
        if not received then
          _cleanup()
    let mutable finalState : CoroutineState<_, _> = Paused
    {
      new CoroutineBase<'TInput, 'TElement, 'TResult>() with
      
      member _.State =
        match &c with
        | NonNullRef value -> value.State
        | _ -> finalState

      member _.TryResume(context : CoroutineContext) =
        match &c with
        | NonNullRef value -> value.TryResume(context)
        | _ -> false
      
      member _.TryResume(input : 'TInput, context : CoroutineContext) =
        match &c with
        | NonNullRef value -> value.TryResume(input, context)
        | _ -> false
      
      member _.Dispose() =
        match &c with
        | NonNullRef value ->
            finalState <- value.State
            if not(isNull(box(Interlocked.CompareExchange(&c, Unchecked.defaultof<_>, value)))) then
              finalState <- value.State
              try
                value.Dispose()
              finally
               _cleanup()
        | _ -> ()
    }
  
  [<CompiledName("TryWith")>]
  let inline tryWith ([<IIL>]_f : unit -> ICoroutine<'TInput, 'TElement, 'TResult>) ([<IIL>]_fail : exn -> ICoroutine<'TInput, 'TElement, 'TResult>) : ICoroutine<'TInput, 'TElement, 'TResult> =
    try
      let c = _f()
      bindState
        c
        (
          // Finish selector
          function
          | Faulted r -> ValueSome r
          | _ -> ValueNone
        )
        (
          // State selector
          function
          | Paused -> Paused
          | Yielded element -> Yielded element
          | Finished result -> Finished result
          | Faulted _ -> raise(InvalidOperationException "Invalid coroutine state.")
        )
        (
          // Finish handler
          fun result -> _fail(
            try
              c.Dispose()
              result
            with
            // Exception from dispose replaces original exception
            | e -> e
          )
        )
        (
          // Dispose handler
          fun() ->
            try
              c.Dispose()
            with
            | e ->
              // Exception during disposal can still be handled
              (_fail e).Dispose()
        )
    with
    | e -> _fail e

  [<CompiledName("Using")>]
  let inline using (x : #IDisposable) ([<IIL>]_f) =
    tryFinally (fun() -> _f(x)) (fun() -> x.Dispose())
  
  [<CompiledName("While")>]
  let inline ``while`` ([<IIL>]_cond : unit -> bool) ([<IIL>]_f : unit -> ICoroutine<'TInput, 'TElement, unit>) : ICoroutine<'TInput, 'TElement, unit> =
    if not(_cond()) then
      zero()
    else
      let mutable c = _f()

      let inline tryContinue() =
        let mutable continuing = true
        let mutable suspended = false
        let inline abort() =
          continuing <- false

        while continuing do
          let current = Volatile.Read(&c)
          match current.State with
          | Finished() ->
            let next =
              try
                // Move to next one
                current.Dispose()
                if _cond() then
                  _f()
                else
                  abort()
                  zero()
              with
              | e ->
                abort()
                fault e
            Interlocked.CompareExchange(&c, next, current) |> ignore
          | Faulted reason ->
            Interlocked.CompareExchange(&c, fromFaulted current reason, current) |> ignore
            abort()
          | _ ->
            suspended <- true
            abort()
        // Ended or yielded/paused 
        suspended

      if not(tryContinue()) then
        // Finished, just return the replacement
        c
      else
      {
        new CoroutineBase<'TInput, 'TElement, unit>() with

        member _.State = Volatile.Read(&c).State

        member _.TryResume(context : CoroutineContext) =
          let current = Volatile.Read(&c)
          if not(current.TryResume(context)) then
            // Finished
            false
          else
            // Moved
            tryContinue() |> ignore
            true

        member _.TryResume(input : 'TInput, context : CoroutineContext) =
          let current = Volatile.Read(&c)
          if not(current.TryResume(input, context)) then
            // Finished
            false
          else
            // Moved
            tryContinue() |> ignore
            true

        member _.Dispose() =
          Volatile.Read(&c).Dispose()
      }

  [<CompiledName("For")>]
  let inline ``for`` (s : _ seq) ([<IIL>]_f) =
    let enumerator = s.GetEnumerator()
    tryFinally
      (fun() -> (
        ``while``
          (fun() -> enumerator.MoveNext())
          (fun() -> _f enumerator.Current)
      ))
      (fun() -> enumerator.Dispose())
  
  [<CompiledName("Create")>]
  let inline create([<IIL>]_f : 'TInput -> ICoroutine<'TInput, 'TElement, 'TResult>) : ICoroutine<'TInput, 'TElement, 'TResult> =
    start (fun() ->
      bind
        (pause())
        (fun input -> _f input)
    )
