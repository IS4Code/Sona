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

            var newIf = new If<FSharpList<string>>(
                then,
                SeqModule.ToList(elseifs),
                FromFlags(elseFlags, Else),
                FromFlags(trailFlags, Trail)
            );

            return newIf;
        }

        public IEnumerable<IEnumerable<KeyValuePair<string, string>>> TreePaths()
        {
            if(ElseIf.Count == 0)
            {
                return new[] { Normal() };
            }
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
}
