// ReSharper disable InconsistentNaming

using ScrubJay.Text;

namespace ScrubJay.Collections;

/// <summary>
/// Represents a Pair of related Values
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct KeyValue<TKey, TValue> :
#if NET7_0_OR_GREATER
    IEqualityOperators<KeyValue<TKey, TValue>, KeyValue<TKey, TValue>, bool>,
#endif
    IEquatable<KeyValue<TKey, TValue>>,
    IEquatable<KeyValuePair<TKey, TValue>>,
    IEquatable<Tuple<TKey, TValue>>,
    IEquatable<ValueTuple<TKey, TValue>>,
    IFormattable
{
    public static implicit operator KeyValue<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => new(keyValuePair.Key, keyValuePair.Value);
    public static implicit operator KeyValue<TKey, TValue>(Tuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);
    public static implicit operator KeyValue<TKey, TValue>(ValueTuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator KeyValuePair<TKey, TValue>(KeyValue<TKey, TValue> keyValue) => new(keyValue.Key, keyValue.Value);
    public static implicit operator Tuple<TKey, TValue>(KeyValue<TKey, TValue> keyValue) => new(keyValue.Key, keyValue.Value);
    public static implicit operator ValueTuple<TKey, TValue>(KeyValue<TKey, TValue> keyValue) => new(keyValue.Key, keyValue.Value);

    public static bool operator ==(KeyValue<TKey, TValue> left, KeyValue<TKey, TValue> right) => left.Equals(right);
    public static bool operator !=(KeyValue<TKey, TValue> left, KeyValue<TKey, TValue> right) => !left.Equals(right);

    
    public readonly TKey Key;
    public readonly TValue Value;

    public KeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public bool Equals(KeyValue<TKey, TValue> other)
    {
        return EqualityComparer<TKey>.Default.Equals(other.Key, Key) && EqualityComparer<TValue>.Default.Equals(other.Value, Value);
    }

    public bool Equals(KeyValuePair<TKey, TValue> other)
    {
        return EqualityComparer<TKey>.Default.Equals(other.Key, Key) && EqualityComparer<TValue>.Default.Equals(other.Value, Value);
    }

    public bool Equals(Tuple<TKey, TValue>? other)
    {
        return other is not null && EqualityComparer<TKey>.Default.Equals(other.Item1, Key) && EqualityComparer<TValue>.Default.Equals(other.Item2, Value);
    }

    public bool Equals(ValueTuple<TKey, TValue> other)
    {
        return EqualityComparer<TKey>.Default.Equals(other.Item1, Key) && EqualityComparer<TValue>.Default.Equals(other.Item2, Value);
    }

    public override bool Equals(object? obj)
        => obj switch
        {
            KeyValue<TKey, TValue> pair => Equals(pair),
            KeyValuePair<TKey, TValue> keyValuePair => Equals(keyValuePair),
            Tuple<TKey, TValue> tuple => Equals(tuple),
            ValueTuple<TKey, TValue> valueTuple => Equals(valueTuple),
            _ => false,
        };

    public override int GetHashCode()
    {
        return Hasher.Combine<TKey, TValue>(Key, Value);
    }

    public string ToString(string? format, IFormatProvider? provider = default)
    {
        var buffer = new TextSpanBuffer();
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