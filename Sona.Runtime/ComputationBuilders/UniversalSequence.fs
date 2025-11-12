namespace Sona.Runtime.ComputationBuilders

open System.Runtime.CompilerServices
open Sona.Runtime.Collections

[<Struct; IsReadOnly; NoEquality; NoComparison>]
type UniversalSequenceBuilder<'TBoolBuilder, 'TUnitBuilder> =
  {
    BoolBuilder : 'TBoolBuilder
    UnitBuilder : 'TUnitBuilder
  }
  member inline _.YieldFrom x = UniversalSequence.yieldFrom x
  member inline _.Delay([<IIL>]_f) = UniversalSequence.delay _f
  member inline _.Run([<IIL>]_f) = UniversalSequence.run _f
