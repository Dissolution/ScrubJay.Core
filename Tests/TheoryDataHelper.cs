using System.Diagnostics.CodeAnalysis;

namespace ScrubJay.Tests;

internal static class TheoryDataHelper
{
    internal static void Shuffle<T>(Span<T> items)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(items);
#else
#if NET6_0_OR_GREATER
        var rand = Random.Shared;
#else
        var rand = new Random();
#endif

        int n = items.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int j = rand.Next(i, n);

            if (j != i)
            {
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
#endif
    }

    [return: NotNullIfNotNull(nameof(array))]
    internal static T[]? ReverseShallowCopy<T>(T[]? array)
    {
        if (array is null)
            return null;
        int len = array.Length;
        T[] copy = new T[len];
        for (int s = 0, e = len - 1; s < len && e >= 0; s++, e--)
        {
            copy[e] = array[s];
        }

        return copy;
    }


    public static TheoryData<T> OneParamCombinations<T>(params T[] values)
    {
        return new TheoryData<T>(values);
    }

    public static TheoryData<T, T> TwoParamCombinations<T>(params T[] values)
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

    public static TheoryData<T, T, T> ThreeParamCombinations<T>(params T[] values)
    {
        var data = new TheoryData<T, T, T>();
        int count = values.Length;
        if (count == 0)
            return data;
        for (var i = 0; i < count; i++)
        for (var j = 0; j < count; j++)
        for (var k = 0; k < count; k++)
        {
            data.Add(values[i], values[j], values[k]);
        }

        return data;
    }


//    public static IEnumerable<object?[]> AllCombinations<T>(int argCount, params T?[] items)
//    {
//        if (argCount < 1)
//            yield break;
//        int itemCount = items.Length;
//        if (itemCount == 0)
//            yield break;
//
//        long combCount = (long)Math.Pow(itemCount, argCount);
//        var indexer = new CombinationIndex(argCount, itemCount);
//        bool got;
//        int[]? indices;
//        for (var c = 0L; c < combCount; c += 1L)
//        {
//            object?[] args = new object?[argCount];
//            got = indexer.TryGetNext(out indices);
//            Debug.Assert(got);
//            Debug.Assert(indices is not null);
//            for (var a = 0; a < argCount; a++)
//            {
//                args[a] = (object?)items[indices![a]];
//            }
//            yield return args;
//        }
//        got = indexer.TryGetNext(out indices);
//        Debug.Assert(!got);
//        Debug.Assert(indices is null);
//    }

    private static int[] GetIndices(int count)
    {
        var indices = new int[count];
        for (var i = count - 1; i >= 0; i--)
        {
            indices[i] = i;
        }

        return indices;
    }

    public static IEnumerable<object?[]> ShuffledCombinations<T>(int argCount, params T?[] items)
    {
        if (argCount < 1)
            yield break;
        int itemCount = items.Length;
        if (itemCount == 0)
            yield break;

        var indices = new List<int[]>();
        for (var a = 0; a < argCount; a++)
        {
            var randIndices = GetIndices(itemCount);
            Shuffle<int>(randIndices);
            indices.Add(randIndices);
        }

        for (var i = 0; i < itemCount; i++)
        {
            object?[] args = new object?[argCount];
            for (var a = 0; a < argCount; a++)
            {
                args[a] = indices[a][i];
            }

            yield return args;
        }
    }
}