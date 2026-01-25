namespace Sona.Runtime.Traits

type ``trait number``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (static member ( + ): ^T -> ^T -> ^T)
  and ^T : (static member ( - ): ^T -> ^T -> ^T)
  and ^T : (static member ( * ): ^T -> ^T -> ^T)
  and ^T : (static member ( / ): ^T -> ^T -> ^T)
  and ^T : (static member Zero: ^T)
  and ^T : (static member One: ^T)
  > = ^T

// DateTime(Offset), DateOnly
type ``trait date``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Year: int)
  and ^T : (member Month: int)
  and ^T : (member Day: int)
  and ^T : (member DayOfYear: int)
  and ^T : (member DayOfWeek: System.DayOfWeek)
  > = ^T

// DateTime(Offset), TimeOnly
type ``trait time``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Hour: int)
  and ^T : (member Minute: int)
  and ^T : (member Second: int)
  and ^T : (member Millisecond: int)
  > = ^T

// DateTime(Offset)
type ``trait datetime``<^T
  when ^T : comparison
  and ^T : equality
  
  and ^T : (member Year: int)
  and ^T : (member Month: int)
  and ^T : (member Day: int)
  and ^T : (member DayOfYear: int)
  and ^T : (member DayOfWeek: System.DayOfWeek)
  
  and ^T : (member Hour: int)
  and ^T : (member Minute: int)
  and ^T : (member Second: int)
  and ^T : (member Millisecond: int)
  > = ``trait date``<``trait time``<^T>>

// TimeSpan
type ``trait timespan``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Days: int)
  and ^T : (member Hours: int)
  and ^T : (member Minutes: int)
  and ^T : (member Seconds: int)
  and ^T : (member Milliseconds: int)
  > = ^T

type ``trait datespan``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Years: int)
  and ^T : (member Months: int)
  > = ^T

type ``trait datetimespan``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Years: int)
  and ^T : (member Months: int)
  and ^T : (member Days: int)
  and ^T : (member Hours: int)
  and ^T : (member Minutes: int)
  and ^T : (member Seconds: int)
  and ^T : (member Milliseconds: int)
  > = ``trait datespan``<``trait timespan``<^T>>

type ``trait iterable``<^T when ^T :> System.Collections.IEnumerable> = ^T
type ``trait iterable``<^T, ^Element when ^T :> System.Collections.Generic.IEnumerable<^Element>> = ^T
type ``trait iterable``<^T, ^Element, ^Enumerator
  when ^T : (member GetEnumerator: unit -> ^Enumerator)
  and ^Enumerator : (member MoveNext: unit -> bool)
  and ^Enumerator : (member Current: ^Element)
  > = ^T

type ``trait class``<^T when ^T : not struct> = ^T
type ``trait struct``<^T when ^T : struct> = ^T
type ``trait delegate``<^T when ^T :> System.Delegate> = ^T
type ``trait delegate``<^T, ^TArgs when ^T : delegate<^TArgs, unit>> = ^T
type ``trait delegate``<^T, ^TArgs, ^TResult when ^T : delegate<^TArgs, ^TResult>> = ^T

type ``trait enum``<^T when ^T : struct and ^T :> System.Enum> = ^T
type ``trait enum``<^T, ^TBase when ^T : enum<^TBase>> = ^T

type ``trait comparable``<^T when ^T : comparison> = ^T
type ``trait equatable``<^T when ^T : equality> = ^T
type ``trait unmanaged``<^T when ^T : unmanaged> = ^T

namespace Sona.Runtime

module Core =
  type object = obj
  type short = int16
  type ushort = uint16
  type long = int64
  type ulong = uint64
  type ``exception`` = System.Exception
  type ``void`` = System.Void

  [<MeasureAnnotatedAbbreviation>]
  type unit<[<Measure>]'M> = unit

  type float64 = double

  type float64<[<Measure>]'M> = double<'M>

  type bigint = System.Numerics.BigInteger

  [<MeasureAnnotatedAbbreviation>]
  type bigint<[<Measure>]'M> = bigint

  type complex = System.Numerics.Complex

  [<MeasureAnnotatedAbbreviation>]
  type complex<[<Measure>]'M> = complex

  [<System.Obsolete("This marks unimplemented code that should be implemented prior to publishing.")>]
  let inline todo<'T> : 'T =
    raise(System.NotImplementedException())
