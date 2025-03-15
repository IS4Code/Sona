namespace Sona.Runtime.CompilerServices

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

[<Sealed; AbstractClass; AllowNullLiteral>]
type UnaryOperators1 = class end

[<Sealed; AbstractClass; AllowNullLiteral>]
type UnaryOperators2 = class end

type UnaryOperators1 with
  static member inline ``operator Length``(_:UnaryOperators1, _:UnaryOperators2, x) =
    (^T : (member Count : int) x)

type UnaryOperators2 with
  static member inline ``operator Length``(_:UnaryOperators1, _:UnaryOperators2, x) =
    (^T : (member Length : int) x)
    
[<RequireQualifiedAccess>]
module UnaryOperators =
  let inline Length x = ((^self1 or ^self2 or ^x) : (static member ``operator Length``: ^self1 * ^self2 * ^x -> int) (null : UnaryOperators1), (null : UnaryOperators2), x)

[<Sealed; AbstractClass; AllowNullLiteral>]
type BinaryOperators1 = class end

[<Sealed; AbstractClass; AllowNullLiteral>]
type BinaryOperators2 = class end

type BinaryOperators1 with
  static member inline ``operator Concat``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x @ y
  
  // Non-generic overload needed to balance the string one
  static member inline ``operator Concat``(_:BinaryOperators1, _:BinaryOperators2, x : System.DBNull[], y : System.DBNull[]) =
    Seq.append x y
  
  // Impossible overload needed with the same signature as the one in BinaryOperators2
  static member inline ``operator Concat``(_:BinaryOperators1, _:BinaryOperators2, _ : ^T, _ : ^T when ^T :> System.Enum and ^T : not struct and ^T : (new : unit -> ^T)) =
    ()

type BinaryOperators2 with
  static member inline ``operator Concat``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    (..) x y
  
  static member inline ``operator Concat``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    Seq.append x y
    
  static member inline ``operator Concat``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x ^ y
    
type BinaryOperators1 with
  static member inline ``operator Pipe``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x |> y
   
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry1(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry2(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry3(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry4(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry5(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry6(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry7(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry8(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry9(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry10(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry11(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry12(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry13(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry14(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry15(x, y)
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    curry16(x, y)
    
  static member inline ``operator RightShift``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x >> y
    
  static member inline ``operator LeftShift``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x << y
  
  // Impossible overloads needed with the same signature as the ones in BinaryOperators2
  static member inline ``operator Pipe``(_:BinaryOperators1, _:BinaryOperators2, _ : ^T, _ : ^T when ^T :> System.Enum and ^T : not struct and ^T : (new : unit -> ^T)) =
    ()
  
  static member inline ``operator Hat``(_:BinaryOperators1, _:BinaryOperators2, _ : ^T, _ : ^T when ^T :> System.Enum and ^T : not struct and ^T : (new : unit -> ^T)) =
    ()
  
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, _ : ^T, _ : ^T when ^T :> System.Enum and ^T : not struct and ^T : (new : unit -> ^T)) =
    ()

  static member inline ``operator RightShift``(_:BinaryOperators1, _:BinaryOperators2, _ : ^T when ^T :> System.Enum and ^T : not struct and ^T : (new : unit -> ^T), _ : int) =
    ()

  static member inline ``operator LeftShift``(_:BinaryOperators1, _:BinaryOperators2, _ : ^T when ^T :> System.Enum and ^T : not struct and ^T : (new : unit -> ^T), _ : int) =
    ()

type BinaryOperators2 with
  static member inline ``operator Pipe``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x ||| y
   
  static member inline ``operator Hat``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x ^^^ y
   
  static member inline ``operator And``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x &&& y
  
  static member inline ``operator RightShift``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x >>> y
  
  static member inline ``operator LeftShift``(_:BinaryOperators1, _:BinaryOperators2, x, y) =
    x <<< y

[<RequireQualifiedAccess>]
module BinaryOperators =
  let inline Concat y x = ((^self1 or ^self2 or ^x) : (static member ``operator Concat``: ^self1 * ^self2 * ^x * _ -> _) (null : BinaryOperators1), (null : BinaryOperators2), x, y)
  
  let inline Pipe y x = ((^self1 or ^self2 or ^x) : (static member ``operator Pipe``: ^self1 * ^self2 * ^x * _ -> _) (null : BinaryOperators1), (null : BinaryOperators2), x, y)
  
  let inline Hat y x = ((^self1 or ^self2 or ^x) : (static member ``operator Hat``: ^self1 * ^self2 * ^x * _ -> _) (null : BinaryOperators1), (null : BinaryOperators2), x, y)
  
  let inline And y x = ((^self1 or ^self2 or ^x) : (static member ``operator And``: ^self1 * ^self2 * ^x * _ -> _) (null : BinaryOperators1), (null : BinaryOperators2), x, y)
  
  let inline RightShift y x = ((^self1 or ^self2 or ^x) : (static member ``operator RightShift``: ^self1 * ^self2 * ^x * _ -> _) (null : BinaryOperators1), (null : BinaryOperators2), x, y)
  
  let inline LeftShift y x = ((^self1 or ^self2 or ^x) : (static member ``operator LeftShift``: ^self1 * ^self2 * ^x * _ -> _) (null : BinaryOperators1), (null : BinaryOperators2), x, y)

  let inline (..) x y = Concat y x
  let inline (<|>) x y = Pipe y x
  let inline (<^>) x y = Hat y x
  let inline (<&>) x y = And y x
  let inline (>>) x y = RightShift y x
  let inline (<<) x y = LeftShift y x
