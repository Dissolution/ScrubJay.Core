using System.Collections.Concurrent;

namespace ScrubJay.Collections.Pooling;

/// <summary>
/// An <see cref="IInstancePool{T}"/> that uses delegates to control its operations
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <c>class</c> instances stored in this <see cref="InstancePool{T}"/>
/// </typeparam>
/// <seealso href="https://github.dev/dotnet/aspnetcore/blob/main/src/ObjectPool/src/DefaultObjectPool.cs"/>
[PublicAPI]
[MustDisposeResource(true)]
public sealed class InstancePool<T> : IInstancePool<T>, IDisposable
    where T : class
{
    private readonly Func<T>        _createInstance;
    private readonly Func<T, bool>? _tryReadyInstance;
    private readonly Func<T, bool>? _tryCleanInstance;
    private readonly Action<T>?     _disposeInstance;


    /// <summary>
    /// The first instance is stored in this dedicated field as we expect to be able to satisfy most rents from it
    /// </summary>
    private T? _firstInstance;

    /// <summary>
    /// Storage for the extra pool instances
    /// </summary>
    /// <remarks>
    /// If this is <c>null</c>, this pool has been disposed
    /// </remarks>
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
        _tryReadyInstance = poolPolicy.TryReadyInstance;
        _tryCleanInstance = poolPolicy.TryCleanInstance;
        _disposeInstance = poolPolicy.DisposeInstance;
    }

    public T Rent()
    {
        while (true)
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
                    instance = _createInstance();
                    if (_tryReadyInstance is not null && !_tryReadyInstance(instance))
                    {
                        throw new InvalidOperationException("Could not Ready brand new Instance");
                    }
                    return instance;
                }
            }

            // we are storing one fewer instance
            Interlocked.Decrement(ref _itemCount);

            if (_tryReadyInstance is not null && !_tryReadyInstance(instance))
            {
                // Could not ready this instance, destroy it
                _disposeInstance?.Invoke(instance);
                // try again
                continue;
            }

            return instance;
        }
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
