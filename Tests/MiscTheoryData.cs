﻿#pragma warning disable CA1710, CA1512

using System.Collections;
using ScrubJay.Collections.NonGeneric;

namespace ScrubJay.Tests;

public sealed class MiscTheoryData : IReadOnlyCollection<object?[]>
{
    private readonly List<object?[]> _rows;

    public int Count => _rows.Count;

    public MiscTheoryData()
    {
        _rows = new(64);
    }

    public void Add<T>(T? value) => _rows.Add([(object?)value]);

    public void Add(object? obj) => _rows.Add([obj]);

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

        int[] lengths = new int[columns];
        lengths.AsSpan().Fill(rows.Count);
        ArrayIndicesEnumerator indices = ArrayIndicesEnumerator.FromLengths(lengths);

        var data = new MiscTheoryData();

        while (indices.TryMoveNext().IsOk(out int[]? index))
        {
            object?[] row = new object?[columns];
            for (int c = 0; c < columns; c++)
            {
                row[c] = rows[index[c]][0];
            }
            data.AddRow(row);
        }

        return data;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<object?[]> GetEnumerator() => _rows.GetEnumerator();
}
