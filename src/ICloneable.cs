namespace ScrubJay;

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
