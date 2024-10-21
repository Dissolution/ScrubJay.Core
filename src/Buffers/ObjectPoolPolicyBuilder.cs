using ScrubJay.Collections;
using ScrubJay.Fluent;

namespace ScrubJay.Buffers;

public sealed class ObjectPoolPolicyBuilder<T> : FluentRecordBuilder<ObjectPoolPolicyBuilder<T>, ObjectPoolPolicy<T>>
    where T : class
{
    public ObjectPoolPolicyBuilder<T> Create(Func<T> createInstance)
    {
        Validate.ThrowIfNull(createInstance);
        _record.CreateInstance = createInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> Clean(Func<T, bool> tryCleanInstance)
    {
        Validate.ThrowIfNull(tryCleanInstance);
        _record.TryCleanInstance = tryCleanInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> Clean(Action<T> cleanInstance)
    {
        Validate.ThrowIfNull(cleanInstance);
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
        Validate.ThrowIfNull(disposeInstance);
        _record.DisposeInstance = disposeInstance;
        return _builder;
    }

    public ObjectPoolPolicyBuilder<T> MaxCapacity(int maxCapacity)
    {
        Validate.InBounds(maxCapacity, Bound.Inclusive(ObjectPool.MinCapacity), Bound.Inclusive(ObjectPool.MaxCapacity)).OkOrThrow();
        _record.MaxCapacity = maxCapacity;
        return _builder;
    }
}