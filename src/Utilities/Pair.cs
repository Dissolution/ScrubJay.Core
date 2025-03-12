#pragma warning disable S907
#pragma warning disable CA1000, CA1045
// ReSharper disable ArrangeThisQualifier

#if NET7_0_OR_GREATER
using ScrubJay.Parsing;
#endif

namespace ScrubJay.Utilities;

[PublicAPI]
public static class Pair
{
#if NET7_0_OR_GREATER
    public static Result<Pair<TKey, TValue>, ParseException> TryParse<TKey, TValue>(text text, IFormatProvider? provider = default)
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
        return pair;


        static ParseException getEx(text text, Exception? innerEx = null)
        {
            return ParseException.Create<Pair<TKey, TValue>>(
                text,
                $"Expected `({typeof(TKey).NameOf()}, {typeof(TValue).NameOf()})`",
                innerEx);
        }
    }
#endif

    public static Pair<TKey, TValue> New<TKey, TValue>(TKey key, TValue value) => new(key, value);

    public static Pair<TKey, TValue> Parse<TKey, TValue>(ref PairBuilder<TKey, TValue> pairText) => pairText.TryGetPair().OkOrThrow();
}

/// <summary>
/// A pair of related values
/// </summary>
/// <typeparam name="TKey">The <see cref="Type"/> of the <see cref="Key"/></typeparam>
/// <typeparam name="TValue">The <see cref="Type"/> of the <see cref="Value"/></typeparam>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Pair<TKey, TValue> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Pair<TKey, TValue>, Pair<TKey, TValue>, bool>,
    IComparisonOperators<Pair<TKey, TValue>, Pair<TKey, TValue>, bool>,
#endif
    IEquatable<Pair<TKey, TValue>>,
    IComparable<Pair<TKey, TValue>>,
    ISpanFormattable,
    IFormattable
{
    // Interop with KeyValuePair, Tuple, and ValueTuple

    public static implicit operator KeyValuePair<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<TKey, TValue>(KeyValuePair<TKey, TValue> kvp) => new(kvp.Key, kvp.Value);

    public static implicit operator Tuple<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<TKey, TValue>(Tuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator ValueTuple<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);
    public static implicit operator Pair<TKey, TValue>(ValueTuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);


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
        Key = key;
        Value = value;
    }

    public void Deconstruct(out TKey key, out TValue value)
    {
        key = Key;
        value = Value;
    }

    public int CompareTo(Pair<TKey, TValue> pair)
    {
        int c = Comparer<TKey>.Default.Compare(Key, pair.Key);
        if (c != 0)
            return c;
        c = Comparer<TValue>.Default.Compare(Value, pair.Value);
        return c;
    }

    public bool Equals(Pair<TKey, TValue> pair)
    {
        return EqualityComparer<TKey>.Default.Equals(Key, pair.Key) &&
            EqualityComparer<TValue>.Default.Equals(Value, pair.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Pair<TKey, TValue> pair => Equals(pair),
            KeyValuePair<TKey, TValue> kvp =>
                EqualityComparer<TKey>.Default.Equals(Key, kvp.Key) &&
                EqualityComparer<TValue>.Default.Equals(Value, kvp.Value),
            Tuple<TKey, TValue> tuple =>
                EqualityComparer<TKey>.Default.Equals(Key, tuple.Item1) &&
                EqualityComparer<TValue>.Default.Equals(Value, tuple.Item2),
            ValueTuple<TKey, TValue> valueTuple =>
                EqualityComparer<TKey>.Default.Equals(Key, valueTuple.Item1) &&
                EqualityComparer<TValue>.Default.Equals(Value, valueTuple.Item2),
            _ => false,
        };
    }

    public override int GetHashCode() => Hasher.Combine<TKey, TValue>(Key, Value);

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
        }.GetResult(out charsWritten);
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        var text = new DefaultInterpolatedStringHandler(4, 2, formatProvider);
        text.AppendLiteral("(");
        text.AppendFormatted<TKey>(Key, format);
        text.AppendLiteral(", ");
        text.AppendFormatted<TValue>(Value, format);
        text.AppendLiteral(")");
        return text.ToStringAndClear();
    }

    public override string ToString() => $"({Key}, {Value})";
}
