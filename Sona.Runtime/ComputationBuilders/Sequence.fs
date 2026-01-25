namespace Sona.Runtime.ComputationBuilders

[<AbstractClass>]
type SequenceBuilder() =
  member inline _.Delay([<IIL>]_f : unit -> _ seq) = _f
  member inline _.Run([<IIL>]_f : unit -> _ seq) = _f()
  member inline _.Yield x = seq { yield x }
  member inline _.YieldFrom x = seq { yield! x }
  member inline _.Zero() = Seq.empty
  member inline _.Combine(first, [<IIL>]_second) = seq { yield! first; yield! _second() }
  member inline _.TryFinally([<IIL>]_first, [<IIL>]_second) = seq {
    try
      yield! _first()
    finally
      _second()
  }
  member inline _.TryWith([<IIL>]_first, [<IIL>]_second) = seq {
    try
      yield! _first()
    with
    | e ->
      yield! _second e
  }
  member inline _.Using(x, [<IIL>]_f) = seq {
    use _ = x
    yield! _f x
  }
  member inline _.While([<IIL>]_cond, [<IIL>]_f) = seq {
    while _cond() do
      yield! _f()
  }
  member inline _.For(s, [<IIL>]_f) = seq {
    for i in s do
      yield! _f i
  }
