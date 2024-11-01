using ScrubJay.Comparison;

namespace ScrubJay.Scratch;

[PublicAPI]
public class Map<TKey, TValue> : ICollection<Pair<TKey, TValue>>
    where TKey : notnull
{
    private readonly UberDict<TKey, TValue> _dict = new UberDict<TKey, TValue>();

    bool ICollection<Pair<TKey, TValue>>.IsReadOnly => false;

    public MapKeyNode this[TKey key] => new MapKeyNode(this, key);

    public int Count => _dict.Count;

    public IReadOnlyCollection<TKey> Keys => new KeySet(this);
    public IReadOnlyCollection<TValue> Values => new ValueCollection(this);

    public Map()
    {
    }

    public MapKeyNode Key(TKey key) => new MapKeyNode(this, key);


    public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

    public bool ContainsValue(TValue value, IEqualityComparer<TValue>? valueComparer = null)
    {
        if (valueComparer is null)
            return _dict.Values.Any(v => EqualityComparer<TValue>.Default.Equals(v, value));
        return _dict.Values.Any(v => valueComparer.Equals(v, value));
    }

    public bool Contains(TKey key, TValue value, IEqualityComparer<TValue>? valueComparer = null)
    {
        if (_dict.TryGetValue(key, out var existingValue))
        {
            valueComparer ??= EqualityComparer<TValue>.Default;
            return valueComparer.Equals(existingValue, value);
        }

        return false;
    }

    public bool Contains(Pair<TKey, TValue> pair) => Contains(pair.Key, pair.Value, null);

    public bool Contains(
        Pair<TKey, TValue> pair,
        IEqualityComparer<TValue>? valueComparer)
        => Contains(pair.Key, pair.Value, valueComparer);

    public void Add(Pair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public Result<Pair<TKey, TValue>, Exception> TryAdd(TKey key, TValue value)
    {
        if (_dict.ContainsKey(key))
            return new ArgumentException($"Map already contains key '{key}'", nameof(key));
        _dict.Add(key, value);
        return Pair(key, value);
    }

    public Result<Pair<TKey, TValue>, Exception> TryAdd(Pair<TKey, TValue> pair)
    {
        if (_dict.ContainsKey(pair.Key))
            return new ArgumentException($"Map already contains key '{pair.Key}'", nameof(pair));
        _dict.Add(pair.Key, pair.Value);
        return pair;
    }

    public TValue AddOrUpdate(TKey key, TValue value)
    {
        _dict[key] = value;
        return value;
    }

    public TValue AddOrUpdate(TKey key, TValue addValue, Func<TValue, TValue> updateValue)
    {
        TValue newValue;
        if (_dict.TryGetValue(key, out var existingValue))
        {
            newValue = updateValue(existingValue);
        }
        else
        {
            newValue = addValue;
        }

        _dict[key] = newValue;
        return newValue;
    }

    public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValue)
    {
        TValue newValue;
        if (_dict.TryGetValue(key, out var existingValue))
        {
            newValue = updateValue(key, existingValue);
        }
        else
        {
            newValue = addValue;
        }

        _dict[key] = newValue;
        return newValue;
    }

    public TValue AddOrUpdate(TKey key, Func<TValue> createValue, Func<TValue, TValue> updateValue)
    {
        TValue newValue;
        if (_dict.TryGetValue(key, out var existingValue))
        {
            newValue = updateValue(existingValue);
        }
        else
        {
            newValue = createValue();
        }

        _dict[key] = newValue;
        return newValue;
    }

    public TValue AddOrUpdate(TKey key, Func<TValue> createValue, Func<TKey, TValue, TValue> updateValue)
    {
        TValue newValue;
        if (_dict.TryGetValue(key, out var existingValue))
        {
            newValue = updateValue(key, existingValue);
        }
        else
        {
            newValue = createValue();
        }

        _dict[key] = newValue;
        return newValue;
    }

    public TValue AddOrUpdate(TKey key, Func<TKey, TValue> createValue, Func<TValue, TValue> updateValue)
    {
        TValue newValue;
        if (_dict.TryGetValue(key, out var existingValue))
        {
            newValue = updateValue(existingValue);
        }
        else
        {
            newValue = createValue(key);
        }

        _dict[key] = newValue;
        return newValue;
    }

    public TValue AddOrUpdate(TKey key, Func<TKey, TValue> createValue, Func<TKey, TValue, TValue> updateValue)
    {
        TValue newValue;
        if (_dict.TryGetValue(key, out var existingValue))
        {
            newValue = updateValue(key, existingValue);
        }
        else
        {
            newValue = createValue(key);
        }

        _dict[key] = newValue;
        return newValue;
    }

    public Option<TValue> TryGetValue(TKey key)
    {
        if (_dict.TryGetValue(key, out var value))
            return Some(value);
        return None<TValue>();
    }

    public Option<Pair<TKey, TValue>> TryGet(TKey key)
    {
        if (_dict.TryGetValue(key, out var value))
            return Pair(key, value);
        return None();
    }

    public TValue GetOrAdd(TKey key, TValue value)
    {
        if (_dict.TryGetValue(key, out var existingValue))
            return existingValue;
        _dict[key] = value;
        return value;
    }

    public TValue GetOrAdd(TKey key, Func<TValue> createValue)
    {
        if (!_dict.TryGetValue(key, out var value))
        {
            value = createValue();
            _dict[key] = value;
        }
        return value;
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> createValue)
    {
        if (!_dict.TryGetValue(key, out var value))
        {
            value = createValue(key);
            _dict[key] = value;
        }

        return value;
    }


    bool ICollection<Pair<TKey, TValue>>.Remove(Pair<TKey, TValue> pair) => TryRemovePair(pair);

    public Option<Pair<TKey, TValue>> TryRemove(TKey key)
    {
        if (_dict.Remove(key, out var value))
            return Pair(key, value);
        return None();
    }

    public Option<Pair<TKey, TValue>> TryRemovePair(
        TKey key,
        TValue value,
        IEqualityComparer<TValue>? valueComparer = null)
    {
        return TryGetValue(key)
            .Where(existingValue => Equate.With(valueComparer).Equals(existingValue, value))
            .Where(_ => _dict.TryRemove(key))
            .Select(_ => Pair(key, value));
    }

    public Option<Pair<TKey, TValue>> TryRemovePair(
        Pair<TKey, TValue> pair,
        IEqualityComparer<TValue>? valueComparer = null)
        => TryRemovePair(pair.Key, pair.Value, valueComparer);

    public void Clear()
    {
        _dict.Clear();
    }

    void ICollection<Pair<TKey, TValue>>.CopyTo(Pair<TKey, TValue>[] array, int arrayIndex)
    {
        Validate.CopyTo(Count, array, arrayIndex).OkOrThrow();
        foreach (var kvp in _dict)
        {
            array[arrayIndex++] = kvp;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<Pair<TKey, TValue>> GetEnumerator()
    {
        foreach (var kvp in _dict)
            yield return kvp;
    }


    public sealed class MapKeyNode :
        //IEquatable<TKey>,
        IEquatable<TValue>
    {
        private readonly Map<TKey, TValue> _map;

        public TKey Key { get; }

        public bool HasValue => _map.ContainsKey(Key);

        public Option<TValue> Value
        {
            get => _map.TryGetValue(Key);
            set
            {
                if (value.HasSome(out var some))
                {
                    _map.AddOrUpdate(Key, some);
                }
                else
                {
                    _map.TryRemove(Key);
                }
            }
        }

        public MapKeyNode(Map<TKey, TValue> map, TKey key)
        {
            _map = map;
            Key = key;
        }

        public Option<TValue> TryGetValue()
        {
            return _map.TryGetValue(Key);
        }

        public bool TrySetValue(TValue value)
        {
            return _map.TryAdd(Key, value);
        }

        public bool TryRemoveValue()
        {
            return _map.TryRemove(Key);
        }

        public bool Equals(TKey? key)
        {
            return Equate.Values(Key, key);
        }

        public bool Equals(TValue? value)
        {
            return _map.TryGetValue(Key).IsSomeAnd(ev => Equate.Values(ev, value));
        }

        public bool Equals(TValue? value, IEqualityComparer<TValue>? valueComparer)
        {
            return _map.TryGetValue(Key).IsSomeAnd(ev => Equate.With(valueComparer).Equals(ev!, value!));
        }

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                TValue value => Equals(value),
                TKey key => Equate.Values(Key, key),
                Pair<TKey, TValue> pair => Equate.Values(Key, pair.Key) && Equals(pair.Value),
                _ => false,
            };
        }

        public override int GetHashCode()
        {
            return Hasher.GetHashCode(Key);
        }

        public override string ToString()
        {
            var text = new DefaultInterpolatedStringHandler(2, 2);
            text.AppendLiteral("[");
            text.AppendFormatted(Key);
            text.AppendLiteral(": ");
            if (Value.HasSome(out var value))
                text.AppendFormatted(value);
            else
                text.AppendLiteral("None");
            return text.ToStringAndClear();
        }
    }

    public sealed class KeySet :
// #if NET6_0_OR_GREATER
//         IReadOnlySet<TKey>,
// #endif
        IReadOnlyCollection<TKey>,
        IEnumerable<TKey>
    {
        private readonly Map<TKey, TValue> _map;

        public int Count => _map.Count;

        internal KeySet(Map<TKey, TValue> map)
        {
            _map = map;
        }

        public bool Contains(TKey key) => _map.ContainsKey(key);

        [MustDisposeResource(false)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MustDisposeResource(false)]
        public IEnumerator<TKey> GetEnumerator() => _map._dict.Keys.GetEnumerator();
    }

    public sealed class ValueCollection : IReadOnlyCollection<TValue>
    {
        private readonly Map<TKey, TValue> _map;

        public int Count => _map.Count;

        internal ValueCollection(Map<TKey, TValue> map)
        {
            _map = map;
        }

        public bool Contains(TValue value) => _map.ContainsValue(value);

        [MustDisposeResource(false)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MustDisposeResource(false)]
        public IEnumerator<TValue> GetEnumerator() => _map._dict.Values.GetEnumerator();
    }
}