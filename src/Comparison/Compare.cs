using System.Reflection;

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
        => _comparers.GetOrAdd(type, static t => FindComparer(t));

    public static IComparer<T> GetComparer<T>()
        => Comparer<T>.Default;

    public static IComparer GetComparerFor(object? obj)
    {
        if (obj is null)
            return ObjectComparer.Default;
        return GetComparer(obj.GetType());
    }

    public static IComparer<T> GetComparerFor<T>(T? _)
        => Comparer<T>.Default;

    public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
        => Comparer<T>.Create((x, y) => compare(x, y));

    public static IComparer<T> CreateComparer<T>(Comparison<T> comparison)
        => Comparer<T>.Create((x, y) => comparison(x, y));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Values<T>(T? left, T? right) => Comparer<T>.Default.Compare(left!, right!);

    public static int Values<T>(T? left, T? right, IComparer<T>? valueComparer)
    {
        if (valueComparer is null)
            return Values<T>(left, right);
        return valueComparer.Compare(left!, right!);
    }

    public static int ComparableValues<L, R>(L? left, R? right)
        where L : IComparable<R>
    {
        if (left is null)
        {
            // null sorts first
            return right is null ? 0 : -1;
        }
        return left.CompareTo(right!);
    }

    public static int Objects(object? left, object? right) => ObjectComparer.Default.Compare(left, right);
}
