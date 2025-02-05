using ScrubJay.Constraints;
using ScrubJay.Fluent;

namespace ScrubJay.Buffers;

[PublicAPI]
public static class ObjectPoolPolicy
{
    public static ObjectPoolPolicy<T> ForType<T>(GenericTypeConstraint.IsNew<T> _ = default)
        where T : class, new()
    {
        return new ObjectPoolPolicy<T>
        {
            CreateInstance = static () => new T(),
        };
    }

    public static ObjectPoolPolicy<T> ForType<T>(GenericTypeConstraint.IsDisposableNew<T> _ = default)
        where T : class, IDisposable, new()
    {
        return new ObjectPoolPolicy<T>
        {
            CreateInstance = static () => new T(),
            DisposeInstance = static inst => inst.Dispose(),
        };
    }

    public static ObjectPoolPolicy<T> ForType<T>(Func<T> createInstance,
        GenericTypeConstraint.IsDisposable<T> _ = default)
        where T : class, IDisposable
    {
        return new ObjectPoolPolicy<T>
        {
            CreateInstance = createInstance,
            DisposeInstance = static inst => inst.Dispose(),
        };
    }

    public static ObjectPoolPolicy<T> ForType<T>(
        Func<T> createInstance,
        Func<T, bool>? tryCleanInstance = null,
        Action<T>? disposeInstance = null)
        where T : class
    {
        return new ObjectPoolPolicy<T>
        {
            CreateInstance = createInstance,
            TryCleanInstance = tryCleanInstance,
            DisposeInstance = disposeInstance,
        };
    }
}

[PublicAPI]
public record class ObjectPoolPolicy<T>
    where T : class
{
    public required Func<T> CreateInstance { get; init; }

    public Func<T, bool>? TryCleanInstance { get; init; }

    public Action<T>? DisposeInstance { get; init; }

    public int MaxCapacity { get; init; } = Environment.ProcessorCount * 2;

    public ObjectPoolPolicy() { }

    [SetsRequiredMembers]
    public ObjectPoolPolicy(Func<T> createInstance)
    {
        CreateInstance = createInstance;
    }

    [SetsRequiredMembers]
    public ObjectPoolPolicy(
        Func<T> createInstance,
        Func<T, bool>? tryCleanInstance,
        Action<T>? disposeInstance = null)
    {
        CreateInstance = createInstance;
        TryCleanInstance = tryCleanInstance;
        DisposeInstance = disposeInstance;
    }

    [SetsRequiredMembers]
    public ObjectPoolPolicy(
        Func<T> createInstance,
        Action<T>? cleanInstance,
        Action<T>? disposeInstance = null)
    {
        CreateInstance = createInstance;
        DisposeInstance = disposeInstance;
        if (cleanInstance is not null)
        {
            TryCleanInstance = inst =>
            {
                cleanInstance(inst);
                return true;
            };
        }
    }
}
