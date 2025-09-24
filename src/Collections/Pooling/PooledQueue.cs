/*
namespace ScrubJay.Collections.Pooled;

[PublicAPI]
[MustDisposeResource]
[DebuggerDisplay("Count = {Count}")]
public class PooledQueue<T> : PooledArray<T>,
    ICollection<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable
{
    private T[] _array;
    private int _head;       // The index from which to dequeue if the queue isn't empty.
    private int _tail;       // The index at which to enqueue if the queue isn't full.
    private int _size;       // Number of elements.
    private int _version;

    bool ICollection<T>.IsReadOnly => false;

    public int Count => _size;

    /// <summary>
    /// Gets the total numbers of elements the internal data structure can hold without resizing.
    /// </summary>
    public int Capacity => _array.Length;


    public PooledQueue()
    {
        _array = Array.Empty<T>();
    }

    public PooledQueue(int minCapacity)
    {
        _array = ArrayPool<T>.Shared.Rent(minCapacity);
    }

    private void GrowTo(int minCapacity)
    {
        Debug.Assert(_array.Length <= minCapacity);
        Debug.Assert(_size <= minCapacity);
        T[] newarray = ArrayPool<T>
        if (_size > 0)
        {
            if (_head < _tail)
            {
                Array.Copy(_array, _head, newarray, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
            }
        }

        _array = newarray;
        _head = 0;
        _tail = (_size == capacity) ? 0 : _size;
        _version++;
    }


    public void Clear()
    {
        _version++;
        _size = 0;
        _head = 0;
        _tail = 0;
    }

    // CopyTo copies a collection into an Array, starting at a particular
    // index into the array.
    public void CopyTo(T[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _size).ThrowIfError();

        int numToCopy = _size;
        if (numToCopy == 0) return;

        int firstPart = Math.Min(_array.Length - _head, numToCopy);
        Array.Copy(_array, _head, array, arrayIndex, firstPart);
        numToCopy -= firstPart;
        if (numToCopy > 0)
        {
            Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
        }
    }


    public void Enqueue(T item)
    {
        _version++;
        int pos = _size;
        int newSize = pos + 1;

        if (newSize >= _array.Length)
            GrowTo(newSize);

        _array[_tail] = item;
        MoveNext(ref _tail);
        _size = newSize;
    }

    public T Peek() => TryPeek().SomeOrThrow("Queue is empty");

    public Option<T> TryPeek()
    {
        if (_size == 0)
        {
            return None<T>();
        }

        return Some(_array[_head]);
    }

    public T Dequeue() => TryDequeue().SomeOrThrow("Queue is empty");

    public Option<T> TryDequeue()
    {
        int head = _head;
        T[] array = _array;

        if (_size == 0)
        {
            return None<T>();
        }

        _version++;
        _size--;
        MoveNext(ref _head);
        return Some(array[head]);
    }

    public bool Contains(T item)
    {
        if (_size == 0)
        {
            return false;
        }

        if (_head < _tail)
        {
            return Array.IndexOf<T>(_array, item, _head, _size) >= 0;
        }

        // We've wrapped around. Check both partitions, the least recently enqueued first.
        return
            Array.IndexOf<T>(_array, item, _head, _array.Length - _head) >= 0 ||
            Array.IndexOf<T>(_array, item, 0, _tail) >= 0;
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Count == 0 ? Enumerator.Empty<T>() : GetEnumerator();
    public PooledQueueEnumerator GetEnumerator() => new PooledQueueEnumerator(this);


    // Iterates over the objects in the queue, returning an array of the
    // objects in the Queue, or an empty array if the queue is empty.
    // The order of elements in the array is first in to last in, the same
    // order produced by successive calls to Dequeue.
    public T[] ToArray()
    {
        if (_size == 0)
        {
            return Array.Empty<T>();
        }

        T[] arr = new T[_size];

        if (_head < _tail)
        {
            Array.Copy(_array, _head, arr, 0, _size);
        }
        else
        {
            Array.Copy(_array, _head, arr, 0, _array.Length - _head);
            Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
        }

        return arr;
    }


    // Increments the index wrapping it if necessary.
    private void MoveNext(ref int index)
    {
        // It is tempting to use the remainder operator here but it is actually much slower
        // than a simple comparison and a rarely taken branch.
        // JIT produces better code than with ternary operator ?:
        int tmp = index + 1;
        if (tmp == _array.Length)
        {
            tmp = 0;
        }
        index = tmp;
    }

    private void ThrowForEmptyQueue()
    {
        Debug.Assert(_size == 0);
        throw Ex.Invalid(SR.InvalidOperation_EmptyQueue);
    }

    public void TrimExcess()
    {
        int threshold = (int)(_array.Length * 0.9);
        if (_size < threshold)
        {
            SetCapacity(_size);
        }
    }

    /// <summary>
    /// Sets the capacity of a <see cref="PooledQueue{T}"/> object to the specified number of entries.
    /// </summary>
    /// <param name="capacity">The new capacity.</param>
    /// <exception cref="ArgumentOutOfRangeException">Passed capacity is lower than entries count.</exception>
    public void TrimExcess(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, _size);

        if (capacity == _array.Length)
            return;

        SetCapacity(capacity);
    }

    /// <summary>
    /// Ensures that the capacity of this Queue is at least the specified <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
    /// <returns>The new capacity of this queue.</returns>
    public int EnsureCapacity(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);

        if (_array.Length < capacity)
        {
            GrowTo(capacity);
        }

        return _array.Length;
    }



    // Implements an enumerator for a Queue.  The enumerator uses the
    // internal version number of the list to ensure that no modifications are
    // made to the list while an enumeration is in progress.
    public struct PooledQueueEnumerator : IEnumerator<T>,
        IEnumerator
    {
        private readonly PooledQueue<T> _q;
        private readonly int _version;
        private int _index;   // -1 = not started, -2 = ended/disposed
        private T? _currentElement;

        internal PooledQueueEnumerator(PooledQueue<T> q)
        {
            _q = q;
            _version = q._version;
            _index = -1;
            _currentElement = default;
        }

        public void Dispose()
        {
            _index = -2;
            _currentElement = default;
        }

        public bool MoveNext()
        {
            if (_version != _q._version) ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

            if (_index == -2)
                return false;

            _index++;

            if (_index == _q._size)
            {
                // We've run past the last element
                _index = -2;
                _currentElement = default;
                return false;
            }

            // Cache some fields in locals to decrease code size
            T[] array = _q._array;
            uint capacity = (uint)array.Length;

            // _index represents the 0-based index into the queue, however the queue
            // doesn't have to start from 0 and it may not even be stored contiguously in memory.

            uint arrayIndex = (uint)(_q._head + _index); // this is the actual index into the queue's backing array
            if (arrayIndex >= capacity)
            {
                // NOTE: Originally we were using the modulo operator here, however
                // on Intel processors it has a very high instruction latency which
                // was slowing down the loop quite a bit.
                // Replacing it with simple comparison/subtraction operations sped up
                // the average foreach loop by 2x.

                arrayIndex -= capacity; // wrap around if needed
            }

            _currentElement = array[arrayIndex];
            return true;
        }

        public T Current
        {
            get
            {
                if (_index < 0)
                    ThrowEnumerationNotStartedOrEnded();
                return _currentElement!;
            }
        }

        private void ThrowEnumerationNotStartedOrEnded()
        {
            Debug.Assert(_index == -1 || _index == -2);
            throw Ex.Invalid(_index == -1 ? SR.InvalidOperation_EnumNotStarted : SR.InvalidOperation_EnumEnded);
        }

        object? IEnumerator.Current
        {
            get { return Current; }
        }

        void IEnumerator.Reset()
        {
            if (_version != _q._version) ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
            _index = -1;
            _currentElement = default;
        }
    }
}
*/
