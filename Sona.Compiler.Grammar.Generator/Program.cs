using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sona.Compiler.States;
using Microsoft.FSharp.Collections;

namespace Sona.Compiler.Grammar.Generator
{
    using static StatementFlags;

    static class Program
    {
        static readonly Dictionary<StatementFlags, string> blockNames = new()
        {
            { None, "ignored" },
            { Terminating, "terminating" },
            { OpenPath, "open" },
            { ReturnPath | InterruptPath, "returning" },
            { OpenPath | ReturnPath | InterruptPath, "conditional" },
            { InterruptPath, "interrupting" },
            { InterruptPath | OpenPath, "interruptible" },
        };

        static readonly Dictionary<string, StatementFlags> blockFlags = new()
        {
            { "ignored_block", None },
            { "terminating_block", Terminating },
            { "open_block", OpenPath },
            { "returning_block", ReturnPath | InterruptPath },
            { "conditional_block", ReturnPath | InterruptPath | OpenPath },
            { "interrupting_block", InterruptPath },
            { "interruptible_block", InterruptPath | OpenPath },
        };

        static void Main(string[] args)
        {
            OutputIf();
        }

        static void OutputIf()
        {
            var ifGrammar = LoadGrammar<If<ImmutableList<string>>>("grammar_if.json");

            SimplifyIf(ifGrammar.Conditional, "ifStatementConditional");
        }

        static void SimplifyIf(IReadOnlyCollection<If<ImmutableList<string>>> grammar, string statementName)
        {
            // Phase 1 - skip redundant groups
            var caseSet = new HashSet<If<FSharpList<string>>>();

            foreach(var parts in grammar)
            {
                var flags = ToFlags(parts.Then);
                string then = FromFlags(flags, parts.Then);
                var elseifs = new List<string>();
                foreach(var elseif in parts.ElseIf)
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

                var elseFlags = ToFlags(parts.Else);
                if(elseFlags != None)
                {
                    // Not ignored - merge
                    elseFlags |= flags;
                }

                // Keep trail flags unchanged
                var trailFlags = ToFlags(parts.Trail);

                var newIf = new If<FSharpList<string>>(
                    then,
                    SeqModule.ToList(elseifs),
                    FromFlags(elseFlags, parts.Else),
                    FromFlags(trailFlags, parts.Trail)
                );

                caseSet.Add(newIf);
            }

            // Phase 2 - form tree from all values
            var caseTreeBuilder = new CategoryTreeMutable();

            foreach(var parts in caseSet)
            {
                // One path ending at trail
                var path1 = TreePath("if", parts.Then, caseTreeBuilder);
                foreach(var elseif in parts.ElseIf)
                {
                    path1 = TreePath("elseif", elseif, path1);
                }
                path1 = TreePath("else", parts.Else, path1);
                _ = TreePath("trail", parts.Trail, path1);

                // Another path with trail second
                var path2 = TreePath("if", parts.Then, caseTreeBuilder);
                path2 = TreePath("trail", parts.Trail, path2);
                foreach(var elseif in parts.ElseIf)
                {
                    path2 = TreePath("elseif", elseif, path2);
                }
                _ = TreePath("else", parts.Else, path2);
            }

            var caseTree = caseTreeBuilder.ToImmutable();

            // Phase 3 - construct grammar
            using var output = new IndentedTextWriter(File.CreateText("out.txt"), "  ");
            output.Write(statementName);
            output.Write(':');
            output.WriteLine();
            output.Indent++;

            bool ifFirst = true;
            // Group by contents regardless of category
            foreach(var ifGroup in caseTree.GroupBy(p => p.Value))
            {
                if(ifFirst)
                {
                    ifFirst = false;
                }
                else
                {
                    // Alternative paths
                    output.WriteLine("|");
                }

                var ifTree = ifGroup.Key;

                output.Write("if ");
                var ifKeys = ifGroup.Select(p => p.Key.Category).ToList();
                output.WriteAlternativeBlocks(ifKeys);
                output.Write(' ');

                var ifFlags = ToFlags(ifKeys);
                output.Write("(elseif ");
                output.WriteAlternativeBlocks(FlagsCoverBlocks(ifFlags));
                output.Write(")* ");

                // Group by contents regardless of trail
                // Only used when the middle can be shared
                var trailGroups = ifTree.Where(p => p.Key.Tag == "trail").GroupBy(p => p.Value).ToList();

                if(trailGroups.Any(g => g.Count() > 1))
                {
                    // Trail can be used to group some of them
                    var excludedTrails = new HashSet<CategoryKey>();

                    if(trailGroups.Count > 1)
                    {
                        output.WriteLine('(');
                        output.Indent++;
                    }

                    bool trailFirst = true;
                    foreach(var trailGroup in trailGroups)
                    {
                        if(trailFirst)
                        {
                            trailFirst = false;
                        }
                        else
                        {
                            // Alternative paths
                            output.WriteLine("|");
                        }

                        // Alternatives for trail
                        var trailKeys = trailGroup.Select(p => p.Key.Category).ToList();
                        foreach(var category in trailKeys)
                        {
                            // Ignore paths ending on these trails
                            excludedTrails.Add(new CategoryKey("trail", category));
                        }

                        // Process else(ifs) inbetween
                        var trailTree = trailGroup.Key;
                        ProcessElse(trailTree, ifFlags, true);

                        // Finish trail
                        output.Write("'end' ");
                        output.WriteAlternativeTrails(trailKeys);
                        output.Write(' ');
                    }

                    if(trailGroups.Count > 1)
                    {
                        output.Indent--;
                        output.WriteLine();
                        output.Write(") ");
                    }

                    ifTree = CategoryTreeImmutable.Empty;
                    /*ifTree = ifTree.StripFrom(excludedTrails);

                    if(!ifTree.Categories.IsEmpty)
                    {
                        output.WriteLine("|");
                    }*/
                }
                else
                {
                    ProcessElse(ifTree, ifFlags, false);
                }

                void ProcessElse(CategoryTreeImmutable parent, StatementFlags parentFlags, bool noTrail)
                {
                    // Group by follow-up
                    var groups = parent.GroupBy(p => (p.Key.Tag, p.Value)).ToList();

                    if(groups.Count == 0)
                    {
                        throw new InvalidOperationException("Empty category entered.");
                    }

                    if(groups.Count > 1)
                    {
                        output.WriteLine('(');
                        output.Indent++;
                    }

                    try
                    {
                        bool groupFirst = true;
                        foreach(var group in groups)
                        {
                            if(groupFirst)
                            {
                                groupFirst = false;
                            }
                            else
                            {
                                // Alternative paths
                                output.WriteLine("|");
                            }

                            var (tag, tree) = group.Key;
                            var keys = group.Select(p => p.Key.Category).ToList();
                            var flags = parentFlags;

                            switch(tag)
                            {
                                case "elseif":
                                {
                                    // Deciding elseif
                                    output.Write("elseif ");
                                    output.WriteAlternativeBlocks(FlagsTransformBlocks(ToFlags(keys), ref flags));
                                    output.Write(' ');

                                    // Covering elseifs
                                    output.Write("(elseif ");
                                    output.WriteAlternativeBlocks(FlagsCoverBlocks(flags));
                                    output.Write(")* ");
                                    break;
                                }

                                case "else":
                                {
                                    bool isIgnored = keys.Remove("ignored");
                                    if(keys.Count == 0)
                                    {
                                        // else is missing fully
                                        break;
                                    }

                                    if(isIgnored)
                                    {
                                        output.Write('(');
                                    }
                                    output.Write("else ");
                                    output.WriteAlternativeBlocks(FlagsTransformBlocks(ToFlags(keys), ref flags));
                                    if(isIgnored)
                                    {
                                        output.Write(")?");
                                    }
                                    output.Write(' ');
                                    if(noTrail)
                                    {
                                        // Trail was already entered
                                        if(!tree.Categories.IsEmpty)
                                        {
                                            throw new InvalidOperationException("Trail not expected but there are remaining paths.");
                                        }
                                        continue;
                                    }
                                    break;
                                }

                                case "trail":
                                {
                                    // Finish trail
                                    output.Write("'end' ");
                                    output.WriteAlternativeTrails(keys);
                                    output.Write(' ');
                                    if(!tree.Categories.IsEmpty)
                                    {
                                        throw new InvalidOperationException("There are remaining paths after trail.");
                                    }
                                    continue;
                                }
                            }

                            // Process follow-up
                            ProcessElse(tree, flags, noTrail);
                        }
                    }
                    finally
                    {
                        if(groups.Count > 1)
                        {
                            output.Indent--;
                            output.WriteLine();
                            output.Write(") ");
                        }
                    }
                }
            }
            output.Write(";");
            output.Flush();
        }

        static StatementFlags ToFlags(string str)
        {
            if(!blockFlags.TryGetValue(str, out var flags))
            {
                str += "_block";
                flags = blockFlags[str];
            }
            return flags & ~Terminating;
        }

        static StatementFlags ToFlags(IReadOnlyCollection<string> list)
        {
            return list.Aggregate(None, (a, c) => a | ToFlags(c));
        }

        static string FromFlags(StatementFlags flags, string original)
        {
            if(ToFlags(original) == flags)
            {
                return blockNames[blockFlags[original]];
            }
            return blockNames[flags];
        }

        static string ToBlock(string category)
        {
            if(category == "ignored")
            {
                throw new ArgumentException("Ignored block is not allowed.", nameof(category));
            }
            return category + "Block";
        }

        static string ToTrail(string category)
        {
            return category + "Trail";
        }

        /// <summary>
        /// Returns block names that transform <paramref name="originalFlags"/> exactly into <paramref name="flags"/>.
        /// </summary>
        static IReadOnlyCollection<string> FlagsTransformBlocks(StatementFlags flags, ref StatementFlags originalFlags)
        {
            // The flags that must be all set (and no more)
            var targetFlags = flags & ~originalFlags;

            if(targetFlags == None && flags != originalFlags)
            {
                throw new ArgumentException("The flags are not transformed by addition.", nameof(flags));
            }

            var blocks = new HashSet<string>();
            foreach(var (flag, block) in blockNames)
            {
                if(flag == None)
                {
                    // Ignore
                    continue;
                }

                // Terminating does not change block flags; original flags are unimportant
                var flagBits = flag & ~(Terminating | originalFlags);

                if(flagBits == targetFlags)
                {
                    // Covered
                    blocks.Add(block);
                }
            }
            originalFlags |= flags;
            return blocks;
        }

        /// <summary>
        /// Returns block names that are covered by <paramref name="flags"/>.
        /// </summary>
        static IReadOnlyCollection<string> FlagsCoverBlocks(StatementFlags flags)
        {
            var blocks = new HashSet<string>();
            foreach(var (flag, block) in blockNames)
            {
                if(flag == None)
                {
                    // Ignore
                    continue;
                }

                // Terminating does not change block flags
                var flagBits = flag & ~Terminating;

                if((flags & flagBits) == flagBits)
                {
                    // Covered
                    blocks.Add(block);
                }
            }
            return blocks;
        }

        static void WriteAlternatives(TextWriter output, IReadOnlyCollection<string> blocks, Func<string, string> map)
        {
            switch(blocks.Count)
            {
                case 0:
                    throw new ArgumentException("Parameter must not be empty.", nameof(blocks));
                case 1:
                    output.Write(map(blocks.First()));
                    break;
                default:
                    output.Write('(');
                    bool first = true;
                    foreach(var block in blocks)
                    {
                        if(first)
                        {
                            first = false;
                        }
                        else
                        {
                            output.Write(" | ");
                        }
                        output.Write(map(block));
                    }
                    output.Write(')');
                    break;
            }
        }

        static void WriteAlternativeBlocks(this TextWriter output, IReadOnlyCollection<string> blocks)
        {
            WriteAlternatives(output, blocks, ToBlock);
        }

        static void WriteAlternativeTrails(this TextWriter output, IReadOnlyCollection<string> blocks)
        {
            WriteAlternatives(output, blocks, ToTrail);
        }

        static Grammar<T> LoadGrammar<T>(string path)
        {
            using var file = File.Open(path, FileMode.Open);
            return JsonSerializer.Deserialize<Grammar<T>>(file)!;
        }

        record struct CategoryKey(string Tag, string Category) : IComparable<CategoryKey>, IComparable
        {
            public int CompareTo(CategoryKey other)
            {
                switch(Tag.CompareTo(other.Tag))
                {
                    case 0:
                        return Category.CompareTo(other.Category);
                    case var num:
                        return num;
                }
            }

            int IComparable.CompareTo(object? obj)
            {
                if(obj is not CategoryKey key)
                {
                    throw new ArgumentException("Incompatible argument type.", nameof(obj));
                }
                return CompareTo(key);
            }
        }

        record CategoryTreeMutable : IEnumerable<KeyValuePair<CategoryKey, CategoryTreeMutable>>
        {
            public Dictionary<CategoryKey, CategoryTreeMutable> Categories { get; } = new();

            public IEnumerator<KeyValuePair<CategoryKey, CategoryTreeMutable>> GetEnumerator()
            {
                return Categories.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public CategoryTreeImmutable ToImmutable()
            {
                return new CategoryTreeImmutable(MapModule.OfSeq(Categories.Select(p => Tuple.Create(p.Key, p.Value.ToImmutable()))));
            }
        }

        static CategoryTreeMutable TreePath(string tag, string category, CategoryTreeMutable root)
        {
            var key = new CategoryKey(tag, category);
            if(!root.Categories.TryGetValue(key, out var tree))
            {
                root.Categories[key] = tree = new();
            }
            return tree;
        }

        record CategoryTreeImmutable(FSharpMap<CategoryKey, CategoryTreeImmutable> Categories) : IEnumerable<KeyValuePair<CategoryKey, CategoryTreeImmutable>>
        {
            public static readonly CategoryTreeImmutable Empty = new(MapModule.Empty<CategoryKey, CategoryTreeImmutable>());

            public IEnumerator<KeyValuePair<CategoryKey, CategoryTreeImmutable>> GetEnumerator()
            {
                return ((IReadOnlyDictionary<CategoryKey, CategoryTreeImmutable>)Categories).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /*public CategoryTreeImmutable StripFrom(IReadOnlySet<CategoryKey> keys)
            {
                return new CategoryTreeImmutable(MapModule.OfSeq(this.Select(p => {
                    if(keys.Contains(p.Key))
                    {
                        return (key: p.Key, map: Empty);
                    }
                    return (key: p.Key, map: p.Value.StripFrom(keys));
                }).Where(t => !t.map.Categories.IsEmpty).Select(t => Tuple.Create(t.key, t.map))));
            }*/
        }
    }

    sealed record Grammar<T>(
        [property: JsonPropertyName("terminating")] ImmutableList<T> Terminating,
        [property: JsonPropertyName("interrupting")] ImmutableList<T> Interrupting,
        [property: JsonPropertyName("interruptible")] ImmutableList<T> Interruptible,
        [property: JsonPropertyName("returning")] ImmutableList<T> Returning,
        [property: JsonPropertyName("conditional")] ImmutableList<T> Conditional
    );

    sealed record If<TCollection>(
        [property: JsonPropertyName("then")] string Then,
        [property: JsonPropertyName("elseif")] TCollection ElseIf,
        [property: JsonPropertyName("else")] string Else,
        [property: JsonPropertyName("trail")] string Trail
    ) where TCollection : IReadOnlyCollection<string>;
}
