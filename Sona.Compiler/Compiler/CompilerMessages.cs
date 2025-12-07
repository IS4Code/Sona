using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Sona.Compiler.Tools;

namespace Sona.Compiler
{
    using static CompilerMessages.DiagnosticProcessing;

    internal static class CompilerMessages
    {
        static readonly HashSet<int> ignoredWarnings = new();
        static readonly HashSet<int> enabledWarnings = new();
        static readonly HashSet<int> errorWarnings = new();
        
        public static IReadOnlyCollection<int> IgnoredWarnings => ignoredWarnings;
        public static IReadOnlyCollection<int> EnabledWarnings => enabledWarnings;
        public static IReadOnlyCollection<int> ErrorWarnings => errorWarnings;

        public static readonly CultureInfo Culture = new("en");

        const string typeMismatch = "Type mismatch: The type returned by this branch is `$2` but was expected to be `$1`.";
        const string tupleTypeMismatch = "Type mismatch: The type returned by this branch is `$4` (a tuple of $3 elements) but was expected to be `$2` (a tuple of $1 elements).";

        static bool IsIgnoredName(string name)
        {
            return name.StartsWith("_", StringComparison.Ordinal) || name.StartsWith("``_", StringComparison.Ordinal);
        }

        static readonly DiagnosticDictionary messageReplacement = new(ignoredWarnings, enabledWarnings, errorWarnings)
        {
            // All control statements may result in these errors
            { 1, None, "ifExpression", typeMismatch }, // The 'if' expression needs to have type '%s' to satisfy context type requirements.
            { 1, None, "ifExpressionTuple", tupleTypeMismatch }, // The 'if' expression needs to return a tuple of length %d of type\n    %s    \nto satisfy context type requirements.
            { 1, None, "elseBranchHasWrongType", typeMismatch }, // All branches of an 'if' expression must return values implicitly convertible to the type of the first branch
            { 1, None, "elseBranchHasWrongTypeTuple", tupleTypeMismatch }, // All branches of an 'if' expression must return values implicitly convertible to the type of the first branch
            { 1, None, "followingPatternMatchClauseHasWrongType", typeMismatch }, // All branches of a pattern match expression must return values implicitly convertible to the type of the first branch
            { 1, None, "followingPatternMatchClauseHasWrongTypeTuple", tupleTypeMismatch }, // All branches of a pattern match expression must return values implicitly convertible to the type of the first branch

            // Explicit discard required
            { 20, Error, "UnitTypeExpected", "This expression results in a value of type `$1`. Observe it or discard it explicitly via `!`." }, // The result of this expression has type '%s' and is implicitly ignored.

            // Recursion checked at run time
            { 21, Enable },

            // No hidden exceptions
            { 25, Error }, // Incomplete pattern matches on this expression.

            // No hidden exceptions; simplify wording and remove F#-specific parts
            { 40, Error, "LetRecCheckedAtRuntime", "This value depends on variables declared later in the code whose value may not yet be available. Consider using functions instead of variables." }, // This and other recursive references to the object(s) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference. This is because you are defining one or more recursive objects, rather than recursive functions. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'.

            // Pattern variables must be lowercase
            { 49, Error, "UpperCaseIdentifierInPattern", "Uppercase identifiers in a pattern must identify a pre-existing value or pattern case." }, // Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name.
            
            // Implicit copies of structs
            { 52, Enable },

            // Ignore for anonymous types
            { 64, None, "NonRigidTypar1", m => null }, // This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near '%s' has been constrained to be type '%s'.

            // Ignore for some names
            { 64, None, "NonRigidTypar2", m => { // This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable '%s has been constrained to be measure '%s'.
                var name = m.Groups[1].Value;
                if(IsIgnoredName(name))
                {
                    // Ignore
                    return null;
                }
                return $"This construct makes the unit-of-measure variable `{name}` be deduced to be always `{m.Groups[2].Value}`.";
            } },

            // Ignore for some names
            { 64, None, "NonRigidTypar3", m => { // This construct causes code to be less generic than indicated by the type annotations. The type variable '%s has been constrained to be type '%s'.
                var name = m.Groups[1].Value;
                if(IsIgnoredName(name))
                {
                    // Ignore
                    return null;
                }
                return $"This construct makes the type variable `{name}` be deduced to be always `{m.Groups[2].Value}`.";
            } },

            // Explicit discard required
            { 193, Error, "FunctionValueUnexpected", "This expression results in a function of type `$1`. Call it or discard it explicitly via `!`." }, // This expression is a function value, i.e. is missing arguments. Its type is %s.

            // Wildcards are not generics
            { 464, Ignore }, // This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless.

            // Different syntax
            { 795, None, "tcUseForInSequenceExpression", "`follow` cannot be used in a collection construction." }, // The use of 'let! x = coll' in sequence expressions is not permitted.
            
            // Different syntax
            { 796, None, "tcTryIllegalInSequenceExpression", "`try`/`catch` cannot be used in a collection construction." }, // 'try'/'with' cannot be used within sequence expressions
            
            // Different syntax
            { 797, None, "tcUseYieldBangForMultipleResults", "In a collection construction, `yield..` is used to generate multiple results." }, // In sequence expressions, multiple results are generated using 'yield!'

            // Different mechanism
            { 821, None, "tcBindingCannotBeUseAndRec", "A `use` declaration is not permitted together with `#pragma recursive`." }, // A binding cannot be marked both 'use' and 'rec'
            
            // Different mechanism
            { 874, None, "tcOnlyRecordFieldsAndSimpleLetCanBeMutable", "A `var` declaration is not permitted together with `#pragma recursive` or `#pragma forwardref`." }, // Mutable 'let' bindings can't be recursive or defined in recursive modules or namespaces

            // Implicit equality/comparison
            { 1178, Enable },

            // Compiler-generated unused variables are reported even when they start on _.
            { 1182, Enable, "chkUnusedValue", m => IsIgnoredName(m.Groups[1].Value) ? null : m.Value }, // The value '%s' is unused

            // Different syntax
            { 1228, None, "tcInvalidUseBangBinding", "A `use` variable initialized with `follow` cannot be declared using a complex pattern." }, // 'use!' bindings must be of the form 'use! <var> = <expr>'
            
            // Suggest solution
            { 3085, None, "tcCustomOperationMayNotBeUsedInConjunctionWithNonSimpleLetBindings", "Using a custom operation together with mutable or recursive variables is not supported. Use a record or another reference type to hold their values." }, // A custom operation may not be used in conjunction with a non-value or recursive 'let' binding in another part of this computation expression

            // Accurate cause, suggest solution
            { 3086, None, "tcCustomOperationMayNotBeUsedHere", "Using a custom operation together with constructs that affect control flow is not supported. Use `follow with` to execute it in a sub-computation." }, // A custom operation may not be used in conjunction with 'use', 'try/with', 'try/finally', 'if/then/else' or 'match' operators within this computation expression

            // Code style
            { 3220, Ignore }, // This method or property is not normally used from F# code, use an explicit tuple pattern for deconstruction instead.

            // Nullness warning; strip prefix
            { 3261, Error, "ConstraintSolverNullnessWarning", "$1." }, // Nullness warning: %s
            
             // Nullness warning
            { 3264, Enable | Error, "tcDowncastFromNullableToWithoutNull", "Casting from `$1` to `$2` can introduce unexpected null values. Cast to `$3 or null` instead or handle the null beforehand." }, // Nullness warning: Downcasting from '%s' into '%s' can introduce unexpected null values.

            // Different syntax
            { 3343, None, "tcRequireMergeSourcesOrBindN", "A variable declaration initializing multiple variables with `follow` may only be used if the computation builder defines either a `$1` method or appropriate `MergeSources` and `Bind` methods." }, // The 'let! ... and! ...' construct may only be used if the computation expression builder defines either a '%s' method or appropriate 'MergeSources' and 'Bind' methods

            // Different syntax
            { 3345, None, "tcInvalidUseBangBindingNoAndBangs", "A `use` declaration initializing multiple variables with `follow` is not supported." }, // use! may not be combined with and!

            // Implicit operator
            { 3387, Enable, "tcAmbiguousImplicitConversion", "This expression uses an implicit conversion from `$1` to `$2`. Use `implicit` to indicate an intentional conversion." }, // This expression has type '%s' and is only made compatible with type '%s' through an ambiguous implicit conversion. Consider using an explicit call to 'op_Implicit'.
            
            // Additional implicit upcast
            { 3388, Enable, "tcSubsumptionImplicitConversionUsed", "This expression uses a widening conversion from `$1` to `$2`. Use `widen` to indicate an intentional conversion." }, // This expression implicitly converts type '%s' to type '%s'.
            
            // Implicit numeric widening
            { 3389, Enable, "tcBuiltInImplicitConversionUsed", "This expression uses a built-in implicit conversion from `$1` to `$2`. Use an explicit operator to indicate an intentional conversion." }, // This expression uses a built-in implicit conversion to convert type '%s' to type '%s'.
            
            // Malformed XML doc comments
            { 3390, Enable }, // This XML comment is invalid: '%s'

            // f() should not work for function(object)
            { 3397, Enable | Error, "tcUnitToObjSubsumption", "An argumentless function call results in a single `unit` argument, however it is being treated as `null` here because the sole parameter has type `object`. Use `null` explicitly when calling this function." }, // This expression uses 'unit' for an 'obj'-typed argument. This will lead to passing 'null' at runtime.
            
            // Ineffective inline lambda; different syntax
            { 3517, Enable, "optFailedToInlineSuggestedValue", m => { // The value '%s' was marked 'InlineIfLambda' but was not determined to have a lambda value.
                var name = m.Groups[1].Value;
                if(IsIgnoredName(name))
                {
                    // Ignore
                    return null;
                }
                return $"The function parameter `{name}` is declared as `inline` but the provided argument cannot be inlined. Consider passing an inlineable anonymous function.";
            } },
            
            // Different syntax
            { 3519, None, "tcInlineIfLambdaUsedOnNonInlineFunctionOrMethod", "An `inline` parameter is allowed only in `inline` functions or methods, and must have a function or delegate type." }, // The 'InlineIfLambda' attribute may only be used on parameters of inlined functions of methods whose type is a function or F# delegate type.

            // Syntactically valid
            { 3521, None, "tcInvalidMemberDeclNameMissingOrHasParen", "A complex pattern is not permitted in this declaration." }, // Invalid member declaration. The name of the member is missing or has parentheses.

            // Triggered by unused generic returns
            { 3559, Ignore }, // A type has been implicitly inferred as 'obj', which may be unintended. Consider adding explicit type annotations.
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
                        // Simple replacement
                        var newMessage = pattern.Replace(message, str);
                        if(newMessage != message)
                        {
                            // Success
                            return newMessage;
                        }
                        break;

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

        sealed class DiagnosticDictionary : Dictionary<int, List<(Regex regex, object replacer)>>
        {
            readonly ICollection<int> ignored, enabled, errors;

            public DiagnosticDictionary(ICollection<int> ignored, ICollection<int> enabled, ICollection<int> errors)
            {
                this.ignored = ignored;
                this.enabled = enabled;
                this.errors = errors;
            }

            public void Add(int code, DiagnosticProcessing processing)
            {
                if((processing & Ignore) != 0)
                {
                    ignored.Add(code);
                }
                if((processing & Enable) != 0)
                {
                    enabled.Add(code);
                }
                if((processing & Error) != 0)
                {
                    errors.Add(code);
                }
            }

            private void Add(int code, DiagnosticProcessing processing, string key, object replacer)
            {
                Add(code, processing);
                if(!this.TryGetValue(code, out var list))
                {
                    this[code] = list = new();
                }
                list.Add((Resources.GetRegex(key, Culture), replacer));
            }

            public void Add(int code, DiagnosticProcessing processing, string key, Func<Match, string?> replacer)
            {
                Add(code, processing, key, (object)replacer);
            }

            public void Add(int code, DiagnosticProcessing processing, string key, string replacer)
            {
                Add(code, processing, key, (object)replacer);
            }
        }

        [Flags]
        internal enum DiagnosticProcessing
        {
            None = 0,
            Ignore = 1,
            Enable = 2,
            Error = 4
        }
    }
}
