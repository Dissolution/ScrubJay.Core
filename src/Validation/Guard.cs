namespace ScrubJay.Validation;

public readonly ref struct LowerBound<T>
{
    public static implicit operator LowerBound<T>(T value) => new(value);
    public static implicit operator LowerBound<T>((T Value, bool IsInclusive) tuple) => new(tuple.Value, tuple.IsInclusive);

    public readonly Option<T> Value;

    public readonly bool IsInclusive;

    public bool IsUnbounded => Value.IsNone();

    public LowerBound()
    {
        this.Value = None;
        this.IsInclusive = false;
    }

    public LowerBound(T value)
    {
        this.Value = Some(value);
        this.IsInclusive = true;
    }

    public LowerBound(T value, bool isInclusive)
    {
        this.Value = Some(value);
        this.IsInclusive = isInclusive;
    }

    public void Deconstruct(out Option<T> value, out bool isInclusive)
    {
        value = Value;
        isInclusive = IsInclusive;
    }

    public bool Contains(T other)
    {
        if (Value.IsSome(out var value))
        {
            int c = Comparer<T>.Default.Compare(value, other);

            if (IsInclusive)
            {
                return c <= 0;
            }
            else
            {
                return c < 0;
            }
        }
        else
        {
            return true;
        }
    }

    public override string ToString()
    {
        if (Value.IsSome(out var value))
        {
            if (IsInclusive)
            {
                return Build($"[{value}..");
            }
            else
            {
                return Build($"({value}..");
            }
        }
        else
        {
            return "..";
        }
    }
}

public readonly ref struct UpperBound<T>
{
    public static implicit operator UpperBound<T>(T value) => new(value);
    public static implicit operator UpperBound<T>((T Value, bool IsInclusive) tuple) => new(tuple.Value, tuple.IsInclusive);

    public readonly Option<T> Value;

    public readonly bool IsInclusive;

    public bool IsUnbounded => Value.IsNone();

    public UpperBound()
    {
        this.Value = None;
        this.IsInclusive = false;
    }

    public UpperBound(T value)
    {
        this.Value = Some(value);
        this.IsInclusive = false;
    }

    public UpperBound(T value, bool isInclusive)
    {
        this.Value = Some(value);
        this.IsInclusive = isInclusive;
    }

    public void Deconstruct(out Option<T> value, out bool isInclusive)
    {
        value = Value;
        isInclusive = IsInclusive;
    }

    public bool Contains(T other)
    {
        if (Value.IsSome(out var value))
        {
            int c = Comparer<T>.Default.Compare(value, other);

            if (IsInclusive)
            {
                return c >= 0;
            }
            else
            {
                return c > 0;
            }
        }
        else
        {
            return true;
        }
    }

    public override string ToString()
    {
        if (Value.IsSome(out var value))
        {
            if (IsInclusive)
            {
                return Build($"..{value}]");
            }
            else
            {
                return Build($"..{value})");
            }
        }
        else
        {
            return "..";
        }
    }
}

[PublicAPI]
public static partial class Guard
{
    private static string GetOutOfBoundsMessage<T>(LowerBound<T> lowerBound, UpperBound<T> upperBound)
    {
        var (lowValue, lowInc) = lowerBound;
        var (hiValue, hiInc) = upperBound;

        return TextBuilder.New
            .Append("must be in ")
            .If(lowValue,
                (tb, lower) => tb
                    .If(lowInc, '[', '(')
                    .Write<T>(lower))
            .Append("..")
            .If(hiValue,
                (tb, upper) => tb
                    .Append<T>(upper)
                    .If(hiInc, ']', ')'))
            .ToStringAndDispose();
    }

    public static T Between<T>(T value,
        LowerBound<T> lowerBound = default,
        UpperBound<T> upperBound = default,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (!lowerBound.Contains(value) || !upperBound.Contains(value))
            throw Ex.ArgRange<T>(value, GetOutOfBoundsMessage<T>(lowerBound, upperBound), valueName);
        return value;
    }

    public static T Between<T>(T value,
        T inclusiveLowerBound,
        T exclusiveUpperBound,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        int c = Comparer<T>.Default.Compare(value, inclusiveLowerBound);
        if (c >= 0)
        {
            c = Comparer<T>.Default.Compare(value, exclusiveUpperBound);
            if (c < 0)
            {
                return value;
            }
        }

        throw Ex.ArgRange<T>(value, $"must be in [{inclusiveLowerBound}..{exclusiveUpperBound})", valueName);
    }
}