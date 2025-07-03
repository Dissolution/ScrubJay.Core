using System.Buffers;

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
        _array = ArrayPool<T>.Shared.Rent(minCapacity);
    }

    [HandlesResourceDisposal]
    ~PooledArray()
    {
        Dispose();
    }

    protected virtual void CopyToNewArray(T[] newArray) => Sequence.CopyTo(_array, newArray);

    public void Grow() => GrowTo(Capacity * 2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        GrowTo(Capacity + (adding * 16));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void GrowTo(int minCapacity)
    {
        Debug.Assert(minCapacity > Capacity);
        T[] array = ArrayPool<T>.Shared.Rent(Math.Max(minCapacity * 2, 64));
        if (_array.Length > 0)
        {
            CopyToNewArray(array);
            ArrayPool<T>.Shared.Return(_array, true);
        }

        _array = array;
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
        ArrayPool<T>.Shared.Return(toReturn, true);
        GC.SuppressFinalize(this);
    }
}