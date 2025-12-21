#pragma warning disable CA1043

namespace ScrubJay.Collections;

public class PooledStack2<T> : IReadOnlyCollection<T>, IEnumerable<T>
{
    private T[] _array;
    private int _position;

    public T this[StackIndex index]
    {
        get => TryGetAt(index).OkOrThrow();
        set => TrySetAt(index, value).ThrowIfError();
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.Length;
        set => throw Ex.NotImplemented();
    }


    public PooledStack2()
    {
        _array = [];
        _position = 0;
    }

    public PooledStack2(int minCapacity)
    {
        _array = ArrayNest.Rent<T>(minCapacity);
        _position = 0;
    }

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
        T[] array = ArrayNest<T>.Rent(Math.Max(minCapacity * 2, 64));
        if (_array.Length > 0)
        {
            _array.AsSpan(0, _position).CopyTo(array);
            ArrayNest.Return(_array, true);
        }

        _array = array;
    }


    internal bool Contains(T item) => throw Ex.NotImplemented();


    public void Push(T item)
    {
        int pos = _position;
        int newPosition = pos + 1;
        if (newPosition > Capacity)
        {
            GrowTo(newPosition);
        }

        _array[pos] = item;
        _position = newPosition;
    }

    public void PushMany(params ReadOnlySpan<T> items)
    {
        int pos = _position;
        int newPosition = pos + items.Length;
        if (newPosition > Capacity)
        {
            GrowTo(newPosition);
        }

        Sequence.CopyTo(items, _array.AsSpan(pos));
        _position = newPosition;
    }

    public void PushMany(T[]? items) => PushMany(items.AsSpan());

    public void PushMany(IEnumerable<T> items)
    {
        int pos = _position;
        if (items is ICollection<T> collection)
        {
            int newPosition = pos + collection.Count;
            if (newPosition > _array.Length)
            {
                GrowTo(newPosition);
            }

            collection.CopyTo(_array, pos);
            _position = newPosition;
        }
        else if (items is IReadOnlyCollection<T> readOnlyCollection)
        {
            int newPosition = pos + readOnlyCollection.Count;
            if (newPosition > _array.Length)
            {
                GrowTo(newPosition);
            }

            var array = _array; // proper size now
            using var e = readOnlyCollection.GetEnumerator();
            while (e.MoveNext())
            {
                array[pos++] = e.Current;
            }

            Debug.Assert(pos == newPosition);
            _position = newPosition;
        }
        else
        {
            foreach (var item in items)
            {
                Push(item);
            }
        }
    }

    public Result TryPushAt(StackIndex index, T item)
    {
        int end = _position;
        int offset = index.GetOffset(end);

        if (offset < 0 || offset > end)
            return Ex.Index(index, end);
        if (offset == end)
        {
            Push(item);
            return true;
        }

        int newSize = end + 1;
        if (newSize >= Capacity)
        {
            GrowTo(newSize);
        }

        _array.SelfCopy(offset..end, (offset + 1)..);
        _array[offset] = item;
        _position = newSize;
        return true;
    }


    public Option<T> TryPeek()
    {
        if (_position > 0)
        {
            return Some(_array[_position - 1]);
        }

        return None;
    }

    public Result<T> TryGetAt(StackIndex index)
    {
        int offset = index.GetOffset(_position);
        if (offset >= 0 && offset < _position)
            return _array[offset];
        return Ex.Index(index, _position);
    }

    public Result TrySetAt(StackIndex index, T item)
    {
        int offset = index.GetOffset(_position);
        if (offset >= 0 && offset < _position)
        {
            _array[offset] = item;
            return true;
        }

        return Ex.Index(index, _position);
    }

    public bool TryCopyTo(Span<T> destination, bool popOrder = true)
    {
        if (popOrder)
        {
            throw Ex.NotImplemented();
        }

        return _array.AsSpan(0, _position).TryCopyTo(destination);
    }


    public void Clear()
    {
        // we do not clear items in the array right away, we leave that for disposal
        _position = 0;
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public StackEnumerator GetEnumerator() => new(this);


    public readonly struct StackEnumerator : IEnumerator<T>
    {
        private readonly PooledStack2<T> _pooledStack2;

        public StackEnumerator(PooledStack2<T> pooledStack2)
        {
            _pooledStack2 = pooledStack2;
        }

        bool IEnumerator.MoveNext()
        {
            throw new NotImplementedException();
        }

        void IEnumerator.Reset()
        {
            throw new NotImplementedException();
        }

        T IEnumerator<T>.Current => throw Ex.NotImplemented();

        object? IEnumerator.Current => throw Ex.NotImplemented();

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}