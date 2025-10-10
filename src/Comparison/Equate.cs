using System.Reflection;

namespace ScrubJay.Comparison;

/// <summary>
/// A helper utility for equality comparison
/// </summary>
[PublicAPI]
public static class Equate
{
    /// <summary>
    /// A cache of T:IEqualityComparer&lt;T&gt; instances
    /// </summary>
    private static readonly ConcurrentTypeMap<object> _equalityComparers = new()
    {
        [typeof(object)] = ObjectRelater.Default,
    };

    private static object GetEqualityComparerInstance(Type type)
    {
        // Return EqualityComparer<T>.Default
        return typeof(EqualityComparer<>)
            .MakeGenericType(type)
            .GetMethod("get_Default", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
            .ThrowIfNull($"Could not find `EqualityComparer<{type:@}>.get_Default` method")
            .Invoke(null, null)
            .ThrowIfNot(typeof(IEqualityComparer<>).MakeGenericType(type));
    }

    public static IEqualityComparer GetComparer(Type type)
    {
        Throw.IfNull(type);
        return _equalityComparers
            .GetOrAdd(type, GetEqualityComparerInstance)
            .ThrowIfNot<IEqualityComparer>();
    }

    public static IEqualityComparer<T> GetComparer<T>()
    {
        return _equalityComparers
            .GetOrAdd<T>(GetEqualityComparerInstance)
            .ThrowIfNot<IEqualityComparer<T>>();
    }



    /// <summary>
    /// Creates an <see cref="IEqualityComparer{T}"/> that uses functions to determine equality
    /// </summary>
    /// <param name="equals">
    /// The <c>Func&lt;T?, T?, bool&gt;</c> that determines if two <typeparamref name="T"/> instances are equal
    /// </param>
    /// <param name="getHashCode">
    /// The <c>Func&lt;T?, int&gt;</c> that determines a <typeparamref name="T"/> instance's hash code
    /// </param>
    public static IEqualityComparer<T> CreateComparer<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => new FuncEqualityComparer<T>(equals, getHashCode);

    public static bool Values<T>(T? left, T? right)
        => GetComparer<T>().Equals(left!, right!);

    public static bool Values<T>(T? left, T? right, IEqualityComparer<T>? comparer)
    {
        return (comparer ?? GetComparer<T>()).Equals(left!, right!);
    }

    public static bool Objects(object? left, object? right)
        => ObjectRelater.Default.Equate(left, right);
}