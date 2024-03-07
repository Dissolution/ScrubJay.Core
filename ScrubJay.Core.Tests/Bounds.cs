using System.Diagnostics;
using ScrubJay.Text;
using ScrubJay.Validation;

namespace ScrubJay.Tests;

public readonly struct Bounds<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Bounds<T>, Bounds<T>, bool>,
    IComparisonOperators<Bounds<T>, Bounds<T>, bool>,
#endif
    IEquatable<Bounds<T>>,
    IComparable<Bounds<T>>
    where T : IEquatable<T>, IComparable<T>
{
    public static bool operator ==(Bounds<T> left, Bounds<T> right) 
        => left.Minimum == right.Minimum && left.Maximum == right.Maximum;

    public static bool operator !=(Bounds<T> left, Bounds<T> right)
        => left.Minimum != right.Minimum || left.Maximum != right.Maximum;

    public static bool operator >(Bounds<T> left, Bounds<T> right) => left.CompareTo(right) > 0;

    public static bool operator >=(Bounds<T> left, Bounds<T> right) => left.CompareTo(right) >= 0;

    public static bool operator <(Bounds<T> left, Bounds<T> right) => left.CompareTo(right) < 0;

    public static bool operator <=(Bounds<T> left, Bounds<T> right) => left.CompareTo(right) <= 0;

    public static Bounds<T> Within([NotNull] T minimum, [NotNull] T maximum)
    {
        Throw.IfNull(minimum);
        Throw.IfNull(maximum);
        return new Bounds<T>(Some(minimum), Some(maximum));
    }

    public static Bounds<T> FromMinimum(T minimum)
    {
        Throw.IfNull(minimum);
        return new Bounds<T>(Some(minimum), None<T>());
    }

    public static Bounds<T> ToMaximum(T maximum)
    {
        Throw.IfNull(maximum);
        return new Bounds<T>(None<T>(), Some(maximum));
    }

    public readonly Option<T> Minimum;

    public readonly Option<T> Maximum;

    private Bounds(Option<T> minimum, Option<T> maximum)
    {
        Debug.Assert(minimum.IsNone() || (minimum.IsSome(out var min) && min is not null));
        Debug.Assert(maximum.IsNone() || (maximum.IsSome(out var max) && max is not null));
        this.Minimum = minimum;
        this.Maximum = maximum;
    }

    public bool Contains(T value)
    {
        if (this.Minimum.IsSome(out var min) &&
            Comparer<T>.Default.Compare(value, min) < 0)
            return false;

        if (this.Maximum.IsSome(out var max) &&
            Comparer<T>.Default.Compare(value, max) > 0)
            return false;

        return true;
    }

    internal void Match(
        Action<T, T> finite,
        Action<T> countablyInfinite,
        Action infinite)
    {
        if (this.Minimum.IsSome(out var min))
        {
            if (this.Maximum.IsSome(out var max))
            {
                finite(min, max);
                return;
            }
            else
            {
                countablyInfinite(min);
                return;
            }
        }
        else
        {
            if (this.Maximum.IsSome(out var max))
            {
                countablyInfinite(max);
                return;
            }
            else
            {
                infinite();
                return;
            }
        }
    }
    
    internal TResult Match<TResult>(
        Func<T, T, TResult> finite,
        Func<T, TResult> countablyInfinite,
        Func<TResult> infinite)
    {
        if (this.Minimum.IsSome(out var min))
        {
            if (this.Maximum.IsSome(out var max))
            {
                return finite(min, max);
            }
            else
            {
                return countablyInfinite(min);
            }
        }
        else
        {
            if (this.Maximum.IsSome(out var max))
            {
                return countablyInfinite(max);
            }
            else
            {
                return infinite();
            }
        }
    }
    
    public int CompareTo(Bounds<T> bounds) => this.Match(
        (thisMin, thisMax) =>
        {
            return bounds.Match(
                (thatMin, thatMax) =>
                {
                    if (thisMin.Equals(thatMin) && thisMax.Equals(thatMax))
                        return 0; // the same!
                        
                    int c;
                        
                    // check for this < that
                    c = thisMax.CompareTo(thatMin);
                    if (c < 0) return c;
                    // check for this > that
                    c = thisMin.CompareTo(thatMax);
                    if (c > 0) return c;
                    // we overlap
                    // is this inside of that?
                    if (thisMin.CompareTo(thatMin) >= 0 && thisMax.CompareTo(thatMax) <= 0)
                    {
                        // this is smaller
                        return -1;
                    }
                    // is that inside of this?
                    if (thisMin.CompareTo(thatMin) <= 0 && thisMax.CompareTo(thatMax) >= 0)
                    {
                        // that is smaller
                        return 1;
                    }
                    // We can't tell a difference because we have no way of determining the actual distance between Min and Max
                    return 0;
                },
                (thatBound) =>
                {
                    // i'm finite, thus smaller
                    return -1;
                },
                () =>
                {
                    // i'm finite, thus smaller
                    return -1;
                });
        },
        (thisBound) =>
        {
            return bounds.Match(
                (thatMin, thatMax) =>
                {
                    // i'm infinite, thus bigger
                    return 1;
                },
                (thatBound) =>
                {
                    // Compare the bounds we have
                    return Comparer<T>.Default.Compare(thisBound, thatBound);
                },
                () =>
                {
                    // i'm countably infinite, thus smaller
                    return -1;
                });
        },
        () =>
        {
            return bounds.Match(
                (thatMin, thatMax) =>
                {
                    // i'm infinite, thus bigger
                    return 1;
                },
                (thatBound) =>
                {
                    // i'm uncountably infinite, thus bigger
                    return 1;
                },
                () =>
                {
                    // two infinite bounds
                    return 0; 
                });
        });

    public bool Equals(Bounds<T> bounds)
    {
        return this.Minimum == bounds.Minimum &&
            this.Maximum == bounds.Maximum;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Bounds<T> bounds)
            return Equals(bounds);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(this.Minimum, this.Maximum);
    }

    public override string ToString()
    {
        var text = StringBuilderPool.Rent();
        if (this.Minimum.IsSome(out var min))
        {
            text.Append('[').Append<T>(min);
        }
        text.Append("..");
        if (this.Maximum.IsSome(out var max))
        {
            text.Append<T>(max).Append(']');
        }
        return text.ToStringAndReturn();
    }
}