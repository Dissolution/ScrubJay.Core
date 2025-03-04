using System.Reflection;
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
        if (type is null)
            return ObjectComparer.Default;
        return _equalityComparers.GetOrAdd(type, static t => FindEqualityComparer(t));
    }

    public static IEqualityComparer<T> GetEqualityComparer<T>() => EqualityComparer<T>.Default;


    public static IEqualityComparer GetEqualityComparerFor(object? obj) => GetEqualityComparer(obj?.GetType());

    public static IEqualityComparer<T> GetEqualityComparerFor<T>(T? _) => EqualityComparer<T>.Default;

    public static IEqualityComparer<T> CreateEqualityComparer<T>(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
        => new FuncEqualityComparer<T>(equals, getHashCode);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Values<T>(T? left, T? right) => EqualityComparer<T>.Default.Equals(left!, right!);

    public static bool Values<T>(T? left, T? right, IEqualityComparer<T>? comparer)
    {
        if (comparer is null)
            return Values<T>(left, right);
        return comparer.Equals(left!, right!);
    }

    public static bool EquatableValues<TLeft, TRight>(TLeft? left, TRight? right)
        where TLeft : IEquatable<TRight>
    {
        if (left is null)
            return right is null;
        return left.Equals(right!);
    }



    public static bool Objects(object? left, object? right) => ObjectComparer.Default.Equals(left, right);





#region Type
    /// <summary>
    /// Are the <see cref="System.Type"/> parameters <paramref name="left"/> and <paramref name="right"/> equal?
    /// </summary>
    public static bool Types(Type? left, Type? right) => left == right;

    /// <summary>
    /// Are the generic <see cref="System.Type">Types</see> <typeparamref name="TL"/> and <typeparamref name="TR"/> equal?
    /// </summary>
    public static bool TypesOf<TL, TR>(TL? _ = default, TR? __ = default) => typeof(TL) == typeof(TR);

    /// <summary>
    /// Are the <see cref="System.Type"/>s of <paramref name="left"/> and <paramref name="right"/> equal?
    /// </summary>
    public static bool TypesOf(object? left, object? right) => left?.GetType() == right?.GetType();
#endregion
}
