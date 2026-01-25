namespace Sona.Runtime.ComputationBuilders

open System.Runtime.CompilerServices
open FSharp.Core.CompilerServices
open System

#nowarn "64" // This construct causes code to be less generic than indicated by the type annotations.

type ArrayBuilder<'T> = { mutable State : ArrayCollector<'T> } with
  member inline _.Zero() = ()
  member inline _.Combine((), [<IIL>]_f : unit -> _) = _f()
  member inline _.Delay([<IIL>]_f : unit -> _) = _f
  member inline _.TryWith([<IIL>]_f : unit -> _, [<IIL>]_fail : _ -> _) =
    try _f() with | e -> _fail e
  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try _f() finally _cleanup()
  member inline _.Using(x, [<IIL>]_f : _ -> _) =
    use _ = x in _f x
  member inline _.While([<IIL>]_cond : unit -> bool, [<IIL>]_body : unit -> unit) =
    while _cond() do _body()
  member inline this.For(s : _ seq, [<IIL>]_f : _ -> unit) =
    use enumerator = s.GetEnumerator()
    this.While(enumerator.MoveNext, (fun() -> _f enumerator.Current))

  member inline this.Yield x = this.State.Add(x)
  member inline this.YieldFrom s = this.State.AddMany(s)
  member inline this.YieldFromFinal s = this.State.AddManyAndClose(s)

  member inline this.Run([<IIL>]_f : unit -> unit) =
    _f()
    this.State.Close()

  member inline _.Run([<IIL>]_f : unit -> 'T array) =
    _f()
    
type ListBuilder<'T> = { mutable State : ListCollector<'T> } with
  member inline _.Zero() = ()
  member inline _.Combine((), [<IIL>]_f : unit -> _) = _f()
  member inline _.Delay([<IIL>]_f : unit -> _) = _f
  member inline _.TryWith([<IIL>]_f : unit -> _, [<IIL>]_fail : _ -> _) =
    try _f() with | e -> _fail e
  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try _f() finally _cleanup()
  member inline _.Using(x, [<IIL>]_f : _ -> _) =
    use _ = x in _f x
  member inline _.While([<IIL>]_cond : unit -> bool, [<IIL>]_body : unit -> unit) =
    while _cond() do _body()
  member inline this.For(s : _ seq, [<IIL>]_f : _ -> unit) =
    use enumerator = s.GetEnumerator()
    this.While(enumerator.MoveNext, (fun() -> _f enumerator.Current))

  member inline this.Yield x = this.State.Add(x)
  member inline this.YieldFrom s = this.State.AddMany(s)
  member inline this.YieldFromFinal s = this.State.AddManyAndClose(s)

  member inline this.Run([<IIL>]_f : unit -> unit) =
    _f()
    this.State.Close()

  member inline _.Run([<IIL>]_f : unit -> 'T list) =
    _f()

[<Struct; IsReadOnly>]
type StringBuilderBuilder<^TState
    when ^TState : (member Clear : unit -> ^TState)
  > = { State : ^TState } with
  member inline _.Zero() = ()
  member inline _.Combine((), [<IIL>]_f : unit -> _) = _f()
  member inline _.Delay([<IIL>]_f : unit -> _) = _f
  member inline _.TryWith([<IIL>]_f : unit -> _, [<IIL>]_fail : _ -> _) =
    try _f() with | e -> _fail e
  member inline _.TryFinally([<IIL>]_f : unit -> _, [<IIL>]_cleanup : unit -> unit) =
    try _f() finally _cleanup()
  member inline _.Using(x, [<IIL>]_f : _ -> _) =
    use _ = x in _f x
  member inline _.While([<IIL>]_cond : unit -> bool, [<IIL>]_body : unit -> unit) =
    while _cond() do _body()
  member inline this.For(s : _ seq, [<IIL>]_f : _ -> unit) =
    use enumerator = s.GetEnumerator()
    this.While(enumerator.MoveNext, (fun() -> _f enumerator.Current))

  member inline this.Return(()) =
    let result = this.State.ToString()
    (^TState : (member Clear : unit -> ^TState) this.State) |> ignore
    result

  member inline this.Run([<IIL>]_f : unit -> unit) =
    _f()
    this.State

  member inline _.Run([<IIL>]_f : unit -> string) =
    _f()
