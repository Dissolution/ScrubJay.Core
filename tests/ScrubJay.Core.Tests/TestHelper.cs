namespace ScrubJay.Tests;

internal static class TestHelper
{
    public static ThreadLocal<InfiniteStack<object?>> InfiniteObjects { get; } = new ThreadLocal<InfiniteStack<object?>>(() => new InfiniteStack<object?>(TestObjects));

    public static IEnumerable<object[]> ToEnumerableObjects<T>(IEnumerable<T> values)
        => values.Select(static value => new object[1] { (object)value! });

    public static IEnumerable<object?[]> ToEnumerableNullableObjects<T>(IEnumerable<T?> values)
        => values.Select(static value => new object?[1] { (object?)value });

    public static IReadOnlyList<object?> TestObjects { get; } = new List<object?>
    {
        null,
        (byte)13,
        (ulong)ulong.MaxValue - 13UL,
        (int)147,
        (int?)null,
        (int?)147,
        (string?)null,
        (string?)string.Empty,
        (string?)"ABC",
        default(Nothing),
        new object(),
        new Exception("BLAH")
    };
}

public sealed class InfiniteStack<T>
{
    private readonly IReadOnlyList<T> _items;
    private readonly int _count;
    private int _index;

    public InfiniteStack(IEnumerable<T> items)
    {
        _items = items as IReadOnlyList<T> ?? items.ToList();
        _count = _items.Count;
        if (_count == 0)
            throw new ArgumentException("", nameof(items));
        _index = 0;
    }


    public T Pop()
    {
        int index = _index;
        _index = ((index + 1) % _count);
        return _items[index];
    }

    public IEnumerable<T> Pop(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return Pop();
        }
    }

    public T[] PopArray(int count)
    {
        if (count <= 0) return Array.Empty<T>();
        T[] array = new T[count];
        for (var i = 0; i < count; i++)
        {
            array[i] = Pop();
        }

        return array;
    }
}