namespace Sona.Runtime.Core

open System
open System.Collections
open System.Collections.Generic

module private SequenceHelpers =
  [<Literal>]
  let listReadOnly = "The collection is read-only."
open SequenceHelpers

[<Interface>]
type ISequenceType<'T> =
  inherit IComparer<'T>
  inherit IEqualityComparer<'T>
  abstract member DefaultStep : 'T
  abstract member GetStep : first : 'T * second : 'T -> 'T
  abstract member GetIndex : first : 'T * step : 'T * item : 'T -> long voption
  abstract member GetItem : first : 'T * step : 'T * index : long -> 'T

[<Struct>]
type BoundedSequence<'T> private(generator : 'T seq, sequenceType : ISequenceType<'T>, stop : 'T) =
  member _.IndexOf item =
    if sequenceType.Compare(item, stop) > 0 then
      ValueNone
    else
      use e = generator.GetEnumerator()
      if not(e.MoveNext()) then
        ValueNone
      else
        let first = e.Current
        if sequenceType.Compare(item, first) < 0 then
          ValueNone
        else
          if not(e.MoveNext()) then
            if sequenceType.Equals(first, item) then
              ValueSome 0L
            else
              ValueNone
          else
            let second = e.Current
            let step = sequenceType.GetStep(first, second)
            sequenceType.GetIndex(item, first, step)

  interface IEnumerable with
    member _.GetEnumerator() = generator.GetEnumerator()

  interface IEnumerable<'T> with
    member _.GetEnumerator() = generator.GetEnumerator()

  interface IList with
    member this.Contains value =
      match value with
      | :? 'T as item -> this.Contains item
      | _ -> false

    member _.CopyTo(array, index) = Unchecked.defaultof<_>
    member _.Count = Unchecked.defaultof<_>
    member _.IndexOf(value) = Unchecked.defaultof<_>
    member _.IsFixedSize = true
    member _.IsReadOnly = true
    member _.IsSynchronized = false
    member _.Item
      with get index = Unchecked.defaultof<_>
      and set _ _ = raise(NotSupportedException listReadOnly)
    member _.Add(_) = raise(NotSupportedException listReadOnly)
    member _.Clear() = raise(NotSupportedException listReadOnly)
    member _.Insert(_, _) = raise(NotSupportedException listReadOnly)
    member _.Remove(_) = raise(NotSupportedException listReadOnly)
    member _.RemoveAt(_) = raise(NotSupportedException listReadOnly)
    member _.SyncRoot = false

  interface IList<'T> with
    member this.Contains item = Unchecked.defaultof<_>
    member _.CopyTo(array, arrayIndex) = Unchecked.defaultof<_>
    member _.Count = Unchecked.defaultof<_>
    member _.IndexOf(item) = Unchecked.defaultof<_>
    member _.IsReadOnly = true
    member _.Item
      with get index = Unchecked.defaultof<_>
      and set _ _ = raise(NotSupportedException listReadOnly)
    member _.Add(_) = raise(NotSupportedException listReadOnly)
    member _.Clear() = raise(NotSupportedException listReadOnly)
    member _.Insert(_, _) = raise(NotSupportedException listReadOnly)
    member _.Remove(_) = raise(NotSupportedException listReadOnly)
    member _.RemoveAt(_) = raise(NotSupportedException listReadOnly)

  interface IReadOnlyList<'T> with
    member _.Count = Unchecked.defaultof<_>
    member _.Item
      with get index = Unchecked.defaultof<_>
