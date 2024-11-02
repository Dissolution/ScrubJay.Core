/* CA1710 - Identifiers should have correct suffix
 *   I do not want to change the name of this class
 */

#pragma warning disable CA1710

namespace ScrubJay.Collections;

/// <summary>
/// An adapter to consume an <see cref="IDictionary"/> with generically-typed Keys and Values
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[PublicAPI]
public sealed class DictionaryAdapter<TKey, TValue> :
    IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
    where TKey : notnull
{
    [return: NotNullIfNotNull(nameof(objKey))]
    private static TKey ObjectToKey(
        [NotNull] object? objKey,
        [CallerArgumentExpression(nameof(objKey))]
        string? keyName = null)
    {
        return objKey switch
        {
            null => throw new ArgumentNullException(keyName, "Key cannot be null"),
            TKey key => key,
            _ => throw new ArgumentException($"Key '{objKey}' is not a '{typeof(TKey).Name}'", keyName),
        };
    }

    [return: NotNullIfNotNull(nameof(objValue))]
    private static TValue? ObjectToValue(
        object? objValue,
        [CallerArgumentExpression(nameof(objValue))]
        string? valueName = null)
    {
        if (objValue.CanBe<TValue>(out var value))
            return value;
        throw new ArgumentException($"Value '{objValue}' is not a '{typeof(TValue).Name}'", valueName);
    }


    private readonly IDictionary _dictionary;

    public TValue this[TKey key]
    {
        get => ObjectToValue(_dictionary[key])!;
        set => _dictionary[key] = value;
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys.Cast<TKey>();

    /// <inheritdoc cref="IDictionary{TKey,TValue}.Keys"/>
    public IReadOnlyCollection<TKey> Keys => _dictionary.Keys.Cast<TKey>().ToHashSet();


    ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values.Cast<TValue>().ToList();

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values.Cast<TValue>();

    /// <inheritdoc cref="IDictionary{TKey,TValue}.Values"/>
    public IReadOnlyCollection<TValue> Values => _dictionary.Values.Cast<TValue>().ToList();

    /// <summary>
    /// Gets the number of items in the <see cref="IDictionary"/>
    /// </summary>
    public int Count => _dictionary.Count;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _dictionary.IsReadOnly;

    public DictionaryAdapter(IDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    public void Add(TKey key, TValue value) => _dictionary.Add((object)key, (object?)value);

    public void Add(KeyValuePair<TKey, TValue> pair) => _dictionary.Add((object)pair.Key, (object?)pair.Value);

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

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Validate.CopyTo(_dictionary.Count, array, arrayIndex).ThrowIfError();

        foreach (DictionaryEntry entry in _dictionary)
        {
            array[arrayIndex++] = new(ObjectToKey(entry.Key), ObjectToValue(entry.Value)!);
        }
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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => _dictionary
            .Cast<DictionaryEntry>()
            .Select(static entry => new KeyValuePair<TKey, TValue>(ObjectToKey(entry.Key), ObjectToValue(entry.Value)!))
            .GetEnumerator();
}