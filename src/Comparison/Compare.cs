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

    public static IComparer<T> GetComparer<T>() => Comparer<T>.Default;

    public static IComparer GetComparerFor(object? obj)
    {
        if (obj is null)
            return ObjectComparer.Default;
        return GetComparer(obj.GetType());
    }

    public static IComparer<T> GetComparerFor<T>(T? _) => Comparer<T>.Default;

    public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
        => Comparer<T>.Create((x, y) => compare(x, y));

    public static IComparer<T> CreateComparer<T>(Comparison<T> comparison)
        => Comparer<T>.Create((x, y) => comparison(x, y));


    public static int Values<T>(T? left, T? right) => Comparer<T>.Default.Compare(left!, right!);

    public static int Values<TLeft, TRight>(TLeft? left, TRight? right)
        where TLeft : IComparable<TRight>
    {
        if (left is null)
        {
            // null sorts first
            return right is null ? 0 : -1;
        }
        return left.CompareTo(right!);
    }

    public static int Objects(object? left, object? right) => ObjectComparer.Compare(left, right);

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