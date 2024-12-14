// ReSharper disable MethodOverloadWithOptionalParameter

using ScrubJay.Constraints;

namespace ScrubJay.Buffers;

public record class ObjectPoolPolicy
{
    public static ObjectPoolPolicy<T> Default<T>(T? _ = default)
        where T : class
        => new();

    public static ObjectPoolPolicy<T> Default<T>(GenericTypeConstraint.IsNew<T> _ = default)
        where T : class, new()
    {
        return new()
        {
            CreateInstance = static () => new(),
        };
    }

    public static ObjectPoolPolicy<T> Default<T>(GenericTypeConstraint.IsDisposable<T> _ = default)
        where T : class, IDisposable
    {
        return new()
        {
            DisposeInstance = static value => value.Dispose(),
        };
    }

    public static ObjectPoolPolicy<T> Default<T>(GenericTypeConstraint.IsDisposableNew<T> _ = default)
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
    public Func<T> CreateInstance { get; set; }
    public Func<T, bool> TryCleanInstance { get; set; }
    public Action<T> DisposeInstance { get; set; }
    public int Capacity { get; set; }

    public ObjectPoolPolicy()
    {
        CreateInstance = Activator.CreateInstance<T>;
        TryCleanInstance = static _ => true;
        DisposeInstance = static _ => { };
        Capacity = ObjectPool.DefaultCapacity;
    }
}