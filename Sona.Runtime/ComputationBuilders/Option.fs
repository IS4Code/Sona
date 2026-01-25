namespace Sona.Runtime.ComputationBuilders

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
  
  member inline this.MergeSources(x : _ option, y : _ option) : _ option =
    this.Bind(x, fun xValue -> (
      this.Bind(y, fun yValue -> (
        this.Return(xValue, yValue)
      ))
    ))
  
  override _.While(cond, func) =
    let mutable continuing = true
    while continuing && cond() do
      if func().IsNone then
        continuing <- false
    if continuing then Some()
    else None

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
  
  member inline this.MergeSources(x : _ voption, y : _ voption) : _ voption =
    this.Bind(x, fun xValue -> (
      this.Bind(y, fun yValue -> (
        this.Return(xValue, yValue)
      ))
    ))
  
  override _.While(cond, func) =
    let mutable continuing = true
    while continuing && cond() do
      if func().IsValueNone then
        continuing <- false
    if continuing then ValueSome()
    else ValueNone
