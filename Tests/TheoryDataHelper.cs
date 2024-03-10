namespace ScrubJay.Core.Tests;

internal static class TheoryDataHelper
{
    public static TheoryData<T, T> AllCombinations<T>(params T[] values)
    {
        var data = new TheoryData<T, T>();
        int count = values.Length;
        if (count == 0)
            return data;
        for (var i = 0; i < count; i++)
        for (var j = 0; j < count; j++)
        {
            data.Add(values[i], values[j]);
        }
        return data;
    }
}