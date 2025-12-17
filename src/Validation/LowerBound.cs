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