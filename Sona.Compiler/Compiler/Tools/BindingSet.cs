using System.Collections.Generic;
using System.Threading;
using Sona.Compiler.States;

namespace Sona.Compiler.Tools
{
    internal struct BindingSet
    {
        IBindingContext? parent;
        Dictionary<string, BindingKind>? dictionary;

        public BindingSet(IBindingContext? parent)
        {
            this.parent = parent;
            dictionary = null;
        }

        public readonly BindingKind Get(string name)
        {
            var dict = dictionary;
            if(dict == null || !dict.TryGetValue(name, out var kind))
            {
                return parent?.Get(name) ?? BindingKind.Undefined;
            }
            return kind;
        }

        public void Set(string name, BindingKind kind)
        {
            LazyInitializer.EnsureInitialized(ref dictionary);

            dictionary![name] = kind;
        }
    }
}
