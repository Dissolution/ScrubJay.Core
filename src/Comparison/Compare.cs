using System.Reflection;

namespace ScrubJay.Comparison;

/// <summary>
/// A helper for comparison
/// </summary>
[PublicAPI]
public static class Compare
{
    /// <summary>
    /// A cache of T:IComparer&lt;T&gt; instances
    /// </summary>
    private static readonly ConcurrentTypeMap<object> _comparers = new()
    {
        [typeof(object)] = ObjectRelater.Default,
    };

    private static object GetComparerInstance(Type type)
    {
        return typeof(Comparer<>)
            .MakeGenericType(type)
            .GetMethod("get_Default", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
            .ThrowIfNull($"Could not find Comparer<{type:@}>.get_Default method")
            .Invoke(null, null)
            .ThrowIfNot(typeof(IComparer<>).MakeGenericType(type));
    }


    public static IComparer GetComparer(Type type)
    {
        Throw.IfNull(type);
        return _comparers
            .GetOrAdd(type, GetComparerInstance)
            .ThrowIfNot<IComparer>();
    }


    public static IComparer<T> GetComparer<T>()
    {
        return _comparers
            .GetOrAdd<T>(GetComparerInstance)
            .ThrowIfNot<IComparer<T>>();
    }

    public static IComparer<T> CreateComparer<T>(Comparison<T> compare)
        => Comparer<T>.Create(compare);

    public static IComparer<T> CreateComparer<T>(Func<T?, T?, int> compare)
        => Comparer<T>.Create((l, r) => compare(l, r));

    public static int Values<T>(T? left, T? right)
        => Comparer<T>.Default.Compare(left!, right!);

    public static int Values<T>(T? left, T? right, IComparer<T>? comparer)
        => (comparer ?? Comparer<T>.Default).Compare(left!, right!);

    public static int Objects(object? left, object? right)
        => ObjectRelater.Default.Compare(left, right);
}