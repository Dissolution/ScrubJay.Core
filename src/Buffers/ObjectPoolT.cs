using System.Collections.Concurrent;

#pragma warning disable S1066, MA0048

namespace ScrubJay.Buffers;

/// <summary>
/// A pool of <typeparamref name="T"/> instances
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of <c>class</c> instances stored in this <see cref="ObjectPool{T}"/>
/// </typeparam>
/// <remarks>
/// - The main purpose of an <see cref="ObjectPool{T}"/> is to help re-use a limited number of
/// <typeparamref name="T"/> instances rather than continuously <c>new</c>-ing them up.<br/>
/// - It is not the goal to keep all returned instances.<br/>
///   - The pool is not meant for storage (short nor long).<br/>
///   - If there is no space in the pool, extra returned instances will be disposed.<br/>
/// - It is implied that if an instance is obtained from a pool, the caller will return it back in a relatively short time.<br/>
///   - Keeping checked out instances for long durations is _ok_, but it reduces the usefulness of pooling.<br/>
///   - Not returning instances to the pool in not detrimental to its work, but is a bad practice.<br/>
///   - If there is no intent to return or re-use the instance, do not use a pool.<br/>
/// - When this pool is Disposed, all instances will also be disposed.<br/>
///   - Any further returned instances will be cleaned, disposed, and discarded.<br/>
/// </remarks>
/// <seealso href="https://github.dev/dotnet/aspnetcore/blob/main/src/ObjectPool/src/DefaultObjectPool.cs"/>
[PublicAPI]
public sealed class ObjectPool<T> : IDisposable
    where T : class
{
    /// <summary>
    /// Whenever an instance is requested but unavailable, this function will create a new instance
    /// </summary>
    private readonly Func<T> _createInstance;

    /// <summary>
    /// Whenever an instance is returned to the pool, this action will ready that instance for re-use
    /// </summary>
    private readonly Func<T, bool> _tryCleanInstance;

    /// <summary>
    /// Whenever an instance returned to the pool would be discarded, this action will dispose that instance
    /// </summary>
    private readonly Action<T> _disposeInstance;


    /// <summary>
    /// The first instance is stored in a dedicated field as we expect to be able to satisfy most requests from it
    /// </summary>
    private T? _firstInstance;

    /// <summary>
    /// Storage for the extra pool instances
    /// </summary>
    private ConcurrentQueue<T>? _instances;

    private readonly int _maxCapacity;
    private int _itemCount;
    
    /// <summary>
    /// Gets the maximum number of instances this <see cref="ObjectPool{T}"/> can store
    /// </summary>
    public int MaxCapacity => _maxCapacity;

    /// <summary>
    /// Gets the current number of instances stored in this <see cref="ObjectPool{T}"/>
    /// </summary>
    public int Count => _itemCount;
    
    internal ObjectPool(ObjectPoolPolicy<T> poolPolicy)
    {
        _createInstance = poolPolicy.CreateInstance;
        _tryCleanInstance = poolPolicy.TryCleanInstance;
        _disposeInstance = poolPolicy.DisposeInstance;

        _firstInstance = null;
        _instances = new ConcurrentQueue<T>();
        _maxCapacity = poolPolicy.MaxCapacity;
        _itemCount = 0;
    }
    
    /// <summary>
    /// Rent a <typeparamref name="T"/> instance from this <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <remarks>
    /// <see cref="Return"/> the instance when you are done with it or there is no utility in using an <see cref="ObjectPool{T}"/>
    /// </remarks>
    public T Rent()
    {
        Throw.IfDisposed(_instances is null, _instances);
        
        var instance = _firstInstance;
        if (instance == null || Interlocked.CompareExchange<T?>(ref _firstInstance, null, instance) != instance)
        {
            if (_instances.TryDequeue(out instance))
            {
                Interlocked.Decrement(ref _itemCount);
                return instance;
            }

            // no object available, so go get a brand new one
            return _createInstance();
        }

        return instance;
    }

    /// <summary>
    /// Returns a <typeparamref name="T"/> instance to this <see cref="ObjectPool{T}"/>
    /// </summary>
    public bool Return(T? instance)
    {
        // skip null instances
        if (instance is null)
            return false;

        // Try to clean the instance
        if (_instances is null || !_tryCleanInstance(instance))
        {
            // could not clean, discard
            _disposeInstance.Invoke(instance);
            return false;
        }

        // Try to store in the first slot
        if (_firstInstance != null || Interlocked.CompareExchange<T?>(ref _firstInstance, instance, null) != null)
        {
            // We are still storing items
            if (Interlocked.Increment(ref _itemCount) < _maxCapacity)
            {
                _instances.Enqueue(instance);
                return true;
            }

            // No room to store, discard
            Interlocked.Decrement(ref _itemCount);
            _disposeInstance.Invoke(instance);
            return false;
        }

        // stored
        return true;
    }

    /// <summary>
    /// <see cref="Rent">Rents</see> a <typeparamref name="T"/> instance,
    /// performs an <paramref name="instanceAction"/> on it,
    /// and then <see cref="Return">Returns</see> it to this <see cref="ObjectPool{T}"/>
    /// </summary>
    /// <param name="instanceAction">
    /// An <see cref="Action{T}"/> to perform on the rented instance
    /// </param>
    public void Borrow(Action<T> instanceAction)
    {
        T instance = Rent();
        instanceAction.Invoke(instance);
        Return(instance);
    }

    /// <summary>
    /// <see cref="Rent">Rents</see> a <typeparamref name="T"/> instance,
    /// performs an <paramref name="instanceFunc"/> on it,
    /// <see cref="Return">Returns</see> it to this <see cref="ObjectPool{T}"/>,
    /// and then returns the result of the <paramref name="instanceFunc"/>
    /// </summary>
    /// <param name="instanceFunc">
    /// An <see cref="Func{T, TResult}"/> to perform on the rented instance and return the result of
    /// </param>
    public TResult Borrow<TResult>(Func<T, TResult> instanceFunc)
    {
        T instance = Rent();
        TResult result = instanceFunc.Invoke(instance);
        Return(instance);
        return result;
    }

    /// <summary>
    /// Dispose all stored <typeparamref name="T"/> instances and start automatic disposal of any further <see cref="Return">Returned</see> instances
    /// </summary>
    public void Dispose()
    {
        T? instance;
        // we use _instances == null to determine disposal in Return()
        var instances = Interlocked.Exchange<ConcurrentQueue<T>?>(ref _instances, null);
        if (instances is not null)
        {
            while (instances.TryDequeue(out instance))
            {
                _disposeInstance?.Invoke(instance);
            }

            // continue to dispose the first instance
            instance = Interlocked.Exchange<T?>(ref _firstInstance, null);
            if (instance is not null)
            {
                _disposeInstance?.Invoke(instance);
            }
        }
    }
}