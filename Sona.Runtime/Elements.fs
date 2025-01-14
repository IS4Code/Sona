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

type number<^T
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
type date<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Year: int)
  and ^T : (member Month: int)
  and ^T : (member Day: int)
  and ^T : (member DayOfYear: int)
  and ^T : (member DayOfWeek: System.DayOfWeek)
  > = ^T

// DateTime(Offset), TimeOnly
type time<^T
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
type datetime<^T
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
  > = date<time<^T>>

// TimeSpan
type timespan<^T
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

type datespan<^T
  when ^T : comparison
  and ^T : equality
  and ^T : (member Years: int)
  and ^T : (member Months: int)
  > = ^T

type iterable<^T, ^U, ^V
  when ^T : (member GetEnumerator: unit -> ^U)
  and ^U : (member MoveNext: unit -> bool)
  and ^U : (member Current: ^V)
  > = ^T
