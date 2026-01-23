namespace Sona.Runtime

open System
open System.Collections.Concurrent
open System.Runtime.CompilerServices

[<Sealed; AbstractClass>]
type private MultivalueCache<'TValue> private() =
  static member val Default =
    if typeof<string>.Equals typeof<'TValue> then
      downcast(box "")
    else
      Unchecked.defaultof<'TValue>

[<Struct; IsReadOnly; CustomEquality; NoComparison>]
type Multivalue<'TValue, 'TContext when 'TContext : equality> internal(transform : Func<'TContext, 'TValue>, cache : ConcurrentDictionary<'TContext, 'TValue>) =
  member _.Item
    with get(context : 'TContext) =
      if isNull cache then
        if isNull transform then
          MultivalueCache<'TValue>.Default
        else
          transform.Invoke(context)
      else
        cache.GetOrAdd(context, transform)
  
  member inline private _.EqualTransform t =
    t = transform
  
  interface IEquatable<Multivalue<'TValue, 'TContext>> with
    member _.Equals(other: Multivalue<'TValue, 'TContext>) =
      other.EqualTransform transform

module Multivalue =
  [<CompiledName("Default")>]
  let inline ``default``<'TValue, 'TContext when 'TContext : equality> =
    Unchecked.defaultof<Multivalue<'TValue, 'TContext>>
    
  [<CompiledName("New")>]
  let ``new``(factory : 'TContext -> 'TValue) =
    Multivalue<'TValue, 'TContext>(factory, new _())
    
  [<CompiledName("From")>]
  let from value =
    Multivalue<'TValue, 'TContext>((fun _ -> value), null)
    
  [<CompiledName("FromLazy")>]
  let inline fromLazy([<InlineIfLambda>]_factory : unit -> 'TValue) =
    let l = lazy(_factory())
    ``new``(fun _ -> l.Value)
    
  [<CompiledName("FromString")>]
  let inline string(str : FormattableString) =
    ``new`` str.ToString
    
  [<CompiledName("FromFormattable")>]
  let inline formatted (format : string) (formattable : #IFormattable) =
    ``new``(fun provider -> formattable.ToString(format, provider))

  [<CompiledName("Bind")>]
  let inline bind ([<InlineIfLambda>]_binder : _ -> Multivalue<_, _>) (multi : Multivalue<_, _>) =
    ``new``(fun ctx -> _binder(multi[ctx])[ctx])
