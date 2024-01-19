using System.Reflection;
using ScrubJay.Collections;

// ReSharper disable MethodOverloadWithOptionalParameter
// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Comparison;

public static partial class Relate
{
    public static class Equal
    {
      
        
        
        
        private static readonly ConcurrentTypeMap<IEqualityComparer> _equalityComparers = new();

        private static IEqualityComparer FindEqualityComparer(Type type)
        {
            return typeof(EqualityComparer<>)
                .MakeGenericType(type)
                .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                .ThrowIfNull()
                .GetValue(null)
                .AsValid<IEqualityComparer>();
        }

        public static IEqualityComparer GetEqualityComparer(Type type)
        {
            return _equalityComparers.GetOrAdd(type, static t => FindEqualityComparer(t));
        }
        public static DefaultEqualityComparer GetEqualityComparer(object? _ = default) => DefaultEqualityComparer.Instance;
        public static DefaultEqualityComparer<T> GetEqualityComparer<T>(T? _ = default) => DefaultEqualityComparer<T>.Instance;

        public static IEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
            => new FuncEqualityComparer<T>(equals, getHashCode);
        public static IEqualityComparer<T> CreateNonNullEqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
            => new FuncEqualityComparer<T>(equals!, getHashCode!);
        
        
        
        
        public static bool Values<T>(T? left, T? right) => EqualityComparer<T>.Default.Equals(left!, right!);

        public static bool Objects(object? left, object? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left is not null)
            {
                return GetEqualityComparer(left.GetType()).Equals(left, right);
            }

            return GetEqualityComparer(right!.GetType()).Equals(right, left);
        }

#region Sequence

#region IEquatable

        public static bool Sequence<T>(T[]? left, T[]? right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right.AsSpan());
        }

        public static bool Sequence<T>(Span<T> left, T[]? right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan());
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan());
        }

        public static bool Sequence<T>(T[]? left, Span<T> right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
        }

        public static bool Sequence<T>(Span<T> left, Span<T> right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
        }

        public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, Constraints.IsEquatable<T> _ = default)
            where T : IEquatable<T>
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

#endregion

#region Open

#if !NET6_0_OR_GREATER
        public static bool Sequence<T>(T[]? left, T[]? right)
        {
            return SpanExtensions.SequenceEqual<T>(left.AsSpan(), right.AsSpan());
        }

        public static bool Sequence<T>(Span<T> left, T[]? right)
        {
            return SpanExtensions.SequenceEqual<T>(left, right.AsSpan());
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right)
        {
            return SpanExtensions.SequenceEqual<T>(left, right.AsSpan());
        }

        public static bool Sequence<T>(T[]? left, Span<T> right)
        {
            return SpanExtensions.SequenceEqual<T>(left.AsSpan(), right);
        }

        public static bool Sequence<T>(Span<T> left, Span<T> right)
        {
            return SpanExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right)
        {
            return SpanExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right)
        {
            return SpanExtensions.SequenceEqual<T>(left.AsSpan(), right);
        }

        public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right)
        {
            return SpanExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        {
            return SpanExtensions.SequenceEqual<T>(left, right);
        }
#else
        public static bool Sequence<T>(T[]? left, T[]? right)
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right.AsSpan());
        }

        public static bool Sequence<T>(Span<T> left, T[]? right)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan());
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan());
        }

        public static bool Sequence<T>(T[]? left, Span<T> right)
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
        }

        public static bool Sequence<T>(Span<T> left, Span<T> right)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right)
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
        }

        public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right);
        }
#endif

#endregion

#region Open w/EqualityComparer

#if !NET6_0_OR_GREATER
        public static bool Sequence<T>(T[]? left, T[]? right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left.AsSpan(), right.AsSpan(), comparer);
        }

        public static bool Sequence<T>(Span<T> left, T[]? right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left, right.AsSpan(), comparer);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left, right.AsSpan(), comparer);
        }

        public static bool Sequence<T>(T[]? left, Span<T> right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left.AsSpan(), right, comparer);
        }

        public static bool Sequence<T>(Span<T> left, Span<T> right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left, right, comparer);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left, right, comparer);
        }

        public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left.AsSpan(), right, comparer);
        }

        public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left, right, comparer);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        {
            return SpanExtensions.SequenceEqual<T>(left, right, comparer);
        }
#else
        public static bool Sequence<T>(T[]? left, T[]? right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right.AsSpan(), comparer);
        }

        public static bool Sequence<T>(Span<T> left, T[]? right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan(), comparer);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, T[]? right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan(), comparer);
        }

        public static bool Sequence<T>(T[]? left, Span<T> right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right, comparer);
        }

        public static bool Sequence<T>(Span<T> left, Span<T> right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right, comparer);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, Span<T> right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right, comparer);
        }

        public static bool Sequence<T>(T[]? left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right, comparer);
        }

        public static bool Sequence<T>(Span<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right, comparer);
        }

        public static bool Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        {
            return MemoryExtensions.SequenceEqual<T>(left, right, comparer);
        }
#endif

#endregion

#region IEnumerable

        public static bool Sequence<T>(T[]? left, IEnumerable<T>? right)
            => Sequence<T>((ReadOnlySpan<T>)left, right);

        public static bool Sequence<T>(Span<T> left, IEnumerable<T>? right)
            => Sequence<T>((ReadOnlySpan<T>)left, right);

        public static bool Sequence<T>(ReadOnlySpan<T> left, IEnumerable<T>? right)
        {
            if (right is null) return false;
            int count = left.Length;
            switch (right)
            {
                case IList<T> list:
                {
                    if (list.Count != count) return false;
                    for (var i = 0; i < count; i++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(left[i], list[i]))
                            return false;
                    }

                    return true;
                }
                case IReadOnlyList<T> roList:
                {
                    if (roList.Count != count) return false;
                    for (var i = 0; i < count; i++)
                    {
                        if (!EqualityComparer<T>.Default.Equals(left[i], roList[i]))
                            return false;
                    }

                    return true;
                }
                case ICollection<T> collection:
                {
                    if (collection.Count != count) return false;
                    using var e = collection.GetEnumerator();
                    for (var i = 0; i < count; i++)
                    {
                        e.MoveNext();
                        if (!EqualityComparer<T>.Default.Equals(left[i], e.Current))
                            return false;
                    }

                    return true;
                }
                case IReadOnlyCollection<T> roCollection:
                {
                    if (roCollection.Count != count) return false;
                    using var e = roCollection.GetEnumerator();
                    for (var i = 0; i < count; i++)
                    {
                        e.MoveNext();
                        if (!EqualityComparer<T>.Default.Equals(left[i], e.Current))
                            return false;
                    }

                    return true;
                }
                default:
                {
                    using var e = right.GetEnumerator();
                    for (var i = 0; i < count; i++)
                    {
                        if (!e.MoveNext())
                            return false;
                        if (!EqualityComparer<T>.Default.Equals(left[i], e.Current))
                            return false;
                    }

                    if (e.MoveNext())
                        return false;
                    return true;
                }
            }
        }

        public static bool Sequence<T>(IEnumerable<T>? left, T[]? right)
            => Sequence<T>(right, left);

        public static bool Sequence<T>(IEnumerable<T>? left, Span<T> right)
            => Sequence<T>(right, left);

        public static bool Sequence<T>(IEnumerable<T>? left, ReadOnlySpan<T> right)
            => Sequence<T>(right, left);

#endregion

#region IEnumerable w/EqualityComparer

        public static bool Sequence<T>(T[]? left, IEnumerable<T>? right, IEqualityComparer<T>? comparer)
            => Sequence<T>((ReadOnlySpan<T>)left, right, comparer);

        public static bool Sequence<T>(Span<T> left, IEnumerable<T>? right, IEqualityComparer<T>? comparer)
            => Sequence<T>((ReadOnlySpan<T>)left, right, comparer);

        public static bool Sequence<T>(ReadOnlySpan<T> left, IEnumerable<T>? right, IEqualityComparer<T>? comparer)
        {
            if (right is null) return false;
            int count = left.Length;
            comparer ??= EqualityComparer<T>.Default;
            switch (right)
            {
                case IList<T> list:
                {
                    if (list.Count != count) return false;
                    for (var i = 0; i < count; i++)
                    {
                        if (!comparer.Equals(left[i], list[i]))
                            return false;
                    }

                    return true;
                }
                case IReadOnlyList<T> roList:
                {
                    if (roList.Count != count) return false;
                    for (var i = 0; i < count; i++)
                    {
                        if (!comparer.Equals(left[i], roList[i]))
                            return false;
                    }

                    return true;
                }
                case ICollection<T> collection:
                {
                    if (collection.Count != count) return false;
                    using var e = collection.GetEnumerator();
                    for (var i = 0; i < count; i++)
                    {
                        e.MoveNext();
                        if (!comparer.Equals(left[i], e.Current))
                            return false;
                    }

                    return true;
                }
                case IReadOnlyCollection<T> roCollection:
                {
                    if (roCollection.Count != count) return false;
                    using var e = roCollection.GetEnumerator();
                    for (var i = 0; i < count; i++)
                    {
                        e.MoveNext();
                        if (!comparer.Equals(left[i], e.Current))
                            return false;
                    }

                    return true;
                }
                default:
                {
                    using var e = right.GetEnumerator();
                    for (var i = 0; i < count; i++)
                    {
                        if (!e.MoveNext())
                            return false;
                        if (!comparer.Equals(left[i], e.Current))
                            return false;
                    }

                    if (e.MoveNext())
                        return false;
                    return true;
                }
            }
        }

        public static bool Sequence<T>(IEnumerable<T>? left, T[]? right, IEqualityComparer<T>? comparer)
            => Sequence<T>(right, left, comparer);

        public static bool Sequence<T>(IEnumerable<T>? left, Span<T> right, IEqualityComparer<T>? comparer)
            => Sequence<T>(right, left, comparer);

        public static bool Sequence<T>(IEnumerable<T>? left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
            => Sequence<T>(right, left, comparer);

#endregion

#endregion

#region Text

        public static bool Text(string? left, string? right)
        {
            return string.Equals(left, right);
        }

        public static bool Text(string? left, ReadOnlySpan<char> right)
        {
            return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
        }

        public static bool Text(string? left, char[]? right)
        {
            return MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);
        }

        public static bool Text(ReadOnlySpan<char> left, string? right)
        {
            return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
        }

        public static bool Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            return MemoryExtensions.SequenceEqual<char>(left, right);
        }

        public static bool Text(ReadOnlySpan<char> left, char[]? right)
        {
            return MemoryExtensions.SequenceEqual<char>(left, right);
        }

        public static bool Text(char[]? left, string? right)
        {
            return MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());
        }

        public static bool Text(char[]? left, ReadOnlySpan<char> right)
        {
            return MemoryExtensions.SequenceEqual<char>(left, right);
        }

        public static bool Text(char[]? left, char[]? right)
        {
            return MemoryExtensions.SequenceEqual<char>(left, right);
        }

#endregion

#region Text w/StringComparison

        public static bool Text(string? left, string? right, StringComparison comparison)
        {
            return string.Equals(left, right, comparison);
        }

        public static bool Text(string? left, ReadOnlySpan<char> right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
        }

        public static bool Text(string? left, char[]? right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left.AsSpan(), right, comparison);
        }

        public static bool Text(ReadOnlySpan<char> left, string? right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
        }

        public static bool Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left, right, comparison);
        }

        public static bool Text(ReadOnlySpan<char> left, char[]? right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left, right, comparison);
        }

        public static bool Text(char[]? left, string? right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left, right.AsSpan(), comparison);
        }

        public static bool Text(char[]? left, ReadOnlySpan<char> right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left, right, comparison);
        }

        public static bool Text(char[]? left, char[]? right, StringComparison comparison)
        {
            return MemoryExtensions.Equals(left, right, comparison);
        }

#endregion

#region Type

        /// <summary>
        /// Are the <paramref name="left"/> and <paramref name="right"/> <see cref="System.Type"/>s exactly the same? 
        /// </summary>
        public static bool Type(Type? left, Type? right) => left == right;

        /// <summary>
        /// Are the <see cref="System.Type"/>s of <paramref name="left"/> and <paramref name="right"/> exactly the same?
        /// </summary>
        public static bool Type<TL, TR>(TL? left, TR? right) => typeof(TL) == typeof(TR);

        /// <summary>
        /// Are the <see cref="System.Type"/>s of <paramref name="left"/> and <paramref name="right"/> exactly the same?
        /// </summary>
        public static bool Type(object? left, object? right) => left?.GetType() == right?.GetType();

#endregion
    }
}

