using ScrubJay.Constraints;
using ScrubJay.Fluent;

namespace ScrubJay.Buffers;

public sealed class ObjectPoolPolicyBuilder<T> : FluentRecordBuilder<ObjectPoolPolicyBuilder<T>, ObjectPoolPolicy<T>>
    where T : class
{
    public ObjectPoolPolicyBuilder<T> Create(Func<T> createInstance)
    {
        Throw.IfNull(createInstance);
        _record.CreateInstance = createInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> Clean(Func<T, bool> tryCleanInstance)
    {
        Throw.IfNull(tryCleanInstance);
        _record.TryCleanInstance = tryCleanInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> Clean(Action<T> cleanInstance)
    {
        Throw.IfNull(cleanInstance);
        _record.TryCleanInstance = inst =>
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
        _record.DisposeInstance = disposeInstance;
        return _builder;
    }
#pragma warning restore S2953

    public ObjectPoolPolicyBuilder<T> MaxCapacity(int maxCapacity)
    {
        Validate.InBounds(maxCapacity, Bound.Inclusive(ObjectPool.MinCapacity), Bound.Inclusive(ObjectPool.MaxCapacity)).ThrowIfError();
        _record.MaxCapacity = maxCapacity;
        return _builder;
    }
}