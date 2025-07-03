using System.Collections.Concurrent;

namespace ScrubJay.Collections;

/// <summary>
/// A <see cref="ConcurrentDictionary{K,TValue}"/> where <c>K</c> is <see cref="Type"/> that supports using a generic type
/// instead of a key
/// </summary>
/// <typeparam name="V">
/// The <see cref="Type"/> of values associated with the <see cref="Type"/> keys
/// </typeparam>
[PublicAPI]
public class ConcurrentTypeMap<V> : ConcurrentDictionary<Type, V>
{
    public ConcurrentTypeMap() { }

    public ConcurrentTypeMap(int capacity)
        : base(Environment.ProcessorCount, capacity) { }

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

    public V GetOrAdd<K>(V value)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.GetOrAdd(typeof(K), value);

    public V GetOrAdd<K>(Func<V> addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.GetOrAdd(typeof(K), _ => addValue());

    public V GetOrAdd<K>(Func<Type, V> addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.GetOrAdd(typeof(K), addValue);

    public bool TryAdd<K>(V addValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.TryAdd(typeof(K), addValue);

    public V AddOrUpdate(Type type, V value)
        => base.AddOrUpdate(type, value, (_, _) => value);

    public V AddOrUpdate<K>(V value)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), value, (_, _) => value);

    public V AddOrUpdate<K>(V addValue, Func<V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), _ => addValue, (_, existingValue) => updateValue(existingValue));

    public V AddOrUpdate<K>(V addValue, Func<Type, V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), _ => addValue, updateValue);

    public V AddOrUpdate<K>(Func<V> createValue, Func<V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), _ => createValue(), (_, existingValue) => updateValue(existingValue));

    public V AddOrUpdate<K>(Func<V> createValue, Func<Type, V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), _ => createValue(), updateValue);

    public V AddOrUpdate<K>(Func<Type, V> createValue, Func<V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), createValue, (_, existingValue) => updateValue(existingValue));

    public V AddOrUpdate<K>(Func<Type, V> createValue, Func<Type, V, V> updateValue)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.AddOrUpdate(typeof(K), createValue, updateValue);

    public bool TryRemove(Type keyType)
        => base.TryRemove(keyType, out _);

    public bool TryRemove<K>()
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.TryRemove(typeof(K), out _);

    public bool TryRemove<K>([MaybeNullWhen(false)] out V value)
#if NET9_0_OR_GREATER
        where K : allows ref struct
#endif
        => base.TryRemove(typeof(K), out value);
}
