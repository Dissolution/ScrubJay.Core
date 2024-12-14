namespace ScrubJay.Buffers;

/// <summary>
/// Static methods for creating <see cref="ObjectPool{T}"/> instances
/// </summary>
[PublicAPI]
public static class ObjectPool
{
    /// <summary>
    /// The default capacity for an <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <remarks>
    /// This defaults to twice the number of processors
    /// </remarks>
    public static readonly int DefaultCapacity = 2 * Environment.ProcessorCount;

    /// <summary>
    /// The minimum capacity for any <see cref="ObjectPool{T}"/>
    /// </summary>
    public const int MinCapacity = 1;

    /// <summary>
    /// The maximum capacity for any <see cref="ObjectPool{T}"/>
    /// </summary>
    public const int MaxCapacity = 0X7FFFFFC7; // == Array.MaxLength

    public static ObjectPool<T> New<T>()
        where T : class
        => New<T>(ObjectPoolPolicy.Default<T>());

    public static ObjectPool<T> New<T>(ObjectPoolPolicy<T> poolPolicy)
        where T : class
        => new ObjectPool<T>(poolPolicy);

    public static ObjectPool<T> New<T>(Action<ObjectPoolPolicyBuilder<T>> buildPolicy)
        where T : class
    {
        var policy = new ObjectPoolPolicy<T>();
        var builder = new ObjectPoolPolicyBuilder<T>(policy);
        buildPolicy(builder);
        return new ObjectPool<T>(policy);
    }
}
