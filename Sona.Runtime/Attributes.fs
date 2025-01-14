namespace System.Diagnostics.CodeAnalysis

open System

[<AttributeUsage(AttributeTargets.Method)>]
[<CompiledName("DoesNotReturnAttribute")>]
type private ``no return``() = inherit Attribute()
