#pragma warning disable CA1816

namespace ScrubJay.Collections.Pooled;

[PublicAPI]
[MustDisposeResource(true)]
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
        _array = ArrayPool<T>.Shared.Rent(minCapacity);
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
            _array = ArrayPool<T>.Shared.Rent(minCapacity);
        }
        else if (minCapacity > capacity)
        {
            _version++;
            T[] newArray = ArrayPool<T>.Shared.Rent(minCapacity);
            CopyToNewArray(newArray);
            T[] toReturn = Interlocked.Exchange<T[]>(ref _array, newArray);
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    [HandlesResourceDisposal]
    public virtual void Dispose()
    {
        T[] toReturn = Interlocked.Exchange<T[]>(ref _array, []);
        ArrayPool<T>.Shared.Return(toReturn);
    }
}
