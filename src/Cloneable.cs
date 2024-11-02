#pragma warning disable MA0048

namespace ScrubJay;

/// <summary>
/// Indicates that an instance is capable of making <b>shallow^</b> clones of itself
/// </summary>
/// <remarks>
/// ^A shallow clone contains the same references as its source instance
/// </remarks>
/// <typeparam name="TSelf">
/// The <see cref="Type"/> of instance that can be shallowly cloned
/// </typeparam>
public interface IShallowCloneable<out TSelf> : ICloneable
    where TSelf : IShallowCloneable<TSelf>
{
    /// <summary>
    /// Creates a new copy of the current instance
    /// </summary>
    /// <returns>
    /// A new <typeparamref name="TSelf"/> instance
    /// </returns>
    TSelf ShallowClone();

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
    /// <inheritdoc />
    object ICloneable.Clone()
    {
        return (object)Clone();
    }
#endif
}

public interface IDeepCloneable<out TSelf>
    where TSelf : IDeepCloneable<TSelf>
{
    TSelf DeepClone();
}