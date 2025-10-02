using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Sona.Compiler.Tools;

namespace Sona.Compiler
{
    internal static class CompilerMessages
    {
        static readonly int[] ignoredWarnings =
        {
            // Wildcards are not generics
            464, // This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless.

            // Code style
            3220, // This method or property is not normally used from F# code, use an explicit tuple pattern for deconstruction instead.

            // Triggered by unused generic returns
            3559, // A type has been implicitly inferred as 'obj', which may be unintended. Consider adding explicit type annotations.
        };

        public static IReadOnlyCollection<int> IgnoredWarnings => ignoredWarnings;

        static readonly int[] enabledWarnings =
        {
            // Recursion checked at run time
            21,

            // Implicit copies of structs
            52,

            // Implicit equality/comparison
            1178,

            // Unused anything not starting with `_`
            1182, // The value '%s' is unused
            
            3387, // This expression has type '%s' and is only made compatible with type '%s' through an ambiguous implicit conversion. Consider using an explicit call to 'op_Implicit'.
            
            // Additional implicit upcast
            3388, // This expression implicitly converts type '%s' to type '%s'.
            
            // Implicit widening
            3389, // This expression uses a built-in implicit conversion to convert type '%s' to type '%s'.
            
            // Malformed XML doc comments
            3390, // This XML comment is invalid: '%s'

            // f() should not work for function(object)
            3397, // This expression uses 'unit' for an 'obj'-typed argument. This will lead to passing 'null' at runtime.
            
            // Ineffective inline lambda
            3517, // The value '%s' was marked 'InlineIfLambda' but was not determined to have a lambda value.
        };

        public static IReadOnlyCollection<int> EnabledWarnings => enabledWarnings;

        static readonly int[] errorWarnings =
        {
            // Explicit discard required
            20, // The result of this expression has type '%s' and is implicitly ignored.

            // No hidden exceptions
            25, // Incomplete pattern matches on this expression.

            // Pattern variables must be lowercase
            49, // Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.
            
            // Explicit discard required
            193, // This expression is a function value, i.e. is missing arguments. Its type is %s.

             // Nullness warning
            3261,
            
            // Ineffective inline lambda
            3517, // The value '%s' was marked 'InlineIfLambda' but was not determined to have a lambda value.
        };

        public static IReadOnlyCollection<int> ErrorWarnings => errorWarnings;

        public static readonly CultureInfo Culture = new CultureInfo("en");

        static readonly MessageReplacementDictionary messageReplacement = new()
        {
            // Ignore for anonymous types
            { 64, "NonRigidTypar1", m => null }, // This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near '%s' has been constrained to be type '%s'.

            // Compiler-generated unused variables are reported even when they start on _.
            { 1182, "chkUnusedValue", m => Syntax.GetIdentifierValue(m.Groups[1].Value).StartsWith("_") ? null : m.Value }, // The value '%s' is unused
        };

        public static string? ProcessDiagnostic(int code, string message)
        {
            if(!messageReplacement.TryGetValue(code, out var list))
            {
                return message;
            }

            // Find actually replaced message
            foreach(var (pattern, replacement) in list)
            {
                bool replaced = false;
                bool ignored = false;

                message = pattern.Replace(message, m => {
                    var result = replacement(m);
                    if(result == null)
                    {
                        // Null result - ignore message completely
                        ignored = true;
                        return "";
                    }
                    if(result != m.Value)
                    {
                        replaced = true;
                    }
                    return result;
                });

                if(ignored)
                {
                    return null;
                }
                if(replaced)
                {
                    return message;
                }
            }
            return message;
        }

        sealed class MessageReplacementDictionary : Dictionary<int, List<(Regex regex, Func<Match, string?>)>>
        {
            public void Add(int code, string key, Func<Match, string?> replacement)
            {
                if(!this.TryGetValue(code, out var list))
                {
                    this[code] = list = new();
                }
                list.Add((Resources.GetRegex(key, Culture), replacement));
            }
        }
    }
}
