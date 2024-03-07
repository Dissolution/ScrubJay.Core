namespace ScrubJay.Collections;

/// <summary>
/// A wrapper for an <see cref="Array"/> that manages typed values in any number of dimensions
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in the <see cref="Array"/>
/// </typeparam>
public sealed class ArrayWrapper<T> :
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
{
    private readonly Array _array;
    private readonly int _rank;
    private readonly int[] _lowerBounds;
    private readonly int[] _upperBounds;
    
    /// <summary>
    /// Gets or sets the item stored at the given <paramref name="indices"/>
    /// </summary>
    /// <param name="indices"></param>
    public T this[params int[] indices]
    {
        get => GetValue(indices);
        set => SetValue(value, indices);
    }

    /// <summary>
    /// The total number of items in this array
    /// </summary>
    public int Count => _array.Length;
    
    /// <summary>
    /// Gets the number of dimensions in this array
    /// </summary>
    public int Dimensions => _rank;
    
    /// <summary>
    /// The inclusive lower-bound for each dimension
    /// </summary>
    public IReadOnlyList<int> LowerBounds => _lowerBounds;
    
    /// <summary>
    /// The inclusive upper-bound for each dimension
    /// </summary>
    public IReadOnlyList<int> UpperBounds => _upperBounds;
    
    
    public ArrayWrapper(Array array)
    {
        if (!array.GetElementType().Implements<T>())
            throw new ArgumentException($"The given array does not store {typeof(T).Name} items", nameof(array));
        _array = array;
        var rank = _rank = array.Rank;
        Span<int> lower = _lowerBounds = new int[rank];
        Span<int> upper = _upperBounds = new int[rank];
        for (var dim = 0; dim < rank; dim++)
        {
            lower[dim] = array.GetLowerBound(dim);
            upper[dim] = array.GetUpperBound(dim);
        }
    }

    /// <summary>
    /// Gets the lower and upper bounds of the given <paramref name="rank"/> (0-based)
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public (int Lower, int Upper) GetBounds(int rank)
    {
        Throw.IfNotIn(rank, 0, _rank - 1);
        return (_lowerBounds[rank], _upperBounds[rank]);
    }
    
    /// <summary>
    /// Gets the value stored at the given <paramref name="indices"/>
    /// </summary>
    public T GetValue(params int[] indices)
    {
        return _array.GetValue(indices).AsNullValid<T>()!;
    }

    /// <summary>
    /// Sets the <paramref name="value"/> stored at the given <paramref name="indices"/>
    /// </summary>
    public void SetValue(T value, params int[] indices)
    {
        _array.SetValue(value, indices);
    }
    
    /// <summary>
    /// Sets the <paramref name="value"/> stored at the given <paramref name="indices"/>
    /// </summary>
    public void SetValue(int[] indices, T value)
    {
        _array.SetValue(value, indices);
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    public ArrayEnumerator<T> GetEnumerator() => new(this);
}