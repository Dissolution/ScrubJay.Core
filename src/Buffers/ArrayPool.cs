namespace ScrubJay.Buffers;

[PublicAPI]
public sealed class ArrayPool<T> : IArrayPool<T>
{
    public static ArrayPool<T> Shared { get; } = new ArrayPool<T>(System.Buffers.ArrayPool<T>.Shared);

    private const int MINIMUM_ARRAY_LENGTH = 0x10;
    private const int MAXIMUM_ARRAY_LENGTH = 0x40000000; // < string.MaxLength, < Array.MaxLength

    private readonly System.Buffers.ArrayPool<T> _arrayPool;
    private readonly bool _clearArray;

    public int MinimumLength { get; }

    private ArrayPool(System.Buffers.ArrayPool<T> arrayPool)
    {
        if (typeof(T).IsValueType)
        {
            this.MinimumLength = 64;
            _clearArray = false;
        }
        else
        {
            this.MinimumLength = MINIMUM_ARRAY_LENGTH;
            _clearArray = true;
        }
        _arrayPool = arrayPool;
    }

    public ArrayPool()
    {
        if (typeof(T).IsValueType)
        {
            this.MinimumLength = 64;
            _clearArray = false;
        }
        else
        {
            this.MinimumLength = MINIMUM_ARRAY_LENGTH;
            _clearArray = true;
        }
        _arrayPool = System.Buffers.ArrayPool<T>.Create();
    }

    public ArrayPool(int minimumLength)
    {
        MinimumLength = minimumLength.Clamp(MINIMUM_ARRAY_LENGTH, MINIMUM_ARRAY_LENGTH);
        _clearArray = !typeof(T).IsValueType;
        _arrayPool = System.Buffers.ArrayPool<T>.Create();
    }

    public ArrayPool(int minimumLength, bool clearArray)
    {
        MinimumLength = minimumLength.Clamp(MINIMUM_ARRAY_LENGTH, MINIMUM_ARRAY_LENGTH);
        _clearArray = clearArray;
        _arrayPool = System.Buffers.ArrayPool<T>.Create();
    }

    public T[] Rent() => _arrayPool.Rent(MinimumLength);

    public T[] Rent(int minLength)
    {
        if (minLength < MinimumLength)
            return _arrayPool.Rent(MinimumLength);
        if (minLength > MAXIMUM_ARRAY_LENGTH)
            return _arrayPool.Rent(MAXIMUM_ARRAY_LENGTH);
        return _arrayPool.Rent(minLength);
    }

    public void Return(T[]? instance)
    {
        if (instance is not null && instance.Length > 0)
        {
            _arrayPool.Return(instance, _clearArray);
        }
    }
}
