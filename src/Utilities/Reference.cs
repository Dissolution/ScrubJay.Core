#pragma warning disable CA1045

namespace ScrubJay.Utilities;

/// <summary>
/// Utility for working with references
/// </summary>
[PublicAPI]
public static class Reference
{
    /// <summary>
    /// Replace the <typeparamref name="T"/> in <paramref name="location"/> with
    /// <paramref name="value"/> and return the <typeparamref name="T"/> that was in <paramref name="location"/>
    /// </summary>
    /// <remarks>
    /// This is a non-locking version of <see cref="Interlocked"/>'s <c>Exchange</c> methods
    /// </remarks>
    [return: NotNullIfNotNull(nameof(location))]
    public static T Exchange<T>(ref T location, T value)
    {
        var original = location;
        location = value;
        return original;
    }
}
