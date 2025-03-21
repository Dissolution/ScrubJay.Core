using System.Collections.Concurrent;

namespace ScrubJay.Pooling;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso href="https://github.dev/dotnet/aspnetcore/blob/main/src/ObjectPool/src/DefaultObjectPool.cs"/>
[PublicAPI]
[MustDisposeResource]
public sealed class InstancePool<T> : IInstancePool<T>, IDisposable
    where T : class
{
    /// <summary>
    /// Whenever an instance is requested but unavailable, this function will create a new instance
    /// </summary>
    private readonly Func<T> _createInstance;

    /// <summary>
    /// <i>Optional</i><br/>
    /// Whenever an instance is returned to the pool, this function will try ready that instance for re-use<br/>
    /// Failure of this function will cause the instance to be disposed instead
    /// </summary>
    private readonly Func<T, bool>? _tryCleanInstance;

    /// <summary>
    /// <i>Optional</i><br/>
    /// Whenever an instance returned to the pool would be discarded, this action will dispose that instance
    /// </summary>
    private readonly Action<T>? _disposeInstance;

    /// <summary>
    /// The first instance is stored in this dedicated field as we expect to be able to satisfy most rents from it
    /// </summary>
    private T? _firstInstance;

    /// <summary>
    /// Storage for the extra pool instances
    /// </summary>
    private ConcurrentQueue<T>? _instances = [];

    private int _itemCount;

    /// <summary>
    /// Gets an approximate count of the instances stored in this pool
    /// </summary>
    public int Count => _itemCount;

    /// <summary>
    /// Gets the maximum number of instances that can be stored in this pool
    /// </summary>
    public int MaxCapacity { get; }

    public InstancePool(InstancePoolPolicy<T> poolPolicy)
    {
        MaxCapacity = poolPolicy.MaxCapacity.Clamp(0, 0x40000000);

        _createInstance = poolPolicy.CreateInstance;
        _tryCleanInstance = poolPolicy.TryCleanInstance;
        _disposeInstance = poolPolicy.DisposeInstance;
    }

    public T Rent()
    {
        // check for disposal
        Throw.IfDisposed(_instances is null, _instances);

        // check first instance and try to take it
        T? instance = _firstInstance;
        if ((instance == null) || (Interlocked.CompareExchange<T?>(ref _firstInstance, null, instance) != instance))
        {
            // There was no instance or we could not take it
            // can we get from instances?
            if (!_instances.TryDequeue(out instance))
            {
                // no instance available, create a new one (item count does not change)
                return _createInstance();
            }
        }

        // we are storing one fewer instance
        Interlocked.Decrement(ref _itemCount);
        return instance;
    }

    public void Return(T? instance)
    {
        // skip null instances
        if (instance is null)
            return;

        // If we're disposed: send to disposal
        if (_instances is null)
            goto dispose;

        // If we have a cleaner and it fails: send to disposal
        if (_tryCleanInstance is not null && !_tryCleanInstance(instance))
            goto dispose;

        // Check if we can store this instance
        if ((_itemCount < MaxCapacity) && (Interlocked.Increment(ref _itemCount) <= MaxCapacity))
        {
            // Try to store in the first slot
            if ((_firstInstance != null) || (Interlocked.CompareExchange<T?>(ref _firstInstance, instance, null) != null))
            {
                // Store in instances
                _instances.Enqueue(instance);
            }

            return; // stored
        }

    dispose:
        // Run any available dispose action
        _disposeInstance?.Invoke(instance);
        // not stored
    }

    /// <summary>
    /// Dispose all stored <typeparamref name="T"/> instances and start automatic disposal of any further <see cref="Return">Returned</see> instances
    /// </summary>
    public void Dispose()
    {
        T? instance;
        // we use _instances == null to determine disposal
        var instances = Interlocked.Exchange<ConcurrentQueue<T>?>(ref _instances, null);
        // but it may have been disposed before
        if (instances is not null)
        {
            // pop out instances
            while (instances.TryDequeue(out instance))
            {
                // clean and dispose
                _ = _tryCleanInstance?.Invoke(instance);
                _disposeInstance?.Invoke(instance);
            }

            // first instance
            instance = Interlocked.Exchange<T?>(ref _firstInstance, null);
            if (instance is not null)
            {
                // clean and dispose
                _ = _tryCleanInstance?.Invoke(instance);
                _disposeInstance?.Invoke(instance);
            }
        }
    }
}
