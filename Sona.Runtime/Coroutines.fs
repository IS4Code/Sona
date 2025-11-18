namespace Sona.Runtime.Coroutines

open System
open System.Threading
open Sona.Runtime.Reflection

open CoroutineHelpers

module Coroutine =
  [<CompiledName("Running")>]
  let running<'TInput, 'TElement>() : ICoroutine<'TInput, 'TElement, IIterableCoroutine<'TInput, 'TElement>> = {
    new AtomicCoroutineBase<'TInput, 'TElement, IIterableCoroutine<'TInput, 'TElement>>(Paused) with

    member this.TryResume(context : CoroutineContext) =
      match context with
      | CoroutineContext.Running value ->
        this.TryFinish(Finished value)
      | _ -> false

    member this.TryResume(input : 'TInput, context : CoroutineContext) =
      Type<'TInput>.IsEmptyDefaultValue input
      && this.TryResume(context)
  }

  [<Sealed>]
  type internal ZeroCoroutine<'TInput, 'TElement> private() =
    inherit CompletedCoroutine<'TInput, 'TElement, unit>(Finished())

    static member val Instance = new ZeroCoroutine<'TInput, 'TElement>()
    
  [<CompiledName("Zero")>]
  let zero() : ICoroutine<'TResumed, 'TElement, unit> = ZeroCoroutine<_, _>.Instance
  
  [<CompiledName("Pause")>]
  let ``pause``() : ICoroutine<'TResumed, 'TElement, 'TResumed> = {
    new AtomicCoroutineBase<'TResumed, 'TElement, 'TResumed>(Paused) with

    member this.TryResume(context : CoroutineContext) =
      Type<'TResumed>.HasEmptyDefaultValue
      && this.TryResume(Unchecked.defaultof<'TResumed>, context)

    member this.TryResume(input : 'TResumed, _ : CoroutineContext) =
      this.TryFinish(Finished input)
  }
  
  [<CompiledName("Yield")>]
  let ``yield``(element : 'TElement) : ICoroutine<'TResumed, 'TElement, 'TResumed> = {
    new AtomicCoroutineBase<'TResumed, 'TElement, 'TResumed>(Yielded element) with

    member this.TryResume(context : CoroutineContext) =
      Type<'TResumed>.HasEmptyDefaultValue
      && this.TryResume(Unchecked.defaultof<'TResumed>, context)

    member this.TryResume(input : 'TResumed, _ : CoroutineContext) =
      this.TryFinish(Finished input)
  }

  [<CompiledName("YieldFrom")>]
  let inline yieldFrom(coroutine : ICoroutine<_, _, unit>) = coroutine
  
  [<CompiledName("Return")>]
  let ``return``(result : 'TResult) : ICoroutine<'TInput, 'TElement, 'TResult> = 
    new CompletedCoroutine<'TInput, 'TElement, 'TResult>(Finished result)

  [<CompiledName("ReturnFrom")>]
  let inline returnFrom(coroutine : ICoroutine<_, _, _>) = coroutine
  
  [<CompiledName("Fault")>]
  let fault(reason : Exception) : ICoroutine<'TInput, 'TElement, 'TResult> =
    new CompletedCoroutine<'TInput, 'TElement, 'TResult>(Faulted reason)
  
  [<CompiledName("FromFaulted")>]
  let inline fromFaulted (coroutine : ICoroutine<_, _, _>) (reason : Exception) =
    try
      coroutine.Dispose()
      fault reason
    with
    | e -> fault e
  
  [<return: Struct>]
  let private (|DelegatedCoroutine|_|) (c : IIterableCoroutine<'TInput, 'TElement>) =
    match c with
    | :? IDelegatingCoroutine<'TInput, 'TElement> as c ->
      match c.Chain with
      | { 
          Active = ValueSome top
          Dependent = dependent
          WakeWhenResumed = wake
        } -> ValueSome(c, dependent, top, wake)
      | _ -> ValueNone
    | _ -> ValueNone
  
  [<CompiledName("Start")>]
  let start(f : unit -> ICoroutine<'TInput, 'TElement, 'TResult>) : ICoroutine<'TInput, 'TElement, 'TResult> =
    try
      let inner = f()

      match inner with
      | :? IDelegatingCoroutine<'TInput, 'TElement> as top ->
        // Operations can be propagated inside
        let mutable active = Unchecked.defaultof<IIterableCoroutine<'TInput, 'TElement>>
        let stack = ResizeArray<IDelegatingCoroutine<'TInput, 'TElement>>()

        let inline pop() =
          let index = stack.Count - 1
          let last = stack[index]
          stack.RemoveAt(index)
          last
        
        let update top =
          // Initialize
          active <- top

          // Continue to the innermost coroutine
          let mutable continuing = true
          while continuing do
            match active with
            | DelegatedCoroutine(bottom, middle, top, wake) ->
              if wake then
                // The coroutine needs to be informed of resumed events
                stack.Add(bottom)
              stack.AddRange(middle)
              active <- top
            | _ ->
              continuing <- false
        
        // Start from the argument
        update top

        let inline tryResume (context : CoroutineContext) (anyInput : bool) (resume : IResumableCoroutine<'TInput> -> CoroutineContext -> bool) : bool =
          match active.Status with
          // Waiting for external input
          | CoroutineStatus.Paused | CoroutineStatus.Yielded ->
            let mutable result = resume active context

            let mutable continuing = true
            while continuing do
              if stack.Count = 0 then
                // Nothing to inform the result of
                continuing <- false
              else
                // Inform the top coroutine about the result
                let top = pop()
                let { Success = ok; Updated = updated } = top.OnUpdated(anyInput, result)
                result <- ok
                
                if not result then
                  // No progress; stop here
                  continuing <- false
                  update top
                elif not updated then
                  // No change in outer; keep active
                  continuing <- false
                  stack.Add(top)
                else
                  // Top is updated, propagate downwards
                  active <- top
            result
          | _ ->
            try
              // Unexpected final state; recurse normally
              resume inner context
            finally
              // Rebuild
              stack.Clear()
              update top
        {
          new DelegatingCoroutineBase<'TInput, 'TElement, 'TResult>() with

          member _.State =
            match active.State with
            // Waiting for external input
            | Paused -> Paused
            | Yielded element -> Yielded element
            // Unexpected final state; recurse normally
            | _ -> inner.State

          member this.TryResume(context : CoroutineContext) =
            tryResume
              (CoroutineContext.derive this context)
              false
              (fun cor ctx -> cor.TryResume(ctx))

          member this.TryResume(input : 'TInput, context : CoroutineContext) =
            tryResume
              (CoroutineContext.derive this context)
              (Type<'TInput>.IsEmptyDefaultValue input)
              (fun cor ctx -> cor.TryResume(input, ctx))
              
          member this.TryResumeThrow(``exception`` : exn, context : CoroutineContext) =
            tryResume
              (CoroutineContext.derive this context)
              true
              (fun cor ctx -> cor.TryResumeThrow(``exception``, ctx))

          member _.Dispose() =
            inner.Dispose()
            
          // If chained, reuse existing information
          member _.Chain : CoroutineChain<'TInput, 'TElement> = {
            Active = ValueSome active
            Dependent = stack
            WakeWhenResumed = false
          }

          member _.OnUpdated(_ : bool, result : bool) = {
            Success = result
            Updated = false
          }
        }
      | _ -> {
        new DelegatingCoroutineBase<'TInput, 'TElement, 'TResult>() with
    
        member _.State = inner.State
    
        member this.TryResume(context : CoroutineContext) =
          let context = CoroutineContext.derive this context
          inner.TryResume(context)
    
        member this.TryResume(input : 'TInput, context : CoroutineContext) =
          let context = CoroutineContext.derive this context
          inner.TryResume(input, context)
    
        member _.Dispose() =
          inner.Dispose()
        
        // If chained, the context is already prepared
        member _.Chain : CoroutineChain<'TInput, 'TElement> = {
          Active = ValueSome inner
          Dependent = Array.Empty<_>()
          WakeWhenResumed = false
        }

        member _.OnUpdated(_ : bool, result : bool) = {
          Success = result
          Updated = false
        }
       }
    with
    | e -> fault e
    
  let bindState (c : ICoroutine<'TInput, 'TElement, 'TIntermediate>) (finishSelector : _ -> 'TArgument voption) (stateSelector : CoroutineState<'TElement, 'TIntermediate> -> CoroutineState<'TElement, 'TResult>) (finishHandler : 'TArgument -> ICoroutine<'TInput, 'TElement, 'TResult>) (disposeHandler : unit -> unit) : ICoroutine<'TInput, 'TElement, 'TResult> =
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
          new DelegatingCoroutineBase<'TInput, 'TElement, 'TResult>() with

          member _.State =
            match &next with
            | NonNullRef value -> value.State
            | _ ->
              let state = c.State
              match finishSelector state with
              | ValueSome _ -> Paused
              | _ -> stateSelector state

          member _.Chain : CoroutineChain<'TInput, 'TElement> =
            match &next with
            | NonNullRef value -> {
                Active = ValueSome value
                Dependent = Array.Empty<_>()
                WakeWhenResumed = false
              }
            | _ -> {
                Active = ValueSome c
                Dependent = Array.Empty<_>()
                WakeWhenResumed = true
              }

          member _.OnUpdated(anyInput : bool, result : bool) =
            match &next with
            | NonNullRef _ -> {
                Success = result
                Updated = false
              }
            | _ ->
              if result then
                // Inner moved, finish if possible
                {
                  Success = result
                  Updated = tryFinish()
                }
              else
                // Inner did not move, but maybe it was exposed as paused
                let ok = not(anyInput) && tryFinish()
                {
                  Success = ok
                  Updated = ok
                }

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
  let tryFinally (f : unit -> ICoroutine<'TInput, 'TElement, 'TResult>) (cleanup : unit -> unit) : ICoroutine<'TInput, 'TElement, 'TResult> =
    let mutable c =
      let mutable received = false
      try
        let c = f()
        received <- true
        c
      finally
        if not received then
          cleanup()
    let mutable finalState : CoroutineState<_, _> = Paused

    let inline dispose(value : ICoroutine<_, _, _>) =
      finalState <- value.State
      if not(isNull(box(Interlocked.CompareExchange(&c, Unchecked.defaultof<_>, value)))) then
        finalState <- value.State
        try
          value.Dispose()
        finally
         cleanup()

    {
      new DelegatingCoroutineBase<'TInput, 'TElement, 'TResult>() with
      
      member _.State =
        match &c with
        | NonNullRef value -> value.State
        | _ -> finalState
        
      member _.Chain : CoroutineChain<'TInput, 'TElement> = {
        Active =
          match &c with
          | NonNullRef value -> ValueSome value
          | _ -> ValueNone
        Dependent = Array.Empty<_>()
        WakeWhenResumed = true
      }
        
      member _.OnUpdated(_ : bool, result : bool) =
        {
          Success = result
          Updated =
            result &&
            match &c with
            | NonNullRef value ->
              match value.State with
              | Finished _ | Faulted _ ->
                dispose value
                true
              | _ -> false
            | _ -> false
        }
      
      member _.Dispose() =
        match &c with
        | NonNullRef value ->
          dispose value
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
  let ``while`` (cond : unit -> bool) (f : unit -> ICoroutine<'TInput, 'TElement, unit>) : ICoroutine<'TInput, 'TElement, unit> =
    if not(cond()) then
      zero()
    else
      let mutable c = f()

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
                if cond() then
                  f()
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
        new DelegatingCoroutineBase<'TInput, 'TElement, unit>() with

        member _.State = Volatile.Read(&c).State

        member _.Chain : CoroutineChain<'TInput, 'TElement> = {
          Active = ValueSome(Volatile.Read(&c))
          Dependent = Array.Empty<_>()
          WakeWhenResumed = true
        }

        member _.OnUpdated(_ : bool, result : bool) =
          {
            Success = result
            Updated = result && tryContinue()
          }

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
