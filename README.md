# Sona Language

Sona is a general-purpose programming/scripting language targetting [.NET](https://dotnet.microsoft.com/en-us/) and powered by [F#](https://learn.microsoft.com/en-us/dotnet/fsharp/).
It draws inspiration from several .NET languages, as well as [Lua](https://www.lua.org/), to offer simple yet powerful syntax with focus on clarity and utility.
Despite its primary paradigm being imperative, it utilizes many features found in functional programming languages, such as first-class functions, tuples and records, as well as powerful type inference.

## Documentation
**For a full overview of its features, please see the [wiki](//github.com/IS4Code/Sona/wiki).** The main distinguishing features are:
* Expressive syntax primarily inspired by Lua, including:
  * Focus on keywords rather than operators to indicate [special syntax elements and constructions](//github.com/IS4Code/Sona/wiki/Symbols-and-operators).
  * Semicolons are optional; whitespace is insignificant.
  * Simplicity and consistency:
    * Overloading is favoured over explicit syntax, such as `..` being used to concatenate strings, lists, or sequences; `&`, `|`, `>>` and `<<` are used both for functional and bitwise manipulation, and so on.
    * Parity between expressions and types: the type of `function(x as string)` is `function(string)`; the type of `(1, "x")` (a tuple) is `(int, string)`, etc.
    * Conversion operators do not require familiarization with spatial metaphors in type theories: `narrow x`/`widen x` are used to convert to a more/less derived type, respectively.
* [Imperative constructs](//github.com/IS4Code/Sona/wiki/Control-statements) (`return`, `break`, `continue`) are built on functional principles, maintaining convenience without sacrificing soundness.
* [Built-in option type](//github.com/IS4Code/Sona/wiki/Built%E2%80%90in-types#options) (`?`), usable in constructions (`some x`, `none`), conversions (`int? x`, `narrow? x`), the coalesce operator (`x ?? y`), or conditional member access (`?.`).
* [Built-in sequence type](//github.com/IS4Code/Sona/wiki/Built%E2%80%90in-types#sequences) (`..`), easily constructible using `{…}` (e.g. `{x, ..anotherSequence, y}`, supporting the spread operator).
* Type-tested formatting in [interpolated strings](//github.com/IS4Code/Sona/wiki/Interpolated-strings): `{x:0.0#}` implies a numeric type; `{x:ddMMyy}` implies a date.

## Support
**To report and track bugs, please use the associated [issues](//github.com/IS4Code/Sona/issues) page.**
For discussing the language or its features, please use [Discord](https://discord.gg/Rn6UGaU7PA).

## Development
The language and associated tooling is currently in the initial development phase.
During this phase, features are expected to be added, modified, or removed as the design decisions and implementation considerations shape the language, until version 1.0.
Since the language is not considered stable at this point, you are welcome to try all its features, but please be cautionary when incorporating them into existing solutions.

**The project is available as a stand-alone [release](//github.com/IS4Code/Sona/releases), containing both the compiler and a playground environment.**
The compiler is also available as a [.NET tool](https://www.nuget.org/packages/Sona.Compiler.Tool), and may be embedded as a service in other solutions as a [NuGet package](https://www.nuget.org/packages/Sona.Compiler).

### Environment
During the initial development, the project is built and tested using .NET 8+, and multi-targeted for .NET Standard 2.0.
The compiler and runtime rely on the latest version of F# to properly function ‒ using the intermediate output in an older version of F# may lead to errors.

### Versions
The project uses semantic versioning:
* **[0.1](//github.com/IS4Code/Sona/releases/tag/v0.1)** ‒ statements, value expressions, attributes.
* **[0.2](//github.com/IS4Code/Sona/releases/tag/v0.2)** ‒ type expressions, conversions and constructions.
* **[0.3](//github.com/IS4Code/Sona/releases/tag/v0.3)** ‒ patterns, exception handling, packages.
* **0.4** ‒ monads/computation blocks. 
* **0.<i>*</i>** ‒ type declarations, generic parameters, namespaces.
* **1.0** ‒ full release of the initial feature set.
