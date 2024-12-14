using ScrubJay.Constraints;
using ScrubJay.Fluent;

namespace ScrubJay.Buffers;

public sealed class ObjectPoolPolicyBuilder<T> : FluentRecordBuilder<ObjectPoolPolicyBuilder<T>, ObjectPoolPolicy<T>>
    where T : class
{
    public ObjectPoolPolicyBuilder() : base(new())
    {
    }

    public ObjectPoolPolicyBuilder(ObjectPoolPolicy<T> record) : base(record)
    {
    }

    public ObjectPoolPolicyBuilder<T> Create(Func<T> createInstance)
    {
        Throw.IfNull(createInstance);
        Record.CreateInstance = createInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> Clean(Func<T, bool> tryCleanInstance)
    {
        Throw.IfNull(tryCleanInstance);
        Record.TryCleanInstance = tryCleanInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> Clean(Action<T> cleanInstance)
    {
        Throw.IfNull(cleanInstance);
        Record.TryCleanInstance = inst =>
        {
            cleanInstance(inst);
            return true;
        };
        return _builder;
    }

#pragma warning disable S2953
    public ObjectPoolPolicyBuilder<T> Dispose(Action<T> disposeInstance)
    {
        Throw.IfNull(disposeInstance);
        Record.DisposeInstance = disposeInstance;
        return _builder;
    }
#pragma warning restore S2953

    public ObjectPoolPolicyBuilder<T> MaxCapacity(int maxCapacity)
    {
        Validate.InBounds(maxCapacity, Bound.Inclusive(ObjectPool.MinCapacity), Bound.Inclusive(ObjectPool.MaxCapacity)).ThrowIfError();
        Record.Capacity = maxCapacity;
        return _builder;
    }
}
