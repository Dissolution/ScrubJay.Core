﻿// ReSharper disable TypeParameterCanBeVariant
namespace ScrubJay;

/// <summary>
/// Supports deep-cloning, which creates a new instance with the same value as an existing instance,
/// preserving no references of any kind
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <remarks>
/// This is opposed to <see cref="ICloneable"/> which only supports boxed, shallow-clones of instances,
/// which do preserve references
/// </remarks>
[PublicAPI]
public interface IDeepCloneable<TSelf>
    where TSelf : IDeepCloneable<TSelf>
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Get a deep clone of a <typeparamref name="TSelf"/> <paramref name="instance"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(instance))]
    public static virtual TSelf? DeepCloneInstance(TSelf? instance)
        => instance is null ? default : instance.DeepClone();
#endif

    /// <summary>
    /// Gets a deep clone of this instance
    /// </summary>
    TSelf DeepClone();
}

[PublicAPI]
public interface ICloneable<TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Get a shallow clone of a <typeparamref name="TSelf"/> <paramref name="instance"/>
    /// </summary>
    [return: NotNullIfNotNull(nameof(instance))]
    public static virtual TSelf? CloneInstance(TSelf? instance)
        => instance is null ? default : instance.Clone();
#endif

#if !NETFRAMEWORK && !NETSTANDARD2_0
    object ICloneable.Clone() => Clone();
#endif

    /// <summary>
    /// Gets a shallow clone of this instance
    /// </summary>
    new TSelf Clone();
}
