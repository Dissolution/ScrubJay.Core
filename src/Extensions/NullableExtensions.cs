// ReSharper disable ConvertNullableToShortForm

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Nullable{T}"/>
/// </summary>
public static class NullableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue<T>(this Nullable<T> nullable, out T value)
        where T : struct
    {
        value = nullable.GetValueOrDefault();
        return nullable.HasValue;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> ToOption<T>(this Nullable<T> nullable)
        where T : struct
    {
        if (nullable.HasValue)
        {
            return Option<T>.Some(nullable.Value);
        }
        return Option<T>.None;
    }
}