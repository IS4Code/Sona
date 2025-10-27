[<AutoOpen>]
module Sona.Runtime.Computations

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<AbstractClass>]
type BaseBuilder<'TZero>() =
  abstract member ZeroImpl : unit -> 'TZero
  abstract member BindImpl : 'TZero * (unit -> 'TZero) -> 'TZero

  member inline _.TryFinally(f : unit -> _, cleanup : unit -> unit) =
    try
      f()
    finally
      cleanup()
  
  member inline _.TryWith(f : unit -> _, fail : _ -> _) =
    try
      f()
    with
    | e -> fail e
  
  member inline this.Using(x : #IDisposable, f : _ -> _ option) =
    this.TryFinally((fun() -> f(x)), (fun() -> x.Dispose()))
  
  member this.While(cond : unit -> bool, f : unit -> 'TZero) =
    if cond() then
      this.BindImpl(f(), fun() -> this.While(cond, f))
    else
      this.ZeroImpl()

  member inline this.For(s : _ seq, f : _ -> 'TZero) =
    let enumerator = s.GetEnumerator()
    this.TryFinally(
      (fun() -> this.While(enumerator.MoveNext, fun() -> (
        this.BindImpl(f(enumerator.Current), fun() -> this.ZeroImpl())
      ))),
      (fun() -> enumerator.Dispose())
    )

[<AbstractClass>]
type OptionBuilder() =
  inherit BaseBuilder<unit option>()

  member inline _.Zero() = Some()

  member inline _.Return(value) = Some value
  member inline _.Delay(f : _ -> _ option) = f
  member inline _.Run(f : _ -> _ option) = f()

  member inline _.ReturnFrom(opt : _ option) = opt
  
  member inline _.ReturnFrom(opt : _ voption) =
    match opt with
    | ValueSome value -> Some value
    | _ -> None
  
  member inline _.ReturnFrom(opt : Result<_, _>) =
    match opt with
    | Ok value -> Some value
    | _ -> None

  member inline this.Bind(opt, func) = this.ReturnFrom(Option.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(ValueOption.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(Result.bind func opt)
  member inline this.Combine(opt : _ option, func : _ -> _ option) = this.Bind(opt, func)

  override this.ZeroImpl() = this.Zero()
  override this.BindImpl(opt, func) = this.Bind(opt, func)

let option = { new OptionBuilder() with member _.ToString() = "option" }

[<AbstractClass>]
type ValueOptionBuilder() =
  inherit BaseBuilder<unit voption>()

  member inline _.Zero() = ValueSome()
  member inline _.Return(value) = ValueSome value
  member inline _.Delay(f : _ -> _ voption) = f
  member inline _.Run(f : _ -> _ voption) = f()
  
  member inline _.ReturnFrom(opt : _ option) =
    match opt with
    | Some value -> ValueSome value
    | _ -> ValueNone
  
  member inline _.ReturnFrom(opt : _ voption) = opt
  
  member inline _.ReturnFrom(opt : Result<_, _>) =
    match opt with
    | Ok value -> Some value
    | _ -> None

  member inline this.Bind(opt, func) = this.ReturnFrom(Option.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(ValueOption.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(Result.bind func opt)
  member inline this.Combine(opt : _ voption, func : _ -> _ voption) = this.Bind(opt, func)
  
  override this.ZeroImpl() = this.Zero()
  override this.BindImpl(opt, func) = this.Bind(opt, func)

let voption = { new OptionBuilder() with member _.ToString() = "voption" }

[<AbstractClass>]
type ResultBuilder<'TError>() =
  inherit BaseBuilder<Result<unit, 'TError>>()

  member inline _.Zero() = Ok()
  member inline _.Return(value) = Ok value
  member inline _.Delay(f : _ -> Result<_, 'TError>) = f
  member inline _.Run(f : _ -> Result<_, 'TError>) = f()
  
  member inline _.ReturnFrom(opt : _ option) =
    match opt with
    | Some value -> Ok value
    | _ -> Error()
  
  member inline _.ReturnFrom(opt : _ voption) =
    match opt with
    | ValueSome value -> Ok value
    | _ -> Error()
  
  member inline _.ReturnFrom(opt : Result<_, 'TError>) = opt
  
  member inline this.Bind(opt, func) = this.ReturnFrom(Option.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(ValueOption.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(Result.bind func opt)
  member inline this.Combine(opt : Result<_, 'TError>, func : _ -> Result<_, 'TError>) = this.Bind(opt, func)
  
  override this.ZeroImpl() = this.Zero()
  override this.BindImpl(opt, func) = this.Bind(opt, func)

[<Sealed>]
type private ResultBuilderImpl<'TError>() =
  inherit ResultBuilder<'TError>()

  static member val Instance : ResultBuilder<'TError> = ResultBuilderImpl<'TError>()

  override _.ToString() = "result"

let result<'TError> = ResultBuilderImpl<'TError>.Instance

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
type GlobalBuilderBase<'TZero when 'TZero : not struct>() =
  inherit BaseBuilder<'TZero>()
  override _.ZeroImpl() = Unchecked.defaultof<_>
  override _.BindImpl(_, func) = func()

[<AbstractClass>]
type GlobalBuilder() =
  inherit GlobalBuilderBase<unit>()

  member inline _.Zero() = ()
  member inline _.Return(value) = value
  member inline _.Delay(f : _ -> _) = f
  member inline _.Run(f : _ -> _) = f()

  static member inline private ErrorNoValue() =
    raise (ArgumentException("The object has no value.", "arg"))

  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : _ option) =
    match arg with
    | Some value -> value
    | _ -> GlobalBuilder.ErrorNoValue()
  
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : _ voption) =
    match arg with
    | ValueSome value -> value
    | _ -> GlobalBuilder.ErrorNoValue()
  
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : Result<_, _>) =
    match arg with
    | Ok value -> value
    | Error err -> raise (ArgumentException(sprintf "The object has an error value %A." err, "arg"))
  
  [<CompilerMessage(noSingleValueWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : _ seq) = System.Linq.Enumerable.Single(arg)

  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : Async<_>) = Async.RunSynchronously arg
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : Task) = arg.GetAwaiter().GetResult()
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : Task<_>) = arg.GetAwaiter().GetResult()
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : ValueTask) = arg.Preserve().GetAwaiter().GetResult()
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline _.ReturnFrom(arg : ValueTask<_>) = arg.Preserve().GetAwaiter().GetResult()
  member inline _.ReturnFrom(arg : Lazy<_>) = arg.Force()

  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : _ option, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : _ voption, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(noValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Result<_, _>, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(noSingleValueWarningMessage, warningNumber)>]
  member inline this.Bind(arg : 'T seq, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Async<_>, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Task, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : Task<_>, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : ValueTask, func) = func(this.ReturnFrom arg)
  [<CompilerMessage(blockingWarningMessage, warningNumber)>]
  member inline this.Bind(arg : ValueTask<_>, func) = func(this.ReturnFrom arg)
  member inline this.Bind(arg : Lazy<_>, func) = func(this.ReturnFrom arg)

let ``global`` = { new GlobalBuilder() with member _.ToString() = "global" }

[<Literal>]
let private emptySequenceException = "The enumerator is not pointing at any element."

[<Literal>]
let private nonThreadSafeException = "The enumerator is not thread-safe."

module UniversalSequence =
  open System.Threading

  let inline zero boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) = {
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

  let inline delay (f : unit -> IUniversalEnumerator<_, _, _>) = f

  let inline run ([<InlineIfLambda>]f) = {
    new IUniversalEnumerable<_, _, _> with
    member _.GetUniversalEnumerator() = f()
  }

  let inline ``yield`` boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) x =
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

  let inline bind boolBuilder ([<InlineIfLambda>]boolWrap) ([<InlineIfLambda>]boolReturnFrom) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) m ([<InlineIfLambda>]f) =
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
              not(isNull(Interlocked.CompareExchange(&inner, f(x), Unchecked.defaultof<_>) |> box))
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

  let inline yieldFrom (x : IUniversalEnumerable<_, _, _>) = x.GetUniversalEnumerator()

  let inline combine boolBuilder ([<InlineIfLambda>]boolWrap) ([<InlineIfLambda>]boolReturnFrom) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) (first : IUniversalEnumerator<_, _, _>) ([<InlineIfLambda>]second : unit -> IUniversalEnumerator<_, _, _>) =
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
                    not(Object.ReferenceEquals(first, Interlocked.CompareExchange(&inner, second(), first)))
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
  
  let inline tryFinally boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) ([<InlineIfLambda>]f) ([<InlineIfLambda>]cleanup : unit -> unit) =
    let mutable enumeratorReceived = false
    try
     let enumerator : IUniversalEnumerator<_, _, _> = f()
     enumeratorReceived <- true

     let mutable finished = 0
     let inline isFinished() = 0 <> Volatile.Read(&finished)
     let inline doCleanup() = 
       if 0 = Interlocked.CompareExchange(&finished, 1, 0) then cleanup()
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
       cleanup()
  
  let inline tryWith boolBuilder ([<InlineIfLambda>]boolWrap) ([<InlineIfLambda>]boolReturnFrom) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) ([<InlineIfLambda>]f : unit -> IUniversalEnumerator<_, _, _>) ([<InlineIfLambda>]fail : exn -> IUniversalEnumerator<_, _, _>) =
    try
     let mutable enumerator : IUniversalEnumerator<_, _, _> = f()

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
                enumerator <- fail e
                boolReturnFrom(enumerator.MoveNextUniversal())
              ))
            ))

            (^B : (member TryWith : _ * _ -> _) boolBuilder, delayed, fun e -> (
              if disposed then
                // The exception comes from the failure enumerator, propagate anyway
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw()
              
              // Disposing caused another exception, use this one
              enumerator <- fail e
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
            enumerator <- fail e
            
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
    | e -> fail e
  
  let inline using boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) (x : #IDisposable) ([<InlineIfLambda>]f) =
    tryFinally boolBuilder boolWrap unitBuilder unitWrap unitReturnFrom (fun() -> f(x)) (fun() -> x.Dispose())
  
  let inline ``while`` boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) ([<InlineIfLambda>]cond : unit -> bool) ([<InlineIfLambda>]f : unit -> IUniversalEnumerator<_, _, _>) =
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
                  if not(cond()) then
                    // Terminate
                    (^B : (member Return : _ -> _) boolBuilder, false)
                  else
                    // Start next iteration
                    let next = f()
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
          if not(cond()) then
            // Terminate
            (^B : (member Return : _ -> _) boolBuilder, false)
          else
            let next = f()
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

  let inline ``for`` boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) (s : _ seq) ([<InlineIfLambda>]f) =
    let enumerator = s.GetEnumerator()
    tryFinally boolBuilder boolWrap unitBuilder unitWrap unitReturnFrom
      (fun() -> (
        ``while`` boolBuilder boolWrap unitBuilder unitWrap unitReturnFrom
          (fun() -> enumerator.MoveNext())
          (fun() -> f(enumerator.Current))
      ))
      (fun() -> enumerator.Dispose())

[<NoEquality; NoComparison>]
type UniversalSequenceBuilder<'TBoolBuilder, 'TUnitBuilder> =
  {
    BoolBuilder : 'TBoolBuilder
    UnitBuilder : 'TUnitBuilder
  }
  member inline _.YieldFrom x = UniversalSequence.yieldFrom x
  member inline _.Delay f = UniversalSequence.delay f
  member inline _.Run f = UniversalSequence.run f

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
  static member inline Bind(self : UniversalSequenceBuilder<_, _>, m, f) =
    UniversalSequence.bind
      self.BoolBuilder
      (fun f -> f())
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m f
  
  [<Extension>]
  static member inline Combine(self : UniversalSequenceBuilder<_, _>, first, second) =
    UniversalSequence.combine
      self.BoolBuilder
      (fun f -> f())
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first second
  
  [<Extension>]
  static member inline TryFinally(self : UniversalSequenceBuilder<_, _>, f, cleanup) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      f cleanup
  
  [<Extension>]
  static member inline TryWith(self : UniversalSequenceBuilder<_, _>, f, fail) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      f fail
  
  [<Extension>]
  static member inline Using(self : UniversalSequenceBuilder<_, _>, obj, f) =
    UniversalSequence.using
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      obj f
  
  [<Extension>]
  static member inline While(self : UniversalSequenceBuilder<_, _>, cond, f) =
    UniversalSequence.``while``
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      cond f
  
  [<Extension>]
  static member inline For(self : UniversalSequenceBuilder<_, _>, s, f) =
    UniversalSequence.``for``
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      s f
    
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
  static member inline Bind(self : UniversalSequenceBuilder<_, _>, m, f) =
    UniversalSequence.bind
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m f
  
  [<Extension>]
  static member inline Combine(self : UniversalSequenceBuilder<_, _>, first, second) =
    UniversalSequence.combine
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first second
  
  [<Extension>]
  static member inline TryFinally(self : UniversalSequenceBuilder<_, _>, f, cleanup) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      f cleanup
  
  [<Extension>]
  static member inline TryWith(self : UniversalSequenceBuilder<_, _>, f, fail) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      f fail
  
  [<Extension>]
  static member inline Using(self : UniversalSequenceBuilder<_, _>, obj, f) =
    UniversalSequence.using
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      obj f
  
  [<Extension>]
  static member inline While(self : UniversalSequenceBuilder<_, _>, cond, f) =
    UniversalSequence.``while``
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      cond f
  
  [<Extension>]
  static member inline For(self : UniversalSequenceBuilder<_, _>, s, f) =
    UniversalSequence.``for``
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      s f

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
  static member inline Bind(self : UniversalSequenceBuilder<_, _>, m, f) =
    UniversalSequence.bind
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m f
  
  [<Extension>]
  static member inline Combine(self : UniversalSequenceBuilder<_, _>, first, second) =
    UniversalSequence.combine
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first second
  
  [<Extension>]
  static member inline TryFinally(self : UniversalSequenceBuilder<_, _>, f, cleanup) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      f cleanup
  
  [<Extension>]
  static member inline TryWith(self : UniversalSequenceBuilder<_, _>, f, fail) =
    UniversalSequence.tryFinally
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      f fail
  
  [<Extension>]
  static member inline Using(self : UniversalSequenceBuilder<_, _>, obj, f) =
    UniversalSequence.using
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      obj f
  
  [<Extension>]
  static member inline While(self : UniversalSequenceBuilder<_, _>, cond, f) =
    UniversalSequence.``while``
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      cond f
  
  [<Extension>]
  static member inline For(self : UniversalSequenceBuilder<_, _>, s, f) =
    UniversalSequence.``for``
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      s f

let inline sequenceOf builder = { BoolBuilder = builder; UnitBuilder = builder }
