using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sona.Compiler.Tools
{
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T?>, IEqualityComparer where T : class
    {
        public static readonly ReferenceEqualityComparer<T> Instance = new();

        public bool Equals(T? x, T? y)
        {
            return Object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T? obj)
        {
            return RuntimeHelpers.GetHashCode(obj!);
        }

        bool IEqualityComparer.Equals(object? x, object? y)
        {
            return Object.ReferenceEquals(x, y);
        }

        int IEqualityComparer.GetHashCode(object? obj)
        {
            return RuntimeHelpers.GetHashCode(obj!);
        }
    }
}
