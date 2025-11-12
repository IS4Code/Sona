[<AutoOpen>]
module Sona.Runtime.Computations

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

type internal IIL = InlineIfLambdaAttribute

let coroutine = { new Coroutines.CoroutineBuilder() with member _.ToString() = "coroutine" }

[<AbstractClass>]
type BaseBuilder<'TZero>() =
  abstract member While : (unit -> bool) * (unit -> 'TZero) -> 'TZero

  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try
      _f()
    finally
      _cleanup()
  
  member inline _.TryWith([<IIL>]_f : unit -> _, [<IIL>]_fail : _ -> _) =
    try
      _f()
    with
    | e -> _fail e
  
  member inline _.Using(x : #IDisposable, [<IIL>]_f : _ -> _ option) =
    try
      _f x
    finally
      x.Dispose()
  
  member inline this.For(s : _ seq, [<IIL>]_f : _ -> 'TZero) =
    let enumerator = s.GetEnumerator()
    try
      this.While(enumerator.MoveNext, (fun() -> _f enumerator.Current))
    finally
      enumerator.Dispose()

[<AbstractClass>]
type OptionBuilder() =
  inherit BaseBuilder<unit option>()

  member inline _.Zero() = Some()

  member inline _.Return(value) = Some value
  member inline _.Delay([<IIL>]_f : _ -> _ option) = _f
  member inline _.Run([<IIL>]_f : _ -> _ option) = _f()

  member inline _.ReturnFrom(opt : _ option) = opt
  
  member inline _.ReturnFrom(opt : _ voption) =
    match opt with
    | ValueSome value -> Some value
    | _ -> None
  
  member inline _.ReturnFrom(opt : Result<_, _>) =
    match opt with
    | Ok value -> Some value
    | _ -> None
  
  member inline this.Bind(opt : _ option, [<IIL>]_func) = Option.bind _func (this.ReturnFrom(opt))
  member inline this.Bind(opt : _ voption, [<IIL>]_func) = Option.bind _func (this.ReturnFrom(opt))
  member inline this.Bind(opt : Result<_, _>, [<IIL>]_func) = Option.bind _func (this.ReturnFrom(opt))
  member inline this.Combine(opt : _ option, [<IIL>]_func) = this.Bind(opt, _func)

  override _.While(cond, func) =
    let mutable continuing = true
    while continuing && cond() do
      if func().IsNone then
        continuing <- false
    if continuing then Some()
    else None

let option = { new OptionBuilder() with member _.ToString() = "option" }

[<AbstractClass>]
type ValueOptionBuilder() =
  inherit BaseBuilder<unit voption>()

  member inline _.Zero() = ValueSome()
  member inline _.Return(value) = ValueSome value
  member inline _.Delay([<IIL>]_f : _ -> _ voption) = _f
  member inline _.Run([<IIL>]_f : _ -> _ voption) = _f()
  
  member inline _.ReturnFrom(opt : _ option) =
    match opt with
    | Some value -> ValueSome value
    | _ -> ValueNone
  
  member inline _.ReturnFrom(opt : _ voption) = opt
  
  member inline _.ReturnFrom(opt : Result<_, _>) =
    match opt with
    | Ok value -> ValueSome value
    | _ -> ValueNone
    
  member inline this.Bind(opt : _ option, [<IIL>]_func) = ValueOption.bind _func (this.ReturnFrom(opt))
  member inline this.Bind(opt : _ voption, [<IIL>]_func) = ValueOption.bind _func (this.ReturnFrom(opt))
  member inline this.Bind(opt : Result<_, _>, [<IIL>]_func) = ValueOption.bind _func (this.ReturnFrom(opt))
  member inline this.Combine(opt : _ voption, [<IIL>]_func) = this.Bind(opt, _func)
  
  override _.While(cond, func) =
    let mutable continuing = true
    while continuing && cond() do
      if func().IsValueNone then
        continuing <- false
    if continuing then ValueSome()
    else ValueNone

let voption = { new ValueOptionBuilder() with member _.ToString() = "voption" }

[<AbstractClass>]
type ResultBaseBuilder<'TZeroOk, 'TZeroError>() =
  inherit BaseBuilder<Result<'TZeroOk, 'TZeroError>>()
  
  member inline _.Delay([<IIL>]_f : _ -> Result<_, _>) = _f
  member inline _.Run([<IIL>]_f : _ -> Result<_, _>) = _f()

  member inline _.ReturnFrom(opt : _ option) =
    match opt with
    | Some value -> Ok value
    | _ -> Error()
  
  member inline _.ReturnFrom(opt : _ voption) =
    match opt with
    | ValueSome value -> Ok value
    | _ -> Error()
  
  member inline _.ReturnFrom(opt : Result<_, _>) = opt

[<AbstractClass>]
type ResultBuilder<'TError>() =
  inherit ResultBaseBuilder<unit, 'TError>()

  member inline _.Zero() = Ok()
  member inline _.Return(value) = Ok value
  
  member inline this.Bind(opt : Result<_, _>, [<IIL>]_func) = Result.bind _func (this.ReturnFrom(opt))
  member inline this.Bind(opt : _ option, [<IIL>]_func) = this.Bind(this.ReturnFrom(opt), _func)
  member inline this.Bind(opt : _ voption, [<IIL>]_func) = this.Bind(this.ReturnFrom(opt), _func)
  member inline this.Combine(opt : Result<_, _>, [<IIL>]_func) = this.Bind(opt, _func)
  
  override _.While(cond, func) =
    let mutable continuing = true
    let mutable errorResult = Unchecked.defaultof<_>
    while continuing && cond() do
      errorResult <- func()
      if errorResult.IsError then
        continuing <- false
    if continuing then Ok()
    else errorResult

[<Sealed>]
type private ResultBuilderImpl<'TError>() =
  inherit ResultBuilder<'TError>()

  static member val Instance : ResultBuilder<'TError> = ResultBuilderImpl<'TError>()

  override _.ToString() = "result"

let result<'TError> = ResultBuilderImpl<'TError>.Instance

[<AbstractClass>]
type ErrorResultBuilder<'TSuccess>() =
  inherit ResultBaseBuilder<'TSuccess, unit>()

  member inline _.Zero() = Error()
  member inline _.Return(value) = Error value
  
  member inline _.Bind(opt : Result<_, _>, [<IIL>]_func) =
    match opt with
    | Ok _ -> opt
    | Error err -> _func err
  
  member inline this.Bind(opt : _ option, [<IIL>]_func) = this.Bind(this.ReturnFrom(opt), _func)
  member inline this.Bind(opt : _ voption, [<IIL>]_func) = this.Bind(this.ReturnFrom(opt), _func)
  member inline this.Combine(opt : Result<_, _>, [<IIL>]_func) = this.Bind(opt, _func)
  
  override _.While(cond, func) =
    let mutable continuing = true
    let mutable okResult = Unchecked.defaultof<_>
    while continuing && cond() do
      okResult <- func()
      if okResult.IsOk then
        continuing <- false
    if continuing then Error()
    else okResult

[<Sealed>]
type private ErrorResultBuilderImpl<'TSuccess>() =
  inherit ErrorResultBuilder<'TSuccess>()

  static member val Instance : ErrorResultBuilder<'TSuccess> = ErrorResultBuilderImpl<'TSuccess>()

  override _.ToString() = "errorResult"

let errorResult<'TSuccess> = ErrorResultBuilderImpl<'TSuccess>.Instance

[<Struct>]
type Immediate<'T> = { Value : 'T }

[<AbstractClass>]
type ImmediateBuilderBase() =
  inherit BaseBuilder<Immediate<unit>>()
  
  member inline _.Delay([<IIL>]_f : unit -> Immediate<_>) = _f
  
  member inline _.ReturnFrom(x : Immediate<_>) : Immediate<_> = x
  member inline _.Return value : Immediate<_> = { Value = value }
  member inline this.Zero() = this.Return(())

  member inline _.Bind(x : Immediate<_>, [<IIL>]_f : _ -> Immediate<_>) = _f x.Value
  member inline this.Combine(x : Immediate<unit>, [<IIL>]_f) = this.Bind(x, _f)

  override this.While(cond, func) =
    while cond() do
      let _ = func() in ()
    this.Zero()

[<AbstractClass>]
type DelayedBuilder() =
  inherit ImmediateBuilderBase()

  member inline _.Run([<IIL>]_f : _ -> Immediate<_>) = fun() -> _f().Value

let delayed = { new DelayedBuilder() with member _.ToString() = "delayed" }

[<AbstractClass>]
type ImmediateBuilder() =
  inherit ImmediateBuilderBase()
  
  member inline _.Run([<IIL>]_f : _ -> Immediate<_>) = _f().Value

let immediate = { new ImmediateBuilder() with member _.ToString() = "immediate" }

[<Literal>]
let private noValueWarningMessage = "This `follow` operator may result in an exception if the argument has no value."
[<Literal>]
let private noSingleValueWarningMessage = "This `follow` operator may result in an exception if the argument does not contain exactly one element."
[<Literal>]
let private blockingWarningMessage = "This `follow` operator may result in a blocking operation."
[<Literal>]
let private warningNumber = 99999
#nowarn "99999"

[<AbstractClass>]
type GlobalBuilder() =
  inherit ImmediateBuilder()

  member inline _.Run([<IIL>]_f : _ -> Immediate<_>) = _f().Value

  static member inline private ErrorNoValue() =
    raise (ArgumentException("The object has no value.", "arg"))

  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : _ option) =
    match arg with
    | Some value -> this.Return(value)
    | _ -> GlobalBuilder.ErrorNoValue()
  
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : _ voption) =
    match arg with
    | ValueSome value -> this.Return(value)
    | _ -> GlobalBuilder.ErrorNoValue()
  
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : Result<_, _>) =
    match arg with
    | Ok value -> this.Return(value)
    | Error err -> raise (ArgumentException(sprintf "The object has an error value %A." err, "arg"))
  
  [<CompilerMessage(noSingleValueWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : _ seq) = this.Return(System.Linq.Enumerable.Single(arg))

  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : Async<_>) = this.Return(Async.RunSynchronously arg)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : Task) = this.Return(arg.GetAwaiter().GetResult())
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : Task<_>) = this.Return(arg.GetAwaiter().GetResult())
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : ValueTask) = this.Return(arg.Preserve().GetAwaiter().GetResult())
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.ReturnFrom(arg : ValueTask<_>) = this.Return(arg.Preserve().GetAwaiter().GetResult())
  member inline this.ReturnFrom(arg : Lazy<_>) = this.Return(arg.Force())

  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : _ option, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : _ voption, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Result<_, _>, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(noSingleValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : 'T seq, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Async<_>, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Task, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Task<_>, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : ValueTask, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : ValueTask<_>, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)
  member inline this.Bind(arg : Lazy<_>, [<IIL>]_func) = this.Bind(this.ReturnFrom arg, _func)

let ``global`` = { new GlobalBuilder() with member _.ToString() = "global" }

[<Literal>]
let private emptySequenceException = "The enumerator is not pointing at any element."

[<Literal>]
let private nonThreadSafeException = "The enumerator is not thread-safe."

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

[<NoEquality; NoComparison; Struct>]
type UniversalSequenceBuilder<'TBoolBuilder, 'TUnitBuilder> =
  {
    BoolBuilder : 'TBoolBuilder
    UnitBuilder : 'TUnitBuilder
  }
  member inline _.YieldFrom x = UniversalSequence.yieldFrom x
  member inline _.Delay([<IIL>]_f) = UniversalSequence.delay _f
  member inline _.Run([<IIL>]_f) = UniversalSequence.run _f

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
      
[<CompiledName("SequenceVia")>]
let inline sequenceVia builder = { BoolBuilder = builder; UnitBuilder = builder }
