[<AutoOpen>]
module Sona.Runtime.Computations

open System
open System.Threading.Tasks
open Sona.Runtime.CompilerServices

[<AbstractClass>]
type OptionBuilder() =
  member inline _.Zero() = Operators.LiftNothing()
  member inline _.Return(value) = Operators.LiftResult value
  
  member inline _.ReturnFrom(opt : _ option) = opt
  member inline _.ReturnFrom(opt : _ voption) = opt
  member inline _.ReturnFrom(opt : Result<_, _>) = opt
  member inline this.ReturnFrom(opt) =
    match Operators.BindToResult opt with
    | struct(true, result) -> this.Return(result)
    | _ -> this.Zero()
  
  member inline _.Bind(opt, func) = Option.bind func opt
  member inline _.Bind(opt, func) = ValueOption.bind func opt
  member inline _.Bind(opt, func) = Result.bind func opt
  member inline this.Bind(opt, func) =
    match Operators.BindToResult opt with
    | struct(true, result) -> this.Return(func(result))
    | _ -> this.Zero()

let option = { new OptionBuilder() with member _.ToString() = "option" }

[<Literal>]
let private noValueWarningMessage = "This `follow` operator may result in an exception if the argument has no value."
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