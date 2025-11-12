namespace System.Diagnostics.CodeAnalysis

open System

[<AttributeUsage(AttributeTargets.Method)>]
[<CompiledName("DoesNotReturnAttribute")>]
type private ``no return``() = inherit Attribute()

namespace System.Runtime.CompilerServices

open System

[<AttributeUsage(AttributeTargets.All, Inherited = false)>]
type internal IsReadOnlyAttribute() = inherit Attribute()
