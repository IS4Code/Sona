namespace Sona.Runtime.Reflection

open System
open System.Reflection

[<Sealed; AbstractClass>]
type Type<'T> private() =
  static member val IsUnit = typeof<unit>.Equals typeof<'T>

  static member val HasNullDefaultValue =
    let t = typeof<'T>
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
      let t = typeof<'T>
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