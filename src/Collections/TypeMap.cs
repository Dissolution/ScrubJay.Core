// ReSharper disable RedundantBaseQualifier

namespace ScrubJay.Collections;

/// <summary>
/// A <see cref="Dictionary{K,TValue}"/> where <c>K</c> is <see cref="Type"/> that supports using a generic type
/// instead of a key
/// </summary>
/// <typeparam name="V">
/// The <see cref="Type"/> of values associated with the <see cref="Type"/> keys
/// </typeparam>
[PublicAPI]
public class TypeMap<V> : Dictionary<Type, V>
{
    public TypeMap() { }

    public TypeMap(int capacity)
        : base(capacity) { }

    public bool ContainsKey<K>()
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.ContainsKey(typeof(K));

    public bool TryGetValue<K>([MaybeNullWhen(false)] out V value)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.TryGetValue(typeof(K), out value);

    public V GetOrAdd<K>(V addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
        if (!base.TryGetValue(keyType, out var value))
        {
            value = addValue;
            base.Add(keyType, addValue);
        }
        return value;
    }

    public V GetOrAdd<K>(Fn<V> addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
        if (!base.TryGetValue(keyType, out var value))
        {
            value = addValue();
            base.Add(keyType, value);
        }
        return value;
    }

    public V GetOrAdd<K>(Fn<Type, V> addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
        if (!base.TryGetValue(keyType, out var value))
        {
            value = addValue(keyType);
            base.Add(keyType, value);
        }
        return value;
    }

    public bool TryAdd<K>(V addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
        if (base.ContainsKey(keyType))
            return false;
        base.Add(keyType, addValue);
        return true;
    }

    public V AddOrUpdate(Type type, V value)
    {
        base[type] = value;
        return value;
    }

    public V AddOrUpdate<K>(V value)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        base[typeof(K)] = value;
        return value;
    }

    public V AddOrUpdate<K>(V addValue, Func<V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
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

    public V AddOrUpdate<K>(V addValue, Func<Type, V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
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

    public V AddOrUpdate<K>(Func<V> createValue, Func<V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
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

    public V AddOrUpdate<K>(Func<V> createValue, Func<Type, V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
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

    public V AddOrUpdate<K>(Func<Type, V> createValue, Func<V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
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

    public V AddOrUpdate<K>(Func<Type, V> createValue, Func<Type, V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
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

    public bool TryRemove<K>()
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.Remove(typeof(K));

    public bool TryRemove<K>([MaybeNullWhen(false)] out V value)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
    {
        var keyType = typeof(K);
        if (base.TryGetValue(keyType, out value))
            return base.Remove(keyType);
        value = default;
        return false;
    }
}
