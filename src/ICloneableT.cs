#pragma warning disable MA0048

namespace ScrubJay;

/// <summary>
/// Supports generic cloning, which creates a new <typeparamref name="TSelf"/> instance with the same value as an existing instance
/// </summary>
/// <typeparam name="TSelf">
/// The <see cref="Type"/> of instance that can be cloned
/// </typeparam>
public interface ICloneable<out TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
    /// <summary>
    /// Creates a new copy of the current instance
    /// </summary>
    /// <returns>
    /// A new <typeparamref name="TSelf"/> instance
    /// </returns>
    new TSelf Clone();

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
    /// <inheritdoc />
    object ICloneable.Clone()
    {
        return (object)Clone();
    }
#endif
}