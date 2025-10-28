// Identifiers should have correct suffix

#pragma warning disable CA1710
// Remove unnecessary cast (IDE0004)
#pragma warning disable IDE0004

namespace ScrubJay.Collections.NonGeneric;

/// <summary>
/// A wrapper around an <see cref="Array"/> that exposes it as a <c>T[]</c>
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public sealed class ArrayAdapter2D<T> :
    /* Does not implement
     * IList<T>, IList, ICollection<T>, nor ICollection
     * as those types imply the capability to Add and Remove
     * that Array does not have
     */
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
{
    private readonly Array _array;
    private readonly int _lowerBounds;
    private readonly int _upperBound;

    public T this[int index]
    {
        get => _array.GetValue(index).As<T>().OkOrThrow()!;
        set => _array.SetValue((object?)value, index);
    }

    public int Count => _array.Length;

    public ArrayAdapter2D(Array array)
    {
        _array = array.ThrowIfNull();
        int dims = array.Rank;
        if (dims != 1)
            throw Ex.Arg(nameof(array), "Array must have a Rank of 1");
        _lowerBounds = array.GetLowerBound(0);
        _upperBound = array.GetUpperBound(0);
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public int IndexOf(T item) => Array.IndexOf(_array, (object?)item);

    public void Clear() => Array.Clear(_array, 0, _array.Length);

    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        Validate.CanCopyTo(array, arrayIndex, Count).ThrowIfError();
        Array.Copy(_array, 0, array, arrayIndex, _array.Length);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator() => _array.Cast<T>().GetEnumerator();
}
