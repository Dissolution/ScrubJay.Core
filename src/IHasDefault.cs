namespace ScrubJay;

/// <summary>
/// Indicates that this type provides a <c>static</c> <see cref="Default"/> instance
/// </summary>
/// <typeparam name="S">
/// <c>typeof(self)</c>
/// </typeparam>
[PublicAPI]
public interface IHasDefault<out S>
    where S : IHasDefault<S>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Get the <c>default</c> <typeparamref name="S"/> instance
    /// </summary>
    static abstract S Default { get; }
#endif
}
