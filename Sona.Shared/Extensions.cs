using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Sona
{
    internal static class Extensions
    {
#if !NET6_0_OR_GREATER
        public static bool TryPeek<T>(this Queue<T> queue, [MaybeNullWhen(false)] out T element)
        {
            if(queue.Count == 0)
            {
                element = default;
                return false;
            }
            element = queue.Peek();
            return true;
        }

        public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T element)
        {
            if(stack.Count == 0)
            {
                element = default;
                return false;
            }
            element = stack.Peek();
            return true;
        }

        public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T element)
        {
            if(stack.Count == 0)
            {
                element = default;
                return false;
            }
            element = stack.Pop();
            return true;
        }

        public static int IndexOfAnyInRange(this ReadOnlySpan<char> span, char from, char to)
        {
            for(int i = 0; i < span.Length; i++)
            {
                var c = span[i];
                if(c >= from && c <= to)
                {
                    return i;
                }
            }
            return -1;
        }

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> segment, int start)
        {
            if(start < 0 || start > segment.Count) throw new ArgumentOutOfRangeException(nameof(start));
            return new ArraySegment<T>(segment.Array, segment.Offset + start, segment.Count - start);
        }

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> segment, int start, int length)
        {
            if(start < 0) throw new ArgumentOutOfRangeException(nameof(start));
            if(start + length > segment.Count) throw new ArgumentOutOfRangeException(nameof(length));
            return new ArraySegment<T>(segment.Array, segment.Offset + start, length);
        }

        public static void CopyTo<T>(this ArraySegment<T> segment, T[] array, int index = 0)
        {
            Array.Copy(segment.Array, segment.Offset, array, index, segment.Count);
        }

        public static bool StartsWith(this string str, char c)
        {
            return str.Length > 0 && str[0] == c;
        }

        public static bool EndsWith(this string str, char c)
        {
            return str.Length > 0 && str[str.Length - 1] == c;
        }

        public static bool Contains(this string str, char c)
        {
            return str.IndexOf(c) != -1;
        }

        public static bool Contains(this string str, string s, StringComparison comparison)
        {
            return str.IndexOf(s, comparison) != -1;
        }
#endif

#if !NET8_0_OR_GREATER
        public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IComparable<T>
        {
            foreach(var element in span)
            {
                if(values.BinarySearch(element) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
#endif

        public static bool IsAsciiLetter(this char c)
        {
#if NET7_0_OR_GREATER
            return Char.IsAsciiLetter(c);
#else
            return (uint)((c | 0x20) - 'a') <= 'z' - 'a';
#endif
        }

        public static bool IsInteger(this decimal d)
        {
#if NET7_0_OR_GREATER
            return Decimal.IsInteger(d);
#else
            return Decimal.Truncate(d) == d;
#endif
        }
    }
}

#if !NET6_0_OR_GREATER
namespace System.Runtime.InteropServices
{
    internal static class CollectionsMarshal
    {
        public static Span<T> AsSpan<T>(List<T> list)
        {
            return ((T[])Cache<T>.ItemsField.GetValue(list)).AsSpan(0, list.Count);
        }

        static class Cache<T>
        {
            public static readonly FieldInfo ItemsField;

            static Cache()
            {
                var type = typeof(List<T>);
                var arrayType = typeof(T[]);
                foreach(var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if(arrayType.Equals(field.FieldType))
                    {
                        ItemsField = field;
                        return;
                    }
                }
                throw new NotSupportedException("The internal item array cannot be retrieved.");
            }
        }
    }
}
#endif
