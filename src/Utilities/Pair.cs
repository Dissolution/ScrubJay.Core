#pragma warning disable S907
#pragma warning disable CA1000, CA1045
// ReSharper disable ArrangeThisQualifier

using ScrubJay.Memory;

namespace ScrubJay.Utilities;

[PublicAPI]
public static class Pair
{
    #if NET7_0_OR_GREATER
    public static Result<Pair<TKey, TValue>, Exception> TryParse<TKey, TValue>(scoped ReadOnlySpan<char> text, IFormatProvider? provider)
        where TKey : ISpanParsable<TKey>
        where TValue : ISpanParsable<TValue>
    {
        var reader = new SpanReader<char>(text);
        var start = reader.TakeWhile('(');
        if (start.Length != 1)
            return new ArgumentException("A Pair's representation must be in the form '(value, value)'", nameof(text));

        var keySpan = reader.TakeUntil(',').Trim();
        if (!TKey.TryParse(keySpan, provider, out var key))
            return new ArgumentException($"Could not parse '{keySpan}' to a {typeof(TKey)}", nameof(text));

        var comma = reader.TakeWhile(',');
        if (comma.Length !=1)
            return new ArgumentException("A Pair's representation must be in the form '(value, value)'", nameof(text));

        var valueSpan = reader.TakeUntil(')').Trim();
        if (!TValue.TryParse(valueSpan, provider, out var value))
            return new ArgumentException($"Could not parse '{valueSpan}' to a {typeof(TValue)}", nameof(text));

        var end = reader.TakeWhile(')');
        if (end.Length != 1 || reader.RemainingCount != 0)
            return new ArgumentException("A Pair's representation must be in the form '(value, value)'", nameof(text));

        return OkEx(New(key, value));
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
    ISpanParsable<Pair<TKey, TValue>>,
    IParsable<Pair<TKey, TValue>>,
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

    public static Pair<TKey, TValue> Parse(string s, IFormatProvider? provider) => throw new NotImplementedException();

    public static Pair<TKey, TValue> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => throw new NotImplementedException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Pair<TKey, TValue> result) => throw new NotImplementedException();

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pair<TKey, TValue> result) => throw new NotImplementedException();



    // public static Result<Pair<TKey, TValue>, Exception> TryParse(scoped ReadOnlySpan<char> text, IFormatProvider? provider)
    // {
    //     var reader = new SpanReader<char>(text);
    //     var start = reader.TakeWhile('(');
    //     if (start.Length != 1)
    //         return new ArgumentException("A Pair's representation must be in the form '(value, value)'", nameof(text));
    //     var keySpan = reader.TakeUntil(',').Trim();
    //     TKey.
    // }


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
            Tuple<TKey, TValue> tuple =>
                EqualityComparer<TKey>.Default.Equals(this.Key, tuple.Item1) &&
                EqualityComparer<TValue>.Default.Equals(this.Value, tuple.Item2),
            ValueTuple<TKey, TValue> valueTuple =>
                EqualityComparer<TKey>.Default.Equals(this.Key, valueTuple.Item1) &&
                EqualityComparer<TValue>.Default.Equals(this.Value, valueTuple.Item2),
            _ => false,
        };
    }

    public override int GetHashCode() => Hasher.Combine<TKey, TValue>(this.Key, this.Value);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null)
    {
        var writer = new FormatWriter(destination);
        if (!writer.TryWrite('('))
            goto FAIL;
        if (!writer.TryWriteFormatted<TKey>(this.Key, format, provider))
            goto FAIL;
        if (!writer.TryWrite(", "))
            goto FAIL;
        if (!writer.TryWriteFormatted<TValue>(this.Value, format, provider))
            goto FAIL;
        if (!writer.TryWrite(')'))
            goto FAIL;

        charsWritten = writer.Count;
        return true;

    FAIL:
        destination.Clear();
        charsWritten = 0;
        return false;
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

    public override string ToString() => $"({this.Key}, {this.Value})";
}
