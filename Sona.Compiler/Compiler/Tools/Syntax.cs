using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
using FSharp.Compiler.Tokenization;
using Microsoft.FSharp.Core;

namespace IS4.Sona.Compiler.Tools
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
                    if(token != null || !tok.IsStringLiteral)
                    {
                        throw new ArgumentException("Argument does not contain a single string literal.", nameof(str));
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
            var tuple = (ITuple)parserToken.GetType().InvokeMember("Item", flags, null, parserToken, null)!;
            return (string)tuple[0]!;
        }
    }
}
