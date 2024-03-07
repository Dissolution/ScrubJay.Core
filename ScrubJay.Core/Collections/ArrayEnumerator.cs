namespace ScrubJay.Collections;

/// <summary>
/// A universal <see cref="IEnumerator{T}"/> for any-dimensional <see cref="Array"/>s
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in the <see cref="Array"/>
/// </typeparam>
public sealed class ArrayEnumerator<T> : 
    IEnumerator<T>, 
    IEnumerator
{
    private readonly ArrayWrapper<T> _wrappedArray;
    private readonly int[] _arrayIndices;
    
    /// <summary>
    /// The current enumeration indices
    /// </summary>
    public int[] Indices => _arrayIndices;
    
    /// <inheritdoc cref="IEnumerator"/>
    object? IEnumerator.Current => _wrappedArray.GetValue(_arrayIndices);
    
    /// <summary>
    /// The currently enumerated item
    /// </summary>
    public T Current => _wrappedArray.GetValue(_arrayIndices);
    
    internal ArrayEnumerator(ArrayWrapper<T> wrappedArray)
    {
        _wrappedArray = wrappedArray;
        // Setup our starting indices
        _arrayIndices = new int[wrappedArray.Dimensions];
        Reset();
    }

    public void Deconstruct(out int[] indices, out T item)
    {
        indices = _arrayIndices;
        item = Current;
    }

    private bool TryIncrementAt(int rank)
    {
        if (rank < 0) return false;
        var arrayIndex = _arrayIndices;
        var rankIndex = arrayIndex[rank];
        var rankBounds = _wrappedArray.GetBounds(rank);
        if (rankIndex >= rankBounds.Upper)
        {
            // We have to be able to roll the one to the left
            if (!TryIncrementAt(rank - 1))
                return false;
            // Reset me
            arrayIndex[rank] = rankBounds.Lower;
            return true;
        }
        
        // Roll Me
        arrayIndex[rank] = rankIndex + 1;
        return true;
    }
    
    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        if (MoveNext())
        {
            value = Current;
            return true;
        }

        value = default;
        return false;
    }
    
    public bool MoveNext()
    {
        return TryIncrementAt(_wrappedArray.Dimensions - 1);
    }

    public void Reset()
    {
        int end = _wrappedArray.Dimensions - 1;
        var lowerBounds = _wrappedArray.LowerBounds;
        // The last index needs to be one lower (pre-incremented)
        _arrayIndices[end] = lowerBounds[end] - 1; 
        // The rest start at their zero
        for (var dim = 0; dim < end; dim++)
        {
            _arrayIndices[dim] = lowerBounds[dim];
        }
    }

    void IDisposable.Dispose() { }
}