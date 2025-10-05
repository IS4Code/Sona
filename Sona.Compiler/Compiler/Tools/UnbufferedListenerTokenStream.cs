using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Sona.Compiler.Tools
{
    internal sealed class UnbufferedListenerTokenStream : UnbufferedTokenStream
    {
        public delegate void ChannelTokenReceiver(IToken token);

        readonly ChannelTokenReceiver? tokenReceiver;

        readonly List<IToken> readFromConstructor = new();

        int absoluteTokenIndex = -1;

        public UnbufferedListenerTokenStream(ITokenSource tokenSource, ChannelTokenReceiver tokenReceiver) : base(tokenSource)
        {
            this.tokenReceiver = tokenReceiver ?? throw new ArgumentNullException(nameof(tokenReceiver));
        }

        public UnbufferedListenerTokenStream(ITokenSource tokenSource, ChannelTokenReceiver tokenReceiver, int bufferSize) : base(tokenSource, bufferSize)
        {
            this.tokenReceiver = tokenReceiver ?? throw new ArgumentNullException(nameof(tokenReceiver));
        }

        public void StartReceiving()
        {
            if(tokenReceiver == null)
            {
                throw new InvalidOperationException();
            }
            // Base constructor calls Add which puts the token into list before tokenReceiver is set
            foreach(var token in readFromConstructor)
            {
                tokenReceiver(token);
            }
            readFromConstructor.Clear();
        }

        protected override void Add(IToken t)
        {
            while(true)
            {
                absoluteTokenIndex += 1;
                if(t.Channel == 0)
                {
                    base.Add(t);
                    if(t is IWritableToken wt)
                    {
                        wt.TokenIndex = absoluteTokenIndex;
                    }
                    return;
                }
                else if(t is IWritableToken wt)
                {
                    wt.TokenIndex = absoluteTokenIndex;
                }
                // Is null before constructor is executed
                if(tokenReceiver != null)
                {
                    tokenReceiver(t);
                }
                else
                {
                    readFromConstructor.Add(t);
                }

                // Get a replacement for this token
                t = TokenSource.NextToken();
            }
        }

        public override string GetText(Interval interval)
        {
            if(tokens[0].TokenIndex > interval.a || tokens[n - 1].TokenIndex < interval.b)
            {
                throw new NotSupportedException($"interval {interval} not in token buffer window");
            }

            int first = Array.BinarySearch(tokens, 0, n, interval.a, TokenPositionComparer.Instance);
            if(first < 0)
            {
                first = ~first;
            }
            int last = Array.BinarySearch(tokens, 0, n, interval.b, TokenPositionComparer.Instance);
            if(last < 0)
            {
                last = ~last;
            }

            var sb = new StringBuilder();
            for(int i = first; i <= last; i++)
            {
                var token = tokens[i];
                sb.Append(token.Text);
            }
            return sb.ToString();
        }

        sealed class TokenPositionComparer : IComparer
        {
            public static readonly IComparer Instance = new TokenPositionComparer();

            private TokenPositionComparer()
            {

            }

            public int Compare(object? x, object? y)
            {
                return GetIndex(x).CompareTo(GetIndex(y));

                static int GetIndex(object? obj)
                {
                    switch(obj)
                    {
                        case IToken { TokenIndex: var index }:
                            return index;
                        case int index:
                            return index;
                    }
                    throw new ArgumentException("Only tokens and integers can be compared.");
                }
            }
        }
    }
}
