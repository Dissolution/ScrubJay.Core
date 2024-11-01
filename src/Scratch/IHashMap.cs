namespace ScrubJay.Scratch;

public interface IHashMapKey<out TKey, TValue>
{
    TKey Key { get; }
    bool HasValue { get; }
    Option<TValue> Value { get; set; }

    bool TryAddValue(TValue value);
    void SetValue(TValue value);
    Option<TValue> TryRemoveValue();
}

public static class HashMapExtensions
{
    public static TValue AddOrUpdate<TKey, TValue>(
        this IHashMap<TKey, TValue> hashMap,
        TKey key,
        TValue addValue,
        Action<TValue> updateValue)
        where TValue : class
    {
        throw new NotImplementedException();
    }
}

public interface IHashMap<TKey, TValue> :
    IReadOnlyCollection<Pair<TKey, TValue>>,
    IEnumerable<Pair<TKey, TValue>>,
    IEnumerable
{
    IEqualityComparer<TKey> KeyEqualityComparer { get; init; }

    TValue this[TKey key] { get; set; }

    IReadOnlyCollection<TKey> Keys { get; }
    IReadOnlyCollection<TValue> Values { get; }


    IHashMap<TKey, TValue> Key(TKey key);


    Result<Pair<TKey, TValue>, Exception> TryAdd(TKey key, TValue value);
    Result<Pair<TKey, TValue>, Exception> TryAdd(Pair<TKey, TValue> pair);

    TValue AddOrUpdate(TKey key, TValue addValue, TValue updatedValue);
    TValue AddOrUpdate(TKey key, TValue addValue, Func<TValue, TValue> updateValue);
    TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValue);
    TValue AddOrUpdate(TKey key, Func<TValue> getAddValue, TValue updatedValue);
    TValue AddOrUpdate(TKey key, Func<TValue> getAddValue, Func<TValue, TValue> updateValue);
    TValue AddOrUpdate(TKey key, Func<TValue> getAddValue, Func<TKey, TValue, TValue> updateValue);
    TValue AddOrUpdate(TKey key, Func<TKey, TValue> getAddValue, TValue updatedValue);
    TValue AddOrUpdate(TKey key, Func<TKey, TValue> getAddValue, Func<TValue, TValue> updateValue);
    TValue AddOrUpdate(TKey key, Func<TKey, TValue> getAddValue, Func<TKey, TValue, TValue> updateValue);

    Pair<TKey, TValue> Set(TKey key, TValue value);
    Pair<TKey, TValue> Set(Pair<TKey, TValue> pair);

    bool ContainsKey(TKey key);
    bool ContainsValue(TValue value, IEqualityComparer<TValue>? valueEqualityComparer = null);
    bool Contains(TKey key, TValue value, IEqualityComparer<TValue>? valueEqualityComparer = null);
    bool Contains(Pair<TKey, TValue> pair);
    bool Contains(Pair<TKey, TValue> pair, IEqualityComparer<TValue>? valueEqualityComparer);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    Result<TValue, Exception> TryRemove(TKey key);
    Result<Pair<TKey, TValue>, Exception> TryRemove(Pair<TKey, TValue> pair);

    /// <summary>
    /// Sets the capacity of this dictionary to what it would be if it had been originally initialized with all its entries
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize the memory overhead
    /// once it is known that no new elements will be added.
    ///
    /// To allocate minimum size storage array, execute the following statements:
    ///
    /// dictionary.Clear();
    /// dictionary.TrimExcess();
    /// </remarks>
    void TrimExcess();

    /// <summary>
    /// Sets the capacity of this dictionary to hold up 'capacity' entries without any further expansion of its backing storage
    /// </summary>
    /// <remarks>
    /// This method can be used to minimize the memory overhead
    /// once it is known that no new elements will be added.
    /// </remarks>
    void TrimExcess(int capacity);

    void Clear();
    void CopyTo(Pair<TKey, TValue>[] array, int arrayIndex = 0);
    UberDict<TKey, TValue> Clone();

    /// <summary>
    /// Ensures that the dictionary can hold up to 'capacity' entries without any further expansion of its backing storage
    /// </summary>
    int EnsureCapacity(int capacity);

    UberDict<TKey, TValue>.UberDictEnumerator GetEnumerator();
}