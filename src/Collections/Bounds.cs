using System.Diagnostics;
using ScrubJay.Memory;
using ScrubJay.Text;
using ScrubJay.Utilities;

namespace ScrubJay.Collections;

public readonly struct Bounds<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Bounds<T>, Bounds<T>, bool>,
    ISpanParsable<Bounds<T>>,
    IParsable<Bounds<T>>,
#endif
#if NET6_0_OR_GREATER
    //ISpanFormattable,
#endif
    IEquatable<Bounds<T>>,
    IFormattable
{
    public static bool operator ==(Bounds<T> left, Bounds<T> right) => left.Equals(right);

    public static bool operator !=(Bounds<T> left, Bounds<T> right) => !left.Equals(right);
    
    #if NET7_0_OR_GREATER
    static Bounds<T> IParsable<Bounds<T>>.Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    static bool IParsable<Bounds<T>>.TryParse(string? s, IFormatProvider? provider, out Bounds<T> result)
    {
        throw new NotImplementedException();
    }

    static Bounds<T> ISpanParsable<Bounds<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    static bool ISpanParsable<Bounds<T>>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Bounds<T> result)
    {
        throw new NotImplementedException();
    }
#endif

    // public static Result<Bounds<T>, Exception> TryParse(ReadOnlySpan<char> text)
    // {
    //     Option<T> min;
    //     bool minInclusive;
    //     Option<T> max;
    //     bool maxInclusive;
    //     
    //     var reader = new SpanReader<char>(text);
    //     var firstChar = reader.TakeAny('[', '(');
    //     if (firstChar.Length == 0)
    //     {
    //         min = None();
    //         minInclusive = default;
    //     }
    //     else if (firstChar.Length > 1)
    //     {
    //         return new ArgumentOutOfRangeException(nameof(text), text.ToString(), "Invalid Bounds");
    //     }
    //     else
    //     {
    //         char ch = firstChar[0];
    //         if (ch == '[')
    //         {
    //             minInclusive = true;
    //         }
    //         else
    //         {
    //             Debug.Assert(ch == '(');
    //             minInclusive = false;
    //         }
    //         
    //         
    //     }
    //
    //     throw new NotImplementedException();
    //
    // }

    public static Bounds<T> Any { get; set; } = new Bounds<T>(None, false, None, false);

    public static Bounds<T> StartAt(T minimum, bool minIsInclusive = true) => new Bounds<T>(minimum, minIsInclusive, None, default);

    public static Bounds<T> EndAt(T maximum, bool maxIsInclusive = true) => new Bounds<T>(None, false, maximum, maxIsInclusive);

    public static Bounds<T> Create(T inclusiveMinimum, T exclusiveMaximum) => new Bounds<T>(inclusiveMinimum, true, exclusiveMaximum, false);

    public static Bounds<T> Create(Option<T> minimum, bool minIsInclusive, Option<T> maximum, bool maxIsInclusive) => new Bounds<T>(minimum, minIsInclusive, maximum, maxIsInclusive);

    public static Bounds<int> Create(Range range)
    {
        if (range.Start.IsFromEnd || range.End.IsFromEnd)
            throw new ArgumentOutOfRangeException(nameof(range), range, "Range Start and End must be specified from 0");
        return new Bounds<int>(range.Start.Value, true, range.End.Value, false);
    }


    public readonly Option<T> Minimum;
    public readonly bool MinimumIsInclusive;

    public readonly Option<T> Maximum;
    public readonly bool MaximumIsInclusive;

    public bool IsUnbounded => Minimum.IsNone() || Maximum.IsNone();

    private Bounds(Option<T> minimum, bool minimumIsInclusive, Option<T> maximum, bool maximumIsInclusive)
    {
        Minimum = minimum;
        MinimumIsInclusive = minimumIsInclusive;
        Maximum = maximum;
        MaximumIsInclusive = maximumIsInclusive;
    }

    public bool Contains(T? value)
    {
        if (Minimum.IsSome(out var min))
        {
            if (value is null)
                return false;

            int c = Comparer<T>.Default.Compare(value, min);
            if (MinimumIsInclusive)
            {
                if (c < 0)
                    return false;
            }
            else
            {
                if (c <= 0)
                    return false;
            }
        }

        if (Maximum.IsSome(out var max))
        {
            if (value is null)
                return false;

            int c = Comparer<T>.Default.Compare(value, max);
            if (MaximumIsInclusive)
            {
                if (c > 0)
                    return false;
            }
            else
            {
                if (c >= 0)
                    return false;
            }
        }

        return true;
    }

    public T Clamp(T value)
    {
        if (Minimum.IsSome(out var min))
        {
            int c = Comparer<T>.Default.Compare(value, min);
            if (MinimumIsInclusive)
            {
                if (c < 0)
                    return min;
            }
            else
            {
                if (c <= 0)
                    return min;
            }
        }

        if (Maximum.IsSome(out var max))
        {
            int c = Comparer<T>.Default.Compare(value, max);
            if (MaximumIsInclusive)
            {
                if (c > 0)
                    return max;
            }
            else
            {
                if (c >= 0)
                    return max;
            }
        }

        return value;
    }

    public bool Equals(Bounds<T> other)
    {
        return other.MinimumIsInclusive == MinimumIsInclusive && other.MaximumIsInclusive == MaximumIsInclusive && other.Minimum == Minimum && other.Maximum == Maximum;
    }

    public override bool Equals(object? obj) => obj is Bounds<T> bounds && Equals(bounds);

    public override int GetHashCode()
    {
        return Hasher.Combine(Minimum, MinimumIsInclusive, Maximum, MaximumIsInclusive);
    }

    public string ToString(string? format, IFormatProvider? _ = default)
    {
        if (format is null)
            return ToString();

        using var text = new InterpolatedStringHandler(4, 2);
        if (Minimum.IsSome(out var min))
        {
            if (MinimumIsInclusive)
            {
                text.AppendFormatted('[');
            }
            else
            {
                text.AppendFormatted('(');
            }

            text.AppendFormatted<T>(min, format);
        }

        text.AppendLiteral("..");
        
        if (Maximum.IsSome(out var max))
        {
            text.AppendFormatted<T>(max, format);
            
            if (MaximumIsInclusive)
            {
                text.AppendFormatted(']');
            }
            else
            {
                text.AppendFormatted(')');
            }
        }

        return text.ToString();
    }

    public override string ToString()
    {
        var text = StringBuilderPool.Rent();

        if (Minimum.IsSome(out var min))
        {
            text.Append(MinimumIsInclusive ? '[' : '(').Append(min);
        }

        text.Append("..");
        if (Maximum.IsSome(out var max))
        {
            text.Append(max).Append(MaximumIsInclusive ? ']' : ')');
        }

        return text.ToStringAndReturn();
    }
}