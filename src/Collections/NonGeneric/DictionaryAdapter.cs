// Identifiers should have correct suffix

using ScrubJay.Exceptions;
using ScrubJay.Text.Rendering;

#pragma warning disable CA1710
// Properties should not copy collections
#pragma warning disable S2365
// Remove unnecessary cast (IDE0004)
#pragma warning disable IDE0004


namespace ScrubJay.Collections.NonGeneric;

/// <summary>
/// An <see cref="IDictionary{K,V}"/> wrapper around an <see cref="IDictionary"/>
/// </summary>
[PublicAPI]
public sealed class DictionaryAdapter<K, V> :
    IDictionary<K, V>, IDictionary,
    IReadOnlyDictionary<K, V>,
    ICollection<KeyValuePair<K, V>>, ICollection,
    IReadOnlyCollection<KeyValuePair<K, V>>,
    IEnumerable<KeyValuePair<K, V>>, IEnumerable
    where K : notnull
{
    [return: NotNull]
    private static K ObjectToKey(object? objKey,
        [CallerArgumentExpression(nameof(objKey))]
        string? keyName = null)
    {
        if (objKey.Is<K>(out var key))
            return key;
        throw Ex.Arg(objKey, $"Invalid Key - '{objKey}' is not a {typeof(K):@}", keyName);
    }

    [return: NotNullIfNotNull(nameof(objValue))]
    private static V? ObjectToValue(object? objValue,
        [CallerArgumentExpression(nameof(objValue))]
        string? valueName = null)
    {
        if (objValue.As<V>(out var value))
            return value;
        throw Ex.Arg(objValue, $"Invalid Value - '{objValue}' is not a {typeof(V):@}", valueName);
    }

    private sealed class KeyCollection : IReadOnlyCollection<K>, ICollection<K>, ICollection
    {
        private readonly DictionaryAdapter<K, V> _adapter;

        private ICollection UntypedKeys => _adapter._dictionary.Keys;

        object ICollection.SyncRoot => UntypedKeys.SyncRoot;

        bool ICollection.IsSynchronized => UntypedKeys.IsSynchronized;

        public int Count => _adapter.Count;

        public bool IsReadOnly => true;


        public KeyCollection(DictionaryAdapter<K, V> adapter)
        {
            _adapter = adapter;
        }

        void ICollection<K>.Add(K item) => Throw.IsReadOnly(this);

        bool ICollection<K>.Remove(K item) => Throw.IsReadOnly<KeyCollection, bool>(this);

        void ICollection<K>.Clear() => Throw.IsReadOnly(this);

        void ICollection.CopyTo(Array array, int index)
        {
            Validate.CanCopyTo(array, index, Count).ThrowIfError();

            var keys = UntypedKeys;
            foreach (object? key in keys)
            {
                array.SetValue(key, index++);
            }
        }

        void ICollection<K>.CopyTo(K[] array, int arrayIndex)
        {
            Validate.CanCopyTo(array, arrayIndex, Count).ThrowIfError();

            var keys = UntypedKeys;
            foreach (object? key in keys)
            {
                array[arrayIndex++] = ObjectToKey(key);
            }
        }

        public bool Contains(K key) => _adapter.ContainsKey(key);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<K> GetEnumerator()
        {
            var e = UntypedKeys.GetEnumerator();
            while (e.MoveNext())
            {
                K key = ObjectToKey(e.Current);
                yield return key;
            }
            e.Dispose();
        }
    }

    private sealed class ValueCollection : IReadOnlyCollection<V>, ICollection<V>, ICollection
    {
        private readonly DictionaryAdapter<K, V> _adapter;

        private ICollection UntypedValues => _adapter._dictionary.Values;

        object ICollection.SyncRoot => UntypedValues.SyncRoot;

        bool ICollection.IsSynchronized => UntypedValues.IsSynchronized;

        public int Count => _adapter.Count;

        public bool IsReadOnly => true;

        public ValueCollection(DictionaryAdapter<K, V> adapter)
        {
            _adapter = adapter;
        }

        void ICollection<V>.Add(V item) => throw Ex.Invalid($"A DictionaryAdapter's Keys are readonly");

        bool ICollection<V>.Remove(V item) => throw Ex.Invalid($"A DictionaryAdapter's Keys are readonly");

        void ICollection<V>.Clear() => throw Ex.Invalid($"A DictionaryAdapter's Keys are readonly");

        void ICollection.CopyTo(Array array, int index)
        {
            Validate.CanCopyTo(array, index, Count).ThrowIfError();

            var values = UntypedValues;
            foreach (object? value in values)
            {
                array.SetValue(value, index++);
            }
        }

        void ICollection<V>.CopyTo(V[] array, int arrayIndex)
        {
            Validate.CanCopyTo(array, arrayIndex, Count).ThrowIfError();

            var values = UntypedValues;
            foreach (object? value in values)
            {
                array[arrayIndex++] = ObjectToValue(value);
            }
        }

        public bool Contains(V value)
        {
            foreach (object? obj in UntypedValues)
            {
                if (EqualityComparer<V>.Default.Equals(ObjectToValue(obj), value))
                    return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<V> GetEnumerator()
        {
            var e = UntypedValues.GetEnumerator();
            while (e.MoveNext())
            {
                V value = ObjectToValue(e.Current);
                yield return value;
            }
            e.Dispose();
        }
    }



    private readonly IDictionary _dictionary;
    private KeyCollection? _keys;
    private ValueCollection? _values;

    bool IDictionary.IsReadOnly => _dictionary.IsReadOnly;
    bool IDictionary.IsFixedSize => _dictionary.IsFixedSize;

    object ICollection.SyncRoot => _dictionary.SyncRoot;
    bool ICollection.IsSynchronized => _dictionary.IsSynchronized;

    bool ICollection<KeyValuePair<K, V>>.IsReadOnly => _dictionary.IsReadOnly;

    object? IDictionary.this[object key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    /// <inheritdoc cref="IDictionary{K,V}.this"/>
    public V this[K key]
    {
        get => ObjectToValue(_dictionary[key])!;
        set => _dictionary[key] = value;
    }

    ICollection IDictionary.Keys => _keys ??= new KeyCollection(this);
    ICollection<K> IDictionary<K, V>.Keys => _keys ??= new KeyCollection(this);
    IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => _keys ??= new KeyCollection(this);
    public IReadOnlyCollection<K> Keys => _keys ??= new KeyCollection(this);

    ICollection IDictionary.Values => _values ??= new ValueCollection(this);
    ICollection<V> IDictionary<K, V>.Values => _values ??= new ValueCollection(this);
    IEnumerable<V> IReadOnlyDictionary<K, V>.Values => _values ??= new ValueCollection(this);
    public IReadOnlyCollection<V> Values => _values ??= new ValueCollection(this);

    /// <summary>
    /// Gets the number of items in the <see cref="IDictionary"/>
    /// </summary>
    public int Count => _dictionary.Count;


    public DictionaryAdapter(IDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    void IDictionary.Add(object key, object? value) => _dictionary.Add(key, value);

    public void Add(K key, V value) => _dictionary.Add((object)key, (object?)value);

    public void Add(KeyValuePair<K, V> pair) => _dictionary.Add((object)pair.Key, (object?)pair.Value);


    bool IDictionary.Contains(object key) => _dictionary.Contains(key);

    public bool ContainsKey(K key) => _dictionary.Contains((object)key);

    public bool Contains(K key, V value)
    {
        if (!_dictionary.Contains((object)key))
            return false;
        var existingValue = ObjectToValue(_dictionary[key]);
        return EqualityComparer<V>.Default.Equals(existingValue, value);
    }

    public bool Contains(KeyValuePair<K, V> pair)
    {
        if (!_dictionary.Contains((object)pair.Key))
            return false;
        var existingValue = ObjectToValue(_dictionary[pair.Key]);
        return EqualityComparer<V>.Default.Equals(existingValue, pair.Value);
    }

    public bool TryGetValue(K key, out V value)
    {
        if (_dictionary.Contains(key))
        {
            value = ObjectToValue(_dictionary[key])!;
            return true;
        }

        value = default!;
        return false;
    }

    void IDictionary.Remove(object key) => _dictionary.Remove(key);
    public bool Remove(K key)
    {
        if (_dictionary.Contains(key))
        {
            _dictionary.Remove(key);
            return true;
        }

        return false;
    }

    public bool Remove(K key, V value)
    {
        if (Contains(key, value))
        {
            _dictionary.Remove(key);
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<K, V> pair)
    {
        if (Contains(pair))
        {
            _dictionary.Remove(pair.Key);
            return true;
        }

        return false;
    }

    public void Clear() => _dictionary.Clear();


    void ICollection.CopyTo(Array array, int index) => _dictionary.CopyTo(array, index);

    void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _dictionary.Count).ThrowIfError();

        foreach (DictionaryEntry entry in _dictionary.Cast<DictionaryEntry>())
        {
            array[arrayIndex++] = new(ObjectToKey(entry.Key), ObjectToValue(entry.Value)!);
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => _dictionary.GetEnumerator();

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        foreach (DictionaryEntry entry in _dictionary.Cast<DictionaryEntry>())
        {
            yield return new KeyValuePair<K, V>(
                ObjectToKey(entry.Key),
                ObjectToValue(entry.Value)!);
        }
    }
}
