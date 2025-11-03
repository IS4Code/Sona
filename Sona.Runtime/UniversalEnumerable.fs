namespace Sona.Runtime

type IUniversalEnumerable<'TValue, 'TUnitMonad, 'TBooleanMonad> =
  interface
    abstract GetUniversalEnumerator : unit -> IUniversalEnumerator<'TValue, 'TUnitMonad, 'TBooleanMonad>
  end
and IUniversalEnumerator<'TValue, 'TUnitMonad, 'TBooleanMonad> =
  interface
    inherit IUniversalDisposable<'TUnitMonad>

    abstract MoveNextUniversal : unit -> 'TBooleanMonad
    abstract Current : 'TValue
  end
and IUniversalDisposable<'TUnitMonad> =
  interface
    abstract DisposeUniversal : unit -> 'TUnitMonad
  end

#nowarn "25" // Incomplete pattern matches on this expression.

module UniversalEnumerable =
  open System
  open System.Collections
  open System.Collections.Generic
  open System.Threading
  open System.Threading.Tasks

  let inline private fromSeqInner (enumerable : ^T) ([<InlineIfLambda>]dispose : ^E -> _) = {
    // Can't use `unit` for some reason
    new IUniversalEnumerable<^U, _, bool> with
    member _.GetUniversalEnumerator() =
      let enumerator = (^T : (member GetEnumerator : unit -> ^E when ^E :> IDisposable) enumerable) in {
        new IUniversalEnumerator<^U, _, bool> with
        member _.Current = (^E : (member Current : ^U) enumerator)
        member _.MoveNextUniversal() = (^E : (member MoveNext : unit -> bool) enumerator)
        member _.DisposeUniversal() = dispose enumerator
      }
  }

  let inline fromSeq enumerable =
    fromSeqInner enumerable (fun (enumerator : #IDisposable) -> enumerator.Dispose())

  let inline toSeq(enumerable : #IUniversalEnumerable<_, unit, bool>) = {
    new IEnumerable<'T> with
    member _.GetEnumerator() =
      let mutable enumerator = enumerable.GetUniversalEnumerator() in {
        new IEnumerator<'T> with
        member _.Current : 'T = enumerator.Current
        member this.Current : obj = upcast(this.Current : 'T)
        member _.Reset() = 
          let mutable current = Volatile.Read(&enumerator)
          try
            let replacement = enumerable.GetUniversalEnumerator()
            if not(Object.ReferenceEquals(current, Interlocked.CompareExchange(&enumerator, replacement, current))) then
              // Already reset
              current <- Unchecked.defaultof<_>
              replacement.DisposeUniversal()
          finally
            if not(isNull(current |> box)) then
              current.DisposeUniversal()
        member _.MoveNext() = enumerator.MoveNextUniversal()
        member _.Dispose() = enumerator.DisposeUniversal()
      }
    member this.GetEnumerator() : IEnumerator = upcast(this.GetEnumerator() : IEnumerator<'T>)
  }

  let inline fromTaskSeq(enumerable : ^T, cancellationToken) = {
    new IUniversalEnumerable<^U, ValueTask, ValueTask<bool>> with
    member _.GetUniversalEnumerator() =
      let enumerator = (^T : (member GetAsyncEnumerator : CancellationToken -> ^E) enumerable, cancellationToken) in {
        new IUniversalEnumerator<^U, ValueTask, ValueTask<bool>> with
        member _.Current = (^E : (member Current : ^U) enumerator)
        member _.MoveNextUniversal() = (^E : (member MoveNextAsync : unit -> ValueTask<bool>) enumerator)
        member _.DisposeUniversal() = (^E : (member DisposeAsync : unit -> ValueTask) enumerator)
      }
  }

  module Internal = 
    let private ctsCancelCallback =
      Action<obj>(function (:? CancellationTokenSource as x) -> x.Cancel())
    
    let linkCancellation(cts : CancellationTokenSource, token : CancellationToken) =
      token.Register(ctsCancelCallback, cts)
  open Internal

  let inline fromAsyncSeq(enumerable : ^T) = {
    new IUniversalEnumerable<^U, Async<unit>, Async<bool>> with
    member _.GetUniversalEnumerator() =
      let cts = new CancellationTokenSource()

      let enumerator = (^T : (member GetAsyncEnumerator : CancellationToken -> ^E) enumerable, cts.Token) in {
        new IUniversalEnumerator<^U, Async<unit>, Async<bool>> with
        member _.Current = (^E : (member Current : ^U) enumerator)
        
        member _.MoveNextUniversal() = async {
          let! token = Async.CancellationToken
          // Use the token for the duration of the operation
          use _ = linkCancellation(cts, token)

          // Get the `MoveNext` result
          let task = (^E : (member MoveNextAsync : unit -> ValueTask<bool>) enumerator)
          if task.IsCompletedSuccessfully then
            return task.Result
          else
            return! Async.AwaitTask(task.AsTask())
        }
        
        member _.DisposeUniversal() = async {
          // Dispose afterwards
          use _ = cts

          let! token = Async.CancellationToken
          // Use the token for the duration of the operation
          use _ = linkCancellation(cts, token)

          let task = (^E : (member DisposeAsync : unit -> ValueTask) enumerator)
          if task.IsCompletedSuccessfully then
            return ()
          else
            return! Async.AwaitTask(task.AsTask())
        }
      }
  }