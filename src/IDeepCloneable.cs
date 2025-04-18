// ReSharper disable TypeParameterCanBeVariant
namespace ScrubJay;

/// <summary>
/// Supports deep-cloning, which creates a new instance with the same value as an existing instance,
/// preserving no references of any kind
/// </summary>
/// <typeparam name="S"></typeparam>
/// <remarks>
/// This is opposed to <see cref="ICloneable"/> which only supports boxed, shallow-clones of instances,
/// which do preserve references
/// </remarks>
[PublicAPI]
public interface IDeepCloneable<S>
    where S : IDeepCloneable<S>
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Get a deep clone of a <typeparamref name="S"/> <paramref name="instance"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(instance))]
    public static virtual S? DeepCloneInstance(S? instance)
        => instance is null ? default : instance.DeepClone();
#endif

    /// <summary>
    /// Gets a deep clone of this instance
    /// </summary>
    S DeepClone();
}
