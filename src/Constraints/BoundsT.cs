#pragma warning disable MA0048

namespace ScrubJay.Constraints;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly record struct Bounds<T>(Option<Bound<T>> Lower, Option<Bound<T>> Upper)
{
    public bool Contains(T? value)
    {
        if (Lower.HasSome(out Bound<T> lowerBounds))
        {
            (T lower, bool lowerInc) = lowerBounds;
            if (lowerInc)
            {
                if (Comparer<T>.Default.Compare(value!, lower!) < 0)
                    return false;
            }
            else
            {
                if (Comparer<T>.Default.Compare(value!, lower!) <= 0)
                    return false;
            }
        }

        if (Upper.HasSome(out Bound<T> upperBounds))
        {
            (T upper, bool upperInc) = upperBounds;
            if (upperInc)
            {
                if (Comparer<T>.Default.Compare(value!, upper!) > 0)
                    return false;
            }
            else
            {
                if (Comparer<T>.Default.Compare(value!, upper!) >= 0)
                    return false;
            }
        }

        return true;
    }

    public bool Contains(T? value, IComparer<T>? comparer)
    {
        comparer ??= Comparer<T>.Default;

        if (Lower.HasSome(out Bound<T> lowerBounds))
        {
            (T lower, bool lowerInc) = lowerBounds;
            if (lowerInc)
            {
                if (comparer.Compare(value!, lower!) < 0)
                    return false;
            }
            else
            {
                if (comparer.Compare(value!, lower!) <= 0)
                    return false;
            }
        }

        if (Upper.HasSome(out Bound<T> upperBounds))
        {
            (T upper, bool upperInc) = upperBounds;
            if (upperInc)
            {
                if (comparer.Compare(value!, upper!) > 0)
                    return false;
            }
            else
            {
                if (comparer.Compare(value!, upper!) >= 0)
                    return false;
            }
        }

        return true;
    }

    [return: NotNullIfNotNull(nameof(value))]
    public T? Clamped(T? value)
    {
        if (Lower.HasSome(out Bound<T> lowerBounds))
        {
            (T lower, bool lowerInc) = lowerBounds;
            if (lowerInc)
            {
                if (Comparer<T>.Default.Compare(value!, lower!) < 0)
                    return lower;
            }
            else
            {
                if (Comparer<T>.Default.Compare(value!, lower!) <= 0)
                    return lower;
            }
        }

        if (Upper.HasSome(out Bound<T> upperBounds))
        {
            (T upper, bool upperInc) = upperBounds;
            if (upperInc)
            {
                if (Comparer<T>.Default.Compare(value!, upper!) > 0)
                    return upper;
            }
            else
            {
                if (Comparer<T>.Default.Compare(value!, upper!) >= 0)
                    return upper;
            }
        }

        return value;
    }

    public override string ToString()
    {
        var text = new Buffer<char>();

        if (Lower.HasSome(out var lower))
        {
            if (lower.IsInclusive)
            {
                text.Write("[");
            }
            else
            {
                text.Write("(");
            }
            text.Write<T>(lower.Value);
        }

        text.Write("..");

        if (Upper.HasSome(out var upper))
        {
            text.Write<T>(upper.Value);

            if (upper.IsInclusive)
            {
                text.Write("]");
            }
            else
            {
                text.Write(")");
            }
        }

        return text.ToStringAndDispose();
    }
}