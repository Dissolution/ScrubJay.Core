#pragma warning disable CA1710

using System.Collections;
using ScrubJay.Collections;

namespace ScrubJay.Tests;

public sealed class MiscTheoryData : IReadOnlyCollection<object?[]>
{
    private readonly List<object?[]> _rows;

    public int Count => _rows.Count;

    public MiscTheoryData()
    {
        _rows = new(64);
    }

    public void Add<T>(T? value) => _rows.Add(new object?[1] { (object?)value });

    public void Add(object? obj) => _rows.Add(new object?[1] { obj });

    public void AddRow(params object?[] objects) => _rows.Add(objects);

    public MiscTheoryData Combinations(int columns)
    {
        if (columns < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(columns));
        }

        var rows = _rows;
        if (rows.Count == 0)
        {
            return new MiscTheoryData();
        }

        var lengths = new int[columns];
        lengths.AsSpan().Fill(rows.Count);
        BoundedArrayIndices boundedIndices = BoundedArrayIndices.Lengths(lengths);

        var data = new MiscTheoryData();

        while (boundedIndices.TryMoveNext(out var indices))
        {
            var row = new object?[columns];
            for (var c = 0; c < columns; c++)
            {
                row[c] = rows[indices[c]][0];
            }
            data.AddRow(row);
        }

        return data;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<object?[]> GetEnumerator() => _rows.GetEnumerator();
}
