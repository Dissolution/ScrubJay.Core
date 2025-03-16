using System.Buffers;

namespace ScrubJay.Pooling;

[PublicAPI]
public sealed class ArrayInstancePool<T> : IArrayInstancePool<T>
{
    public static ArrayInstancePool<T> Shared { get; } = new ArrayInstancePool<T>(ArrayPool<T>.Shared, null, null);

    /// <summary>
    /// The maximum length of any returned array
    /// </summary>
    /// <remarks>
    /// This is less than <c>string.MaxLength</c> and less than <c>Array.MaxLength</c>
    /// </remarks>
    private const int MAXIMUM_ARRAY_LENGTH = 0x40000000;


    private readonly ArrayPool<T> _arrayPool;
    private readonly int _minimumLength;
    private readonly bool _clearArray;

    private ArrayInstancePool(ArrayPool<T> arrayPool, int? minLength, bool? clearArray)
    {
        _arrayPool = arrayPool;
#if NETFRAMEWORK || NETSTANDARD2_0
        if (typeof(T).IsValueType)
        {
            // Set a higher min as these cost less memory to store (usually)
            if (minLength.TryGetValue(out int minLen))
            {
                _minimumLength = minLen.Clamp(0, MAXIMUM_ARRAY_LENGTH);
            }
            else
            {
                _minimumLength = 1024;
            }
        }
        else
        {
            if (minLength.TryGetValue(out int minLen))
            {
                _minimumLength = minLen.Clamp(0, MAXIMUM_ARRAY_LENGTH);
            }
            else
            {
                // 16 classes
                _minimumLength = 16;
            }
        }

        // We want to clear these arrays to clear any references they may contain
        _clearArray = clearArray ?? true;
#else
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            if (minLength.TryGetValue(out int minLen))
            {
                _minimumLength = minLen.Clamp(0, MAXIMUM_ARRAY_LENGTH);
            }
            else
            {
                // 16 classes
                _minimumLength = 16;
            }

            // We want to clear these arrays to clear any references they contain
            _clearArray = clearArray ?? true;
        }
        else
        {
            // Set a higher min as these cost less memory to store (usually)
            if (minLength.TryGetValue(out int minLen))
            {
                _minimumLength = minLen.Clamp(0, MAXIMUM_ARRAY_LENGTH);
            }
            else
            {
                // 1KB of values
                _minimumLength = (1024 * 1024) / Notsafe.SizeOf<T>();
            }

            // No need to clear
            _clearArray = clearArray ?? false;
        }
#endif
    }

    public ArrayInstancePool()
        : this(ArrayPool<T>.Create(), null, null)
    {
    }

    public ArrayInstancePool(int minimumLength)
        : this(ArrayPool<T>.Create(), minimumLength, null)
    {
    }

    public ArrayInstancePool(int minimumLength, bool clearArray)
        : this(ArrayPool<T>.Create(), minimumLength, clearArray)
    {
    }

    public T[] Rent() => _arrayPool.Rent(_minimumLength);

    public T[] Rent(int minLength) => _arrayPool.Rent(minLength.Clamp(_minimumLength, MAXIMUM_ARRAY_LENGTH));

    public void Return(T[]? instance)
    {
        if (instance is not null && (instance.Length > 0))
        {
            _arrayPool.Return(instance, _clearArray);
        }
    }
}
