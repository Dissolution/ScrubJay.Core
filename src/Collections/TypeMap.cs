// ReSharper disable RedundantBaseQualifier

namespace ScrubJay.Collections;

/// <summary>
/// A <see cref="Dictionary{TKey,TValue}"/> where <c>TKey</c> is <see cref="Type"/> that supports using a generic type
/// instead of a key
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of values associated with the <see cref="Type"/> keys
/// </typeparam>
[PublicAPI]
public class TypeMap<TValue> : Dictionary<Type, TValue>
{
    public TypeMap() { }

    public TypeMap(int capacity)
        : base(capacity) { }

    public bool ContainsKey<TKey>()
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.ContainsKey(typeof(TKey));

    public bool TryGetValue<TKey>([MaybeNullWhen(false)] out TValue value)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.TryGetValue(typeof(TKey), out value);

    public TValue GetOrAdd<TKey>(TValue addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (!base.TryGetValue(keyType, out var value))
        {
            value = addValue;
            base.Add(keyType, addValue);
        }
        return value;
    }

    public TValue GetOrAdd<TKey>(Fn<TValue> addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (!base.TryGetValue(keyType, out var value))
        {
            value = addValue();
            base.Add(keyType, value);
        }
        return value;
    }

    public TValue GetOrAdd<TKey>(Fn<Type, TValue> addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (!base.TryGetValue(keyType, out var value))
        {
            value = addValue(keyType);
            base.Add(keyType, value);
        }
        return value;
    }

    public bool TryAdd<TKey>(TValue addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.ContainsKey(keyType))
            return false;
        base.Add(keyType, addValue);
        return true;
    }

    public TValue AddOrUpdate(Type type, TValue value)
    {
        base[type] = value;
        return value;
    }

    public TValue AddOrUpdate<TKey>(TValue value)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        base[typeof(TKey)] = value;
        return value;
    }

    public TValue AddOrUpdate<TKey>(TValue addValue, Func<TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out var value))
        {
            value = updateValue(value);
            base[keyType] = value;
            return value;
        }
        else
        {
            base[keyType] = addValue;
            return addValue;
        }
    }

    public TValue AddOrUpdate<TKey>(TValue addValue, Func<Type, TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out var value))
        {
            value = updateValue(keyType, value);
            base[keyType] = value;
            return value;
        }
        else
        {
            base[keyType] = addValue;
            return addValue;
        }
    }

    public TValue AddOrUpdate<TKey>(Func<TValue> createValue, Func<TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out var value))
        {
            value = updateValue(value);
        }
        else
        {
            value = createValue();
        }
        base[keyType] = value;
        return value;
    }

    public TValue AddOrUpdate<TKey>(Func<TValue> createValue, Func<Type, TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out var value))
        {
            value = updateValue(keyType, value);
        }
        else
        {
            value = createValue();
        }
        base[keyType] = value;
        return value;
    }

    public TValue AddOrUpdate<TKey>(Func<Type, TValue> createValue, Func<TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out var value))
        {
            value = updateValue(value);
        }
        else
        {
            value = createValue(keyType);
        }
        base[keyType] = value;
        return value;
    }

    public TValue AddOrUpdate<TKey>(Func<Type, TValue> createValue, Func<Type, TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out var value))
        {
            value = updateValue(keyType, value);
        }
        else
        {
            value = createValue(keyType);
        }
        base[keyType] = value;
        return value;
    }

    public bool TryRemove(Type keyType) => base.Remove(keyType);

    public bool TryRemove<TKey>()
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.Remove(typeof(TKey));

    public bool TryRemove<TKey>([MaybeNullWhen(false)] out TValue value)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
    {
        var keyType = typeof(TKey);
        if (base.TryGetValue(keyType, out value))
            return base.Remove(keyType);
        value = default;
        return false;
    }
}
