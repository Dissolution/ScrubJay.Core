namespace ScrubJay.Extensions;

[PublicAPI]
public static class PredicateExtensions
{
    public static Fn<T, bool> Not<T>(this Fn<T, bool> predicate)
        => value => !predicate(value);

    public static Fn<T, bool> And<T>(this Fn<T, bool> left, Fn<T, bool> right)
        => value => left(value) && right(value);

    public static Fn<T, bool> Or<T>(this Fn<T, bool> left, Fn<T, bool> right)
        => value => left(value) || right(value);

    public static Fn<T, bool> Xor<T>(this Fn<T, bool> left, Fn<T, bool> right)
        => value => left(value) ^ right(value);
}
