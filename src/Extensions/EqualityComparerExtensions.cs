﻿namespace ScrubJay.Extensions;

public static class EqualityComparerExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNullSafeHashCode<T>(this IEqualityComparer<T> equalityComparer, [AllowNull] T value)
    {
        if (value is null)
            return 0;
        return equalityComparer.GetHashCode(value);
    }
}