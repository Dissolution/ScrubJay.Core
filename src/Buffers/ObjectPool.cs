namespace ScrubJay.Buffers;

/// <summary>
/// Static methods for creating <see cref="ObjectPool{T}"/> instances
/// </summary>
[PublicAPI]
public static class ObjectPool
{
    /// <summary>
    /// The default capacity for a pool
    /// </summary>
    /// <remarks>
    /// The first instance, plus a pool twice the size of the number of processors
    /// </remarks>
    internal static readonly int DefaultCapacity = 1 + (2 * Environment.ProcessorCount);

    public const int MinCapacity = 1;
    
    /// <summary>
    /// The maximum capacity for a pool
    /// </summary>
    public const int MaxCapacity = 0X7FFFFFC7; // == Array.MaxLength
    
    
    
    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <typeparam name="T">
    /// An instance <c>class</c> type
    /// </typeparam>
    /// <param name="factory">
    /// A <see cref="Func{T}"/> to create new <typeparamref name="T"/> instances
    /// </param>
    /// <param name="clean">
    /// An optional <see cref="Action{T}"/> to perform on <typeparamref name="T"/> instances when they are returned
    /// </param>
    /// <param name="dispose">
    /// An optional <see cref="Action{T}"/> to perform on <typeparamref name="T"/> instances when they are discarded
    /// </param>
    /// <returns>
    /// A newly configured <see cref="ObjectPool{T}"/>
    /// </returns>
    public static ObjectPool<T> Create<T>(
        Func<T> factory,
        Action<T>? clean = null,
        Action<T>? dispose = null)
        where T : class
    {
        return new ObjectPool<T>(factory, clean, dispose);
    }
    
    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> that creates <c>new</c> <typeparamref name="T"/> instances
    /// </summary>
    /// <typeparam name="T">
    /// An instance <c>class</c> type
    /// </typeparam>
    /// <param name="clean">
    /// An optional <see cref="Action{T}"/> to perform on <typeparamref name="T"/> instances when they are returned
    /// </param>
    /// <param name="dispose">
    /// An optional <see cref="Action{T}"/> to perform on <typeparamref name="T"/> instances when they are discarded
    /// </param>
    /// <returns>
    /// A newly configured <see cref="ObjectPool{T}"/>
    /// </returns>
    public static ObjectPool<T> Create<T>(
        Action<T>? clean = null,
        Action<T>? dispose = null, 
        // ReSharper disable once InvalidXmlDocComment
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        Constraints.IsNew<T> _ = default)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        where T : class, new()
    {
        return new ObjectPool<T>(static () => new(), clean, dispose);
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> that automatically disposes <typeparamref name="T"/> instances
    /// </summary>
    /// <typeparam name="T">
    /// An instance <c>class</c> type
    /// </typeparam>
    /// <param name="factory">
    /// A <see cref="Func{T}"/> to create new <typeparamref name="T"/> instances
    /// </param>
    /// <param name="clean">
    /// An optional <see cref="Action{T}"/> to perform on <typeparamref name="T"/> instances when they are returned
    /// </param>
    /// <returns>
    /// A newly configured <see cref="ObjectPool{T}"/>
    /// </returns>
    public static ObjectPool<T> Create<T>(
        Func<T> factory,
        Action<T>? clean = null, 
        // ReSharper disable once InvalidXmlDocComment
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        Constraints.IsDisposable<T> _ = default)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        where T : class, IDisposable
    {
        return new ObjectPool<T>(factory, clean, static item => item.Dispose());
    }

    /// <summary>
    /// Creates a new <see cref="ObjectPool{T}"/> that creates <c>new</c>, automatically disposed <typeparamref name="T"/> instances
    /// </summary>
    /// <typeparam name="T">
    /// An instance <c>class</c> type
    /// </typeparam>
    /// <param name="clean">
    /// An optional <see cref="Action{T}"/> to perform on <typeparamref name="T"/> instances when they are returned
    /// </param>
    /// <returns>
    /// A newly configured <see cref="ObjectPool{T}"/>
    /// </returns>
    public static ObjectPool<T> Create<T>(
        Action<T>? clean = null, 
        // ReSharper disable once InvalidXmlDocComment
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        Constraints.IsDisposableNew<T> _ = default)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        where T : class, IDisposable, new()
    {
        return new ObjectPool<T>(static () => new(), clean, static item => item.Dispose());
    }
}