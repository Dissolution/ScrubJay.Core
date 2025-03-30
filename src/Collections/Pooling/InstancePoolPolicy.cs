namespace ScrubJay.Collections.Pooling;

[PublicAPI]
public record class InstancePoolPolicy<T>
    where T : class
{
    public required Func<T> CreateInstance { get; init; }

    public Func<T, bool>? TryReadyInstance { get; init; }

    public Func<T, bool>? TryCleanInstance { get; init; }

    public Action<T>? DisposeInstance { get; init; }

    public int MaxCapacity { get; init; } = Environment.ProcessorCount * 2;

    public InstancePoolPolicy() { }

    [SetsRequiredMembers]
    public InstancePoolPolicy(
        Func<T> createInstance,
        Func<T, bool>? tryReadyInstance = null,
        Func<T, bool>? tryCleanInstance = null,
        Action<T>? disposeInstance = null)
    {
        CreateInstance = createInstance;
        TryReadyInstance = tryReadyInstance;
        TryCleanInstance = tryCleanInstance;
        DisposeInstance = disposeInstance;
    }
}
