#pragma warning disable S907
#pragma warning disable CA1000, CA1045
// ReSharper disable ArrangeThisQualifier


using ScrubJay.Text.Rendering;
#if NET7_0_OR_GREATER
using ScrubJay.Parsing;
#endif

namespace ScrubJay.Utilities;

[PublicAPI]
public static class Pair
{
#if NET7_0_OR_GREATER
    public static Result<Pair<TKey, TValue>> TryParse<TKey, TValue>(text text, IFormatProvider? provider = null)
        where TKey : ISpanParsable<TKey>
        where TValue : ISpanParsable<TValue>
    {
        var reader = new SpanReader<char>(text);

        reader.TrySkipWhile(char.IsWhiteSpace);

        if (reader.RemainingCount == 0)
            return getEx(text);

        char ch = reader.Take();
        if (ch != '(')
            return getEx(text);

        var takeUntilComma = reader.TryTakeUntilMatches(',');
        if (takeUntilComma.StopReason == StopReason.EndOfSpan)
            return getEx(text);

        var keySpan = takeUntilComma.Span;
        if (!TKey.TryParse(keySpan, provider, out var key))
            return getEx(text, ParseException.Create<TKey>(keySpan));

        reader.Take();

        var takeUntilRightParenthesis = reader.TryTakeUntilMatches(')');
        if (takeUntilRightParenthesis.StopReason == StopReason.EndOfSpan)
            return getEx(text);

        var valueSpan = takeUntilRightParenthesis.Span;
        if (!TValue.TryParse(valueSpan, provider, out var value))
            return getEx(text, ParseException.Create<TValue>(valueSpan));

        reader.Take();

        reader.TrySkipWhile(char.IsWhiteSpace);
        if (reader.RemainingCount > 0)
            return getEx(text);

        var pair = new Pair<TKey, TValue>(key, value);
        return Ok(pair);


        static ParseException getEx(text text, Exception? innerEx = null)
        {
            return ParseException.Create<Pair<TKey, TValue>>(
                text,
                $"Expected `({typeof(TKey).Render()}, {typeof(TValue).Render()})`",
                innerEx);
        }
    }
#endif

    public static Pair<K, V> New<K, V>(K key, V value) => new(key, value);

    public static Pair<K, V> Parse<K, V>(ref PairBuilder<K, V> pairText) => pairText.TryGetPair().OkOrThrow();
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
    IFormattable
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
}
