namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Func{T,R}">Func&lt;T, bool&gt;</see> predicates
/// </summary>
[PublicAPI]
public static class PredicateExtensions
{
    public static Func<T, bool> Not<T>(this Func<T, bool> predicate)
        => value => !predicate(value);

    public static Func<T, bool> And<T>(this Func<T, bool> left, Func<T, bool> right)
        => value => left(value) && right(value);

    public static Func<T, bool> Or<T>(this Func<T, bool> left, Func<T, bool> right)
        => value => left(value) || right(value);

    public static Func<T, bool> Xor<T>(this Func<T, bool> left, Func<T, bool> right)
        => value => left(value) ^ right(value);
}
