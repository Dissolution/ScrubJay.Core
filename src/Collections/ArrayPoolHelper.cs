using System.Buffers;

namespace ScrubJay.Collections;

internal static class ArrayPoolHelper
{
    public const int MIN_CAPACITY = 1024;

    public static T[] Rent<T>(int capacity)
    {
        capacity = Math.Max(MIN_CAPACITY, capacity);
        return ArrayPool<T>.Shared.Rent(capacity);
    }

    public static void ExpandBy<T>(ref T[] array, int count, int adding)
    {
        var newCapacity = (array.Length + adding) * 2;
        var newArray = ArrayPool<T>.Shared.Rent(newCapacity);
        array.AsSpan(0, count).CopyTo(newArray);
        var toReturn = array;
        array = newArray;
        if (toReturn.Length > 0)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }
}