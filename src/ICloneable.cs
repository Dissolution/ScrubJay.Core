﻿// ReSharper disable TypeParameterCanBeVariant

namespace ScrubJay;

/// <summary>
/// Supports shallow-cloning, which creates a new instance with the same value as an existing instance,
/// preserving all references
/// </summary>
/// <typeparam name="S">
/// The <see cref="Type"/> of this instance (self)
/// </typeparam>
[PublicAPI]
public interface ICloneable<S> : ICloneable
    where S : ICloneable<S>
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Get a shallow clone of a <typeparamref name="S"/> <paramref name="instance"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(instance))]
    public static virtual S? CloneInstance(S? instance)
        => instance is null ? default : instance.Clone();
#endif

#if !NETFRAMEWORK && !NETSTANDARD2_0
    object ICloneable.Clone() => Clone();
#endif

    /// <summary>
    /// Gets a shallow clone of this instance
    /// </summary>
    new S Clone();
}
