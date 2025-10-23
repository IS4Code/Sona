[<AutoOpen>]
module Sona.Runtime.Computations

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

[<AbstractClass>]
type OptionBuilder() =
  member inline _.Zero() = None
  member inline _.Return(value) = Some value
  
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

let option = { new OptionBuilder() with member _.ToString() = "option" }

[<AbstractClass>]
type ValueOptionBuilder() =
  member inline _.Zero() = ValueNone
  member inline _.Return(value) = ValueSome value
  
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

let voption = { new OptionBuilder() with member _.ToString() = "voption" }

[<AbstractClass>]
type ResultBuilder() =
  member inline _.Return(value) = Ok value
  
  member inline _.ReturnFrom(opt : _ option) =
    match opt with
    | Some value -> Ok value
    | _ -> Error()
  
  member inline _.ReturnFrom(opt : _ voption) =
    match opt with
    | ValueSome value -> Ok value
    | _ -> Error()
  
  member inline _.ReturnFrom(opt : Result<_, _>) = opt
  
  member inline this.Bind(opt, func) = this.ReturnFrom(Option.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(ValueOption.bind func opt)
  member inline this.Bind(opt, func) = this.ReturnFrom(Result.bind func opt)

let result = { new ResultBuilder() with member _.ToString() = "result" }

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
  member inline _.Zero() = ()
  member inline _.Return(value) = value

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

module GenericSequence =
  open System.Threading

  let inline zero boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) = {
    new IGenericEnumerator<_, _, _> with

    member _.Current = raise(InvalidOperationException emptySequenceException)

    member _.MoveNextGeneric() =
      boolWrap(fun() -> (
        (^B : (member Return : _ -> _) boolBuilder, false)
      ))

    member _.DisposeGeneric() =
      unitWrap(fun() -> (
        (^U : (member Zero : unit -> _) unitBuilder)
      ))
  }

  let inline delay (f : unit -> IGenericEnumerator<_, _, _>) = f

  let inline ``yield`` boolBuilder ([<InlineIfLambda>]boolWrap) unitBuilder ([<InlineIfLambda>]unitWrap) x =
   let mutable state = -1 in {
    new IGenericEnumerator<_, _, _> with

    member _.Current =
      match Volatile.Read(&state) with
      | 0 -> x
      | _ -> raise(InvalidOperationException emptySequenceException)
    
    member _.MoveNextGeneric() =
      boolWrap(fun() -> (
        (^B : (member Return : _ -> _) boolBuilder, Interlocked.Increment(&state) = 0)
      ))
    
    member _.DisposeGeneric() =
      unitWrap(fun() -> (
        (^U : (member Zero : unit -> _) unitBuilder)
      ))
   }

  let inline bind boolBuilder ([<InlineIfLambda>]boolWrap) ([<InlineIfLambda>]boolReturnFrom) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) m ([<InlineIfLambda>]f) =
   let mutable inner = Unchecked.defaultof<IGenericEnumerator<_, _, _>> in {
    new IGenericEnumerator<_, _, _> with

    member _.Current =
      if not(isNull(Volatile.Read(&inner) |> box)) then inner.Current
      else raise(InvalidOperationException emptySequenceException)

    member _.MoveNextGeneric() =
      if not(isNull(Volatile.Read(&inner) |> box)) then
        // Never changes once set
        inner.MoveNextGeneric()
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
            boolReturnFrom(inner.MoveNextGeneric())
          ))
        ))

    member _.DisposeGeneric() =
      if not(isNull(Volatile.Read(&inner) |> box)) then
        // Never changes once set
        inner.DisposeGeneric()
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
          unitReturnFrom(inner.DisposeGeneric())
        ))
   }

  let inline yieldFrom (x : IGenericEnumerable<_, _, _>) = x.GetGenericEnumerator()

  let inline combine boolBuilder ([<InlineIfLambda>]boolWrap) ([<InlineIfLambda>]boolReturnFrom) unitBuilder ([<InlineIfLambda>]unitWrap) ([<InlineIfLambda>]unitReturnFrom) (first : IGenericEnumerator<_, _, _>) (second : unit -> IGenericEnumerator<_, _, _>) =
   let mutable inner = Unchecked.defaultof<IGenericEnumerator<_, _, _>> in {
    new IGenericEnumerator<_, _, _> with

    member _.Current =
      if not(isNull(Volatile.Read(&inner) |> box)) then inner.Current
      else raise(InvalidOperationException emptySequenceException)
    
    member _.MoveNextGeneric() =
      let enumerator = Volatile.Read(&inner)
      if not(isNull(enumerator |> box)) && not(Object.ReferenceEquals(first, enumerator |> box)) then
        // Delegate to the second enumerator (will never change)
        enumerator.MoveNextGeneric()
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
            boolReturnFrom(enumerator.MoveNextGeneric())
          else
            // Decide based on the result
            (^B : (member Bind : _ * _ -> _) boolBuilder, enumerator.MoveNextGeneric(), fun hasNext -> (
              // Initialize with the continuation
              if hasNext then
                // Just return the information
                (^B : (member Return : _ -> _) boolBuilder, true)
              else
                // Cleanup and move to the next
                (^U : (member Bind : _ * _ -> _) unitBuilder, enumerator.DisposeGeneric(), fun () -> (
                  if
                    // Still on the first enumerator
                    Object.ReferenceEquals(first, Volatile.Read(&inner)) &&
                    // But changed before the new value could be moved inside
                    not(Object.ReferenceEquals(first, Interlocked.CompareExchange(&inner, second(), first)))
                    then raise(InvalidOperationException nonThreadSafeException)
                  
                  // Return the inner result
                  boolReturnFrom(inner.MoveNextGeneric())
                ))
            ))
        ))
    
    member _.DisposeGeneric() =
      let enumerator = Volatile.Read(&inner)
      if not(isNull(enumerator |> box)) && not(Object.ReferenceEquals(first, enumerator |> box)) then
        // Delegate to the second enumerator (will never change)
        enumerator.DisposeGeneric()
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
          unitReturnFrom(inner.DisposeGeneric())
        ))
   }

  let inline run ([<InlineIfLambda>]f) = {
    new IGenericEnumerable<_, _, _> with
    member _.GetGenericEnumerator() = f()
  }

[<NoEquality; NoComparison>]
type GenericSequenceBuilder<'TBoolBuilder, 'TUnitBuilder> =
  {
    BoolBuilder : 'TBoolBuilder
    UnitBuilder : 'TUnitBuilder
  }
  member inline _.YieldFrom x = GenericSequence.yieldFrom x
  member inline _.Delay f = GenericSequence.delay f
  member inline _.Run f = GenericSequence.run f

[<AbstractClass; Extension>]
type GenericSequenceBuilderExtensions1 internal() =
  [<Extension>]
  static member inline Zero(self : GenericSequenceBuilder<_, _>) =
    GenericSequence.zero
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
  
  [<Extension>]
  static member inline Yield(self : GenericSequenceBuilder<_, _>, x) =
    GenericSequence.``yield``
      self.BoolBuilder
      (fun f -> f())
      self.UnitBuilder
      (fun f -> f())
      x
  
  [<Extension>]
  static member inline Bind(self : GenericSequenceBuilder<_, _>, m, f) =
    GenericSequence.bind
      self.BoolBuilder
      (fun f -> f())
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m f
  
  [<Extension>]
  static member inline Combine(self : GenericSequenceBuilder<_, _>, first, second) =
    GenericSequence.combine
      self.BoolBuilder
      (fun f -> f())
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> f())
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first second
    
[<AbstractClass; Extension>]
type GenericSequenceBuilderExtensions2 internal() =
  inherit GenericSequenceBuilderExtensions1()
  
  [<Extension>]
  static member inline Zero(self : GenericSequenceBuilder<_, _>) =
    GenericSequence.zero
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
  
  [<Extension>]
  static member inline Yield(self : GenericSequenceBuilder<_, _>, x) =
    GenericSequence.``yield``
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      x
  
  [<Extension>]
  static member inline Bind(self : GenericSequenceBuilder<_, _>, m, f) =
    GenericSequence.bind
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m f
  
  [<Extension>]
  static member inline Combine(self : GenericSequenceBuilder<_, _>, first, second) =
    GenericSequence.combine
      self.BoolBuilder
      (fun f -> (^B : (member Delay : _ -> _) self.BoolBuilder, f))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Delay : _ -> _) self.UnitBuilder, f))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first second

[<Sealed; AbstractClass; Extension>]
type GenericSequenceBuilderExtensions3 private() =
  inherit GenericSequenceBuilderExtensions2()
  
  [<Extension>]
  static member inline Zero(self : GenericSequenceBuilder<_, _>) =
    GenericSequence.zero
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
  
  [<Extension>]
  static member inline Yield(self : GenericSequenceBuilder<_, _>, x) =
    GenericSequence.``yield``
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      x
  
  [<Extension>]
  static member inline Bind(self : GenericSequenceBuilder<_, _>, m, f) =
    GenericSequence.bind
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      m f
  
  [<Extension>]
  static member inline Combine(self : GenericSequenceBuilder<_, _>, first, second) =
    GenericSequence.combine
      self.BoolBuilder
      (fun f -> (^B : (member Run : _ -> _) self.BoolBuilder, (^B : (member Delay : _ -> _) self.BoolBuilder, f)))
      (fun m -> (^B : (member ReturnFrom : _ -> _) self.BoolBuilder, m))
      self.UnitBuilder
      (fun f -> (^U : (member Run : _ -> _) self.UnitBuilder, (^U : (member Delay : _ -> _) self.UnitBuilder, f)))
      (fun m -> (^U : (member ReturnFrom : _ -> _) self.UnitBuilder, m))
      first second

let inline sequenceOf builder = { BoolBuilder = builder; UnitBuilder = builder }
