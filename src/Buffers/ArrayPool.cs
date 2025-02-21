namespace ScrubJay.Buffers;

[PublicAPI]
public sealed class ArrayPool<T> : IArrayPool<T>
{
    public static ArrayPool<T> Shared { get; } = new ArrayPool<T>(System.Buffers.ArrayPool<T>.Shared);

    private const int MinimumArrayLength = 0x10;
    private const int MaximumArrayLength = 0x40000000; // < string.MaxLength, < Array.MaxLength

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
            this.MinimumLength = MinimumArrayLength;
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
            this.MinimumLength = MinimumArrayLength;
            _clearArray = true;
        }
        _arrayPool = System.Buffers.ArrayPool<T>.Create();
    }

    public ArrayPool(int minimumLength)
    {
        MinimumLength = minimumLength.Clamp(MinimumArrayLength, MinimumArrayLength);
        _clearArray = !typeof(T).IsValueType;
        _arrayPool = System.Buffers.ArrayPool<T>.Create();
    }

    public ArrayPool(int minimumLength, bool clearArray)
    {
        MinimumLength = minimumLength.Clamp(MinimumArrayLength, MinimumArrayLength);
        _clearArray = clearArray;
        _arrayPool = System.Buffers.ArrayPool<T>.Create();
    }

    public T[] Rent() => _arrayPool.Rent(MinimumLength);

    public T[] Rent(int minLength)
    {
        if (minLength <= 0)
            return [];
        if (minLength < MinimumLength)
            return _arrayPool.Rent(MinimumLength);
        if (minLength > MaximumArrayLength)
            return _arrayPool.Rent(MaximumArrayLength);
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
