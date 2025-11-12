namespace Sona.Runtime.Reflection

open System
open System.Reflection

[<Sealed; AbstractClass>]
type Type<'T> private() =
  static let T = typeof<'T>
  static let NullableT = Nullable.GetUnderlyingType(T)
  
  static member IsValueType = T.IsValueType

  static member val IsUnit = typeof<unit>.Equals T
  
  static member val HasNullDefaultValue =
    not(isNull(Nullable.GetUnderlyingType(T))) ||
    (not(T.IsValueType) && (
      match T.GetCustomAttribute(typeof<AllowNullLiteralAttribute>) with
      | NonNull (:? AllowNullLiteralAttribute as attr) -> attr.Value
      | _ -> false
      ||
      match T.GetCustomAttribute(typeof<CompilationRepresentationAttribute>) with
      | NonNull (:? CompilationRepresentationAttribute as attr) -> (attr.Flags &&& CompilationRepresentationFlags.UseNullAsTrueValue) = CompilationRepresentationFlags.UseNullAsTrueValue
      | _ -> false
    ))
  
  static member val HasEmptyDefaultValue =
    Type<'T>.IsUnit || (
      T.IsGenericType && let tdef = T.GetGenericTypeDefinition() in (
        typedefof<_ option>.Equals tdef
        || typedefof<_ voption>.Equals tdef
      )
    )

  static member IsEmptyDefaultValue(instance : 'T) =
    Type<'T>.IsUnit || (
      Type<'T>.HasEmptyDefaultValue
      && System.Runtime.CompilerServices.RuntimeHelpers.Equals(Unchecked.defaultof<'T>, instance)
    )

  static member GetBoxedType(instance : 'T) : Type =
    if not(isNull(NullableT)) then
      NullableT
    elif Type<'T>.IsValueType || isNull(box instance) then
      T
    else
      instance.GetType()