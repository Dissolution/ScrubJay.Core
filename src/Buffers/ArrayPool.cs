using System.Buffers;

namespace ScrubJay.Buffers;

/// <summary>
/// A wrapper around <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
/// </summary>
[PublicAPI]
public static class ArrayPool
{
    /// <summary>
    /// The minimum capacity for a <see cref="Rent{T}()">Rented</see> <c>T[]</c>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of values that will be stored in the array
    /// </typeparam>
    public static int MinCapacity<T>() => typeof(T) == typeof(char) ? 64 : 16; // larger char pools, 16 was tested as the minimum

    /// <summary>
    /// The maximum capacity for a <see cref="Rent{T}()">Rented</see> <c>T[]</c>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of values that will be stored in the array
    /// </typeparam>
    public static int MaxCapacity<T>() => typeof(T) == typeof(char) ? 0x3FFFFFDF : 0X7FFFFFC7; // string.MaxLength, Array.MaxLength

    public static (int Min, int Max) Capacities<T>() => (MinCapacity<T>(), MaxCapacity<T>());

    /// <summary>
    /// Rents a <see cref="Array">T[]</see> with a <see cref="Array.Length"/> of at least <see cref="MinCapacity{T}"/>
    /// from <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Array">T[]</see>
    /// </typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Rent<T>() => ArrayPool<T>.Shared.Rent(MinCapacity<T>());

    /// <summary>
    /// Rents a <see cref="Array">T[]</see> with a <see cref="Array.Length"/> of at least <paramref name="minCapacity"/>
    /// from <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Array">T[]</see>
    /// </typeparam>
    /// <param name="minCapacity">
    /// The minimum <see cref="Array.Length"/> for the returned <see cref="Array">T[]</see>
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Rent<T>(int minCapacity)
    {
        if (minCapacity <= 0)
            return [];

        int capacity;
        var (min, max) = Capacities<T>();
        if (minCapacity < min)
            capacity = min;
        else if (minCapacity > max)
            capacity = max;
        else
            capacity = minCapacity;
        return ArrayPool<T>.Shared.Rent(capacity);
    }

    /// <summary>
    /// Returns a <see cref="Rent{T}()">Rented</see> <see cref="Array">T[]</see> back to <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </summary>
    /// <param name="array">
    /// The <see cref="Array">T[]</see> to return, may be <c>null</c>
    /// </param>
    /// <param name="clearArray">
    /// Set to <c>true</c> to have the array <see cref="Array.Clear(Array,int,int)">Cleared</see> before being returned to <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </param>
    /// <remarks>
    /// <c>null</c> and empty arrays will simply be discarded
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return<T>(T[]? array, bool clearArray = false)
    {
        if (array is not null && array.Length > 0)
        {
            ArrayPool<T>.Shared.Return(array, clearArray);
        }
    }
}