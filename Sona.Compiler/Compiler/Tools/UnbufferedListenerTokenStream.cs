using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace IS4.Sona.Compiler.Tools
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

            Init(ref readFromConstructor, tokenReceiver);
        }

        public UnbufferedListenerTokenStream(ITokenSource tokenSource, ChannelTokenReceiver tokenReceiver, int bufferSize) : base(tokenSource, bufferSize)
        {
            this.tokenReceiver = tokenReceiver ?? throw new ArgumentNullException(nameof(tokenReceiver));

            Init(ref readFromConstructor, tokenReceiver);
        }

        private void Init(ref List<IToken> list, ChannelTokenReceiver tokenReceiver)
        {
            // Base constructor calls Add which puts the token into list before tokenReceiver is set
            foreach(var token in list)
            {
                tokenReceiver(token);
            }
            // No longer needed
            list = null!;
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
    }
}
