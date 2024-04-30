using System.Collections.Concurrent;

namespace ScrubJay.Collections;

/// <summary>
/// A <see cref="ConcurrentDictionary{TKey,TValue}"/> where <c>TKey</c> is <see cref="Type"/>
/// </summary>
/// <typeparam name="TValue">
/// The <see cref="Type"/> of values associated with the <see cref="Type"/> keys
/// </typeparam>
public class ConcurrentTypeMap<TValue> : ConcurrentDictionary<Type, TValue>
{
    public ConcurrentTypeMap()
        : base()
    {
    }

    public ConcurrentTypeMap(int capacity)
        : base(Environment.ProcessorCount, capacity)
    {
    }

    public bool ContainsKey<TKey>()
        => base.ContainsKey(typeof(TKey));

    public bool TryGetValue<TKey>([MaybeNullWhen(false)] out TValue value)
        => base.TryGetValue(typeof(TKey), out value);

    public TValue GetOrAdd<TKey>(TValue addValue)
        => base.GetOrAdd(typeof(TKey), addValue);

    public TValue GetOrAdd<TKey>(Func<Type, TValue> addValue)
        => base.GetOrAdd(typeof(TKey), addValue);

    public TValue AddOrUpdate(Type type, TValue value)
        => base.AddOrUpdate(type, value, (_, _) => value);

    public TValue AddOrUpdate<TKey>(TValue value)
        => base.AddOrUpdate(typeof(TKey), value, (_, _) => value);

    public TValue AddOrUpdate<TKey>(TValue addValue, Func<TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(TKey), _ => addValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<TKey>(TValue addValue, Func<Type, TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(TKey), _ => addValue, updateValue);

    public TValue AddOrUpdate<TKey>(Func<TValue> createValue, Func<TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(TKey), _ => createValue(), (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<TKey>(Func<TValue> createValue, Func<Type, TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(TKey), _ => createValue(), updateValue);

    public TValue AddOrUpdate<TKey>(Func<Type, TValue> createValue, Func<TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(TKey), createValue, (_, existingValue) => updateValue(existingValue));

    public TValue AddOrUpdate<TKey>(Func<Type, TValue> createValue, Func<Type, TValue, TValue> updateValue)
        => base.AddOrUpdate(typeof(TKey), createValue, updateValue);
}