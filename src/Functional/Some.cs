namespace ScrubJay.Functional;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Some evaluates as <c>true</c>, all other methods are a direct pass-through to <see cref="Value"/><br/>
/// Some should ideally only be used as a pass-through to an end <see cref="Option{T}"/> as it implicitly converts
/// </remarks>
[PublicAPI]
public readonly struct Some<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Some<T>, Some<T>, bool>,
    IEqualityOperators<Some<T>, T, bool>,
    IComparisonOperators<Some<T>, Some<T>, bool>,
    IComparisonOperators<Some<T>, T, bool>,
#endif
    IEquatable<Some<T>>,
    IEquatable<T>,
    IComparable<Some<T>>,
    IComparable<T>,
    IEnumerable<T>
{
    public static implicit operator bool(Some<T> _) => true;
    public static bool operator true(Some<T> _) => true;
    public static bool operator false(Some<T> _) => false;
    
    public static bool operator ==(Some<T> left, Some<T> right) => EqualityComparer<T>.Default.Equals(left.Value, right.Value);
    public static bool operator !=(Some<T> left, Some<T> right) => !EqualityComparer<T>.Default.Equals(left.Value, right.Value);
    public static bool operator ==(Some<T> left, T? right) => EqualityComparer<T>.Default.Equals(left.Value, right!);
    public static bool operator !=(Some<T> left, T? right) => !EqualityComparer<T>.Default.Equals(left.Value, right!);
    
    public static bool operator >(Some<T> left, Some<T> right) => Comparer<T>.Default.Compare(left.Value, right.Value) > 0;
    public static bool operator >(Some<T> left, T right) => Comparer<T>.Default.Compare(left.Value, right) > 0;
    public static bool operator >=(Some<T> left, Some<T> right) => Comparer<T>.Default.Compare(left.Value, right.Value) >= 0;
    public static bool operator >=(Some<T> left, T right) => Comparer<T>.Default.Compare(left.Value, right) >= 0;
    public static bool operator <(Some<T> left, Some<T> right) => Comparer<T>.Default.Compare(left.Value, right.Value) < 0;
    public static bool operator <(Some<T> left, T right) => Comparer<T>.Default.Compare(left.Value, right) < 0;
    public static bool operator <=(Some<T> left, Some<T> right) => Comparer<T>.Default.Compare(left.Value, right.Value) <= 0;
    public static bool operator <=(Some<T> left, T right) => Comparer<T>.Default.Compare(left.Value, right) <= 0;

    /// <summary>
    /// The underlying Some value
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// Construct a new <see cref="Some{T}"/> containing the <paramref name="someValue"/>
    /// </summary>
    public Some(T someValue)
    {
        this.Value = someValue;
    }
    public void Deconstruct(out T someValue)
    {
        someValue = Value;
    }

    public int CompareTo(Some<T> other) => Comparer<T>.Default.Compare(this.Value!, other.Value!);
    public int CompareTo(T? other) => Comparer<T>.Default.Compare(this.Value!, other!);

    public bool Equals(Some<T> some) => EqualityComparer<T>.Default.Equals(Value!, some.Value!);
    public bool Equals(T? someValue) => EqualityComparer<T>.Default.Equals(Value!, someValue!);
    public bool Equals(bool isSome) => isSome;
    public override bool Equals(object? obj) => obj switch
    {
        Some<T> some => Equals(some),
        T someValue => Equals(someValue),
        bool isSome => Equals(isSome),
        _ => false,
    };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        // Same behavior as the Option this should end up as
        yield return Value;
    }

    public override int GetHashCode() => Hasher.GetHashCode<T>(Value);
    public override string ToString() => $"Some({Value})";
}