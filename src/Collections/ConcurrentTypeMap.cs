using System.Collections.Concurrent;

namespace ScrubJay.Collections;

/// <summary>
/// A <see cref="ConcurrentDictionary{TKey,TValue}"/> where <c>TKey</c> is <see cref="Type"/> that supports using a generic type
/// instead of a key
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of values associated with the <see cref="Type"/> keys
/// </typeparam>
[PublicAPI]
public class ConcurrentTypeMap<TValue> : ConcurrentDictionary<Type, TValue>
{
    public ConcurrentTypeMap() { }

    public ConcurrentTypeMap(int capacity)
        : base(Environment.ProcessorCount, capacity) { }

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

    public TValue GetOrAdd<TKey>(TValue value)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.GetOrAdd(typeof(TKey), value);

    public TValue GetOrAdd<TKey>(Func<TValue> addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.GetOrAdd(typeof(TKey), _ => addValue());

    public TValue GetOrAdd<TKey>(Func<Type, TValue> addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.GetOrAdd(typeof(TKey), addValue);

    public bool TryAdd<TKey>(TValue addValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.TryAdd(typeof(TKey), addValue);

    public TValue AddOrUpdate(Type type, TValue value)
        => base.AddOrUpdate(type, value, (_, _) => value);

    public TValue AddOrUpdate<TKey>(TValue value)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), value, (_, _) => value);

    public TValue AddOrUpdate<TKey>(TValue addValue, Func<TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), _ => addValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<TKey>(TValue addValue, Func<Type, TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), _ => addValue, updateValue);

    public TValue AddOrUpdate<TKey>(Func<TValue> createValue, Func<TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), _ => createValue(), (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<TKey>(Func<TValue> createValue, Func<Type, TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), _ => createValue(), updateValue);

    public TValue AddOrUpdate<TKey>(Func<Type, TValue> createValue, Func<TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), createValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<TKey>(Func<Type, TValue> createValue, Func<Type, TValue, TValue> updateValue)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.AddOrUpdate(typeof(TKey), createValue, updateValue);

    public bool TryRemove(Type keyType)
        => base.TryRemove(keyType, out _);

    public bool TryRemove<TKey>()
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.TryRemove(typeof(TKey), out _);

    public bool TryRemove<TKey>([MaybeNullWhen(false)] out TValue value)
#if NET9_0_OR_GREATER
        where TKey : allows ref struct
#endif
        => base.TryRemove(typeof(TKey), out value);
}
