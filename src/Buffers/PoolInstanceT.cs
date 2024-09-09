namespace ScrubJay.Buffers;

[PublicAPI]
public sealed class PoolInstance<T> : IDisposable
    where T : class
{
    private readonly ObjectPool<T> _pool;
    private T? _instance;

    /// <summary>
    /// Gets the instance that this <see cref="PoolInstance{T}"/> manages
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if this <see cref="PoolInstance{T}"/> has been <see cref="Dispose">Disposed</see>
    /// </exception>
    public T Instance => _instance ?? throw new ObjectDisposedException(this.GetType().Name);

    /// <summary>
    /// Has this <see cref="PoolInstance{T}"/> been disposed?
    /// </summary>
    public bool IsDisposed => _instance is null;
        
    internal PoolInstance(ObjectPool<T> pool, T instance)
    {
        Debug.Assert(instance is not null);
        _pool = pool;
        _instance = instance;
    }

    /// <summary>
    /// Disposes this <see cref="PoolInstance{T}"/> by returning <see cref="Instance"/> to its source <see cref="ObjectPool{T}"/>
    /// </summary>
    public void Dispose()
    {
        T? instance = Interlocked.Exchange(ref _instance, null);
        _pool.Return(instance);
    }
}