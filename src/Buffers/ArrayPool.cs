using System.Buffers;

namespace ScrubJay.Buffers;

/// <summary>
/// A wrapper around <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
/// </summary>
[PublicAPI]
public static class ArrayPool
{
    /// <summary>
    /// The minimum capacity for any array returned from <see cref="Rent{T}()"/>
    /// </summary>
    public const int MinCapacity = 16; // tested

    /// <summary>
    /// The maximum capacity for any array returned from <see cref="Rent{T}()"/>
    /// </summary>
    public const int MaxCapacity = 0X7FFFFFC7; // == Array.MaxLength


    /// <summary>
    /// Rents a <see cref="Array">T[]</see> with a <see cref="Array.Length"/> of at least <see cref="MinCapacity"/>
    /// from <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Array">T[]</see>
    /// </typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Rent<T>() => ArrayPool<T>.Shared.Rent(MinCapacity);

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
        int capacity = minCapacity switch
        {
            < MinCapacity => MinCapacity,
            > MaxCapacity => MaxCapacity,
            _ => minCapacity,
        };
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