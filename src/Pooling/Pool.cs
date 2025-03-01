using static ScrubJay.Constraints.GenericTypeConstraint;

namespace ScrubJay.Pooling;

/// <summary>
///
/// </summary>
[PublicAPI]
public static class Pool
{
    [MustDisposeResource]
    public static InstancePool<T> FromPolicy<T>(InstancePoolPolicy<T> policy)
        where T : class
        => new(policy);

    [MustDisposeResource]
    public static InstancePool<T> Create<T>(Func<T> createInstance)
        where T : class
        => new(new(createInstance));

    [MustDisposeResource]
    public static InstancePool<T> Create<T>(
        Func<T> createInstance,
        Func<T, bool>? tryCleanInstance,
        Action<T>? disposeInstance = null)
        where T : class
        => new(new(createInstance, tryCleanInstance, disposeInstance));

    [MustDisposeResource]
    public static InstancePool<T> Create<T>(
        Func<T> createInstance,
        Action<T>? cleanInstance,
        Action<T>? disposeInstance = null)
        where T : class
    {
        Func<T, bool>? tryCleanInstance = null;
        if (cleanInstance is not null)
        {
            tryCleanInstance = inst =>
            {
                cleanInstance(inst);
                return true;
            };
        }
        return new(new(createInstance, tryCleanInstance, disposeInstance));
    }

    public static InstancePool<T> Default<T>(IsNew<T> _ = default)
        where T : class, new()
        => new(new InstancePoolPolicy<T>(static () => new()));

    public static InstancePool<T> Default<T>(IsDisposable<T> _ = default)
        where T : class, IDisposable, new()
        => new(new InstancePoolPolicy<T>(
            createInstance: static () => new(),
            cleanInstance: null,
            disposeInstance: static instance => instance.Dispose()));


}
