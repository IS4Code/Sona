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

            // No hidden exceptions
            40, // This and other recursive references to the object(s) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference. This is because you are defining one or more recursive objects, rather than recursive functions. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'.

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
            // Simplify wording and remove F#-specific parts
            { 40, "LetRecCheckedAtRuntime", "This value depends on variables declared later in the code whose value may not yet be available. Consider using functions instead of variables." }, // This and other recursive references to the object(s) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference. This is because you are defining one or more recursive objects, rather than recursive functions. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'.

            // Ignore for anonymous types
            { 64, "NonRigidTypar1", m => null }, // This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near '%s' has been constrained to be type '%s'.

            // Different mechanism
            { 821, "tcBindingCannotBeUseAndRec", "A 'use' declaration is not permitted together with '#pragma recursive'." }, // A binding cannot be marked both 'use' and 'rec'
            
            // Different mechanism
            { 874, "tcOnlyRecordFieldsAndSimpleLetCanBeMutable", "A 'var' declaration is not permitted together with '#pragma recursive' or '#pragma forwardref'." }, // Mutable 'let' bindings can't be recursive or defined in recursive modules or namespaces

            // Compiler-generated unused variables are reported even when they start on _.
            { 1182, "chkUnusedValue", m => Syntax.GetIdentifierValue(m.Groups[1].Value).StartsWith("_", StringComparison.Ordinal) ? null : m.Value }, // The value '%s' is unused

            // Not a warning anymore
            { 3517, "optFailedToInlineSuggestedValue", "The function parameter is declared as 'inline' but the provided argument cannot be inlined. Consider passing an inlineable anonymous function." }, // The value '%s' was marked 'InlineIfLambda' but was not determined to have a lambda value.

            // Syntactically valid
            { 3521, "tcInvalidMemberDeclNameMissingOrHasParen", "A complex pattern is not permitted in this declaration." }, // Invalid member declaration. The name of the member is missing or has parentheses.
        };

        public static string? ProcessDiagnostic(int code, string message)
        {
            if(!messageReplacement.TryGetValue(code, out var list))
            {
                return message;
            }

            // Find actually replaced message
            foreach(var (pattern, replacer) in list)
            {
                switch(replacer)
                {
                    case string str:
                        // Simple replacement, always succeeds
                        return pattern.Replace(message, str);

                    case Func<Match, string?> func:
                        // Replacement function
                        bool replaced = false;
                        bool ignored = false;

                        message = pattern.Replace(message, m => {
                            var result = func(m);
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
                        break;
                }
            }
            return message;
        }

        sealed class MessageReplacementDictionary : Dictionary<int, List<(Regex regex, object replacer)>>
        {
            private void Add(int code, string key, object replacer)
            {
                if(!this.TryGetValue(code, out var list))
                {
                    this[code] = list = new();
                }
                list.Add((Resources.GetRegex(key, Culture), replacer));
            }

            public void Add(int code, string key, Func<Match, string?> replacer)
            {
                Add(code, key, (object)replacer);
            }

            public void Add(int code, string key, string replacer)
            {
                Add(code, key, (object)replacer);
            }
        }
    }
}
