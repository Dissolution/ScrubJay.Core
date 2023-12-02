namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="IEnumerator{T}"/> and <see cref="IEnumerator"/>
/// </summary>
public static class EnumeratorExtensions
{
    public static bool TryMoveNext<T>(
        this IEnumerator<T> enumerator,
        [MaybeNullWhen(false)] out T? value)
    {
        if (enumerator.MoveNext())
        {
            value = enumerator.Current;
            return true;
        }

        value = default;
        return false;
    }
    
    public static bool TryMoveNext(
        this IEnumerator enumerator,
        [MaybeNullWhen(false)] out object? value)
    {
        if (enumerator.MoveNext())
        {
            value = enumerator.Current;
            return true;
        }

        value = default;
        return false;
    }
}