// ReSharper disable TypeParameterCanBeVariant

namespace ScrubJay;

/// <summary>
/// Indicates that instances of this type support deep-cloning:<br/>
/// creating a new instance with the same values as an existing instance
/// <i>without</i> preserving any references
/// </summary>
/// <typeparam name="S">
/// <c>typeof(self)</c>
/// </typeparam>
[PublicAPI]
public interface IDeepCloneable<S>
    where S : IDeepCloneable<S>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    /// <summary>
    /// Create a deep clone of this instance
    /// </summary>
    S DeepClone();
}