using System.Buffers;

namespace ScrubJay.Buffers;

public static class ArrayPool
{
    public const int MIN_CAPACITY = 16; // tested

    public const int MAX_CAPACITY = 0X7FFFFFC7; // == Array.MaxLength

    public static T[] Rent<T>()
    {
        return ArrayPool<T>.Shared.Rent(MIN_CAPACITY);
    }

    public static T[] Rent<T>(int minCapacity)
    {
        if (minCapacity < MIN_CAPACITY)
        {
            minCapacity = MIN_CAPACITY;
        }
        else if (minCapacity > MAX_CAPACITY)
        {
            minCapacity = MAX_CAPACITY;
        }

        return ArrayPool<T>.Shared.Rent(minCapacity);
    }

    public static void Return<T>(T[]? array, bool clearArray = false)
    {
        if (array is not null && array.Length > 0)
        {
            ArrayPool<T>.Shared.Return(array, clearArray);
        }
    }

    public static void ReturnAndNull<T>(ref T[]? array, bool clearArray = true)
    {
        if (array is not null)
        {
            if (array.Length > 0)
            {
                ArrayPool<T>.Shared.Return(array, clearArray);
            }

            array = null;
        }
    }
}