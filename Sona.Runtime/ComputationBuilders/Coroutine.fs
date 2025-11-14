namespace Sona.Runtime.ComputationBuilders

open Sona.Runtime.Coroutines

[<AbstractClass>]
type CoroutineBuilder() =
  member inline _.Delay([<IIL>]_f : unit -> ICoroutine<_, _, _>) = _f
  member inline _.Run([<IIL>]_f) = Coroutine.start _f
  member inline _.Yield x : ICoroutine<unit, _, unit> = Coroutine.``yield`` x
  member inline _.YieldFrom x = Coroutine.yieldFrom x
  member inline _.Return x = Coroutine.``return`` x
  member inline _.ReturnFrom x = Coroutine.returnFrom x
  member inline _.Zero() = Coroutine.zero()
  member inline _.Bind(c, [<IIL>]_f) = Coroutine.bind c _f
  member inline _.Combine(first, [<IIL>]_second) = Coroutine.combine first _second
  member inline _.TryFinally([<IIL>]_first, [<IIL>]_second) = Coroutine.tryFinally _first _second
  member inline _.TryWith([<IIL>]_first, [<IIL>]_second) = Coroutine.tryWith _first _second
  member inline _.Using(x, [<IIL>]_f) = Coroutine.using x _f
  member inline _.While([<IIL>]_cond, [<IIL>]_f) = Coroutine.``while`` _cond _f
  member inline _.For(s, [<IIL>]_f) = Coroutine.``for`` s _f
  
  [<CustomOperation("pause", MaintainsVariableSpaceUsingBind = true)>]
  member inline _.Pause x =
    Coroutine.bind x
      (fun result ->
        Coroutine.combine
          (Coroutine.pause())
          (fun() -> Coroutine.``return`` result))
