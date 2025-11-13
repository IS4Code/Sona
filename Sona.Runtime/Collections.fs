namespace Sona.Runtime.Collections

open System
open Sona.Runtime

module private CollectionHelpers =
  [<Literal>]
  let emptySequenceException = "The enumerator is not pointing at any element."

  [<Literal>]
  let nonThreadSafeException = "The enumerator is not thread-safe."
  
  type internal IIL = InlineIfLambdaAttribute

open CollectionHelpers

module UniversalSequence =
  open System.Threading
  
  [<CompiledName("Zero")>]
  let inline zero boolBuilder ([<IIL>]boolWrap) unitBuilder ([<IIL>]unitWrap) = {
    new IUniversalEnumerator<_, _, _> with

    member _.Current = raise(InvalidOperationException emptySequenceException)

    member _.MoveNextUniversal() =
      boolWrap(fun() -> (
        (^B : (member Return : _ -> _) boolBuilder, false)
      ))

    member _.DisposeUniversal() =
      unitWrap(fun() -> (
        (^U : (member Return : _ -> _) unitBuilder, ())
      ))
  }
  
  [<CompiledName("Delay")>]
  let inline delay ([<IIL>]_f : unit -> IUniversalEnumerator<_, _, _>) = _f
  
  [<CompiledName("Run")>]
  let inline run ([<IIL>]_f) = {
    new IUniversalEnumerable<_, _, _> with
    member _.GetUniversalEnumerator() = _f()
  }
  
  [<CompiledName("Yield")>]
  let inline ``yield`` boolBuilder ([<IIL>]boolWrap) unitBuilder ([<IIL>]unitWrap) x =
   let mutable state = -1 in {
    new IUniversalEnumerator<_, _, _> with

    member _.Current =
      match Volatile.Read(&state) with
      | 0 -> x
      | _ -> raise(InvalidOperationException emptySequenceException)
    
    member _.MoveNextUniversal() =
      boolWrap(fun() -> (
        (^B : (member Return : _ -> _) boolBuilder, Interlocked.Increment(&state) = 0)
      ))
    
    member _.DisposeUniversal() =
      unitWrap(fun() -> (
        (^U : (member Return : _ -> _) unitBuilder, ())
      ))
   }
   
  [<CompiledName("Bind")>]
  let inline bind boolBuilder ([<IIL>]boolWrap) ([<IIL>]boolReturnFrom) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) m ([<IIL>]_f) =
   let mutable inner = Unchecked.defaultof<IUniversalEnumerator<_, _, _>> in {
    new IUniversalEnumerator<_, _, _> with

    member _.Current =
      if not(isNull(Volatile.Read(&inner) |> box)) then inner.Current
      else raise(InvalidOperationException emptySequenceException)

    member _.MoveNextUniversal() =
      if not(isNull(Volatile.Read(&inner) |> box)) then
        // Never changes once set
        inner.MoveNextUniversal()
      else
        boolWrap(fun() -> (
          (^B : (member Bind : _ * _ -> _) boolBuilder, m, fun x -> (
            // Initialize with the continuation
            if
              // Still empty
              isNull(Volatile.Read(&inner) |> box) &&
              // But non-empty before the value could be moved inside
              not(isNull(Interlocked.CompareExchange(&inner, _f x, Unchecked.defaultof<_>) |> box))
              then raise(InvalidOperationException nonThreadSafeException)
            
            // Return the inner result
            boolReturnFrom(inner.MoveNextUniversal())
          ))
        ))

    member _.DisposeUniversal() =
      if not(isNull(Volatile.Read(&inner) |> box)) then
        // Never changes once set
        inner.DisposeUniversal()
      else
        unitWrap(fun() -> (
          // Initialize with an empty value
          if
            // Still empty
            isNull(Volatile.Read(&inner) |> box) &&
            // But non-empty before the value could be moved inside
            not(isNull(Interlocked.CompareExchange(&inner, zero boolBuilder boolWrap unitBuilder unitWrap, Unchecked.defaultof<_>) |> box))
            then raise(InvalidOperationException nonThreadSafeException)
          
          // Return the inner result
          unitReturnFrom(inner.DisposeUniversal())
        ))
   }
   
  [<CompiledName("YieldFrom")>]
  let inline yieldFrom (x : IUniversalEnumerable<_, _, _>) = x.GetUniversalEnumerator()
  
  [<CompiledName("Combine")>]
  let inline combine boolBuilder ([<IIL>]boolWrap) ([<IIL>]boolReturnFrom) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) (first : IUniversalEnumerator<_, _, _>) ([<IIL>]_second : unit -> IUniversalEnumerator<_, _, _>) =
   let mutable inner = Unchecked.defaultof<IUniversalEnumerator<_, _, _>> in {
    new IUniversalEnumerator<_, _, _> with

    member _.Current =
      if not(isNull(Volatile.Read(&inner) |> box)) then inner.Current
      else raise(InvalidOperationException emptySequenceException)
    
    member _.MoveNextUniversal() =
      let enumerator = Volatile.Read(&inner)
      if not(isNull(enumerator |> box)) && not(Object.ReferenceEquals(first, enumerator |> box)) then
        // Delegate to the second enumerator (will never change)
        enumerator.MoveNextUniversal()
      else
        boolWrap(fun() -> (
          // Attempt to read
          let enumerator =
            let current = Volatile.Read(&inner)
            if isNull(current |> box) then
              // First read
              if not(isNull(Interlocked.CompareExchange(&inner, first, Unchecked.defaultof<_>) |> box)) then
                // Non-empty before the value could be moved inside
                raise(InvalidOperationException nonThreadSafeException)
              first
            else
              current
          
          // Enumerator is available
          if not(Object.ReferenceEquals(first, enumerator)) then
            // Is in the second enumerator, just delegate to it
            boolReturnFrom(enumerator.MoveNextUniversal())
          else
            // Decide based on the result
            (^B : (member Bind : _ * _ -> _) boolBuilder, enumerator.MoveNextUniversal(), fun hasNext -> (
              // Initialize with the continuation
              if hasNext then
                // Just return the information
                (^B : (member Return : _ -> _) boolBuilder, true)
              else
                // Cleanup and move to the next
                (^U : (member Bind : _ * _ -> _) unitBuilder, enumerator.DisposeUniversal(), fun() -> (
                  if
                    // Still on the first enumerator
                    Object.ReferenceEquals(first, Volatile.Read(&inner)) &&
                    // But changed before the new value could be moved inside
                    not(Object.ReferenceEquals(first, Interlocked.CompareExchange(&inner, _second(), first)))
                    then raise(InvalidOperationException nonThreadSafeException)
                  
                  // Return the inner result
                  boolReturnFrom(inner.MoveNextUniversal())
                ))
            ))
        ))
    
    member _.DisposeUniversal() =
      let enumerator = Volatile.Read(&inner)
      if not(isNull(enumerator |> box)) && not(Object.ReferenceEquals(first, enumerator |> box)) then
        // Delegate to the second enumerator (will never change)
        enumerator.DisposeUniversal()
      else
        unitWrap(fun() -> (
          // Initialize with an empty value
          if
            // Is empty
            isNull(Volatile.Read(&inner) |> box) &&
            // But non-empty before the value could be moved inside
            not(isNull(Interlocked.CompareExchange(&inner, zero boolBuilder boolWrap unitBuilder unitWrap, Unchecked.defaultof<_>) |> box))
            then raise(InvalidOperationException nonThreadSafeException)
          
          // Return the inner result
          unitReturnFrom(inner.DisposeUniversal())
        ))
   }
  
  [<CompiledName("TryFinally")>]
  let inline tryFinally boolBuilder ([<IIL>]boolWrap) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) ([<IIL>]_f) ([<IIL>]_cleanup : unit -> unit) =
    let mutable enumeratorReceived = false
    try
     let enumerator : IUniversalEnumerator<_, _, _> = _f()
     enumeratorReceived <- true

     let mutable finished = 0
     let inline isFinished() = 0 <> Volatile.Read(&finished)
     let inline doCleanup() = 
       if 0 = Interlocked.CompareExchange(&finished, 1, 0) then _cleanup()
     {
      new IUniversalEnumerator<_, _, _> with
        
      member _.Current = enumerator.Current

      member _.MoveNextUniversal() =
        if isFinished() then enumerator.MoveNextUniversal()
        else boolWrap(fun() -> (
          // Signal if there are more elements to read
          let mutable continuing = false
          
          let delayed = (^B : (member Delay : _ -> _) boolBuilder, fun() -> (
            (^B : (member Bind : _ * _ -> _) boolBuilder, enumerator.MoveNextUniversal(), fun hasNext -> (
              // Prevent cleanup
              if hasNext then continuing <- true
              (^B : (member Return : _ -> _) boolBuilder, hasNext)
            ))
          ))
          
          (^B : (member TryFinally : _ * _ -> _) boolBuilder, delayed, fun() -> (
            if not continuing then
              // There was an exception - cleanup immediately
              doCleanup()
          ))
        ))
      
      member _.DisposeUniversal() =
        if isFinished() then enumerator.DisposeUniversal()
        else unitWrap(fun() -> (
          let delayed = (^U : (member Delay : _ -> _) unitBuilder, fun() -> (
            unitReturnFrom(enumerator.DisposeUniversal())
          ))
          
          (^U : (member TryFinally : _ * _ -> _) unitBuilder, delayed, fun() -> doCleanup())
        ))
     }
    finally
     if not enumeratorReceived then
       _cleanup()
  
  [<CompiledName("TryWith")>]
  let inline tryWith boolBuilder ([<IIL>]boolWrap) ([<IIL>]boolReturnFrom) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) ([<IIL>]_f : unit -> IUniversalEnumerator<_, _, _>) ([<IIL>]_fail : exn -> IUniversalEnumerator<_, _, _>) =
    try
     let mutable enumerator : IUniversalEnumerator<_, _, _> = _f()

     let mutable failed = 0
     let inline isFailed() = 0 <> Volatile.Read(&failed)
     let inline doFail e = 
       if 0 <> Interlocked.CompareExchange(&failed, 1, 0) then
         // Already failed; propagate
         System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw()
     
     {
      new IUniversalEnumerator<_, _, _> with
  
      member _.Current = enumerator.Current
  
      member _.MoveNextUniversal() =
        if isFailed() then enumerator.MoveNextUniversal()
        else boolWrap(fun() -> (
          let delayed = (^B : (member Delay : _ -> _) boolBuilder, fun() -> (
            boolReturnFrom(enumerator.MoveNextUniversal())
          ))
          
          (^B : (member TryWith : _ * _ -> _) boolBuilder, delayed, fun e -> (
            doFail e
            
            // Signal if dispose was successful
            let mutable disposed = false

            let delayed = (^B : (member Delay : _ -> _) boolBuilder, fun() -> (
              (^U : (member Bind : _ * _ -> _) unitBuilder, enumerator.DisposeUniversal(), fun() -> (
                disposed <- true

                // Replace enumerator and start enumerating
                enumerator <- _fail e
                boolReturnFrom(enumerator.MoveNextUniversal())
              ))
            ))

            (^B : (member TryWith : _ * _ -> _) boolBuilder, delayed, fun e -> (
              if disposed then
                // The exception comes from the failure enumerator, propagate anyway
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw()
              
              // Disposing caused another exception, use this one
              enumerator <- _fail e
              boolReturnFrom(enumerator.MoveNextUniversal())
            ))
          ))
        ))
      
      member _.DisposeUniversal() =
        if isFailed() then enumerator.DisposeUniversal()
        else unitWrap(fun() -> (
          let delayed = (^U : (member Delay : _ -> _) unitBuilder, fun() -> (
            unitReturnFrom(enumerator.DisposeUniversal())
          ))
        
          (^U : (member TryWith : _ * _ -> _) unitBuilder, delayed, fun e -> (
            doFail e

            // Cleanup is requested but handler must be given a chance to run
            enumerator <- _fail e
            
            let delayed = (^U : (member Delay : _ -> _) unitBuilder, fun() -> (
              (^B : (member Bind : _ * _ -> _) boolBuilder, enumerator.MoveNextUniversal(), fun (_ : bool) -> (
                // We don't care about whether there is next element since the enumerator will be disposed
                (^U : (member Return : _ -> _) unitBuilder, ())
              ))
            ))

            (^U : (member TryFinally : _ * _ -> _) unitBuilder, delayed, fun() -> (
              // Finally dispose
              unitReturnFrom(enumerator.DisposeUniversal())
            ))
          ))
        ))
     }
    with
    | e -> _fail e
  
  [<CompiledName("Using")>]
  let inline using boolBuilder ([<IIL>]boolWrap) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) (x : #IDisposable) ([<IIL>]_f) =
    tryFinally boolBuilder boolWrap unitBuilder unitWrap unitReturnFrom (fun() -> _f(x)) (fun() -> x.Dispose())
  
  [<CompiledName("While")>]
  let inline ``while`` boolBuilder ([<IIL>]boolWrap) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) ([<IIL>]_cond : unit -> bool) ([<IIL>]_f : unit -> IUniversalEnumerator<_, _, _>) =
   let mutable inner = Unchecked.defaultof<IUniversalEnumerator<_, _, _>> in {
    new IUniversalEnumerator<_, _, _> with

    member _.Current =
      if not(isNull(Volatile.Read(&inner) |> box)) then inner.Current
      else raise(InvalidOperationException emptySequenceException)
    
    member _.MoveNextUniversal() =
      boolWrap(fun() -> (
        let rec loop (enumerator : IUniversalEnumerator<_, _, _>) =
          (^B : (member Bind : _ * _ -> _) boolBuilder, enumerator.MoveNextUniversal(), fun hasNext -> (
            // Initialize with the continuation
            if hasNext then
              // Just return the information
              (^B : (member Return : _ -> _) boolBuilder, true)
            else
              // Cleanup and move to the next
              (^U : (member Bind : _ * _ -> _) unitBuilder, enumerator.DisposeUniversal(), fun() -> (
                let current = Volatile.Read(&inner)
                if Object.ReferenceEquals(enumerator, current) then
                  // Still on the enumerator
                  if not(_cond()) then
                    // Terminate
                    (^B : (member Return : _ -> _) boolBuilder, false)
                  else
                    // Start next iteration
                    let next = _f()
                    if not(Object.ReferenceEquals(enumerator, Interlocked.CompareExchange(&inner, next, enumerator))) then
                      // But changed before the new value could be moved inside
                      raise(InvalidOperationException nonThreadSafeException)
                    // Try the next enumerator
                    loop next
                else
                  loop current
              ))
          ))

        // Attempt to read
        let current = Volatile.Read(&inner)
        if isNull(current |> box) then
          // First read
          if not(_cond()) then
            // Terminate
            (^B : (member Return : _ -> _) boolBuilder, false)
          else
            let next = _f()
            if not(isNull(Interlocked.CompareExchange(&inner, next, Unchecked.defaultof<_>) |> box)) then
              // Non-empty before the value could be moved inside
              raise(InvalidOperationException nonThreadSafeException)
            // Go inside
            loop next
        else
          loop current
      ))
    
    member _.DisposeUniversal() =
      unitWrap(fun() -> (
        let current = Volatile.Read(&inner)
        if isNull(current |> box) then
          // Did not even run
          (^U : (member Return : _ -> _) unitBuilder, ())
        else
          // Set to an empty value
          if not(Object.ReferenceEquals(current, Interlocked.CompareExchange(&inner, zero boolBuilder boolWrap unitBuilder unitWrap, current)))
            then raise(InvalidOperationException nonThreadSafeException)
          // Dispose
          unitReturnFrom(current.DisposeUniversal())
      ))
   }
   
  [<CompiledName("For")>]
  let inline ``for`` boolBuilder ([<IIL>]boolWrap) unitBuilder ([<IIL>]unitWrap) ([<IIL>]unitReturnFrom) (s : _ seq) ([<IIL>]_f) =
    let enumerator = s.GetEnumerator()
    tryFinally boolBuilder boolWrap unitBuilder unitWrap unitReturnFrom
      (fun() -> (
        ``while`` boolBuilder boolWrap unitBuilder unitWrap unitReturnFrom
          (fun() -> enumerator.MoveNext())
          (fun() -> _f enumerator.Current)
      ))
      (fun() -> enumerator.Dispose())
