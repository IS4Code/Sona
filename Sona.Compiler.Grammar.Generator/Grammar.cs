using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Microsoft.FSharp.Collections;
using Sona.Compiler.States;

namespace Sona.Compiler.Grammar.Generator
{
    using static StatementFlags;
    using static Extensions;

    interface IGrammar
    {
        IEnumerable<IGrammarElement> Terminating { get; }
        IEnumerable<IGrammarElement> Interrupting { get; }
        IEnumerable<IGrammarElement> Interruptible { get; }
        IEnumerable<IGrammarElement> Returning { get; }
        IEnumerable<IGrammarElement> Conditional { get; }
    }

    sealed record Grammar<T>(
        [property: JsonPropertyName("terminating")] ImmutableList<T> Terminating,
        [property: JsonPropertyName("interrupting")] ImmutableList<T> Interrupting,
        [property: JsonPropertyName("interruptible")] ImmutableList<T> Interruptible,
        [property: JsonPropertyName("returning")] ImmutableList<T> Returning,
        [property: JsonPropertyName("conditional")] ImmutableList<T> Conditional
    ) : IGrammar where T : class, IGrammarElement
    {
        IEnumerable<IGrammarElement> IGrammar.Terminating => Terminating;
        IEnumerable<IGrammarElement> IGrammar.Interrupting => Interrupting;
        IEnumerable<IGrammarElement> IGrammar.Interruptible => Interruptible;
        IEnumerable<IGrammarElement> IGrammar.Returning => Returning;
        IEnumerable<IGrammarElement> IGrammar.Conditional => Conditional;
    }

    interface IGrammarElement
    {
        string RuleName { get; }
        IEnumerable<IEnumerable<KeyValuePair<string, string>>> TreePaths();
        IGrammarElement CollapseParts();
    }

    sealed record If<TCollection>(
        [property: JsonPropertyName("then")] string Then,
        [property: JsonPropertyName("elseif")] TCollection ElseIf,
        [property: JsonPropertyName("else")] string Else,
        [property: JsonPropertyName("trail")] string Trail
    ) : IGrammarElement where TCollection : IReadOnlyCollection<string>
    {
        public string RuleName => "ifStatement";

        public IGrammarElement CollapseParts()
        {
            var flags = ToFlags(Then);
            string then = FromFlags(flags, Then);
            var elseifs = new List<string>();
            foreach(var elseif in ElseIf)
            {
                var newFlags = ToFlags(elseif);
                if((newFlags & ~flags) == 0)
                {
                    // Flags are already covered
                    continue;
                }
                // Merge new flags
                flags |= newFlags;
                elseifs.Add(FromFlags(flags, elseif));
            }

            var elseFlags = ToFlags(Else);
            if(elseFlags != None)
            {
                // Not ignored - merge
                elseFlags |= flags;
            }

            // Keep trail flags unchanged
            var trailFlags = ToFlags(Trail);

            return new If<FSharpList<string>>(
                then,
                SeqModule.ToList(elseifs),
                FromFlags(elseFlags, Else),
                FromFlags(trailFlags, Trail)
            );
        }

        public IEnumerable<IEnumerable<KeyValuePair<string, string>>> TreePaths()
        {
            return new[] { Normal(), TrailSecond() };

            IEnumerable<KeyValuePair<string, string>> Normal()
            {
                yield return new("if", Then);
                foreach(var elseif in ElseIf)
                {
                    yield return new("elseif", elseif);
                }
                yield return new("else", Else);
                yield return new("trail", Trail);
            }

            IEnumerable<KeyValuePair<string, string>> TrailSecond()
            {
                yield return new("if", Then);
                yield return new("trail", Trail);
                foreach(var elseif in ElseIf)
                {
                    yield return new("elseif", elseif);
                }
                yield return new("else", Else);
            }
        }
    }

    sealed record Switch<TCollection>(
        [property: JsonPropertyName("case")] TCollection Case,
        [property: JsonPropertyName("else")] string Else,
        [property: JsonPropertyName("trail")] string Trail
    ) : IGrammarElement where TCollection : IReadOnlyCollection<string>
    {
        public string RuleName => "switchStatement";

        public IGrammarElement CollapseParts()
        {
            StatementFlags? flags = null;
            var cases = new List<string>();
            foreach(var @case in Case)
            {
                var newFlags = ToFlags(@case);
                if(flags == null)
                {
                    flags = newFlags;
                }
                else
                {
                    if((newFlags & ~flags) == 0)
                    {
                        // Flags are already covered
                        continue;
                    }
                    // Merge new flags
                    flags |= newFlags;
                }
                cases.Add(FromFlags(flags.GetValueOrDefault(), @case));
            }

            var elseFlags = ToFlags(Else);
            if(flags == null)
            {
                flags = elseFlags;
            }
            else
            {
                if(elseFlags != None)
                {
                    // Not ignored - merge
                    elseFlags |= flags.GetValueOrDefault();
                }
            }

            // Keep trail flags unchanged
            var trailFlags = ToFlags(Trail);

            return new Switch<FSharpList<string>>(
                SeqModule.ToList(cases),
                FromFlags(elseFlags, Else),
                FromFlags(trailFlags, Trail)
            );
        }

        public IEnumerable<IEnumerable<KeyValuePair<string, string>>> TreePaths()
        {
            return new[] { Normal(), TrailSecond() };

            IEnumerable<KeyValuePair<string, string>> Normal()
            {
                yield return new("switch", "unused");
                foreach(var @case in Case)
                {
                    yield return new("case", @case);
                }
                yield return new("else", Else);
                yield return new("trail", Trail);
            }

            IEnumerable<KeyValuePair<string, string>> TrailSecond()
            {
                yield return new("switch", "unused");
                yield return new("trail", Trail);
                foreach(var @case in Case)
                {
                    yield return new("case", @case);
                }
                yield return new("else", Else);
            }
        }
    }

    abstract record TryBase<TCollection>(
        [property: JsonPropertyName("try")] string Try,
        [property: JsonPropertyName("case")] TCollection Case,
        [property: JsonPropertyName("finally")] string Finally,
        [property: JsonPropertyName("trail")] string Trail
    ) : IGrammarElement where TCollection : IReadOnlyCollection<string>
    {
        public abstract string RuleName { get; }

        //public string RuleName => $"try{(Case.Count > 0 ? "Catch" : "")}{(Finally != "ignored_block" ? "Finally" : "")}Statement";

        public IGrammarElement CollapseParts()
        {
            var flags = ToFlags(Try);
            var @try = FromFlags(flags, Try);
            var cases = new List<string>();
            bool first = true;
            foreach(var @case in Case)
            {
                var newFlags = ToFlags(@case);
                if((newFlags & ~flags) == 0 && !first)
                {
                    // Flags are already covered
                    continue;
                }
                // Merge new flags
                flags |= newFlags;
                cases.Add(FromFlags(flags, @case));
                first = false;
            }

            // Keep finally flags unchanged because they are in series
            var finallyFlags = ToFlags(Finally);

            // Keep trail flags unchanged
            var trailFlags = ToFlags(Trail);

            return CreateNew(
                @try,
                SeqModule.ToList(cases),
                FromFlags(finallyFlags, Finally),
                FromFlags(trailFlags, Trail)
            );
        }

        protected abstract TryBase<T> CreateNew<T>(string @try, T @case, string @finally, string trail) where T : IReadOnlyCollection<string>;

        public IEnumerable<IEnumerable<KeyValuePair<string, string>>> TreePaths()
        {
            return new[] { Normal(), TrailSecond() };

            IEnumerable<KeyValuePair<string, string>> Normal()
            {
                yield return new("try", Try);
                foreach(var @case in Case)
                {
                    yield return new("case", @case);
                }
                yield return new("finally", Finally);
                yield return new("trail", Trail);
            }

            IEnumerable<KeyValuePair<string, string>> TrailSecond()
            {
                yield return new("try", Try);
                yield return new("trail", Trail);
                foreach(var @case in Case)
                {
                    yield return new("case", @case);
                }
                yield return new("finally", Finally);
            }
        }
    }

    sealed record TryCatch<TCollection> : TryBase<TCollection> where TCollection : IReadOnlyCollection<string>
    {
        public override string RuleName => "tryCatchStatement";

        public TryCatch(string @try, TCollection @case, string @finally, string trail) : base(@try, @case, @finally, trail)
        {

        }

        protected override TryBase<T> CreateNew<T>(string @try, T @case, string @finally, string trail)
        {
            return new TryCatch<T>(@try, @case, @finally, trail);
        }
    }

    sealed record TryFinally<TCollection> : TryBase<TCollection> where TCollection : IReadOnlyCollection<string>
    {
        public override string RuleName => "tryFinallyStatement";

        public TryFinally(string @try, TCollection @case, string @finally, string trail) : base(@try, @case, @finally, trail)
        {

        }

        protected override TryBase<T> CreateNew<T>(string @try, T @case, string @finally, string trail)
        {
            return new TryFinally<T>(@try, @case, @finally, trail);
        }
    }

    sealed record TryCatchFinally<TCollection> : TryBase<TCollection> where TCollection : IReadOnlyCollection<string>
    {
        public override string RuleName => "tryCatchFinallyStatement";

        public TryCatchFinally(string @try, TCollection @case, string @finally, string trail) : base(@try, @case, @finally, trail)
        {

        }

        protected override TryBase<T> CreateNew<T>(string @try, T @case, string @finally, string trail)
        {
            return new TryCatchFinally<T>(@try, @case, @finally, trail);
        }
    }
}
