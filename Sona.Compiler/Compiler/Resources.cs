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

        static Regex RegexFromMethod(MethodInfo method)
        {
            var methodParams = method.GetParameters();
            var args = new object[methodParams.Length];
            for(int i = 0; i < args.Length; i++)
            {
                var type = methodParams[i].ParameterType;
                if(!typeof(string).Equals(type))
                {
                    throw new NotSupportedException("Resources with non-string parameters are not supported.");
                }
                args[i] = $"{{{i}}}";
            }

            var result = method.Invoke(null, args);

            switch(result)
            {
                case string str:
                    return RegexFromString(str);
                case Tuple<int, string> tuple:
                    return RegexFromString(tuple.Item2);
                default:
                    throw new NotSupportedException($"Unexpected resource result of type '{result?.GetType().ToString() ?? "<null>"}'.");
            }
        }
    }
}
