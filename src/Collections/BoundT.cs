namespace ScrubJay.Collections;


[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Bound<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Bound<T>, Bound<T>, bool>,
#endif
    IEquatable<Bound<T>>
{
    public static bool operator ==(Bound<T> left, Bound<T> right) => left.Equals(right);
    public static bool operator !=(Bound<T> left, Bound<T> right) => !left.Equals(right);

    public readonly T Value;
    public readonly bool IsInclusive;
    
    public Bound(T value, bool isInclusive)
    {
        this.Value = value;
        this.IsInclusive = isInclusive;
    }

    public void Deconstruct(out T value, out bool isInclusive)
    {
        value = Value;
        isInclusive = IsInclusive;
    }

    public bool Equals(Bound<T> other)
    {
        return other.IsInclusive == this.IsInclusive && EqualityComparer<T>.Default.Equals(other.Value, this.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Bound<T> bound && Equals(bound);

    public override int GetHashCode() => Hasher.Combine<T, bool>(Value, IsInclusive);

    public override string ToString() => IsInclusive ? $"[{Value}]" : $"({Value})";
}