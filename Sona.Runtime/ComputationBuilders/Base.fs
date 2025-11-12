namespace Sona.Runtime.ComputationBuilders

open System

type internal IIL = InlineIfLambdaAttribute

[<AbstractClass>]
type BaseBuilder<'TZero>() =
  abstract member While : (unit -> bool) * (unit -> 'TZero) -> 'TZero

  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try
      _f()
    finally
      _cleanup()
  
  member inline _.TryWith([<IIL>]_f : unit -> _, [<IIL>]_fail : _ -> _) =
    try
      _f()
    with
    | e -> _fail e
  
  member inline _.Using(x : #IDisposable, [<IIL>]_f : _ -> _ option) =
    try
      _f x
    finally
      x.Dispose()
  
  member inline this.For(s : _ seq, [<IIL>]_f : _ -> 'TZero) =
    let enumerator = s.GetEnumerator()
    try
      this.While(enumerator.MoveNext, (fun() -> _f enumerator.Current))
    finally
      enumerator.Dispose()
