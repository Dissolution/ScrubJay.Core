using static ScrubJay.Utilities.GenericTypeConstraint;

namespace ScrubJay.Collections.Pooling;

/// <summary>
///
/// </summary>
[PublicAPI]
public static class InstancePool
{
    [MustDisposeResource]
    public static InstancePool<T> FromPolicy<T>(InstancePoolPolicy<T> policy)
        where T : class
        => new(policy);

    [MustDisposeResource]
    public static InstancePool<T> Create<T>(
        Func<T> createInstance,
        Func<T, bool>? tryReadyInstance = null,
        Action<T>? readyInstance = null,
        Func<T, bool>? tryCleanInstance = null,
        Action<T>? cleanInstance = null,
        Action<T>? disposeInstance = null)
        where T : class
    {
        if (tryReadyInstance is null && readyInstance is not null)
        {
            tryReadyInstance = inst =>
            {
                readyInstance(inst);
                return true;
            };
        }
        if (tryCleanInstance is null && cleanInstance is not null)
        {
            tryCleanInstance = inst =>
            {
                cleanInstance(inst);
                return true;
            };
        }
        var policy = new InstancePoolPolicy<T>(
            createInstance: createInstance,
            tryReadyInstance: tryReadyInstance,
            tryCleanInstance: tryCleanInstance,
            disposeInstance: disposeInstance);
        return new InstancePool<T>(policy);
    }


    public static InstancePool<T> Default<T>(IsNew<T> _ = default)
        where T : class, new()
        => new(new InstancePoolPolicy<T>(static () => new()));

    public static InstancePool<T> Default<T>(IsDisposableNew<T> _ = default)
        where T : class, IDisposable, new()
        => new(new InstancePoolPolicy<T>(
            createInstance: static () => new(),
            disposeInstance: static instance => instance.Dispose()));


}
