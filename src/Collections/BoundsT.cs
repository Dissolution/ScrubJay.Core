#pragma warning disable MA0048

namespace ScrubJay.Collections;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Bounds<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Bounds<T>, Bounds<T>, bool>,
#endif
IEquatable<Bounds<T>>
{
    public static bool operator ==(Bounds<T> left, Bounds<T> right) => left.Equals(right);
    public static bool operator !=(Bounds<T> left, Bounds<T> right) => !left.Equals(right);   

    public readonly Option<Bound<T>> Lower;
    public readonly Option<Bound<T>> Upper;

    public Bounds(Option<Bound<T>> lower, Option<Bound<T>> upper)
    {
        this.Lower = lower;
        this.Upper = upper;
    }

    public bool Contains(T? value)
    {
        if (Lower.IsSome(out Bound<T> lowerBounds))
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

        if (Upper.IsSome(out Bound<T> upperBounds))
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

    [return: NotNullIfNotNull(nameof(value))]
    public T? Clamped(T? value)
    {
        if (Lower.IsSome(out Bound<T> lowerBounds))
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

        if (Upper.IsSome(out Bound<T> upperBounds))
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

    public bool Equals(Bounds<T> other) => other.Lower.Equals(this.Lower) && other.Upper.Equals(this.Upper);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Bounds<T> bounds && Equals(bounds);

    public override int GetHashCode() => Hasher.Combine(Lower, Upper);

    public override string ToString()
    {
        var text = new DefaultInterpolatedStringHandler(4, 2);
        
        if (Lower.IsSome(out var lower))
        {
            if (lower.IsInclusive)
            {
                text.AppendLiteral("[");
            }
            else
            {
                text.AppendLiteral("(");
            }
            text.AppendFormatted<T>(lower.Value);
        }
        
        text.AppendLiteral("..");
        
        if (Upper.IsSome(out var upper))
        {
            text.AppendFormatted<T>(upper.Value);
            
            if (upper.IsInclusive)
            {
                text.AppendLiteral("]");
            }
            else
            {
                text.AppendLiteral(")");
            }
        }

        return text.ToStringAndClear();
    }
}