#pragma warning disable CA1816

namespace ScrubJay.Collections.Pooling;

[PublicAPI]
public abstract class PooledArray<T> : IDisposable
{
    protected internal T[] _array;
    protected internal int _version;

    /// <summary>
    /// Gets the current capacity to store items<br/>
    /// This will be automatically increased if required or by calling <see cref="Grow"/>
    /// </summary>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.Length;
    }

    protected PooledArray()
    {
        _array = [];
    }

    protected PooledArray(int minCapacity)
    {
        _array = ArrayInstancePool<T>.Shared.Rent(minCapacity);
    }

    [HandlesResourceDisposal]
    ~PooledArray()
    {
        this.Dispose();
    }

    protected virtual void CopyToNewArray(T[] newArray) => Sequence.CopyTo(_array, newArray);

    public void Grow() => GrowTo(Capacity * 2);

    public void GrowBy(int adding)
    {
        if (adding > 0)
        {
            GrowTo((Capacity + adding) * 2);
        }
    }

    public void GrowTo(int minCapacity)
    {
        int capacity = _array.Length;
        if (capacity == 0)
        {
            _version++;
            _array = ArrayInstancePool<T>.Shared.Rent(minCapacity);
        }
        else if (minCapacity > capacity)
        {
            _version++;
            T[] newArray = ArrayInstancePool<T>.Shared.Rent(minCapacity);
            CopyToNewArray(newArray);
            T[] toReturn = Interlocked.Exchange<T[]>(ref _array, newArray);
            ArrayInstancePool<T>.Shared.Return(toReturn);
        }
    }

    protected virtual void OnDisposing()
    {
        // default do nothing
    }

    [HandlesResourceDisposal]
    public void Dispose()
    {
        OnDisposing();
        T[] toReturn = Interlocked.Exchange<T[]>(ref _array, []);
        ArrayInstancePool<T>.Shared.Return(toReturn);
        GC.SuppressFinalize(this);
    }
}
