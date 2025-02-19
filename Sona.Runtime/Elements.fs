namespace Sona.Runtime

type Marker<^T> = struct
  static member inline Choice(_: Marker<^T>, x: ^T, [<InlineIfLambda>]f1: ^T -> _, [<InlineIfLambda>]f2: _ -> _) = f1 x
  static member inline Choice(_: Marker<^T>, x: ^a, [<InlineIfLambda>]f1: ^T -> _, [<InlineIfLambda>]f2: ^a -> _) = f2 x
end

[<Struct>]
type NumberFormat = NumberFormat
[<Struct>]
type TimeFormat = TimeFormat
[<Struct>]
type DayTimeFormat = DayTimeFormat
[<Struct>]
type DayFormat = DayFormat
[<Struct>]
type DateFormat = DateFormat
[<Struct>]
type DateTimeFormat = DateTimeFormat

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
  and ^T : (member Microsecond: int)
  and ^T : (member Nanosecond: int)
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
  and ^T : (member Microsecond: int)
  and ^T : (member Nanosecond: int)
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
  and ^T : (member Microseconds: int)
  and ^T : (member Nanoseconds: int)
  > = ^T

type ``trait datespan``<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Years: int)
  and ^T : (member Months: int)
  > = ^T

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

type ``trait comparison``<^T when ^T : comparison> = ^T
type ``trait equality``<^T when ^T : equality> = ^T
type ``trait unmanaged``<^T when ^T : unmanaged> = ^T
