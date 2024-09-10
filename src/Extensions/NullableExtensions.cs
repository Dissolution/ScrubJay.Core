namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Nullable{T}"/>
/// </summary>
[PublicAPI]
public static class NullableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once ConvertNullableToShortForm
    public static bool TryGetValue<T>(this Nullable<T> nullable, out T value)
        where T : struct
    {
        value = nullable.GetValueOrDefault();
        return nullable.HasValue;
    }
}