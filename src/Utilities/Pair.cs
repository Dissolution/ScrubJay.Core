#if NET7_0_OR_GREATER
using ScrubJay.Parsing;
#endif

namespace ScrubJay.Utilities;

[PublicAPI]
public static class Pair
{
#if NET7_0_OR_GREATER
    public static Result<Pair<K, V>> TryParse<K, V>(text text, IFormatProvider? provider = null)
        where K : ISpanParsable<K>
        where V : ISpanParsable<V>
    {
        var reader = new SpanReader<char>(text);

        reader.SkipWhile(char.IsWhiteSpace);

        if (reader.IsCompleted)
            return getEx(text);

        char ch = reader.Take();
        if (ch != '(')
            return getEx(text);

        var keySpan = reader.TakeUntilMatching(',');
        if (reader.IsCompleted)
            return getEx(text);

        if (!K.TryParse(keySpan, provider, out var key))
            return getEx(text, Ex.Parse<K>(keySpan));

        reader.Take();

        var valueSpan = reader.TakeUntilMatching(')');
        if (reader.IsCompleted)
            return getEx(text);

        if (!V.TryParse(valueSpan, provider, out var value))
            return getEx(text, Ex.Parse<V>(valueSpan));

        reader.Take();

        reader.SkipWhile(char.IsWhiteSpace);
        if (!reader.IsCompleted)
            return getEx(text);

        var pair = new Pair<K, V>(key, value);
        return Ok(pair);


        static ParseException getEx(text text, Exception? innerEx = null)
        {
            return Ex.Parse<Pair<K, V>>(
                text,
                $"Expected `({typeof(K):@}, {typeof(V):@})`",
                innerEx);
        }
    }
#endif

    public static Pair<K, V> New<K, V>(K key, V value) => new(key, value);
}

/// <summary>
/// A pair of related values
/// </summary>
/// <typeparam name="K">The <see cref="Type"/> of the <see cref="Key"/></typeparam>
/// <typeparam name="V">The <see cref="Type"/> of the <see cref="Value"/></typeparam>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Pair<K, V> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Pair<K, V>, Pair<K, V>, bool>,
    IComparisonOperators<Pair<K, V>, Pair<K, V>, bool>,
#endif
    IEquatable<Pair<K, V>>,
    IComparable<Pair<K, V>>,
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable,
    IRenderable
{
    // Interop with KeyValuePair, Tuple, and ValueTuple

    public static implicit operator KeyValuePair<K, V>(Pair<K, V> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<K, V>(KeyValuePair<K, V> kvp) => new(kvp.Key, kvp.Value);

    public static implicit operator Tuple<K, V>(Pair<K, V> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<K, V>(Tuple<K, V> tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator ValueTuple<K, V>(Pair<K, V> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<K, V>(ValueTuple<K, V> tuple) => new(tuple.Item1, tuple.Item2);


    public static bool operator ==(Pair<K, V> left, Pair<K, V> right) => left.Equals(right);
    public static bool operator !=(Pair<K, V> left, Pair<K, V> right) => !left.Equals(right);
    public static bool operator >(Pair<K, V> left, Pair<K, V> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Pair<K, V> left, Pair<K, V> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Pair<K, V> left, Pair<K, V> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Pair<K, V> left, Pair<K, V> right) => left.CompareTo(right) <= 0;


    public readonly K Key;
    public readonly V Value;

    public Pair(K key, V value)
    {
        Key = key;
        Value = value;
    }

    public void Deconstruct(out K key, out V value)
    {
        key = Key;
        value = Value;
    }

    public int CompareTo(Pair<K, V> pair)
    {
        int c = Comparer<K>.Default.Compare(Key, pair.Key);
        if (c != 0)
            return c;
        c = Comparer<V>.Default.Compare(Value, pair.Value);
        return c;
    }

    public bool Equals(Pair<K, V> pair)
    {
        return EqualityComparer<K>.Default.Equals(Key, pair.Key) &&
            EqualityComparer<V>.Default.Equals(Value, pair.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Pair<K, V> pair => Equals(pair),
            KeyValuePair<K, V> kvp =>
                EqualityComparer<K>.Default.Equals(Key, kvp.Key) &&
                EqualityComparer<V>.Default.Equals(Value, kvp.Value),
            Tuple<K, V> tuple =>
                EqualityComparer<K>.Default.Equals(Key, tuple.Item1) &&
                EqualityComparer<V>.Default.Equals(Value, tuple.Item2),
            ValueTuple<K, V> valueTuple =>
                EqualityComparer<K>.Default.Equals(Key, valueTuple.Item1) &&
                EqualityComparer<V>.Default.Equals(Value, valueTuple.Item2),
            _ => false,
        };
    }

    public override int GetHashCode() => Hasher.HashMany<K, V>(Key, Value);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = null)
    {
        return new TryFormatWriter(destination)
        {
            '(',
            { Key, format, provider },
            ", ",
            { Value, format, provider },
            ')',
        }.Wrote(out charsWritten);
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        return TextBuilder.New
            .Append('(')
            .Format(Key, format, provider)
            .Append(", ")
            .Format(Value, format, provider)
            .Append(')')
            .ToStringAndDispose();
    }

    public override string ToString() => $"({Key}, {Value})";

    public void RenderTo(TextBuilder builder)
    {
        builder.Append($"({Key:@}, {Value:@})");
    }
}
