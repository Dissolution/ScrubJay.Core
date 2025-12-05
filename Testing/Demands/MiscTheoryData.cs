using System.Collections;

namespace ScrubJay.Functional.Tests;

public class MiscTheoryData : IReadOnlyCollection<object?[]>
{
    private readonly List<object?[]> _rows;

    public int Count => _rows.Count;

    public MiscTheoryData()
    {
        _rows = new(64);
    }

    public void Add<T>(T? value)
    {
        _rows.Add(new object?[1] { (object?)value });


    }

    public void Add(object? obj)
    {
        _rows.Add(new object?[1] { obj });
    }

    public void AddRow(params object?[] objects)
    {
        _rows.Add(objects);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<object?[]> GetEnumerator() => _rows.GetEnumerator();
}