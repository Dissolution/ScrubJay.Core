namespace ScrubJay.Tests;

public static class TheoryDataHelper
{
    public static TheoryData<T> OneParamCombinations<T>(params T[] values) => new(values);

    public static TheoryData<T, T> TwoParamCombinations<T>(params T[] values)
    {
        var data = new TheoryData<T, T>();
        int count = values.Length;
        if (count == 0)
            return data;
        for (int i = 0; i < count; i++)
        for (int j = 0; j < count; j++)
        {
            data.Add(values[i], values[j]);
        }

        return data;
    }

    public static TheoryData<T, T, T> ThreeParamCombinations<T>(params T[] values)
    {
        var data = new TheoryData<T, T, T>();
        int count = values.Length;
        if (count == 0)
            return data;
        for (int i = 0; i < count; i++)
        for (int j = 0; j < count; j++)
        for (int k = 0; k < count; k++)
        {
            data.Add(values[i], values[j], values[k]);
        }

        return data;
    }
}