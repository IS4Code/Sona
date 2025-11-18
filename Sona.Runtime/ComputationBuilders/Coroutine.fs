namespace Sona.Runtime.ComputationBuilders

open System
open Sona.Runtime.Coroutines

[<AbstractClass>]
type CoroutineBuilder() =
  member inline _.Delay([<IIL>]_f : unit -> ICoroutine<_, _, _>) = _f
  member inline _.Run([<IIL>]_f) = Coroutine.run _f
  member inline _.Yield x : ICoroutine<unit, _, unit> = Coroutine.``yield`` x
  member inline _.YieldFrom x = x : ICoroutine<_, _, unit>
  member inline _.Return x = Coroutine.fromResult x
  member inline _.ReturnFrom x = x : ICoroutine<_, _, _>
  member inline _.Zero() = Coroutine.empty()
  member inline _.Bind(c, [<IIL>]_f) = Coroutine.bind c _f
  member inline _.Combine(first : ICoroutine<_, _, unit>, [<IIL>]_second) = Coroutine.bind first _second
  member inline _.TryFinally([<IIL>]_first, [<IIL>]_second) = Coroutine.withCleanup _first _second
  member inline _.TryWith([<IIL>]_first, [<IIL>]_second) = Coroutine.withCatch _first _second
  member inline _.Using(x : #IDisposable, [<IIL>]_f) = Coroutine.withCleanup (fun() -> _f(x)) (fun() -> x.Dispose())
  member inline _.While([<IIL>]_cond, [<IIL>]_f) = Coroutine.loop _cond _f
  member inline _.For(s : _ seq, [<IIL>]_f) =
    let enumerator = s.GetEnumerator()
    Coroutine.withCleanup
      (fun() -> (
        Coroutine.loop
          (fun() -> enumerator.MoveNext())
          (fun() -> _f enumerator.Current)
      ))
      (fun() -> enumerator.Dispose())
  
  [<CustomOperation("pause", MaintainsVariableSpaceUsingBind = true)>]
  member inline _.Pause x =
    Coroutine.bind x
      (fun result ->
        Coroutine.bind
          (Coroutine.pause())
          (fun() -> Coroutine.fromResult result))
