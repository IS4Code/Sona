using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;

namespace Sona.Compiler.Tools
{
    internal static class StatePool
    {
        static readonly ConcurrentDictionary<Type, DefaultObjectPool<ScriptState>> byType = new();

        public static TState Get<TState>() where TState : ScriptState, new()
        {
            return (TState)Cache<TState>.Pool.Get();
        }

        public static void Return(ScriptState state)
        {
            if(!byType.TryGetValue(state.GetType(), out var pool))
            {
                return;
            }
            pool.Return(state);
        }

        static class Cache<TState> where TState : ScriptState, new()
        {
            public static readonly DefaultObjectPool<ScriptState> Pool = byType.GetOrAdd(typeof(TState), static _ => new(Policy.Instance));

            sealed class Policy : PooledObjectPolicy<ScriptState>
            {
                public static readonly Policy Instance = new();

                private Policy()
                {

                }

                public override ScriptState Create()
                {
                    return new TState();
                }

                public override bool Return(ScriptState state)
                {
                    return true;
                }
            }
        }
    }
}
