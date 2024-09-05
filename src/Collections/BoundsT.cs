using ScrubJay.Buffers;
using ScrubJay.Memory;
using ScrubJay.Text;

namespace ScrubJay.Collections;

/// <summary>
/// Bounding conditional for <typeparamref name="T"/> values
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public readonly struct Bounds<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Bounds<T>, Bounds<T>, bool>,
#endif
    IEquatable<Bounds<T>>,
    IFormattable
{
    public static bool operator ==(Bounds<T> left, Bounds<T> right) => left.Equals(right);
    public static bool operator !=(Bounds<T> left, Bounds<T> right) => !left.Equals(right);


    public readonly Option<T> Minimum;
    public readonly bool MinimumIsInclusive;

    public readonly Option<T> Maximum;
    public readonly bool MaximumIsInclusive;

    public bool IsUnbounded => Minimum.IsNone() || Maximum.IsNone();

    public Bounds(Option<T> minimum, bool minimumIsInclusive, Option<T> maximum, bool maximumIsInclusive)
    {
        this.Minimum = minimum;
        this.MinimumIsInclusive = minimumIsInclusive;
        this.Maximum = maximum;
        this.MaximumIsInclusive = maximumIsInclusive;
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
                {
                    return false;
                }
            }
            else
            {
                if (c <= 0)
                {
                    return false;
                }
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
                {
                    return false;
                }
            }
            else
            {
                if (c >= 0)
                {
                    return false;
                }
            }
        }

        // Any value matches None, None
        return true;
    }

    public T Clamped(T value)
    {
        if (Minimum.IsSome(out var min))
        {
            int c = Comparer<T>.Default.Compare(value, min);
            if (MinimumIsInclusive)
            {
                if (c < 0)
                {
                    return min;
                }
            }
            else
            {
                if (c <= 0)
                {
                    return min;
                }
            }
        }

        if (Maximum.IsSome(out var max))
        {
            int c = Comparer<T>.Default.Compare(value, max);
            if (MaximumIsInclusive)
            {
                if (c > 0)
                {
                    return max;
                }
            }
            else
            {
                if (c >= 0)
                {
                    return max;
                }
            }
        }

        return value;
    }

    public void Clamp(ref T value)
    {
        value = Clamped(value);
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

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
    {
        SpanWriter<char> writer = new(destination);

        if (Minimum.IsSome(out var min))
        {
            if (MinimumIsInclusive)
            {
                if (!writer.TryWrite('['))
                    goto Fail;
            }
            else
            {
                if (!writer.TryWrite('('))
                    goto Fail;
            }

            if (!writer.TryWriteFormatted<T>(min, format.ToString(), provider))
                goto Fail;
        }

        if (!writer.TryWrite(".."))
            goto Fail;

        if (Maximum.IsSome(out var max))
        {
            if (!writer.TryWriteFormatted<T>(max, format.ToString(), provider))
                goto Fail;

            if (MaximumIsInclusive)
            {
                if (!writer.TryWrite(']'))
                    goto Fail;
            }
            else
            {
                if (!writer.TryWrite(')'))
                    goto Fail;
            }
        }

        charsWritten = writer.Count;
        return true;

    Fail:
        writer.Clear();
        charsWritten = 0;
        return false;
    }

    public string ToString(string? format, IFormatProvider? _ = default)
    {
        var text = new TextBuffer();
        if (Minimum.IsSome(out var min))
        {
            text.Append(MinimumIsInclusive ? '[' : '(');
            text.AppendFormatted<T>(min, format);
        }

        text.Append(", ");

        if (Maximum.IsSome(out var max))
        {
            text.AppendFormatted<T>(max, format);
            text.Append(MaximumIsInclusive ? ']' : ')');
        }

        return text.ToStringAndDispose();
    }

    public override string ToString() => ToString(default, default);
}