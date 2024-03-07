using System.Data;

namespace ScrubJay.Collections;

public sealed class CollectionWrapper<T> : ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>
{
    private readonly ICollection _collection;

    public int Count => _collection.Count;

    public bool IsReadOnly => true;

    public CollectionWrapper(ICollection collection)
    {
        _collection = collection;
    }

    public void Add(T item) => throw new ReadOnlyException();

    public void Clear() => throw new ReadOnlyException();

    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public bool Remove(T item) => throw new ReadOnlyException();


    public bool Contains(T item) => _collection.OfType<T>().Any(thing => EqualityComparer<T>.Default.Equals(thing, item));

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator() => _collection.OfType<T>().GetEnumerator();
}

public sealed class DictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly IDictionary _dictionary;
    private CollectionWrapper<TKey>? _keys = null;
    private CollectionWrapper<TValue>? _values = null;

    public TValue this[TKey key]
    {
        get => _dictionary[key].AsValid<TValue>();
        set => _dictionary[key] = value;
    }

    public int Count => _dictionary.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _dictionary.IsReadOnly;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys ??= new CollectionWrapper<TKey>(_dictionary.Keys);
    public IReadOnlyCollection<TKey> Keys => _keys ??= new CollectionWrapper<TKey>(_dictionary.Keys);

    ICollection<TValue> IDictionary<TKey, TValue>.Values => _values ??= new CollectionWrapper<TValue>(_dictionary.Values);
    public IReadOnlyCollection<TValue> Values => _values ??= new CollectionWrapper<TValue>(_dictionary.Values);

    public DictionaryWrapper(IDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    public void Add(KeyValuePair<TKey, TValue> pair) => Add(pair.Key, pair.Value);

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.Contains(key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> pair)
    {
        return TryGetValue(pair.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, pair.Value);
    }

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
        if (_dictionary.Contains(key))
        {
            return _dictionary[key].Is(out value);
        }

        value = default;
        return false;
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(TKey key)
    {
        if (_dictionary.Contains(key))
        {
            _dictionary.Remove(key);
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> pair)
    {
        if (Contains(pair))
            return Remove(pair.Key);
        return false;
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (DictionaryEntry entry in _dictionary)
        {
            if (entry.Key.Is<TKey>(out var key) &&
                entry.Value.Is<TValue>(out var value))
            {
                yield return new(key, value);
            }
        }
    }
}