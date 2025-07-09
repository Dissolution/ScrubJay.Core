// Identifiers should have correct suffix

using ScrubJay.Text.Rendering;

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
     * as those types imply the capacity to Add and Remove from a collection
     * that Array does not have
     */
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
{
    [return: NotNullIfNotNull(nameof(objValue))]
    private static T? ObjectToValue(
        object? objValue,
        [CallerArgumentExpression(nameof(objValue))]
        string? valueName = null)
    {
        if (objValue.As<T>(out var value))
            return value;
        throw new ArgumentException($"Value '{objValue}' is not a '{typeof(T).Render()}'", valueName);
    }

    private readonly Array _array;
    private readonly int _lowerBounds;
    private readonly int _upperBound;

    public T this[int index]
    {
        get => ObjectToValue(_array.GetValue(index))!;
        set => _array.SetValue((object?)value, index);
    }

    public int Count => _array.Length;

    public ArrayAdapter2D(Array array)
    {
        _array = array.ThrowIfNull();
        int dims = array.Rank;
        if (dims != 1)
            throw new ArgumentException("Array must have a Rank of 1", nameof(array));
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
