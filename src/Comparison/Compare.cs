using System.Reflection;
using ScrubJay.Collections;

// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Comparison;

/// <summary>
/// A helper for all Order Comparison
/// </summary>
[PublicAPI]
public static class Compare
{
    private static readonly ConcurrentTypeMap<IComparer> _comparers = new()
    {
        [typeof(object)] = ObjectComparer.Default,
    };

    private static IComparer FindComparer(Type type)
    {
        return typeof(Comparer<>)
            .MakeGenericType(type)
            .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
            .ThrowIfNull()
            .GetValue(null)
            .ThrowIfNot<IComparer>();
    }

    public static IComparer GetComparer(Type type)
    {
        return _comparers.GetOrAdd(type, static t => FindComparer(t));
    }

    public static IComparer<T> GetComparer<T>()
    {
        return _comparers.GetOrAdd<T>(static _ => Comparer<T>.Default).ThrowIfNot<IComparer<T>>();
    }

    public static IComparer GetComparerFor(object? obj)
    {
        if (obj is null)
            return ObjectComparer.Default;
        return GetComparer(obj.GetType());
    }
    
    public static IComparer<T> GetComparerFor<T>(T? _) => GetComparer<T>();

    public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
        => Comparer<T>.Create((x, y) => compare(x, y));
    

    public static int Values<T>(T? left, T? right) => GetComparer<T>().Compare(left!, right!);

    public static int Objects(object? left, object? right) => ObjectComparer.Compare(left, right);
        
        

#region Sequence
    /* T[], Span<T>, ReadOnlySpan<T>, IEnumerable<T> */
    
#region IComparable
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(T[]? left, T[]? right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(Span<T> left, T[]? right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(ReadOnlySpan<T> left, T[]? right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(T[]? left, Span<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(Span<T> left, Span<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(ReadOnlySpan<T> left, Span<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(T[]? left, ReadOnlySpan<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(Span<T> left, ReadOnlySpan<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComparableSequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        where T : IComparable<T>
    {
        return MemoryExtensions.SequenceCompareTo<T>(left, right);
    }
#endregion

#region Open
    public static int Sequence<T>(T[]? left, T[]? right)
    {
        return SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right));
    }

    public static int Sequence<T>(Span<T> left, T[]? right)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right));
    }

    public static int Sequence<T>(ReadOnlySpan<T> left, T[]? right)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right));
    }

    public static int Sequence<T>(T[]? left, Span<T> right)
    {
        return SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right);
    }

    public static int Sequence<T>(Span<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right);
    }

    public static int Sequence<T>(ReadOnlySpan<T> left, Span<T> right)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right);
    }

    public static int Sequence<T>(T[]? left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right);
    }

    public static int Sequence<T>(Span<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right);
    }

    public static int Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right);
    }
#endregion

#region w/Comparer
    public static int Sequence<T>(T[]? left, T[]? right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), new ReadOnlySpan<T>(right), comparer);
    }

    public static int Sequence<T>(Span<T> left, T[]? right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right), comparer);
    }

    public static int Sequence<T>(ReadOnlySpan<T> left, T[]? right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, new ReadOnlySpan<T>(right), comparer);
    }

    public static int Sequence<T>(T[]? left, Span<T> right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right, comparer);
    }

    public static int Sequence<T>(Span<T> left, Span<T> right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
    }

    public static int Sequence<T>(ReadOnlySpan<T> left, Span<T> right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
    }

    public static int Sequence<T>(T[]? left, ReadOnlySpan<T> right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right, comparer);
    }

    public static int Sequence<T>(Span<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
    }

    public static int Sequence<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, IComparer<T>? comparer)
    {
        return SpanExtensions.SequenceCompareTo<T>(left, right, comparer);
    }
#endregion

#region IEnumerable
    public static int Sequence<T>(T[]? left, IEnumerable<T>? right, IComparer<T>? comparer = default)
        => SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(left), right, comparer);

    public static int Sequence<T>(Span<T> left, IEnumerable<T>? right, IComparer<T>? comparer = default)
        => SpanExtensions.SequenceCompareTo<T>(left, right, comparer);

    public static int Sequence<T>(ReadOnlySpan<T> left, IEnumerable<T>? right, IComparer<T>? comparer = default)
        => SpanExtensions.SequenceCompareTo<T>(left, right, comparer);

    public static int Sequence<T>(IEnumerable<T>? left, T[]? right, IComparer<T>? comparer = default)
        => SpanExtensions.SequenceCompareTo<T>(new ReadOnlySpan<T>(right), left, comparer);

    public static int Sequence<T>(IEnumerable<T>? left, Span<T> right, IComparer<T>? comparer = default)
        => SpanExtensions.SequenceCompareTo<T>(right, left, comparer);

    public static int Sequence<T>(IEnumerable<T>? left, ReadOnlySpan<T> right, IComparer<T>? comparer = default)
        => SpanExtensions.SequenceCompareTo<T>(right, left, comparer);

    public static int Sequence<T>(IEnumerable<T>? left, IEnumerable<T>? right, IComparer<T>? comparer = default) 
        => EnumerableExtensions.SequenceCompareTo<T>(left, right, comparer);
#endregion
#endregion

#region Text
    public static int Text(string? left, string? right)
    {
        return string.CompareOrdinal(left, right);
    }

    public static int Text(string? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left.AsSpan(), right);
    }

    public static int Text(string? left, char[]? right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left.AsSpan(), right);
    }

    public static int Text(ReadOnlySpan<char> left, string? right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left, right.AsSpan());
    }

    public static int Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left, right);
    }

    public static int Text(ReadOnlySpan<char> left, char[]? right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left, right);
    }

    public static int Text(char[]? left, string? right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left, right.AsSpan());
    }

    public static int Text(char[]? left, ReadOnlySpan<char> right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left, right);
    }

    public static int Text(char[]? left, char[]? right)
    {
        return MemoryExtensions.SequenceCompareTo<char>(left, right);
    }
#endregion

#region Text w/StringComparison
    public static int Text(string? left, string? right, StringComparison comparison)
    {
        return string.Compare(left, right, comparison);
    }

    public static int Text(string? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left.AsSpan(), right, comparison);
    }

    public static int Text(string? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left.AsSpan(), right, comparison);
    }

    public static int Text(ReadOnlySpan<char> left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left, right.AsSpan(), comparison);
    }

    public static int Text(ReadOnlySpan<char> left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left, right, comparison);
    }

    public static int Text(ReadOnlySpan<char> left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left, right, comparison);
    }

    public static int Text(char[]? left, string? right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left, right.AsSpan(), comparison);
    }

    public static int Text(char[]? left, ReadOnlySpan<char> right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left, right, comparison);
    }

    public static int Text(char[]? left, char[]? right, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(left, right, comparison);
    }
#endregion
}