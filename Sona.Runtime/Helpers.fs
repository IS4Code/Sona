namespace Sona.Runtime.CompilerServices.Internal

[<AbstractClass; AllowNullLiteral>]
type Priority1 internal() = class end

[<AbstractClass; AllowNullLiteral>]
type Priority2 internal() =
  inherit Priority1()
 
[<AbstractClass; AllowNullLiteral>]
type Priority3 internal() =
  inherit Priority2()

[<Sealed; AllowNullLiteral>]
type Priority4 internal() =
  inherit Priority3()

[<Sealed; AbstractClass; AllowNullLiteral>]
type UnitMarker<[<Measure>]'M> = class end

namespace Sona.Runtime.CompilerServices.Extensions
[<AutoOpen(path = "Sona.Runtime.CompilerServices.Extensions")>]do()

open System
open System.Runtime.CompilerServices
open Sona.Runtime.CompilerServices.Internal

[<AbstractClass; Extension>]
type SequenceExtensions internal() = 
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : byref<^T> when ^T : struct, _:Priority1) = &o
  
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : ^T array, _:Priority2): ^T seq = o
  
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : ^T list, _:Priority2): ^T seq = o
  
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : ArraySegment<^T>, _:Priority2) = o
  
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : Span<^T>, _:Priority2) = o
  
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : ReadOnlySpan<^T>, _:Priority2) = o

[<Sealed; AbstractClass; Extension>]
type SequenceExtensions2 private() = 
  inherit SequenceExtensions()
  
  [<Extension>]
  static member inline ``operator AsEnumerable``(o : ^T when ^T : not struct, _:Priority1) = o

[<Sealed; AbstractClass; Extension>]
type VariableExtensions private() = 
  [<Extension>]
  static member inline ``operator Assign``(var : byref<^T>, value : ^T) =
    var <- value
    &var

namespace Sona.Runtime.CompilerServices
open System
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices
open Sona.Runtime.Core
open Sona.Runtime.Traits
open Sona.Runtime.CompilerServices.Internal

[<Sealed; AbstractClass>]
type SequenceHelpers private() =
  static member inline DisposeEnumerator(_:Priority1, o : byref<^T>
    when ^T : not struct) =
    match box o with
    | :? IDisposable as d -> d.Dispose()
    | _ -> ()

  static member inline DisposeEnumerator(_:Priority2, o : byref<^T>
    when ^T : struct) = ()
    
  static member inline DisposeEnumerator(_:Priority3, o : byref<^T>
    when ^T : struct
    and ^T :> IDisposable) =
    o.Dispose()

module SequenceHelpers =
  [<Literal>]
  let Marker : Priority4 = null

[<AbstractClass; AllowNullLiteral>]
type OperatorsBase internal() = class end

[<Sealed; AbstractClass; AllowNullLiteral>]
type Operators1 = class inherit OperatorsBase end

[<Sealed; AbstractClass; AllowNullLiteral>]
type Operators2 = class inherit OperatorsBase end

type Operators1 with
  static member inline ``operator Throw``(_:Operators1, _:Operators2, x) =
    let _ = (^T : (member ``throw()``: unit -> _) x)
    raise (ArgumentException("The object's 'throw()' operator implementation did not throw an exception.", nameof x))
    x
    
  static member inline ``operator Rethrow``(_:Operators1, _:Operators2, x) =
    let _ = (^T : (member ``rethrow()``: unit -> _) x)
    raise (ArgumentException("The object's 'rethrow()' operator implementation did not throw an exception.", nameof x))
    x
  
  static member inline ``operator Throw``(_:Operators1, _:Operators2, x) =
    failwith x
    x
  
  // Including this prevents strings from being inferred in Throw
  static member inline ``operator Throw``(_:Operators1, _:Operators2, x : RuntimeWrappedException) =
    raise x
    x
  
  static member inline ``operator Rethrow``(_:Operators1, _:Operators2, x : ExceptionDispatchInfo) =
    x.Throw()
    x
  
  static member inline ``operator LiftResult``(_:Operators1, _:Operators2, _ : Nullable<^T>, x : ^T) =
    Nullable.op_Implicit(x)
  
  static member inline ``operator LiftResult``(_:Operators1, _:Operators2, _ : ^T option, x : ^T) =
    Some x
  
  static member inline ``operator LiftResult``(_:Operators1, _:Operators2, _ : ^T voption, x : ^T) =
    ValueSome x
  
  static member inline ``operator LiftResult``(_:Operators1, _:Operators2, _ : Result<^T, _>, x : ^T) =
    Ok x
  
  static member inline ``operator BindToLiftedResult``(_:Operators1, _:Operators2, x : Nullable<_>, y) =
    match x with
    | NonNullV value -> struct(true, ((^self1 or ^self2 or ^y) : (static member ``operator LiftResult``: ^self1 * ^self2 * ^y * _ -> _) (null : Operators1), (null : Operators2), y, value))
    | NullV -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToLiftedResult``(_:Operators1, _:Operators2, x : _ option, y) =
    match x with
    | Some value -> struct(true, ((^self1 or ^self2 or ^y) : (static member ``operator LiftResult``: ^self1 * ^self2 * ^y * _ -> _) (null : Operators1), (null : Operators2), y, value))
    | None -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToLiftedResult``(_:Operators1, _:Operators2, x : _ voption, y) =
    match x with
    | ValueSome value -> struct(true, ((^self1 or ^self2 or ^y) : (static member ``operator LiftResult``: ^self1 * ^self2 * ^y * _ -> _) (null : Operators1), (null : Operators2), y, value))
    | ValueNone -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToLiftedResult``(_:Operators1, _:Operators2, x : Result<_, _>, y) =
    match x with
    | Ok value -> struct(true, ((^self1 or ^self2 or ^y) : (static member ``operator LiftResult``: ^self1 * ^self2 * ^y * _ -> _) (null : Operators1), (null : Operators2), y, value))
    | Error _ -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToResult``(_:Operators1, _:Operators2, x : Nullable<_>) =
    match x with
    | NonNullV value -> struct(true, value)
    | NullV -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToResult``(_:Operators1, _:Operators2, x : _ option) =
    match x with
    | Some value -> struct(true, value)
    | None -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToResult``(_:Operators1, _:Operators2, x : _ voption) =
    match x with
    | ValueSome value -> struct(true, value)
    | ValueNone -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToResult``(_:Operators1, _:Operators2, x : Result<_, _>) =
    match x with
    | Ok value -> struct(true, value)
    | Error _ -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:Operators2, x : Nullable<_>) =
    match x with
    | NonNullV value -> struct(true, value)
    | NullV -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:Operators2, x : _ option) =
    match x with
    | Some value -> struct(true, value)
    | None -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:Operators2, x : _ voption) =
    match x with
    | ValueSome value -> struct(true, value)
    | ValueNone -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:Operators2, x : Result<_, _>) =
    match x with
    | Ok value -> struct(true, value)
    | Error _ -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator Convert``(_:Operators1, _:Operators2, x : ^T, _ : ^U) =
    ((^T or ^U) : (static member op_Explicit : ^T -> ^U) x)
  
  static member inline ``operator ConvertInvariant``(_:Operators1, _:Operators2, x : ^T, _ : ^U) =
    ((^T or ^U) : (static member op_Explicit : ^T -> ^U) x)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : float<^M1>, _ : UnitMarker<^M2>, _ : float<^M2>) : float<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : float32<^M1>, _ : UnitMarker<^M2>, _ : float32<^M2>) : float32<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : decimal<^M1>, _ : UnitMarker<^M2>, _ : decimal<^M2>) : decimal<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : int<^M1>, _ : UnitMarker<^M2>, _ : int<^M2>) : int<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : sbyte<^M1>, _ : UnitMarker<^M2>, _ : sbyte<^M2>) : sbyte<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : int16<^M1>, _ : UnitMarker<^M2>, _ : int16<^M2>) : int16<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : int64<^M1>, _ : UnitMarker<^M2>, _ : int64<^M2>) : int64<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : nativeint<^M1>, _ : UnitMarker<^M2>, _ : nativeint<^M2>) : nativeint<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : uint<^M1>, _ : UnitMarker<^M2>, _ : uint<^M2>) : uint<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : byte<^M1>, _ : UnitMarker<^M2>, _ : byte<^M2>) : byte<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : uint16<^M1>, _ : UnitMarker<^M2>, _ : uint16<^M2>) : uint16<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : uint64<^M1>, _ : UnitMarker<^M2>, _ : uint64<^M2>) : uint64<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : unativeint<^M1>, _ : UnitMarker<^M2>, _ : unativeint<^M2>) : unativeint<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : unit<^M1>, _ : UnitMarker<^M2>, _ : unit<^M2>) : unit<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : bigint<^M1>, _ : UnitMarker<^M2>, _ : bigint<^M2>) : bigint<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:Operators2, x : complex<^M1>, _ : UnitMarker<^M2>, _ : complex<^M2>) : complex<^M2> =
    (# "" x : _ #)
  
  static member inline ``operator ConvertEnum``(_:Operators1, _:Operators2, x : ^T, _ : ^U) : ^U =
    LanguagePrimitives.EnumOfValue(x)
  
  static member inline ``operator ConvertEnum``(_:Operators1, _:Operators2, x : string, _ : ^T when ^T : enum<_>) : ^T =
    match Enum.TryParse<^T>(x) with
    | (true, result) -> result
    // Expose the exception on failure
    | _ -> downcast Enum.Parse(typeof<^T>, x)
  
  static member inline ``operator Default``(_:Operators1, _:Operators2, _ : Nullable<^T>) : Nullable<^T> =
    Nullable()
  
  static member inline ``operator Default``(_:Operators1, _:Operators2, _ : ^T option) : ^T option =
    None
  
  static member inline ``operator Default``(_:Operators1, _:Operators2, _ : ^T voption) : ^T voption =
    ValueNone
  
  static member inline ``operator Default``(_:Operators1, _:Operators2, ()) =
    ()
  
  static member inline ``operator Default``(_:Operators1, _:OperatorsBase, _ : ^T when ^T : unmanaged and ^T : struct and ^T : (new : unit -> ^T)) : ^T =
    Unchecked.defaultof<^T>
  
  // Impossible overloads needed with the same signature as the one in Operators2
  static member inline ``operator BindToLiftedResult``(_:Operators1, _:OperatorsBase, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T), _) = ()
  
  static member inline ``operator BindToResult``(_:Operators1, _:Operators2, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T)) = ()
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:Operators2, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T)) = ()
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:OperatorsBase, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T)) = ()
  
  static member inline ``operator TryConversion``(_:Operators1, _:Operators2, _ : ^T, _ : ^T -> ^U when ^U :> Enum and ^U : not struct and ^U : (new : unit -> ^U)) = ()
  
  static member inline ``operator TryConversionValue``(_:Operators1, _:Operators2, _ : ^T, _ : ^T -> ^U when ^U :> Enum and ^U : not struct and ^U : (new : unit -> ^U)) = ()
  
  static member inline ``operator ConvertUnit``(_:OperatorsBase, _:Operators2, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T), _ : UnitMarker<_>, _) = ()
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:OperatorsBase, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T), _ : UnitMarker<_>, _) = ()
  
  static member inline ``operator Default``(_:Operators1, _:Operators2, _ : ^T when ^T :> Enum and ^T : not struct and ^T : (new : unit -> ^T)) = ()

type Operators2 with
  static member inline ``operator Throw``(_:Operators1, _:Operators2, x : #Exception) =
    raise x
    x
  
  static member inline ``operator Rethrow``(_:Operators1, _:Operators2, x : #Exception) =
    ExceptionDispatchInfo.Capture(x).Throw()
    x
  
  // Including this prevents ExceptionDispatchInfo from being inferred in Rethrow
  static member inline ``operator Rethrow``(_:Operators1, _:Operators2, x : RuntimeWrappedException) =
    ExceptionDispatchInfo.Capture(x).Throw()
    x
  
  static member inline ``operator LiftResult``(_:Operators1, _:Operators2, _ : ^T, x : ^T) =
    x
  
  static member inline ``operator BindToLiftedResult``(_:Operators1, _:OperatorsBase, x, y) =
    match x with
    | NonNull value -> struct(true, ((^self1 or ^self2 or ^y) : (static member ``operator LiftResult``: ^self1 * ^self2 * ^y * _ -> _) (null : Operators1), (null : Operators2), y, value))
    | Null -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator BindToResult``(_:Operators1, _:Operators2, x) =
    match x with
    | NonNull value -> struct(true, value)
    | Null -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:Operators2, x) =
    match x with
    | NonNull value -> struct(true, value)
    | Null -> struct(false, Unchecked.defaultof<_>)
  
  static member inline ``operator OptionalBindToResult``(_:Operators1, _:OperatorsBase, x : ^T when ^T : struct) =
    struct(true, x)
  
  static member inline ``operator Convert``(_:Operators1, _:Operators2, x : ^T, y : ^U) =
    ((^T or ^U) : (static member Parse : ^T -> ^U) x)
    
  static member inline ``operator ConvertInvariant``(_:Operators1, _:Operators2, x : ^T, y : ^U) =
    ((^T or ^U) : (static member Parse : ^T * IFormatProvider -> ^U) x, System.Globalization.CultureInfo.InvariantCulture)
  
  static member inline ``operator TryConversion``(_:Operators1, _:Operators2, x : ^T, f : ^T -> ^U) =
    try Some(f(x)) with
    | (:? FormatException) | (:? OverflowException) | (:? InvalidCastException) | (:? ArgumentException) -> None
  
  static member inline ``operator TryConversion``(_:Operators1, _:Operators2, n : Nullable<^T>, f : _ -> ^U) =
    if n.HasValue then ((^self1 or ^self2 or ^T) : (static member ``operator TryConversion``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U option) (null : Operators1), (null : Operators2), n.GetValueOrDefault(), f)
    else None
  
  static member inline ``operator TryConversion``(_:Operators1, _:Operators2, o : ^T option, f : _ -> ^U) =
    match o with
    | Some x -> ((^self1 or ^self2 or ^T) : (static member ``operator TryConversion``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U option) (null : Operators1), (null : Operators2), x, f)
    | _ -> None
  
  static member inline ``operator TryConversion``(_:Operators1, _:Operators2, o : ^T voption, f : _ -> ^U) =
    match o with
    | ValueSome x -> ((^self1 or ^self2 or ^T) : (static member ``operator TryConversion``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U option) (null : Operators1), (null : Operators2), x, f)
    | _ -> None
  
  static member inline ``operator TryConversion``(_:Operators1, _:Operators2, r : Result<^T, _>, f : _ -> ^U) =
    match r with
    | Ok x -> ((^self1 or ^self2 or ^T) : (static member ``operator TryConversion``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U option) (null : Operators1), (null : Operators2), x, f)
    | _ -> None
  
  static member inline ``operator TryConversionValue``(_:Operators1, _:Operators2, x : ^T, f : ^T -> ^U) =
    try ValueSome(f(x)) with
    | (:? FormatException) | (:? OverflowException) | (:? InvalidCastException) | (:? ArgumentException) -> ValueNone
  
  static member inline ``operator TryConversionValue``(_:Operators1, _:Operators2, n : Nullable<^T>, f : _ -> ^U) =
    if n.HasValue then ((^self1 or ^self2 or ^T) : (static member ``operator TryConversionValue``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U voption) (null : Operators1), (null : Operators2), n.GetValueOrDefault(), f)
    else ValueNone
  
  static member inline ``operator TryConversionValue``(_:Operators1, _:Operators2, o : ^T option, f : _ -> ^U) =
    match o with
    | Some x -> ((^self1 or ^self2 or ^T) : (static member ``operator TryConversionValue``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U voption) (null : Operators1), (null : Operators2), x, f)
    | _ -> ValueNone
  
  static member inline ``operator TryConversionValue``(_:Operators1, _:Operators2, o : ^T voption, f : _ -> ^U) =
    match o with
    | ValueSome x -> ((^self1 or ^self2 or ^T) : (static member ``operator TryConversionValue``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U voption) (null : Operators1), (null : Operators2), x, f)
    | _ -> ValueNone
  
  static member inline ``operator TryConversionValue``(_:Operators1, _:Operators2, r : Result<^T, _>, f : _ -> ^U) =
    match r with
    | Ok x -> ((^self1 or ^self2 or ^T) : (static member ``operator TryConversionValue``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U voption) (null : Operators1), (null : Operators2), x, f)
    | _ -> ValueNone
  
  static member inline ``operator ConvertUnit``(_:OperatorsBase, _:Operators2, x : ^T, m : UnitMarker<_>, y : ^U) : ^U =
    if true then (# "" x : _ #)
    else ((^T or ^U) : (static member ``operator UnitCompatibility``: _ * _ * _ -> unit) x, m, y) ; Unchecked.defaultof<_>
  
  static member inline ``operator ConvertUnit``(_:Operators1, _:OperatorsBase, x : ^T, m : UnitMarker<_>, y : ^U) : ^U =
    if true then (# "" x : _ #)
    else ((^T or ^U) : (static member ``operator UnitCompatibility``: _ * _ * _ -> unit) y, m, x) ; Unchecked.defaultof<_>
  
  static member inline ``operator ConvertEnum``(_:Operators1, _:Operators2, x : ^T, _ : ^U) : ^U =
    LanguagePrimitives.EnumToValue(x)
  
  static member inline ``operator Default``(_:Operators1, _:Operators2, _ : ^T) : ^T =
    null

module Operators =
  let inline Throw(x) =
    let _ = ((^self1 or ^self2 or ^x) : (static member ``operator Throw``: ^self1 * ^self2 * ^x -> ^x) (null : Operators1), (null : Operators2), x)
    Unchecked.defaultof<_>
    
  let inline Rethrow(x) =
    let _ = ((^self1 or ^self2 or ^x) : (static member ``operator Rethrow``: ^self1 * ^self2 * ^x -> ^x) (null : Operators1), (null : Operators2), x)
    Unchecked.defaultof<_>

  let inline ThrowObject(x : obj) =
    (# "throw" x #)
    Unchecked.defaultof<_>
  
  let inline BindToLiftedResult(y)(x) =
    ((^self1 or ^self2 or ^x) : (static member ``operator BindToLiftedResult``: ^self1 * ^self2 * ^x * _ -> struct(bool * _)) (null : Operators1), (null : Operators2), x, y)
  
  let inline BindToResult x =
    ((^self1 or ^self2 or ^x) : (static member ``operator BindToResult``: ^self1 * ^self2 * ^x -> struct(bool * _)) (null : Operators1), (null : Operators2), x)
  
  let inline OptionalBindToResult x =
    ((^self1 or ^self2 or ^x) : (static member ``operator OptionalBindToResult``: ^self1 * ^self2 * ^x -> struct(bool * _)) (null : Operators1), (null : Operators2), x)
  
  let inline Convert(y)(x) =
    ((^self1 or ^self2 or ^x) : (static member ``operator Convert``: ^self1 * ^self2 * ^x * ^y -> ^y) (null : Operators1), (null : Operators2), x, y)
  
  let inline ConvertInvariant(y)(x) =
    ((^self1 or ^self2 or ^x) : (static member ``operator ConvertInvariant``: ^self1 * ^self2 * ^x * ^y -> ^y) (null : Operators1), (null : Operators2), x, y)
  
  let inline TryConversion(x : ^T)(f : _ -> ^U) =
    ((^self1 or ^self2 or ^T) : (static member ``operator TryConversion``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U option) (null : Operators1), (null : Operators2), x, f)
  
  let inline TryConversionValue(x : ^T)(f : _ -> ^U) =
    ((^self1 or ^self2 or ^T) : (static member ``operator TryConversionValue``: ^self1 * ^self2 * ^T * (_ -> ^U) -> ^U voption) (null : Operators1), (null : Operators2), x, f)
  
  let inline Implicit<^T, ^U when (^T or ^U) : (static member op_Implicit: ^T -> ^U)> x =
    ((^T or ^U) : (static member op_Implicit: ^T -> ^U) x)

  let inline Explicit<^T, ^U when (^T or ^U) : (static member op_Explicit: ^T -> ^U)> x =
    ((^T or ^U) : (static member op_Explicit: ^T -> ^U) x)
  
  let inline ConvertUnit<^T, ^U, [<Measure>]^M when (Operators1 or Operators2 or ^T) : (static member ``operator ConvertUnit`` : Operators1 * Operators2 * ^T * UnitMarker<^M> * ^U -> ^U)>(x : ^T) : ^U =
    ((^self1 or ^self2 or ^T) : (static member ``operator ConvertUnit``: ^self1 * ^self2 * ^T * _ * ^U -> ^U) (null : Operators1), (null : Operators2), x, (null : UnitMarker<^M>), Unchecked.defaultof<^U>)
  
  let inline ConvertEnum<^T, ^U when (Operators1 or Operators2 or ^T) : (static member ``operator ConvertEnum`` : Operators1 * Operators2 * ^T * ^U -> ^U)>(x : ^T) : ^U =
    ((^self1 or ^self2 or ^T) : (static member ``operator ConvertEnum``: ^self1 * ^self2 * ^T * ^U -> ^U) (null : Operators1), (null : Operators2), x, Unchecked.defaultof<^U>)

  [<GeneralizableValue>]
  let inline Default<^T when (Operators1 or Operators2 or ^T) : (static member ``operator Default``: Operators1 * Operators2 * ^T -> ^T)> : ^T =
    ((^self1 or ^self2 or ^T) : (static member ``operator Default``: ^self1 * ^self2 * ^T -> ^T) (null : Operators1), (null : Operators2), Unchecked.defaultof<^T>)

module Patterns =
  [<return: Struct>]
  let inline (|RuntimeWrappedException|_|)(x : obj) =
    match x with
    | :? RuntimeWrappedException as wrapped -> ValueSome (wrapped.WrappedException)
    | :? Exception -> ValueNone
    | _ -> ValueSome x

[<AbstractClass; AllowNullLiteral>]
type InferenceBase internal() =
  static member inline ``operator date|timespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``operator date|datespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``operator date|datetimespan``(_:InferenceBase, x:``trait date``<^T>): ^T = x
  static member inline ``operator time|timespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``operator time|datespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``operator time|datetimespan``(_:InferenceBase, x:``trait time``<^T>): ^T = x
  static member inline ``operator datetime|timespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
  static member inline ``operator datetime|datespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
  static member inline ``operator datetime|datetimespan``(_:InferenceBase, x:``trait datetime``<^T>): ^T = x
 
[<Sealed; AbstractClass; AllowNullLiteral>]
type Inference private() =
  inherit InferenceBase()
  static member inline ``operator date|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``operator date|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``operator date|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x
  static member inline ``operator time|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``operator time|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``operator time|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x
  static member inline ``operator datetime|timespan``(_:InferenceBase, x:``trait timespan``<^T>): ^T = x
  static member inline ``operator datetime|datespan``(_:InferenceBase, x:``trait datespan``<^T>): ^T = x
  static member inline ``operator datetime|datetimespan``(_:InferenceBase, x:``trait datetimespan``<^T>): ^T = x

  static member inline ``date|timespan`` x = ((^self or ^x) : (static member ``operator date|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``date|datespan`` x = ((^self or ^x) : (static member ``operator date|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``date|datetimespan`` x = ((^self or ^x) : (static member ``operator date|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|timespan`` x = ((^self or ^x) : (static member ``operator time|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|datespan`` x = ((^self or ^x) : (static member ``operator time|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``time|datetimespan`` x = ((^self or ^x) : (static member ``operator time|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|timespan`` x = ((^self or ^x) : (static member ``operator datetime|timespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|datespan`` x = ((^self or ^x) : (static member ``operator datetime|datespan``: ^self * ^x -> ^x) (null : Inference), x)
  static member inline ``datetime|datetimespan`` x = ((^self or ^x) : (static member ``operator datetime|datetimespan``: ^self * ^x -> ^x) (null : Inference), x)

[<Sealed; AbstractClass; AllowNullLiteral>]
type Tuples =
  static member inline ToTree(_ : unit) = ()
  
  static member inline ToTree(t : Tuple<_>) = (t.Item1, ())
  static member inline ToTree((a, b)) = (a, (b, ()))
  static member inline ToTree((a, b, c)) = (a, (b, (c, ())))
  static member inline ToTree((a, b, c, d)) = (a, (b, (c, (d, ()))))
  static member inline ToTree((a, b, c, d, e)) = (a, (b, (c, (d, (e, ())))))
  static member inline ToTree((a, b, c, d, e, f)) = (a, (b, (c, (d, (e, (f, ()))))))
  static member inline ToTree((a, b, c, d, e, f, g)) = (a, (b, (c, (d, (e, (f, (g, ())))))))
  static member inline ToTree((a, b, c, d, e, f, g, h)) = (a, (b, (c, (d, (e, (f, (g, (h, ()))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))
  static member inline ToTree((a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))
  
  static member inline ToTree(_ : ValueTuple) = ()
  static member inline ToTree(t : ValueTuple<_>) = (t.Item1, ())
  static member inline ToTree(struct(a, b)) = (a, (b, ()))
  static member inline ToTree(struct(a, b, c)) = (a, (b, (c, ())))
  static member inline ToTree(struct(a, b, c, d)) = (a, (b, (c, (d, ()))))
  static member inline ToTree(struct(a, b, c, d, e)) = (a, (b, (c, (d, (e, ())))))
  static member inline ToTree(struct(a, b, c, d, e, f)) = (a, (b, (c, (d, (e, (f, ()))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g)) = (a, (b, (c, (d, (e, (f, (g, ())))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h)) = (a, (b, (c, (d, (e, (f, (g, (h, ()))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))
  static member inline ToTree(struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)) = (a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))
  
  static member inline FromTree(_ : unit) = ()
  static member inline FromTree((a, ())) = a
  static member inline FromTree((a, (b, ()))) = (a, b)
  static member inline FromTree((a, (b, (c, ())))) = (a, b, c)
  static member inline FromTree((a, (b, (c, (d, ()))))) = (a, b, c, d)
  static member inline FromTree((a, (b, (c, (d, (e, ())))))) = (a, b, c, d, e)
  static member inline FromTree((a, (b, (c, (d, (e, (f, ()))))))) = (a, b, c, d, e, f)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, ())))))))) = (a, b, c, d, e, f, g)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, ()))))))))) = (a, b, c, d, e, f, g, h)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))) = (a, b, c, d, e, f, g, h, i)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))) = (a, b, c, d, e, f, g, h, i, j)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))) = (a, b, c, d, e, f, g, h, i, j, k)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  static member inline FromTree((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))) = (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
  
  static member inline FromTreeValue(_ : unit) = ()
  static member inline FromTreeValue((a, ())) = a
  static member inline FromTreeValue((a, (b, ()))) = struct(a, b)
  static member inline FromTreeValue((a, (b, (c, ())))) = struct(a, b, c)
  static member inline FromTreeValue((a, (b, (c, (d, ()))))) = struct(a, b, c, d)
  static member inline FromTreeValue((a, (b, (c, (d, (e, ())))))) = struct(a, b, c, d, e)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, ()))))))) = struct(a, b, c, d, e, f)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, ())))))))) = struct(a, b, c, d, e, f, g)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, ()))))))))) = struct(a, b, c, d, e, f, g, h)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, ())))))))))) = struct(a, b, c, d, e, f, g, h, i)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, ()))))))))))) = struct(a, b, c, d, e, f, g, h, i, j)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, ())))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, ()))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, ())))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, ()))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, ())))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o)
  static member inline FromTreeValue((a, (b, (c, (d, (e, (f, (g, (h, (i, (j, (k, (l, (m, (n, (o, (p, ()))))))))))))))))) = struct(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
  
  static member inline ``operator Append``(_:Tuples, _ : unit, _ : unit) = ()
  static member inline ``operator Append``(_:Tuples, _ : unit, b : (_ * unit)) = b
  static member inline ``operator Append``(_:Tuples, _ : unit, b : (_ * (_ * _))) = b

  static member inline ``operator Append``(_:Tuples, a : (_ * unit), _ : unit) = a
  static member inline ``operator Append``(_:Tuples, (a1, ()), b : (_ * unit)) = (a1, b)
  static member inline ``operator Append``(_:Tuples, (a1, ()), b : (_ * (_ * _))) = (a1, b)

  static member inline ``operator Append``(_:Tuples, a : (_ * (_ * _)), _ : unit) = a
  static member inline ``operator Append``(_:Tuples, (a1, (a2, aRest : ^aRest)), b : (^b1 * unit)) =
    (a1, (a2, ((^self or ^aRest) : (static member ``operator Append`` : ^self * ^aRest * (^b1 * unit) -> _) (null : Tuples), aRest, b)))
  static member inline ``operator Append``(_:Tuples, (a1, (a2, aRest : ^aRest)), b : (^b1 * (^b2 * ^bRest))) =
    (a1, (a2, ((^self or ^aRest) : (static member ``operator Append`` : ^self * ^aRest * (^b1 * (^b2 * ^bRest)) -> _) (null : Tuples), aRest, b)))
  
  static member inline Append(a : unit, b : unit) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : unit, b : (_ * unit)) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : unit, b : (_ * (_ * _))) = Tuples.``operator Append``((null : Tuples), a, b)

  static member inline Append(a : (_ * unit), b : unit) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * unit), b : (_ * unit)) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * unit), b : (_ * (_ * _))) = Tuples.``operator Append``((null : Tuples), a, b)

  static member inline Append(a : (_ * (_ * _)), b : unit) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * (_ * _)), b : (_ * unit)) = Tuples.``operator Append``((null : Tuples), a, b)
  static member inline Append(a : (_ * (_ * _)), b : (_ * (_ * _))) = Tuples.``operator Append``((null : Tuples), a, b)
