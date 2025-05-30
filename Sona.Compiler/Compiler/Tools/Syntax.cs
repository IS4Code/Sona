using System;
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

        public static string GetStringLiteralValue(string str)
        {
            var text = SourceText.ofString(str);
            FSharpToken? token = null;
            FSharpLexer.Tokenize(
                text,
                FSharpFunc<FSharpToken, Unit>.FromConverter(tok => {
                    if(token != null || (!tok.IsStringLiteral && !tok.Kind.IsChar))
                    {
                        throw new ArgumentException("Argument does not contain a single string or character literal.", nameof(str));
                    }
                    token = tok;
                    return null!;
                }),
                langVersion: null,
                strictIndentation: null,
                filePath: null,
                conditionalDefines: null,
                flags: null,
                pathMap: null,
                ct: null
            );
            if(token is not { } tok)
            {
                throw new ArgumentException("Argument is empty.", nameof(str));
            }
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty;
            var parserToken = tok.GetType().InvokeMember("tok", flags, null, tok, null)!;
            var item = parserToken.GetType().InvokeMember("Item", flags, null, parserToken, null)!;
            if(item is char c)
            {
                return c.ToString();
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
            throw new ArgumentException("Argument is not a recognized literal.", nameof(str));
        }
    }
}
