#pragma warning disable CS1574, CS1584, CS1581, CS1580
#pragma warning disable CA1040, CA1716

namespace ScrubJay;

/// <summary>
/// Indicates that this type provides a static <see cref="Default"/> instance
/// </summary>
/// <typeparam name="S">
/// <c>self</c>
/// </typeparam>
[PublicAPI]
public interface IHasDefault<out S>
    where S : IHasDefault<S>
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Get the default <typeparamref name="S"/> instance
    /// </summary>
    static abstract S Default { get; }
#endif
}
