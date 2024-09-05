// ReSharper disable InconsistentNaming

using ScrubJay.Text;

namespace ScrubJay.Collections;

/// <summary>
/// Represents a Pair of related Values
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
public readonly struct KeyValue<K, V> :
#if NET7_0_OR_GREATER
    IEqualityOperators<KeyValue<K, V>, KeyValue<K, V>, bool>,
#endif
    IEquatable<KeyValue<K, V>>,
    IEquatable<KeyValuePair<K, V>>,
    IEquatable<Tuple<K, V>>,
    IEquatable<ValueTuple<K, V>>,
    IFormattable
{
    public static implicit operator KeyValue<K, V>(KeyValuePair<K, V> keyValuePair) => new(keyValuePair.Key, keyValuePair.Value);
    public static implicit operator KeyValue<K, V>(Tuple<K, V> tuple) => new(tuple.Item1, tuple.Item2);
    public static implicit operator KeyValue<K, V>(ValueTuple<K, V> tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator KeyValuePair<K, V>(KeyValue<K, V> keyValue) => new(keyValue.Key, keyValue.Value);
    public static implicit operator Tuple<K, V>(KeyValue<K, V> keyValue) => new(keyValue.Key, keyValue.Value);
    public static implicit operator ValueTuple<K, V>(KeyValue<K, V> keyValue) => new(keyValue.Key, keyValue.Value);

    public static bool operator ==(KeyValue<K, V> left, KeyValue<K, V> right) => left.Equals(right);
    public static bool operator !=(KeyValue<K, V> left, KeyValue<K, V> right) => !left.Equals(right);

    
    public readonly K Key;
    public readonly V Value;

    public KeyValue(K key, V value)
    {
        Key = key;
        Value = value;
    }

    public bool Equals(KeyValue<K, V> other)
    {
        return EqualityComparer<K>.Default.Equals(other.Key, Key) && EqualityComparer<V>.Default.Equals(other.Value, Value);
    }

    public bool Equals(KeyValuePair<K, V> other)
    {
        return EqualityComparer<K>.Default.Equals(other.Key, Key) && EqualityComparer<V>.Default.Equals(other.Value, Value);
    }

    public bool Equals(Tuple<K, V>? other)
    {
        return other is not null && EqualityComparer<K>.Default.Equals(other.Item1, Key) && EqualityComparer<V>.Default.Equals(other.Item2, Value);
    }

    public bool Equals(ValueTuple<K, V> other)
    {
        return EqualityComparer<K>.Default.Equals(other.Item1, Key) && EqualityComparer<V>.Default.Equals(other.Item2, Value);
    }

    public override bool Equals(object? obj)
        => obj switch
        {
            KeyValue<K, V> pair => Equals(pair),
            KeyValuePair<K, V> keyValuePair => Equals(keyValuePair),
            Tuple<K, V> tuple => Equals(tuple),
            ValueTuple<K, V> valueTuple => Equals(valueTuple),
            _ => false,
        };

    public override int GetHashCode()
    {
        return Hasher.Combine<K, V>(Key, Value);
    }

    public string ToString(string? format, IFormatProvider? provider = default)
    {
        var buffer = new TextBuffer();
        buffer.Append('[');
        buffer.AppendFormatted(Key, format, provider);
        buffer.Append(", ");
        buffer.AppendFormatted(Value, format, provider);
        buffer.Append(']');
        return buffer.ToStringAndDispose();
    }

    public override string ToString()
    {
        return $"[{Key}, {Value}]";
    }
}