using ScrubJay.Utilities;

namespace ScrubJay.Collections;

/// <summary>
/// Represents a Pair of related Values
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public readonly struct Pair<TKey, TValue> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Pair<TKey, TValue>, Pair<TKey, TValue>, bool>,
#endif
    IEquatable<Pair<TKey, TValue>>,
    IEquatable<KeyValuePair<TKey, TValue>>,
    IEquatable<Tuple<TKey, TValue>>,
    IEquatable<ValueTuple<TKey, TValue>>,
    IFormattable
{
    public static implicit operator Pair<TKey, TValue>(KeyValuePair<TKey, TValue> keyValuePair) => new(keyValuePair.Key, keyValuePair.Value);
    public static implicit operator Pair<TKey, TValue>(Tuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);
    public static implicit operator Pair<TKey, TValue>(ValueTuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator KeyValuePair<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Tuple<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator ValueTuple<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);

    public static bool operator ==(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => left.Equals(right);
    public static bool operator !=(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => !left.Equals(right);

    public readonly TKey Key;
    public readonly TValue Value;

    public Pair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public bool Equals(Pair<TKey, TValue> other)
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
            Pair<TKey, TValue> pair => Equals(pair),
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
        string? keyStr;
        if (Key is IFormattable)
        {
            keyStr = ((IFormattable)Key).ToString(format, provider);
        }
        else
        {
            keyStr = Key?.ToString();
        }

        string? valueStr;
        if (Value is IFormattable)
        {
            valueStr = ((IFormattable)Value).ToString(format, provider);
        }
        else
        {
            valueStr = Value?.ToString();
        }

        return $"[{keyStr}, {valueStr}]";
    }

    public override string ToString()
    {
        return $"[{Key}, {Value}]";
    }
}