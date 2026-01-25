namespace Sona.Runtime.ComputationBuilders

type internal IIL = InlineIfLambdaAttribute

[<AbstractClass>]
type BaseBuilder() =
  member inline _.TryWith([<IIL>]_f : unit -> _, [<IIL>]_fail : _ -> _) =
    try _f()
    with | e -> _fail e

  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try _f()
    finally _cleanup()

  member inline _.Using(x, [<IIL>]_f : _ -> _) =
    use _ = x in _f x

[<AbstractClass>]
type BaseBuilder<'TZero>() =
  inherit BaseBuilder()

  abstract member While : (unit -> bool) * (unit -> 'TZero) -> 'TZero
  
  member inline this.For(s : _ seq, [<IIL>]_f : _ -> 'TZero) =
    let enumerator = s.GetEnumerator()
    try this.While(enumerator.MoveNext, (fun() -> _f enumerator.Current))
    finally enumerator.Dispose()
