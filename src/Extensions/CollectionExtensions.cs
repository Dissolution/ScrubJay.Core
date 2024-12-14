namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Returns <c>true</c> if <paramref name="collection"/> is <c>null</c> or has a Count of 0
    /// </summary>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this ICollection<T>? collection)
        => collection is null || collection.Count == 0;

    /// <summary>
    /// Returns <c>true</c> if <paramref name="collection"/> is <c>null</c> or has a Count of 0
    /// </summary>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IReadOnlyCollection<T>? collection)
        => collection is null || collection.Count == 0;
}
