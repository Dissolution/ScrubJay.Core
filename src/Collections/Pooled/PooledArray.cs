#pragma warning disable CA1816

namespace ScrubJay.Collections.Pooled;

[PublicAPI]
[MustDisposeResource(true)]
public abstract class PooledArray<T> : IDisposable
{
    protected T[] _array;

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
        _array = Array.Empty<T>();
    }

    protected PooledArray(int minCapacity)
    {
        _array = ArrayPool<T>.Shared.Rent(minCapacity);
    }

    protected virtual void CopyToNewArray(T[] newArray) => Sequence.CopyTo(_array, newArray);

    public void Grow()
    {
        int capacity = _array.Length;
        if (capacity == 0)
        {
            _array = ArrayPool<T>.Shared.Rent();
        }
        else
        {
            T[] newArray = ArrayPool<T>.Shared.Rent(capacity * 2);
            CopyToNewArray(newArray);
            T[] toReturn = Interlocked.Exchange<T[]>(ref _array, newArray);
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    public void GrowTo(int minCapacity)
    {
        int capacity = _array.Length;
        if (capacity == 0)
        {
            _array = ArrayPool<T>.Shared.Rent(minCapacity);
        }
        else if (minCapacity > capacity)
        {
            T[] newArray = ArrayPool<T>.Shared.Rent(minCapacity);
            CopyToNewArray(newArray);
            T[] toReturn = Interlocked.Exchange<T[]>(ref _array, newArray);
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    [HandlesResourceDisposal]
    public virtual void Dispose()
    {
        T[] toReturn = Interlocked.Exchange<T[]>(ref _array, Array.Empty<T>());
        ArrayPool<T>.Shared.Return(toReturn);
    }
}
