#pragma warning disable CS1574, CS1584, CS1581, CS1580
#pragma warning disable CA1040, CA1716

namespace ScrubJay;

/// <summary>
/// Indicates that this type has a specified <see cref="Default"/> value
/// </summary>
/// <typeparam name="TSelf">
/// <c>self</c>
/// </typeparam>
public interface IHasDefault<out TSelf>
    where TSelf : IHasDefault<TSelf>
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Get the default <typeparamref name="TSelf"/>
    /// </summary>
    static abstract TSelf Default { get; }
#endif
}
