namespace ScrubJay.Utilities;

[PublicAPI]
public readonly struct Pair<TKey, TValue> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Pair<TKey, TValue>, Pair<TKey, TValue>, bool>,
    IComparisonOperators<Pair<TKey, TValue>, Pair<TKey, TValue>, bool>,
#endif
    IEquatable<Pair<TKey, TValue>>,
    IComparable<Pair<TKey, TValue>>,
    IFormattable
{
    public static implicit operator KeyValuePair<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<TKey, TValue>(KeyValuePair<TKey, TValue> kvp) => new(kvp.Key, kvp.Value);
    public static implicit operator ValueTuple<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<TKey, TValue>(ValueTuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);
    public static implicit operator Tuple<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<TKey, TValue>(Tuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);

    public static bool operator ==(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => left.Equals(right);
    public static bool operator !=(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => !left.Equals(right);
    public static bool operator >(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Pair<TKey, TValue> left, Pair<TKey, TValue> right) => left.CompareTo(right) <= 0;

    public readonly TKey Key;
    public readonly TValue Value;

    public Pair(TKey key, TValue value)
    {
        this.Key = key;
        this.Value = value;
    }

    public void Deconstruct(out TKey key, out TValue value)
    {
        key = this.Key;
        value = this.Value;
    }

    public int CompareTo(Pair<TKey, TValue> pair)
    {
        int c = Comparer<TKey>.Default.Compare(this.Key, pair.Key);
        if (c != 0)
            return c;
        c = Comparer<TValue>.Default.Compare(this.Value, pair.Value);
        return c;
    }

    public bool Equals(Pair<TKey, TValue> pair)
    {
        return EqualityComparer<TKey>.Default.Equals(this.Key, pair.Key) &&
            EqualityComparer<TValue>.Default.Equals(this.Value, pair.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Pair<TKey, TValue> pair => Equals(pair),
            KeyValuePair<TKey, TValue> kvp =>
                EqualityComparer<TKey>.Default.Equals(this.Key, kvp.Key) &&
                EqualityComparer<TValue>.Default.Equals(this.Value, kvp.Value),
            ValueTuple<TKey, TValue> valueTuple =>
                EqualityComparer<TKey>.Default.Equals(this.Key, valueTuple.Item1) &&
                EqualityComparer<TValue>.Default.Equals(this.Value, valueTuple.Item2),
            Tuple<TKey, TValue> tuple =>
                EqualityComparer<TKey>.Default.Equals(this.Key, tuple.Item1) &&
                EqualityComparer<TValue>.Default.Equals(this.Value, tuple.Item2),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine<TKey, TValue>(this.Key, this.Value);
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        var text = new DefaultInterpolatedStringHandler(4, 2, formatProvider);
        text.AppendLiteral("(");
        text.AppendFormatted<TKey>(this.Key, format);
        text.AppendLiteral(", ");
        text.AppendFormatted<TValue>(this.Value, format);
        text.AppendLiteral(")");
        return text.ToStringAndClear();
    }

    public override string ToString()
    {
        return $"({this.Key}, {this.Value})";
    }
}