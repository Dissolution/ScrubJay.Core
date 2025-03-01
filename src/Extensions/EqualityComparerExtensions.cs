namespace ScrubJay.Extensions;

[PublicAPI]
public static class EqualityComparerExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNullSafeHashCode<T>(this IEqualityComparer<T> equalityComparer, [AllowNull] T value)
    {
        if (value is null)
            return Hasher.NullHash;
        return equalityComparer.GetHashCode(value);
    }
}
