using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using Sona.Compiler.States;
using Microsoft.FSharp.Collections;

namespace Sona.Compiler.Grammar.Generator
{
    using static StatementFlags;
    using static Extensions;
    using CategoryTreeMutable = TreeMutable<Extensions.CategoryKey>;
    using CategoryTreeImmutable = TreeImmutable<Extensions.CategoryKey>;

    static class Program
    {
        static void Main(string[] args)
        {
            using var output = new IndentedTextWriter(File.CreateText("out.txt"), "  ");

            WriteGrammar(LoadGrammar<If<ImmutableList<string>>>("grammar_if.json"), output);
            output.WriteLine();

            WriteGrammar(LoadGrammar<If<ImmutableList<string>>>("grammar_if_no_trail.json"), output);
            output.WriteLine();

            WriteGrammar(LoadGrammar<If<ImmutableList<string>>>("grammar_if_trail_from_else.json"), output, "TrailFromElse");
            output.WriteLine();

            WriteGrammar(LoadGrammar<If<ImmutableList<string>>>("grammar_if_trail.json"), output, "Trail");
            output.WriteLine();

            WriteGrammar(LoadGrammar<Switch<ImmutableList<string>>>("grammar_switch_simple.json"), output);
            output.WriteLine();

            WriteGrammar(LoadGrammar<Switch<ImmutableList<string>>>("grammar_switch_interrupted.json"), output);
            output.WriteLine();

            WriteGrammar(LoadGrammar<Switch<ImmutableList<string>>>("grammar_switch_no_trail_simple.json"), output);
            output.WriteLine();

            WriteGrammar(LoadGrammar<Switch<ImmutableList<string>>>("grammar_switch_trail_simple.json"), output, "Trail");
            output.WriteLine();

            WriteGrammar(LoadGrammar<Switch<ImmutableList<string>>>("grammar_switch_no_trail_interrupted.json"), output);
            output.WriteLine();

            WriteGrammar(LoadGrammar<Switch<ImmutableList<string>>>("grammar_switch_trail_interrupted.json"), output, "Trail");
            output.WriteLine();
        }

        static void WriteGrammar(IGrammar grammar, IndentedTextWriter output, string suffix = "")
        {
            if(grammar.Terminating is { } terminating)
            {
                WriteRule(terminating, output, "Terminating" + suffix);
                output.WriteLine();
            }
            if(grammar.Interrupting is { } interrupting)
            {
                WriteRule(interrupting, output, "Interrupting" + suffix);
                output.WriteLine();
            }
            if(grammar.Interruptible is { } interruptible)
            {
                WriteRule(interruptible, output, "Interruptible" + suffix);
                output.WriteLine();
            }
            if(grammar.Returning is { } returning)
            {
                WriteRule(returning, output, "Returning" + suffix);
                output.WriteLine();
            }
            if(grammar.Conditional is { } conditional)
            {
                WriteRule(conditional, output, "Conditional" + suffix);
                output.WriteLine();
            }
        }

        static void WriteRule(IEnumerable<IGrammarElement> grammar, IndentedTextWriter output, string statementCategory)
        {
            // Phase 1 - skip redundant groups
            var caseSet = grammar.Select(parts => parts.CollapseParts()).ToHashSet();

            // Phase 2 - form tree from all values and group by paths
            var groups = caseSet.BuildTree().GroupByPaths().Reverse().ToList();

            if(groups.Count == 0)
            {
                // Empty
                return;
            }

            // Phase 3 - construct grammar
            output.Write(caseSet.Sample(e => e.RuleName));
            output.Write(statementCategory);
            output.Write(':');
            output.WriteLine();
            output.Indent++;

            bool firstAlternative = true;

            foreach(var alternative in groups)
            {
                if(firstAlternative)
                {
                    firstAlternative = false;
                }
                else
                {
                    // Alternative paths
                    output.WriteLine("|");
                }

                var mainTree = alternative.Key;

                // Write main rule
                var mainTag = alternative.Sample(p => p.Key.Tag);
                output.Write(mainTag);
                output.Write(' ');

                // Write main blocks
                var mainCategories = alternative.GetCategories();
                StatementFlags mainFlags;
                if(mainCategories.Count == 1 && mainCategories[0] == "unused")
                {
                    // Unused main categories
                    mainFlags = StatementFlags.None;
                }
                else
                {
                    switch(mainTag)
                    {
                        case "if":
                            output.WriteSingleBlockCover(mainCategories, "elseif");
                            output.Write(' ');
                            break;
                    }

                    mainFlags = ToFlags(mainCategories);
                }

                // Group by contents regardless of trail
                // Only used when the middle can be shared
                var trailGroups = mainTree.Where(p => p.Key.Tag == "trail").GroupByPaths().Reverse().ToList();

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
                        ProcessMiddle(trailTree, mainFlags, false, true);

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

                    mainTree = CategoryTreeImmutable.Empty;
                }
                else
                {
                    ProcessMiddle(mainTree, mainFlags, true, false);
                }

                void ProcessMiddle(CategoryTreeImmutable parent, StatementFlags parentFlags, bool ignoreTrail, bool noTrail)
                {
                    // Group by follow-up
                    var groups =
                        (parentFlags == StatementFlags.None
                        // Do not group if at the beginning
                        ? parent.SelectMany(p => new[] { p }.GroupBy(p => (p.Key.Tag, p.Value)))
                        : parent.GroupBy(p => (p.Key.Tag, p.Value))).Reverse().ToList();

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
                            var (tag, tree) = group.Key;

                            if(tag == "trail" && ignoreTrail)
                            {
                                continue;
                            }

                            if(groupFirst)
                            {
                                groupFirst = false;
                            }
                            else
                            {
                                // Alternative paths
                                output.WriteLine("|");
                            }

                            var keys = group.Select(p => p.Key.Category).ToList();
                            var flags = parentFlags;

                            switch(tag)
                            {
                                case "elseif" or "case":
                                {
                                    // Deciding block
                                    output.Write(tag);

                                    if(flags == StatementFlags.None)
                                    {
                                        // First case
                                        output.Write(' ');
                                        output.WriteSingleBlockCover(keys, tag);
                                        output.Write(' ');
                                        flags = ToFlags(keys);
                                    }
                                    else
                                    {
                                        output.Write(' ');
                                        output.WriteAlternativeBlocks(FlagsTransformBlocks(ToFlags(keys), ref flags));
                                        output.Write(' ');

                                        // Covering block
                                        output.Write('(');
                                        output.Write(tag);
                                        output.Write(' ');
                                        output.WriteAlternativeBlocks(FlagsCoverBlocks(flags));
                                        output.Write(")* ");
                                    }
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
                                    if(flags == StatementFlags.None)
                                    {
                                        output.WriteAlternativeBlocks(keys);
                                        flags = ToFlags(keys);
                                    }
                                    else
                                    {
                                        output.WriteAlternativeBlocks(FlagsTransformBlocks(ToFlags(keys), ref flags));
                                    }
                                    if(isIgnored)
                                    {
                                        output.Write(")?");
                                    }
                                    output.Write(' ');
                                    if(noTrail)
                                    {
                                        // Trail was already entered
                                        if(!tree.Nodes.IsEmpty)
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
                                    if(!tree.Nodes.IsEmpty)
                                    {
                                        throw new InvalidOperationException("There are remaining paths after trail.");
                                    }
                                    continue;
                                }
                            }

                            // Process follow-up
                            ProcessMiddle(tree, flags, false, noTrail);
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
            output.WriteLine(";");
            output.Indent--;
            output.Flush();
        }
    }
        
    static class Extensions
    {
        public static CategoryTreeImmutable BuildTree(this IEnumerable<IGrammarElement> grammar)
        {
            var caseTreeBuilder = new CategoryTreeMutable();

            foreach(var parts in grammar)
            {
                foreach(var path in parts.TreePaths())
                {
                    // Starting from root
                    var node = caseTreeBuilder;

                    foreach(var (key, value) in path)
                    {
                        // Add node to tree

                        node = node.Add(new(key, value));
                    }
                }
            }

            return caseTreeBuilder.ToImmutable();
        }

        public static IEnumerable<IGrouping<TreeImmutable<TKey>, KeyValuePair<TKey, TreeImmutable<TKey>>>> GroupByPaths<TKey>(this IEnumerable<KeyValuePair<TKey, TreeImmutable<TKey>>> tree) where TKey : notnull
        {
            return tree.GroupBy(p => p.Value);
        }

        public static TResult Sample<TElement, TResult>(this IEnumerable<TElement> enumerable, Func<TElement, TResult> selector)
        {
            return enumerable.Select(selector).Distinct().Single();
        }

        public static List<string> GetCategories<T>(this IEnumerable<KeyValuePair<CategoryKey, T>> enumerable)
        {
            return enumerable.Select(p => p.Key.Category).ToList();
        }

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

        static readonly Dictionary<string, HashSet<string>> simplifiedBlockNames = new()
        {
            { "interruptingCover", new() { "terminating", "interrupting" } },
            { "returningCover", new() { "terminating", "interrupting", "returning" } },
            { "openCover", new() { "terminating", "open" } },
            { "interruptibleCover", new() { "terminating", "open", "interrupting", "interruptible" } },
            { "conditionalCover", new() { "terminating", "open", "interrupting", "interruptible", "returning", "conditional" } },

            { "openToInterruptible", new() { "open", "interruptible" } },
            { "openToConditional", new() { "open", "interruptible", "conditional" } },
            { "interruptingToInterruptible", new() { "interrupting", "interruptible" } },
            { "returningToConditional", new() { "returning", "conditional" } }
        };

        static readonly Dictionary<string, HashSet<string>> simplifiedTrailNames = new()
        {
            { "interruptingCover", new() { "terminating", "interrupting" } },
            { "returningCover", new() { "terminating", "interrupting", "returning" } },
            { "interruptibleCover", new() { "open", "interruptible" } },
            { "conditionalCover", new() { "open", "interruptible", "conditional" } }
        };

        public static StatementFlags ToFlags(string str)
        {
            if(!blockFlags.TryGetValue(str, out var flags))
            {
                str += "_block";
                flags = blockFlags[str];
            }
            return flags & ~Terminating;
        }

        public static StatementFlags ToFlags(IReadOnlyCollection<string> list)
        {
            return list.Aggregate(None, (a, c) => a | ToFlags(c));
        }

        public static string FromFlags(StatementFlags flags, string original)
        {
            if(ToFlags(original) == flags)
            {
                return blockNames[blockFlags[original]];
            }
            return blockNames[flags];
        }

        public static string ToBlock(string category)
        {
            if(category == "ignored")
            {
                throw new ArgumentException("Ignored block is not allowed.", nameof(category));
            }
            return category + "Block";
        }

        public static string ToTrail(string category)
        {
            return category + "Trail";
        }

        /// <summary>
        /// Returns block names that transform <paramref name="originalFlags"/> exactly into <paramref name="flags"/>.
        /// </summary>
        public static IReadOnlyCollection<string> FlagsTransformBlocks(StatementFlags flags, ref StatementFlags originalFlags)
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
        public static IReadOnlyCollection<string> FlagsCoverBlocks(StatementFlags flags)
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

        public static void WriteAlternatives(TextWriter output, IReadOnlyCollection<string> blocks, Func<string, string> map)
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

        private static IReadOnlyCollection<string> SimplifyAlternatives(IReadOnlyCollection<string> collection, Dictionary<string, HashSet<string>> labels)
        {
            foreach(var (key, set) in labels)
            {
                if(set.Count == collection.Count && collection.All(set.Contains))
                {
                    return new[] { key };
                }
            }
            return collection;
        }

        public static void WriteAlternativeBlocks(this TextWriter output, IReadOnlyCollection<string> blocks)
        {
            WriteAlternatives(output, SimplifyAlternatives(blocks, simplifiedBlockNames), ToBlock);
        }

        public static void WriteAlternativeTrails(this TextWriter output, IReadOnlyCollection<string> trails)
        {
            WriteAlternatives(output, SimplifyAlternatives(trails, simplifiedTrailNames), ToTrail);
        }

        public static void WriteSingleBlockCover(this TextWriter output, IReadOnlyCollection<string> blocks, string coverTag)
        {
            switch(blocks.Count)
            {
                case 1:
                    var category = blocks.First();
                    output.Write(ToBlock(category));
                    output.Write(' ');
                    output.Write('(');
                    output.Write(coverTag);
                    output.Write(' ');
                    output.WriteAlternativeBlocks(FlagsCoverBlocks(ToFlags(category)));
                    output.Write(")*");
                    break;
                default:
                    throw new ArgumentException("Parameter must have exactly one element.", nameof(blocks));
            }
        }

        public static Grammar<T> LoadGrammar<T>(string path) where T : class, IGrammarElement
        {
            using var file = File.Open(path, FileMode.Open);
            return JsonSerializer.Deserialize<Grammar<T>>(file)!;
        }

        public record struct CategoryKey(string Tag, string Category) : IComparable<CategoryKey>, IComparable
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
    }

    record TreeMutable<TKey> : IEnumerable<KeyValuePair<TKey, TreeMutable<TKey>>> where TKey : notnull
    {
        public Dictionary<TKey, TreeMutable<TKey>> Nodes { get; } = new();

        public IEnumerator<KeyValuePair<TKey, TreeMutable<TKey>>> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TreeImmutable<TKey> ToImmutable()
        {
            return new TreeImmutable<TKey>(MapModule.OfSeq(Nodes.Select(p => Tuple.Create(p.Key, p.Value.ToImmutable()))));
        }

        public TreeMutable<TKey> Add(TKey key)
        {
            if(!Nodes.TryGetValue(key, out var tree))
            {
                Nodes[key] = tree = new();
            }
            return tree;
        }
    }

    record TreeImmutable<TKey>(FSharpMap<TKey, TreeImmutable<TKey>> Nodes) : IEnumerable<KeyValuePair<TKey, TreeImmutable<TKey>>> where TKey : notnull
    {
        public static readonly TreeImmutable<TKey> Empty = new(MapModule.Empty<TKey, TreeImmutable<TKey>>());

        public IEnumerator<KeyValuePair<TKey, TreeImmutable<TKey>>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<TKey, TreeImmutable<TKey>>)Nodes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
