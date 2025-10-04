using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
using FSharp.Compiler.Tokenization;
using Microsoft.FSharp.Core;

namespace Sona.Compiler.Tools
{
    internal static class Syntax
    {
        public static bool IsValidIdentifierName(string name)
        {
            return PrettyNaming.IsIdentifierName(name);
        }

        static readonly Regex invalidEnclosedIdentifier = new(@"[\r\n\t]|``", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        public static bool IsValidEnclosedIdentifierName(string name)
        {
            return name.Length != 0 && !name.EndsWith('`') && !invalidEnclosedIdentifier.IsMatch(name);
        }

        static readonly char[] escapedChars = Enumerable.Range(0, 32).Select(i => (char)i).Concat(new[] { '"', '\\' }).ToArray();
        public static IEnumerable<string> EscapeString(string part)
        {
            int start = 0;
            while(part.AsSpan(start).IndexOfAny(escapedChars) is (not -1) and int offset)
            {
                yield return part.Substring(start, offset);
                start += offset;
                yield return part[start] switch
                {
                    '\a' => @"\a",
                    '\b' => @"\b",
                    '\f' => @"\f",
                    '\n' => @"\n",
                    '\r' => @"\r",
                    '\t' => @"\t",
                    '\v' => @"\v",
                    '\\' => @"\\",
                    '"' => @"\""",
                    var c => $@"\u{(ushort)c:X4}"
                };
                start++;
            }

            yield return part.Substring(start);
        }

        static readonly char[] nameTrimChars = { '@' };
        public static string GetIdentifierFromName(string name)
        {
            return name.TrimStart(nameTrimChars);
        }

        static readonly char[] targetTrimChars = { '#', ':', ' ', '\t', '\xc' };
        public static string GetAttributeTargetFromToken(string token)
        {
            return token.Trim(targetTrimChars);
        }

        private static void Tokenize(string str, Converter<FSharpToken, Unit> visitor)
        {
            var text = SourceText.ofString(str);
            FSharpLexer.Tokenize(
                text,
                FSharpFunc<FSharpToken, Unit>.FromConverter(visitor),
                langVersion: null,
                strictIndentation: null,
                filePath: null,
                conditionalDefines: null,
                flags: null,
                pathMap: null,
                ct: null
            );
        }

        public static string GetStringLiteralValue(string str)
        {
            FSharpToken? token = null;
            Tokenize(str, tok => {
                if(token != null || (!tok.IsStringLiteral && !tok.Kind.IsChar))
                {
                    throw new ArgumentException("Argument does not contain a single string or character literal.", nameof(str));
                }
                token = tok;
                return null!;
            });
            return GetTokenValue(token);
        }

        public static string GetIdentifierValue(string str)
        {
            FSharpToken? token = null;
            Tokenize(str, tok => {
                if(token != null || (!tok.IsIdentifier))
                {
                    throw new ArgumentException("Argument does not contain a single identifier.", nameof(str));
                }
                token = tok;
                return null!;
            });
            return GetTokenValue(token);
        }

        private static string GetTokenValue(FSharpToken? token)
        {
            token = token ?? throw new ArgumentNullException(nameof(token));

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty;
            var parserToken = token.GetType().InvokeMember("tok", flags, null, token, null)!;
            var item = parserToken.GetType().InvokeMember("Item", flags, null, parserToken, null)!;
            if(item is char c)
            {
                return c.ToString();
            }
            if(item is string s)
            {
                return s;
            }
#if NETCOREAPP2_0_OR_GREATER || NET471_OR_GREATER
            if(item is ITuple t)
            {
                return (string)t[0]!;
            }
#else
            if(item?.GetType().GetProperty("Item1") is { } prop)
            {
                return (string)prop.GetValue(item);
            }
#endif
            throw new ArgumentException("Argument does not have a recognized value.", nameof(token));
        }
    }
}
