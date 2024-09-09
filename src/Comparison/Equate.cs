﻿using System.Reflection;
using ScrubJay.Collections;

// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Comparison;

/// <summary>
/// A helper for all Equality Comparison
/// </summary>
[PublicAPI]
public static class Equate
{
    private static readonly ConcurrentTypeMap<IEqualityComparer> _equalityComparers = new()
    {
        [typeof(object)] = ObjectComparer.Default,
    };

    private static IEqualityComparer FindEqualityComparer(Type type)
    {
        return typeof(EqualityComparer<>)
            .MakeGenericType(type)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull()
            .GetValue(null)
            .ThrowIfNot<IEqualityComparer>();
    }

    public static IEqualityComparer GetEqualityComparer(Type? type)
    {
        if (type is null) return ObjectComparer.Default;
        return _equalityComparers.GetOrAdd(type, static t => FindEqualityComparer(t));
    }

    public static IEqualityComparer<T> GetEqualityComparer<T>()
    {
        return _equalityComparers.GetOrAdd<T>(static _ => EqualityComparer<T>.Default).ThrowIfNot<IEqualityComparer<T>>();
    }
    
    
    public static IEqualityComparer GetEqualityComparerFor(object? obj) => GetEqualityComparer(obj?.GetType());
    
    public static IEqualityComparer<T> GetEqualityComparerFor<T>(T? _) => GetEqualityComparer<T>();

    public static IEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
        => new FuncEqualityComparer<T>(equals, getHashCode);

    
    
    public static bool Values<T>(T? left, T? right) => GetEqualityComparer<T>().Equals(left!, right!);

    public static bool Objects(object? left, object? right) => ObjectComparer.Equals(left, right);

#region Sequence
#region IEquatable
    public static bool EquatableSequence<T>(T[]? left, T[]? right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right.AsSpan());
    }

    public static bool EquatableSequence<T>(Span<T> left, T[]? right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan());
    }

    public static bool EquatableSequence<T>(ReadOnlySpan<T> left, T[]? right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, right.AsSpan());
    }

    public static bool EquatableSequence<T>(T[]? left, Span<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
    }

    public static bool EquatableSequence<T>(Span<T> left, Span<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool EquatableSequence<T>(ReadOnlySpan<T> left, Span<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool EquatableSequence<T>(T[]? left, ReadOnlySpan<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left.AsSpan(), right);
    }

    public static bool EquatableSequence<T>(Span<T> left, ReadOnlySpan<T> right)
        where T : IEquatable<T>
    {
        return MemoryExtensions.SequenceEqual<T>(left, right);
    }

    public static bool EquatableSequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
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
    public static bool Sequence<T>(T[]? left, IEnumerable<T>? right, IEqualityComparer<T>? comparer = default)
        => SpanExtensions.SequenceEqual<T>(left, right, comparer);

    public static bool Sequence<T>(Span<T> left, IEnumerable<T>? right, IEqualityComparer<T>? comparer = default)
        => SpanExtensions.SequenceEqual<T>(left, right, comparer);

    public static bool Sequence<T>(ReadOnlySpan<T> left, IEnumerable<T>? right, IEqualityComparer<T>? comparer = default)
        => SpanExtensions.SequenceEqual<T>(left, right, comparer);

    public static bool Sequence<T>(IEnumerable<T>? left, T[]? right, IEqualityComparer<T>? comparer)
        => SpanExtensions.SequenceEqual<T>(right, left, comparer);

    public static bool Sequence<T>(IEnumerable<T>? left, Span<T> right, IEqualityComparer<T>? comparer)
        => SpanExtensions.SequenceEqual<T>(right, left, comparer);

    public static bool Sequence<T>(IEnumerable<T>? left, ReadOnlySpan<T> right, IEqualityComparer<T>? comparer)
        => SpanExtensions.SequenceEqual<T>(right, left, comparer);

    public static bool Sequence<T>(IEnumerable<T>? left, IEnumerable<T>? right, IEqualityComparer<T>? comparer = default)
    {
        if (left is null)
            return right is null;
        if (right is null)
            return false;
        return Enumerable.SequenceEqual(left, right, comparer);
    }
#endregion
#endregion

#region Text
    public static bool Text(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.Ordinal);
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