namespace ScrubJay.Functional;

public struct Box
{
    private object? _obj;

    public bool ContainsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _obj is null;
    }

    public Box(object? obj)
    {
        _obj = obj;
    }

    public Option<T> Get<T>() => _obj.As<T>();

    public T GetOrSet<T>(T value)
    {
        if (_obj is T val)
        {
            return val;
        }

        _obj = (object?)value;
        return value;
    }

    public void Set<T>(T value)
    {
        _obj = (object?)value;
    }
}
