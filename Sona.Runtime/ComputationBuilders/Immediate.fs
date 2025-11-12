namespace Sona.Runtime.ComputationBuilders

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System

[<Struct; IsReadOnly; NoEquality; NoComparison>]
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
  
[<AbstractClass>]
type ImmediateBuilder() =
  inherit ImmediateBuilderBase()
  
  member inline _.Run([<IIL>]_f : _ -> Immediate<_>) = _f().Value

module private GlobalErrors =
  [<Literal>]
  let noValueWarningMessage = "This `follow` operator may result in an exception if the argument has no value."
  [<Literal>]
  let noSingleValueWarningMessage = "This `follow` operator may result in an exception if the argument does not contain exactly one element."
  [<Literal>]
  let blockingWarningMessage = "This `follow` operator may result in a blocking operation."
  [<Literal>]
  let warningNumber = 99999
open GlobalErrors
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
