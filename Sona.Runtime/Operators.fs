namespace Sona.Runtime

open System
open System.Runtime.CompilerServices

module private helpers =
  let inline curry1(func, a)() = func(a)
  let inline curry2(func, a)(b) = func(a, b)
  let inline curry3(func, a)(b, c) = func(a, b, c)
  let inline curry4(func, a)(b, c, d) = func(a, b, c, d)
  let inline curry5(func, a)(b, c, d, e) = func(a, b, c, d, e)
  let inline curry6(func, a)(b, c, d, e, f) = func(a, b, c, d, e, f)
  let inline curry7(func, a)(b, c, d, e, f, g) = func(a, b, c, d, e, f, g)
  let inline curry8(func, a)(b, c, d, e, f, g, h) = func(a, b, c, d, e, f, g, h)
  let inline curry9(func, a)(b, c, d, e, f, g, h, i) = func(a, b, c, d, e, f, g, h, i)
  let inline curry10(func, a)(b, c, d, e, f, g, h, i, j) = func(a, b, c, d, e, f, g, h, i, j)
  let inline curry11(func, a)(b, c, d, e, f, g, h, i, j, k) = func(a, b, c, d, e, f, g, h, i, j, k)
  let inline curry12(func, a)(b, c, d, e, f, g, h, i, j, k, l) = func(a, b, c, d, e, f, g, h, i, j, k, l)
  let inline curry13(func, a)(b, c, d, e, f, g, h, i, j, k, l, m) = func(a, b, c, d, e, f, g, h, i, j, k, l, m)
  let inline curry14(func, a)(b, c, d, e, f, g, h, i, j, k, l, m, n) = func(a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  let inline curry15(func, a)(b, c, d, e, f, g, h, i, j, k, l, m, n, o) = func(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  let inline curry16(func, a)(b, c, d, e, f, g, h, i, j, k, l, m, n, o, p) = func(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
open helpers

#nowarn "62" "86"

[<Extension>]
type ListExtensions private () =
  [<Extension>] static member inline ``#``(this: ^T when ^T : (member Count : int)) = this.Count
  [<Extension>] static member inline ``..``(this, other) = this @ other

[<Extension>]
type FunctionExtensions private () =
  [<Extension>] static member inline ``|``(this, func) = this |> func
  [<Extension>] static member inline ``&``(this, a) = curry1(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry2(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry3(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry4(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry5(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry6(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry7(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry8(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry9(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry10(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry11(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry12(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry13(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry14(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry15(this, a)
  [<Extension>] static member inline ``&``(this, a) = curry16(this, a)
  [<Extension>] static member inline ``>>``(this, func) = this >> func
  [<Extension>] static member inline ``<<``(this, func) = this << func

[<Extension>]
type BitExtensions private () =
  [<Extension>] static member inline ``|``(this, other) = this ||| other
  [<Extension>] static member inline ``^``(this, other) = this ^^^ other
  [<Extension>] static member inline ``&``(this, other) = this &&& other
  [<Extension>] static member inline ``>>``(this, other) = this >>> other
  [<Extension>] static member inline ``<<``(this, other) = this <<< other

[<Extension>]
type StringExtensions private () =
  [<Extension>] static member inline ``#``(this: ^T when ^T : (member Length : int)) = this.Length
  [<Extension>] static member inline ``..``(this, other) = this ^ other
  [<Extension>] static member inline ``throw()``(this) = failwith this

[<Extension>]
type RangeExtensions private () =
  [<Extension>] static member inline ``..``(this, other) = (..) this other

[<Extension>]
type ExceptionExtensions private () =
  [<Extension>] static member inline ``throw()``(this) = raise this

[<Extension>]
type SequenceExtensions private () =
  [<Extension>]
  static member inline ``..``(this, other) = Seq.append this other

[<Extension>]
type NullExtensions private () =
  [<Extension>]
  static member inline ``?``(this: ^T when ^T : null, [<InlineIfLambda>]action: ^T -> ^U): ^U voption =
    match this with
    | null -> ValueNone
    | _ -> this |> action |> ValueSome
  [<Extension>]
  static member inline ``?``(this: ^T when ^T : null, [<InlineIfLambda>]action: ^T -> ^U voption): ^U voption =
    match this with
    | null -> ValueNone
    | _ -> this |> action
  [<Extension>]
  static member inline ``?``(this: ^T when ^T : null, [<InlineIfLambda>]action: ^T -> ^U option): ^U voption =
    match this with
    | null -> ValueNone
    | _ -> this |> action |> ValueOption.ofOption
  [<Extension>]
  static member inline ``?``(this: ^T when ^T : null, [<InlineIfLambda>]action: ^T -> Nullable<^U>): ^U voption =
    match this with
    | null -> ValueNone
    | _ -> this |> action |> ValueOption.ofNullable
  [<Extension>]
  static member inline ``??``(this: ^T when ^T : null, [<InlineIfLambda>]alternative: unit -> ^T): ^T =
    match this with
    | null -> alternative()
    | _ -> this
  [<Extension>]
  static member inline ``??``(this: ^T when ^T : null, [<InlineIfLambda>]alternative: unit -> ^T voption): ^T voption =
    match this with
    | null -> alternative()
    | _ -> this |> ValueSome
  [<Extension>]
  static member inline ``??``(this: ^T when ^T : null, [<InlineIfLambda>]alternative: unit -> ^T option): ^T voption =
    match this with
    | null -> alternative() |> ValueOption.ofOption
    | _ -> this |> ValueSome
  [<Extension>]
  static member inline ``??``(this: ^T when ^T : null, [<InlineIfLambda>]alternative: unit -> Nullable<^T>): ^T voption =
    match this with
    | null -> alternative() |> ValueOption.ofNullable
    | _ -> this |> ValueSome

[<Extension>]
type ValueOptionExtensions private () =
  [<Extension>]
  static member inline ``?``(this: ^T voption, [<InlineIfLambda>]action: ^T -> ^U): ^U voption =
    match this with
    | ValueSome(value) -> value |> action |> ValueSome
    | _ -> ValueNone
  [<Extension>]
  static member inline ``?``(this: ^T voption, [<InlineIfLambda>]action: ^T -> ^U voption): ^U voption =
    match this with
    | ValueSome(value) -> value |> action
    | _ -> ValueNone
  [<Extension>]
  static member inline ``?``(this: ^T voption, [<InlineIfLambda>]action: ^T -> ^U option): ^U voption =
    match this with
    | ValueSome(value) -> value |> action |> ValueOption.ofOption
    | _ -> ValueNone
  [<Extension>]
  static member inline ``?``(this: ^T voption, [<InlineIfLambda>]action: ^T -> Nullable<^U>): ^U voption =
    match this with
    | ValueSome(value) -> value |> action |> ValueOption.ofNullable
    | _ -> ValueNone
  [<Extension>]
  static member inline ``??``(this: ^T voption, [<InlineIfLambda>]alternative: unit -> ^T): ^T =
    match this with
    | ValueSome(value) -> value
    | _ -> alternative()
  [<Extension>]
  static member inline ``??``(this: ^T voption, [<InlineIfLambda>]alternative: unit -> ^T voption): ^T voption =
    match this with
    | ValueSome _ -> this
    | _ -> alternative()
  [<Extension>]
  static member inline ``??``(this: ^T voption, [<InlineIfLambda>]alternative: unit -> ^T option): ^T voption =
    match this with
    | ValueSome _ -> this
    | _ -> alternative() |> ValueOption.ofOption
  [<Extension>]
  static member inline ``??``(this: ^T voption, [<InlineIfLambda>]alternative: unit -> Nullable<^T>): ^T voption =
    match this with
    | ValueSome _ -> this
    | _ -> alternative() |> ValueOption.ofNullable

[<Extension>]
type OptionExtensions private () =
  [<Extension>]
  static member inline ``?``(this: ^T option, [<InlineIfLambda>]action: ^T -> ^U): ^U voption =
    match this with
    | Some(value) -> value |> action |> ValueSome
    | _ -> ValueNone
  [<Extension>]
  static member inline ``?``(this: ^T option, [<InlineIfLambda>]action: ^T -> ^U voption): ^U voption =
    match this with
    | Some(value) -> value |> action
    | _ -> ValueNone
  [<Extension>]
  static member inline ``?``(this: ^T option, [<InlineIfLambda>]action: ^T -> ^U option): ^U voption =
    match this with
    | Some(value) -> value |> action |> ValueOption.ofOption
    | _ -> ValueNone
  [<Extension>]
  static member inline ``?``(this: ^T option, [<InlineIfLambda>]action: ^T -> Nullable<^U>): ^U voption =
    match this with
    | Some(value) -> value |> action |> ValueOption.ofNullable
    | _ -> ValueNone
  [<Extension>]
  static member inline ``??``(this: ^T option, [<InlineIfLambda>]alternative: unit -> ^T): ^T =
    match this with
    | Some(value) -> value
    | _ -> alternative()
  [<Extension>]
  static member inline ``??``(this: ^T option, [<InlineIfLambda>]alternative: unit -> ^T voption): ^T voption =
    match this with
    | Some(value) -> value |> ValueSome
    | _ -> alternative()
  [<Extension>]
  static member inline ``??``(this: ^T option, [<InlineIfLambda>]alternative: unit -> ^T option): ^T voption =
    match this with
    | Some(value) -> value |> ValueSome
    | _ -> alternative() |> ValueOption.ofOption
  [<Extension>]
  static member inline ``??``(this: ^T option, [<InlineIfLambda>]alternative: unit -> Nullable<^T>): ^T voption =
    match this with
    | Some(value) -> value |> ValueSome
    | _ -> alternative() |> ValueOption.ofNullable

[<Extension>]
type NullableExtensions private () =
  [<Extension>]
  static member inline ``?``(this: Nullable<^T>, [<InlineIfLambda>]action: ^T -> ^U): ^U voption =
    if this.HasValue then this.GetValueOrDefault() |> action |> ValueSome
    else ValueNone
  [<Extension>]
  static member inline ``?``(this: Nullable<^T>, [<InlineIfLambda>]action: ^T -> ^U voption): ^U voption =
    if this.HasValue then this.GetValueOrDefault() |> action
    else ValueNone
  [<Extension>]
  static member inline ``?``(this: Nullable<^T>, [<InlineIfLambda>]action: ^T -> ^U option): ^U voption =
    if this.HasValue then this.GetValueOrDefault() |> action |> ValueOption.ofOption
    else ValueNone
  [<Extension>]
  static member inline ``?``(this: Nullable<^T>, [<InlineIfLambda>]action: ^T -> Nullable<^U>): ^U voption =
    if this.HasValue then this.GetValueOrDefault() |> action |> ValueOption.ofNullable
    else ValueNone
  [<Extension>]
  static member inline ``??``(this: Nullable<^T>, [<InlineIfLambda>]alternative: unit -> ^T): ^T =
    if this.HasValue then this.GetValueOrDefault()
    else alternative()
  [<Extension>]
  static member inline ``??``(this: Nullable<^T>, [<InlineIfLambda>]alternative: unit -> ^T voption): ^T voption =
    if this.HasValue then this.GetValueOrDefault() |> ValueSome
    else alternative()
  [<Extension>]
  static member inline ``??``(this: Nullable<^T>, [<InlineIfLambda>]alternative: unit -> ^T option): ^T voption =
    if this.HasValue then this.GetValueOrDefault() |> ValueSome
    else alternative() |> ValueOption.ofOption
  [<Extension>]
  static member inline ``??``(this: Nullable<^T>, [<InlineIfLambda>]alternative: unit -> Nullable<^T>): ^T voption =
    if this.HasValue then this.GetValueOrDefault() |> ValueSome
    else alternative() |> ValueOption.ofNullable

[<Extension>]
type NumberFormatExtensions private () =
  [<Extension>]
  static member inline ``{}``(this: NumberFormat, value: number<_>) = value
 
[<Extension>]
type TimeFormatExtensions private () =
  [<Extension>]
  static member inline ``{}``(this: TimeFormat, value: time<_>) = value
 
[<Extension>]
type DateFormatExtensions private () =
  [<Extension>]
  static member inline ``{}``(this: DateFormat, value: date<_>) = value

[<Extension>]
type TimeSpanFormatExtensions private () =
  [<Extension>]
  static member inline ``{}``(this: TimeFormat, value: timespan<_>) = value
  [<Extension>]
  static member inline ``{}``(this: DayTimeFormat, value: timespan<_>) = value
  [<Extension>]
  static member inline ``{}``(this: DayFormat, value: timespan<_>) = value
