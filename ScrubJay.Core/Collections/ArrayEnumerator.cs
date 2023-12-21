namespace ScrubJay.Collections;

/// <summary>
/// A universal <see cref="IEnumerator{T}"/> for any-dimensional <see cref="Array"/>s
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in the <see cref="Array"/>
/// </typeparam>
public sealed class ArrayEnumerator<T> : 
    IEnumerator<T>, 
    IEnumerator,
    IDisposable
{
    private Array _array;
    private readonly int _rank;
    private readonly int[] _lowerBounds;
    private readonly int[] _upperBounds;
    
    private readonly int[] _arrayIndices;

    /// <summary>
    /// The <see cref="Array"/>'s Rank -- or Number of Dimensions (1-based)
    /// </summary>
    public int Rank => _rank;
    
    /// <summary>
    /// The current enumeration indices
    /// </summary>
    public int[] Indices => _arrayIndices;
    
    /// <inheritdoc cref="IEnumerator"/>
    object IEnumerator.Current => _array.GetValue(_arrayIndices)!;
    
    /// <summary>
    /// The currently enumerated item
    /// </summary>
    public T Current => (T)_array.GetValue(_arrayIndices)!;
    
    internal ArrayEnumerator(Array array, int rank, int[] lowerBounds, int[] upperBounds)
    {
        _array = array;
        _rank = rank;
        _lowerBounds = lowerBounds;
        _upperBounds = upperBounds;
        
        // Setup our starting indices
        _arrayIndices = new int[rank];
        Reset();
    }

    public void Deconstruct(out int[] indices, out T item)
    {
        indices = _arrayIndices;
        item = Current;
    }

    private bool TryIncrementAt(int index)
    {
        if (index < 0) return false;
        var arrayIndex = _arrayIndices;
        var rankValue = arrayIndex[index];
        if (rankValue >= _upperBounds[index])
        {
            // We have to be able to roll the one to the left
            if (!TryIncrementAt(index - 1))
                return false;
            // Reset me
            arrayIndex[index] = _lowerBounds[index];
            return true;
        }
        
        // Roll Me
        arrayIndex[index] = rankValue + 1;
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
        return TryIncrementAt(_rank - 1);
    }

    public void Reset()
    {
        int end = _rank - 1;
        // The last index needs to be one lower (pre-incremented)
        _arrayIndices[end] = _lowerBounds[end] - 1; 
        // The rest start at their zero
        for (var dim = 0; dim < end; dim++)
        {
            _arrayIndices[dim] = _lowerBounds[dim];
        }
    }

    void IDisposable.Dispose()
    {
        _array = null!;
    }
}