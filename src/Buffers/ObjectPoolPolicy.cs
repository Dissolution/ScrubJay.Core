namespace ScrubJay.Buffers;

public record class ObjectPoolPolicy
{
    public static ObjectPoolPolicy<T> Default<T>(Constraints.IsNew<T> _ = default)
        where T : class, new()
    {
        return new()
        {
            CreateInstance = static () => new(),
        };
    }
    
    public static ObjectPoolPolicy<T> Default<T>(Constraints.IsDisposable<T> _ = default)
        where T : class, IDisposable
    {
        return new()
        {
            DisposeInstance = static value => value.Dispose(),
        };
    }
    
    public static ObjectPoolPolicy<T> Default<T>(Constraints.IsDisposableNew<T> _ = default)
        where T : class, IDisposable, new()
    {
        return new()
        {
            CreateInstance = static () => new(),
            DisposeInstance = static value => value.Dispose(),
        };
    }
}

public sealed record class ObjectPoolPolicy<T> : ObjectPoolPolicy
    where T : class
{
    public static ObjectPoolPolicy<T> Default => new();
    
    public Func<T> CreateInstance { get; set; }
    public Func<T, bool> TryCleanInstance { get; set; }
    public Action<T> DisposeInstance { get; set; }
    public int MaxCapacity { get; set; }

    public ObjectPoolPolicy()
    {
        CreateInstance = Activator.CreateInstance<T>;
        TryCleanInstance = static _ => true;
        DisposeInstance = static _ => { };
        MaxCapacity = ObjectPool.DefaultMaxCapacity;
    }
}