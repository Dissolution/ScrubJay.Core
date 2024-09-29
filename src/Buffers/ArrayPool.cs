using System.Buffers;

namespace ScrubJay.Buffers;

/// <summary>
/// <see cref="ArrayPool{T}"/> manager
/// </summary>
[PublicAPI]
public static class ArrayPool
{
    /// <summary>
    /// The minimum capacity of an array returned from <see cref="Rent{T}()"/>
    /// </summary>
    public const int MinCapacity = 16; // tested

    /// <summary>
    /// The maximum capacity of an array returned from <see cref="Rent{T}()"/>
    /// </summary>
    public const int MaxCapacity = 0X7FFFFFC7; // == Array.MaxLength
    
    /// <summary>
    /// Rents a <c>T[]</c> with at least a <see cref="Array.Length"/> of <see cref="MinCapacity"/>
    /// from <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// that should be <see cref="Return{T}">Returned</see>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Array"/>
    /// </typeparam>
    public static T[] Rent<T>()
    {
        return ArrayPool<T>.Shared.Rent(MinCapacity);
    }

    /// <summary>
    /// Rents a <c>T[]</c> with at least a <see cref="Array.Length"/> of <paramref name="minLength"/>
    /// from <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// that should be <see cref="Return{T}">Returned</see>
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of items in the <see cref="Array"/>
    /// </typeparam>
    /// <param name="minLength">
    /// The minimum <see cref="Array.Length"/> the returned array can have
    /// </param>
    public static T[] Rent<T>(int minLength)
    {
        minLength = minLength switch
        {
            < MinCapacity => MinCapacity,
            > MaxCapacity => MaxCapacity,
            _ => minLength,
        };
        return ArrayPool<T>.Shared.Rent(minLength);
    }

    /// <summary>
    /// Returns a <see cref="Rent{T}()">Rented</see> <c>T[]</c> back to <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </summary>
    /// <param name="array">
    /// The <c>T[]</c> to return, may be <c>null</c>
    /// </param>
    /// <param name="clearArray">
    /// Set to <c>true</c> to have the array cleared before being returned to <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </param>
    public static void Return<T>(T[]? array, bool clearArray = false)
    {
        if (array is not null && array.Length > 0)
        {
            ArrayPool<T>.Shared.Return(array, clearArray);
        }
    }
}