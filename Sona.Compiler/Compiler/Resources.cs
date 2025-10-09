using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using FSharp.Compiler.Diagnostics;

namespace Sona.Compiler
{
    internal static class Resources
    {
        static readonly Assembly assembly = typeof(CompilerDiagnostics).Assembly;

        static readonly ResourceManager manager = new("fsstrings", assembly);
        static readonly Type type = assembly.GetType("FSComp.SR", true)!;

        static readonly ConcurrentDictionary<(CultureInfo, string), Regex> patterns = new();

        static readonly Regex formatRegex = new(@"\{(\d+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static Regex GetRegex(string key, CultureInfo culture)
        {
            return patterns.GetOrAdd((culture, key), static pair => {
                var (culture, key) = pair;
                if(manager.GetString(key, culture) is { } value)
                {
                    return RegexFromString(value);
                }
                if(type.GetMethod(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) is { } method)
                {
                    return RegexFromMethod(method);
                }
                throw new ArgumentException($"The key '{key}' is not found in the compiler resources.", nameof(key));
            });
        }

        static Regex RegexFromString(string value)
        {
            // Replace all interpolations with wildcards

            var sb = new StringBuilder();

            int pos = 0;
            foreach(Match match in formatRegex.Matches(value))
            {
                var index = Int32.Parse(match.Groups[1].Value);

                sb.Append(Regex.Escape(value.Substring(pos, match.Index - pos)));
                sb.Append($"(?<{index + 1}>.*?)");
                pos = match.Index + match.Length;
            }

            sb.Append(Regex.Escape(value.Substring(pos, value.Length - pos)));

            return new(sb.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
        }

        const int intSubstStart = Int32.MaxValue / 2;
        static readonly Regex intSubstRegex = new("[0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        static Regex RegexFromMethod(MethodInfo method)
        {
            bool hasIntSubsts = false;

            var methodParams = method.GetParameters();
            var args = new object[methodParams.Length];
            for(int i = 0; i < args.Length; i++)
            {
                var type = methodParams[i].ParameterType;

                switch(Type.GetTypeCode(type))
                {
                    case TypeCode.String:
                        args[i] = $"{{{i}}}";
                        break;
                    case TypeCode.Int32:
                        hasIntSubsts = true;
                        args[i] = intSubstStart + i;
                        break;
                    default:
                        throw new NotSupportedException($"Resources with parameters of type {type} are not supported.");
                }
            }

            var result = method.Invoke(null, args);

            string text = result switch
            {
                string str => str,
                Tuple<int, string> tuple => tuple.Item2,
                _ => throw new NotSupportedException($"Unexpected resource result of type '{result?.GetType().ToString() ?? "<null>"}'.")
            };

            if(hasIntSubsts)
            {
                text = intSubstRegex.Replace(text, m => {
                    var value = m.Value;
                    if(!Int32.TryParse(value, out var num) || num < intSubstStart)
                    {
                        return value;
                    }
                    return $"{{{num - intSubstStart}}}";
                });
            }

            return RegexFromString(text);
        }
    }
}
