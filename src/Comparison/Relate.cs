using System.Reflection;

namespace ScrubJay.Comparison;

/// <summary>
/// A helper for comparison
/// </summary>
[PublicAPI]
public static class Relate
{
    /// <summary>
    /// A cache of T:IComparer&lt;T&gt; instances
    /// </summary>
    private static readonly ConcurrentTypeMap<object> _comparers = new()
    {
        [typeof(object)] = ObjectComparer.Default,
    };

    private static object GetComparerInstance(Type type)
    {
        if (type.IsByRef
#if NETSTANDARD2_1 || NET8_0_OR_GREATER
            || type.IsByRefLike)
#else
           )
#endif
        {
            throw new NotImplementedException();
        }

        return typeof(Comparer<>)
            .MakeGenericType(type)
            .GetMethod("get_Default", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
            .ThrowIfNull($"Could not find Comparer<{type:@}>.get_Default method")
            .Invoke(null, null)
            .ThrowIfNot(typeof(IComparer<>).MakeGenericType(type));
    }

    public static IComparer<T> GetComparer<T>()
    {
        return _comparers.GetOrAdd<T>(GetComparerInstance)
            .ThrowIfNot<IComparer<T>>();
    }

    public static IComparer GetComparer(Type type)
    {
        Throw.IfNull(type);
        return _comparers.GetOrAdd(type, GetComparerInstance)
            .ThrowIfNot<IComparer>();
    }

    public static IComparer<T> CreateComparer<T>(Comparison<T> compare)
        => Comparer<T>.Create(compare);

    public static IComparer<T> CreateComparer<T>(Fn<T?, T?, int> compare)
        => Comparer<T>.Create((l, r) => compare(l, r));


    public static int Values<T>(T? left, T? right)
        => GetComparer<T>().Compare(left!, right!);

    public static int Values<T>(T? left, T? right, IComparer<T>? valueComparer)
    {
        if (valueComparer is null)
            return Values<T>(left, right);
        return valueComparer.Compare(left!, right!);
    }

    public static int Objects(object? left, object? right)
        => ObjectComparer.Default.Relate(left, right);
}