namespace ScrubJay.Collections;

/// <summary>
/// A universal <see cref="IEnumerator{T}"/> for any-dimensional <see cref="Array"/>s
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
    
    public T this[params int[] indices]
    {
        get => GetValue(indices);
        set => SetValue(value, indices);
    }

    public int Count => _array.Length;
    public int Rank => _rank;
    public IReadOnlyList<int> LowerBounds => _lowerBounds;
    public IReadOnlyList<int> UpperBounds => _upperBounds;
    
    public ArrayWrapper(Array array)
    {
        if (!array.GetElementType().Implements<T>())
            throw new ArgumentException();
        _array = array;
        var rank = _rank = array.Rank;
        var lower = _lowerBounds = new int[rank];
        var upper = _upperBounds = new int[rank];
        for (var dim = 0; dim < rank; dim++)
        {
            lower[dim] = array.GetLowerBound(dim);
            upper[dim] = array.GetUpperBound(dim);
        }
    }

    /// <summary>
    /// Gets the value stored at the given <paramref name="indices"/>
    /// </summary>
    public T GetValue(params int[] indices)
    {
        return _array.GetValue(indices).AsValid<T>();
    }

    /// <summary>
    /// Sets the <paramref name="value"/> stored at the given <paramref name="indices"/>
    /// </summary>
    public void SetValue(T value, params int[] indices)
    {
        _array.SetValue(value, indices);
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    public ArrayEnumerator<T> GetEnumerator() => new(_array, _rank, _lowerBounds, _upperBounds);
}