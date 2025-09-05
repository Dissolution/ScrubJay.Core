namespace ScrubJay.Functional;

[PublicAPI]
public static class EnumerableExtensions
{
    public static IEnumerable<O> SelectWhere<I, O>(
        this IEnumerable<I> enumerable,
        Func<I, Option<O>> selectWhere)
    {
        foreach (I input in enumerable)
        {
            if (selectWhere(input).IsSome(out var output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<O> SelectWhere<I, O>(
        this IEnumerable<I> enumerable,
        Func<I, Result<O>> selectWhere)
    {
        foreach (I input in enumerable)
        {
            if (selectWhere(input).IsOk(out var output))
            {
                yield return output;
            }
        }
    }

    public static IEnumerable<O> SelectWhere<I, O, E>(
        this IEnumerable<I> enumerable,
        Func<I, Result<O, E>> selectWhere)
    {
        foreach (I input in enumerable)
        {
            if (selectWhere(input).IsOk(out var output))
            {
                yield return output;
            }
        }
    }
}