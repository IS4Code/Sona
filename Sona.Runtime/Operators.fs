namespace Sona.Runtime.CompilerServices.Extensions
[<AutoOpen(path = "Sona.Runtime.CompilerServices.Extensions")>]do()

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

[<Extension>]
type RangeExtensions private () =
  [<Extension>] static member inline ``..``(this, other) = (..) this other

[<Extension>]
type SequenceExtensions private () =
  [<Extension>]
  static member inline ``..``(this, other) = Seq.append this other
