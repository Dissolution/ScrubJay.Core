// ReSharper disable TypeParameterCanBeVariant

namespace ScrubJay;

/// <summary>
/// Indicates that instances of this type support shallow-cloning:<br/>
/// creating a new instance with the same values as an existing instance
/// <i>by</i> preserving <i>all</i> references
/// </summary>
/// <typeparam name="S">
/// <c>typeof(self)</c>
/// </typeparam>
[PublicAPI]
public interface ICloneable<S>
    where S : ICloneable<S>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    /// <summary>
    /// Creates a shallow clone of this instance
    /// </summary>
    S Clone();
}