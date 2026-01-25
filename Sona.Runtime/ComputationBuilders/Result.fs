namespace Sona.Runtime.ComputationBuilders

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
  
  member inline this.MergeSources(x : Result<_, _>, y : Result<_, _>) : Result<_, _> =
    this.Bind(x, fun xValue -> (
      this.Bind(y, fun yValue -> (
        this.Return(xValue, yValue)
      ))
    ))
  
  override _.While(cond, func) =
    let mutable continuing = true
    let mutable errorResult = Unchecked.defaultof<_>
    while continuing && cond() do
      errorResult <- func()
      if errorResult.IsError then
        continuing <- false
    if continuing then Ok()
    else errorResult

[<AbstractClass>]
type ErrorResultBuilder<'TSuccess>() =
  inherit ResultBaseBuilder<'TSuccess, unit>()

  member inline _.Zero() = Error()
  member inline _.Return(value) = Error value
  
  member inline _.Bind(opt : Result<_, _>, [<IIL>]_func) =
    match opt with
    | Ok res -> Ok res
    | Error err -> _func err
  
  member inline this.Bind(opt : _ option, [<IIL>]_func) = this.Bind(this.ReturnFrom(opt), _func)
  member inline this.Bind(opt : _ voption, [<IIL>]_func) = this.Bind(this.ReturnFrom(opt), _func)
  member inline this.Combine(opt : Result<_, _>, [<IIL>]_func) = this.Bind(opt, _func)
  
  member inline this.MergeSources(x : Result<_, _>, y : Result<_, _>) : Result<_, _> =
    this.Bind(x, fun xValue -> (
      this.Bind(y, fun yValue -> (
        this.Return(xValue, yValue)
      ))
    ))
  
  override _.While(cond, func) =
    let mutable continuing = true
    let mutable okResult = Unchecked.defaultof<_>
    while continuing && cond() do
      okResult <- func()
      if okResult.IsOk then
        continuing <- false
    if continuing then Error()
    else okResult
