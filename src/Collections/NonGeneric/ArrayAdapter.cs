#pragma warning disable CA1710

namespace ScrubJay.Collections.NonGeneric;

[PublicAPI]
public sealed class ArrayAdapter<T> :
    /* Does not implement IList<T>, IList, ICollection<T>, nor ICollection
     * as those types imply some ability to Add and Remove from a collection
     * that does not exist for an Array
     */
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable
{
    private readonly Array _array;

    [return: NotNullIfNotNull(nameof(objValue))]
    private static T? ObjectToValue(
        object? objValue,
        [CallerArgumentExpression(nameof(objValue))]
        string? valueName = null)
    {
        if (objValue.CanBe<T>(out var value))
            return value;
        throw new ArgumentException($"Value '{objValue}' is not a '{typeof(T).Name}'", valueName);
    }

    public T this[int index]
    {
        get => ObjectToValue(_array.GetValue(index))!;
        set => _array.SetValue((object?)value, index);
    }

    public int Count => _array.Length;

    public ArrayAdapter(Array array)
    {
        _array = array.ThrowIfNull();
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public int IndexOf(T item)
    {
        return Array.IndexOf(_array, (object?)item);
    }

    public void Clear()
    {
        Array.Clear(_array, 0, _array.Length);
    }

    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        Validate.CopyTo(Count, array, arrayIndex).ThrowIfError();
        Array.Copy(_array, 0, array, arrayIndex, _array.Length);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator() => _array.Cast<T>().GetEnumerator();
}