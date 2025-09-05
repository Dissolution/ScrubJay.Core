// ReSharper disable ConvertNullableToShortForm
namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Nullable{T}"/>
/// </summary>
[PublicAPI]
public static class NullableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue<T>(this Nullable<T> nullable, out T value)
        where T : struct
    {
        // fastest path to value, no checks
        value = nullable.GetValueOrDefault();
        return nullable.HasValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> ToOption<T>(this Nullable<T> nullable)
        where T : struct
    {
        if (nullable.HasValue)
            return Some<T>(nullable.GetValueOrDefault());
        return None;
    }
}