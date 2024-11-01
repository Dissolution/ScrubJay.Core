namespace ScrubJay.Collections;

/// <summary>
/// A typed <see cref="IEnumerator{T}"/> over an <see cref="Array"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class ArrayEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    // the array being enumerated
    private T[]? _array;
    // the number of items to enumerate over
    private int _count;
    // the current enumeration index
    private int _index;

    object? IEnumerator.Current => Current;

    /// <inheritdoc cref="IEnumerator{T}.Current"/>
    public T Current
    {
        get
        {
            Throw.IfDisposed(_array is null, this);
            if (_index < 0)
                throw new InvalidOperationException("Enumeration has not yet started");
            if (_index >= _count)
                throw new InvalidOperationException("Enumeration has finished");
            return _array[_index];
        }
    }

    /// <summary>
    /// Gets the current position of the enumerator
    /// </summary>
    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _index;
    }

    internal ArrayEnumerator(T[] array, int count)
    {
        _array = array;
        _count = count;
        _index = -1;
    }

    /// <inheritdoc cref="IEnumerator.MoveNext"/>
    public bool MoveNext()
    {
        int newIndex = _index + 1;
        if (newIndex >= _count)
            return false;
        _index = newIndex;
        return true;
    }

    /// <summary>
    /// Tries to advance the enumerator to the next item in the array
    /// </summary>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the next item
    /// </returns>
    public Option<T> TryMoveNext()
    {
        int newIndex = _index + 1;
        if (newIndex >= _count)
            return None<T>();
        _index = newIndex;
        return Some(_array![newIndex]);
    }

    /// <inheritdoc cref="IEnumerator.Reset"/>
    public void Reset()
    {
        _index = -1;
    }

    /// <summary>
    /// Removes all references to the underlying array and stops all further enumeration
    /// </summary>
    public void Dispose()
    {
        // Clear my references
        _count = 0; // stops enumeration
        _index = -1;
        _array = null;
    }
}