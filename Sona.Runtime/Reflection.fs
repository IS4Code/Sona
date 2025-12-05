namespace Sona.Runtime.Reflection

open System
open System.Collections
open System.Collections.Generic
open System.Linq.Expressions
open System.Reflection

#nowarn "1204" // This function is for use by compiled F# code and should not be used directly

module private ReflectionHelpers =
  open FSharp.Core.Operators.OperatorIntrinsics
  
  [<Literal>]
  let bindingFlags : BindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic

  let intrinsicRanges =
    let dict = Dictionary<Type, _>()
    let long = typeof<int64>
    let ulong = typeof<uint64>
    let ienumerable = typedefof<_ seq>

    let exampleType = (RangeInt32 0 11 10000).GetType()

    if exampleType.IsNested && exampleType.Name.StartsWith("Range", StringComparison.Ordinal) then
      let operatorsType = exampleType.DeclaringType
      for t in operatorsType.GetNestedTypes(bindingFlags) do
        if t.Name.StartsWith("Range", StringComparison.Ordinal) && not t.IsGenericTypeDefinition then
          match t.GetField("stop", bindingFlags) with
          | NonNull(stop) ->
            let element = stop.FieldType

            dict[t] <-
              struct((
                let param = Expression.Parameter(ienumerable.MakeGenericType(element))
                let cast = Expression.Convert(param, t)
                let stop = Expression.Field(cast, stop)
                Expression.Lambda(stop, param).Compile()),
                
                let param1 = Expression.Parameter(element)
                let param2 = Expression.Parameter(element)
                try
                  let minus = Expression.Subtract(param1, param2)
                  Expression.Lambda(minus, param1, param2).Compile()
                with | _ ->
                  try
                    let minus = Expression.Subtract(Expression.Convert(param1, ulong), Expression.Convert(param2, ulong))
                    Expression.Lambda(Expression.Convert(minus, element), param1, param2).Compile()
                  with | _ ->
                    let minus = Expression.Subtract(Expression.Convert(param1, long), Expression.Convert(param2, long))
                    Expression.Lambda(Expression.Convert(minus, element), param1, param2).Compile()
              )
          | _ -> ()
    dict

[<Sealed; AbstractClass>]
type Type<'T> private() =
  static let t = typeof<'T>

  static member val IsUnit = typeof<unit>.Equals typeof<'T>

  static member val HasNullDefaultValue =
    not(isNull(Nullable.GetUnderlyingType(t))) ||
    (not(t.IsValueType) && (
      match t.GetCustomAttribute(typeof<AllowNullLiteralAttribute>) with
      | NonNull (:? AllowNullLiteralAttribute as attr) -> attr.Value
      | _ -> false
      ||
      match t.GetCustomAttribute(typeof<CompilationRepresentationAttribute>) with
      | NonNull (:? CompilationRepresentationAttribute as attr) -> (attr.Flags &&& CompilationRepresentationFlags.UseNullAsTrueValue) = CompilationRepresentationFlags.UseNullAsTrueValue
      | _ -> false
    ))

  static member val HasEmptyDefaultValue =
    Type<'T>.IsUnit || (
      t.IsGenericType && let tdef = t.GetGenericTypeDefinition() in (
        typedefof<_ option>.Equals tdef
        || typedefof<_ voption>.Equals tdef
      )
    )

  static member IsEmptyDefaultValue(instance : 'T) =
    Type<'T>.IsUnit || (
      Type<'T>.HasEmptyDefaultValue
      && System.Runtime.CompilerServices.RuntimeHelpers.Equals(Unchecked.defaultof<'T>, instance)
    )

  static member TryGetSequenceBounds(sequence : 'T seq) : struct('T * 'T * 'T) voption =
    match ReflectionHelpers.intrinsicRanges.TryGetValue(sequence.GetType()) with
    | (true, ((:? Func<'T seq, 'T> as stop), (:? Func<'T, 'T, 'T> as minus))) ->
      let stop = stop.Invoke(sequence)

      use enumerator = sequence.GetEnumerator()
      if enumerator.MoveNext() then
        let start = enumerator.Current
        ValueSome(
          if enumerator.MoveNext() then
            let step = minus.Invoke(enumerator.Current, start)
            struct(start, step, stop)
          else
            struct(start, Unchecked.defaultof<'T>, stop)
        )
      else
        ValueNone
    | _ ->
      ValueNone
