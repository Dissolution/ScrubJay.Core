using System.Buffers;

namespace ScrubJay.Collections.Pooling;

[PublicAPI]
public static class ArrayNest
{
    public static T[] Rent<T>() => ArrayNest<T>.Rent();
    public static T[] Rent<T>(int minCapacity) => ArrayNest<T>.Rent(minCapacity);
    public static void Return<T>(T[]? array) => ArrayNest<T>.Return(array);
    public static void Return<T>(T[]? array, bool clear) => ArrayNest<T>.Return(array, clear);
}

[PublicAPI]
public static class ArrayNest<T>
{
    private static readonly int _minCapacity;
    private static readonly bool _clear;

    static ArrayNest()
    {
        var type = typeof(T);
        if (type.IsValueType)
        {
            if (type == typeof(char))
            {
                _minCapacity = 1024;
                _clear = true;
            }
            else
            {
                _minCapacity = 256;
                _clear = false;
            }
        }
        else
        {
            _minCapacity = 64;
            _clear = true;
        }
    }

    public static T[] Rent() => [];

    public static T[] Rent(int minCapacity)
    {
        return ArrayPool<T>.Shared.Rent(Math.Max(_minCapacity, minCapacity));
    }

    public static void Return(T[]? array)
    {
        if (array is not null && array.Length > 0)
        {
            ArrayPool<T>.Shared.Return(array, _clear);
        }
    }

    public static void Return(T[]? array, bool clear)
    {
        if (array is not null && array.Length > 0)
        {
            ArrayPool<T>.Shared.Return(array, clear);
        }
    }
}