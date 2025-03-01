namespace ScrubJay.Pooling;

[PublicAPI]
public record class InstancePoolPolicy<T>
    where T : class
{
    public required Func<T> CreateInstance { get; init; }

    public Func<T, bool>? TryCleanInstance { get; init; }

    public Action<T>? DisposeInstance { get; init; }

    public int MaxCapacity { get; init; } = Environment.ProcessorCount * 2;

    public InstancePoolPolicy() { }

    [SetsRequiredMembers]
    public InstancePoolPolicy(Func<T> createInstance)
    {
        CreateInstance = createInstance;
    }

    [SetsRequiredMembers]
    public InstancePoolPolicy(
        Func<T> createInstance,
        Func<T, bool>? tryCleanInstance,
        Action<T>? disposeInstance = null)
    {
        CreateInstance = createInstance;
        TryCleanInstance = tryCleanInstance;
        DisposeInstance = disposeInstance;
    }

    [SetsRequiredMembers]
    public InstancePoolPolicy(
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
