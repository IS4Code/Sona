namespace Sona.Runtime

type IGenericEnumerable<'TValue, 'TUnitMonad, 'TBooleanMonad> =
  interface
    abstract GetGenericEnumerator : unit -> IGenericEnumerator<'TValue, 'TUnitMonad, 'TBooleanMonad>
  end
and IGenericEnumerator<'TValue, 'TUnitMonad, 'TBooleanMonad> =
  interface
    inherit IGenericDisposable<'TUnitMonad>

    abstract MoveNextGeneric : unit -> 'TBooleanMonad
    abstract Current : 'TValue
  end
and IGenericDisposable<'TUnitMonad> =
  interface
    abstract DisposeGeneric : unit -> 'TUnitMonad
  end

#nowarn "25" // Incomplete pattern matches on this expression.

module GenericEnumerable =
  open System
  open System.Threading
  open System.Threading.Tasks

  let inline fromTaskSeq(enumerable : ^T, cancellationToken) = {
    new IGenericEnumerable<^U, ValueTask, ValueTask<bool>> with
    member _.GetGenericEnumerator() =
      let enumerator = (^T : (member GetAsyncEnumerator : CancellationToken -> ^E) enumerable, cancellationToken) in {
        new IGenericEnumerator<^U, ValueTask, ValueTask<bool>> with
        member _.Current = (^E : (member Current : ^U) enumerator)
        member _.MoveNextGeneric() = (^E : (member MoveNextAsync : unit -> ValueTask<bool>) enumerator)
        member _.DisposeGeneric() = (^E : (member DisposeAsync : unit -> ValueTask) enumerator)
      }
  }

  module Internal = 
    let private ctsCancelCallback =
      Action<obj>(function (:? CancellationTokenSource as x) -> x.Cancel())
    
    let linkCancellation(cts : CancellationTokenSource, token : CancellationToken) =
      token.Register(ctsCancelCallback, cts)
  open Internal

  let inline fromAsyncSeq(enumerable : ^T) = {
    new IGenericEnumerable<^U, Async<unit>, Async<bool>> with
    member _.GetGenericEnumerator() =
      let cts = new CancellationTokenSource()

      let enumerator = (^T : (member GetAsyncEnumerator : CancellationToken -> ^E) enumerable, cts.Token) in {
        new IGenericEnumerator<^U, Async<unit>, Async<bool>> with
        member _.Current = (^E : (member Current : ^U) enumerator)
        
        member _.MoveNextGeneric() = async {
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
        
        member _.DisposeGeneric() = async {
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