// Identifiers should have correct suffix
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
public sealed class DictionaryAdapter<TKey, TValue> :
    IDictionary<TKey, TValue>, IDictionary,
    IReadOnlyDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>, ICollection,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    where TKey : notnull
{
    [return: NotNull]
    private static TKey ObjectToKey(object? objKey,
        [CallerArgumentExpression(nameof(objKey))]
        string? keyName = null)
    {
        if (objKey is TKey key)
        {
            return key;
        }
        throw new ArgumentException($"Invalid Key - '{objKey}' is not a {typeof(TKey).NameOf()}", keyName);
    }

    [return: NotNullIfNotNull(nameof(objValue))]
    private static TValue ObjectToValue(object? objValue,
        [CallerArgumentExpression(nameof(objValue))]
        string? valueName = null)
    {
        if (objValue.CanBe<TValue>(out TValue? value))
            return value!;
        throw new ArgumentException($"Invalid Value - '{objValue}' is not a {typeof(TValue).NameOf()}", valueName);
    }

    private sealed class KeyCollection : IReadOnlyCollection<TKey>, ICollection<TKey>, ICollection
    {
        private readonly DictionaryAdapter<TKey, TValue> _adapter;

        private ICollection UntypedKeys => _adapter._dictionary.Keys;

        object ICollection.SyncRoot => UntypedKeys.SyncRoot;

        bool ICollection.IsSynchronized => UntypedKeys.IsSynchronized;

        public int Count => _adapter.Count;

        public bool IsReadOnly => true;


        public KeyCollection(DictionaryAdapter<TKey, TValue> adapter)
        {
            _adapter = adapter;
        }

        void ICollection<TKey>.Add(TKey item) => Throw.IsReadOnly(this);

        bool ICollection<TKey>.Remove(TKey item) => Throw.IsReadOnly<KeyCollection, bool>(this);

        void ICollection<TKey>.Clear() => Throw.IsReadOnly(this);

        void ICollection.CopyTo(Array array, int index)
        {
            Validate.CanCopyTo(array, index, Count).ThrowIfError();

            var keys = UntypedKeys;
            foreach (var key in keys)
            {
                array.SetValue(key, index++);
            }
        }

        void ICollection<TKey>.CopyTo(TKey[] array, int arrayIndex)
        {
            Validate.CanCopyTo(array, arrayIndex, Count).ThrowIfError();

            var keys = UntypedKeys;
            foreach (var key in keys)
            {
                array[arrayIndex++] = ObjectToKey(key);
            }
        }

        public bool Contains(TKey key) => _adapter.ContainsKey(key);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TKey> GetEnumerator()
        {
            var e = UntypedKeys.GetEnumerator();
            while (e.MoveNext())
            {
                TKey key = ObjectToKey(e.Current);
                yield return key;
            }
            Disposable.TryDispose(e);
        }
    }

    private sealed class ValueCollection : IReadOnlyCollection<TValue>, ICollection<TValue>, ICollection
    {
        private readonly DictionaryAdapter<TKey, TValue> _adapter;

        private ICollection UntypedValues => _adapter._dictionary.Values;

        object ICollection.SyncRoot => UntypedValues.SyncRoot;

        bool ICollection.IsSynchronized => UntypedValues.IsSynchronized;

        public int Count => _adapter.Count;

        public bool IsReadOnly => true;

        public ValueCollection(DictionaryAdapter<TKey, TValue> adapter)
        {
            _adapter = adapter;
        }

        void ICollection<TValue>.Add(TValue item) => throw new InvalidOperationException($"A DictionaryAdapter's Keys are readonly");

        bool ICollection<TValue>.Remove(TValue item) => throw new InvalidOperationException($"A DictionaryAdapter's Keys are readonly");

        void ICollection<TValue>.Clear() => throw new InvalidOperationException($"A DictionaryAdapter's Keys are readonly");

        void ICollection.CopyTo(Array array, int index)
        {
            Validate.CanCopyTo(array, index, Count).ThrowIfError();

            var values = UntypedValues;
            foreach (var value in values)
            {
                array.SetValue(value, index++);
            }
        }

        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
            Validate.CanCopyTo(array, arrayIndex, Count).ThrowIfError();

            var values = UntypedValues;
            foreach (var value in values)
            {
                array[arrayIndex++] = ObjectToValue(value);
            }
        }

        public bool Contains(TValue value)
        {
            foreach (object? obj in UntypedValues)
            {
                if (EqualityComparer<TValue>.Default.Equals(ObjectToValue(obj), value))
                    return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TValue> GetEnumerator()
        {
            var e = UntypedValues.GetEnumerator();
            while (e.MoveNext())
            {
                TValue value = ObjectToValue(e.Current);
                yield return value;
            }
        }
    }



    private readonly IDictionary _dictionary;
    private KeyCollection? _keys;
    private ValueCollection? _values;

    bool IDictionary.IsReadOnly => _dictionary.IsReadOnly;
    bool IDictionary.IsFixedSize => _dictionary.IsFixedSize;

    object ICollection.SyncRoot => _dictionary.SyncRoot;
    bool ICollection.IsSynchronized => _dictionary.IsSynchronized;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _dictionary.IsReadOnly;

    object? IDictionary.this[object key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    /// <inheritdoc cref="IDictionary{K,V}.this"/>
    public TValue this[TKey key]
    {
        get => ObjectToValue(_dictionary[key])!;
        set => _dictionary[key] = value;
    }

    ICollection IDictionary.Keys => _keys ??= new KeyCollection(this);
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys ??= new KeyCollection(this);
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _keys ??= new KeyCollection(this);
    public IReadOnlyCollection<TKey> Keys => _keys ??= new KeyCollection(this);

    ICollection IDictionary.Values => _values ??= new ValueCollection(this);
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _values ??= new ValueCollection(this);
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _values ??= new ValueCollection(this);
    public IReadOnlyCollection<TValue> Values => _values ??= new ValueCollection(this);

    /// <summary>
    /// Gets the number of items in the <see cref="IDictionary"/>
    /// </summary>
    public int Count => _dictionary.Count;


    public DictionaryAdapter(IDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    void IDictionary.Add(object key, object? value) => _dictionary.Add(key, value);

    public void Add(TKey key, TValue value) => _dictionary.Add((object)key, (object?)value);

    public void Add(KeyValuePair<TKey, TValue> pair) => _dictionary.Add((object)pair.Key, (object?)pair.Value);


    bool IDictionary.Contains(object key) => _dictionary.Contains(key);

    public bool ContainsKey(TKey key) => _dictionary.Contains((object)key);

    public bool Contains(TKey key, TValue value)
    {
        if (!_dictionary.Contains((object)key))
            return false;
        var existingValue = ObjectToValue(_dictionary[key]);
        return EqualityComparer<TValue>.Default.Equals(existingValue, value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> pair)
    {
        if (!_dictionary.Contains((object)pair.Key))
            return false;
        var existingValue = ObjectToValue(_dictionary[pair.Key]);
        return EqualityComparer<TValue>.Default.Equals(existingValue, pair.Value);
    }

    public bool TryGetValue(TKey key, out TValue value)
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
    public bool Remove(TKey key)
    {
        if (_dictionary.Contains(key))
        {
            _dictionary.Remove(key);
            return true;
        }

        return false;
    }

    public bool Remove(TKey key, TValue value)
    {
        if (Contains(key, value))
        {
            _dictionary.Remove(key);
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> pair)
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

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _dictionary.Count).ThrowIfError();

        foreach (DictionaryEntry entry in _dictionary.Cast<DictionaryEntry>())
        {
            array[arrayIndex++] = new(ObjectToKey(entry.Key), ObjectToValue(entry.Value)!);
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => _dictionary.GetEnumerator();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (DictionaryEntry entry in _dictionary.Cast<DictionaryEntry>())
        {
            yield return new KeyValuePair<TKey, TValue>(
                ObjectToKey(entry.Key),
                ObjectToValue(entry.Value));
        }
    }
}
