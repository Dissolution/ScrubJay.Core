namespace ScrubJay.Functional;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TOk"></typeparam>
/// <remarks>
/// Ok evaluates as <c>true</c>, all other methods are a direct pass-through to <see cref="Value"/><br/>
/// Ok should ideally only be used as a pass-through to an end <see cref="Result{TOk,TError}"/> as it implicitly converts
/// </remarks>
[PublicAPI]
public readonly struct Ok<TOk> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Ok<TOk>, Ok<TOk>, bool>,
    IEqualityOperators<Ok<TOk>, TOk, bool>,
    IComparisonOperators<Ok<TOk>, Ok<TOk>, bool>,
    IComparisonOperators<Ok<TOk>, TOk, bool>,
#endif
    IEquatable<Ok<TOk>>,
    IEquatable<TOk>,
    IComparable<Ok<TOk>>,
    IComparable<TOk>,
    IEnumerable<TOk>
{
    public static implicit operator bool(Ok<TOk> _) => true;
    public static bool operator true(Ok<TOk> _) => true;
    public static bool operator false(Ok<TOk> _) => false;
    
    public static bool operator ==(Ok<TOk> left, Ok<TOk> right) => EqualityComparer<TOk>.Default.Equals(left.Value, right.Value);
    public static bool operator !=(Ok<TOk> left, Ok<TOk> right) => !EqualityComparer<TOk>.Default.Equals(left.Value, right.Value);
    public static bool operator ==(Ok<TOk> left, TOk? right) => EqualityComparer<TOk>.Default.Equals(left.Value, right!);
    public static bool operator !=(Ok<TOk> left, TOk? right) => !EqualityComparer<TOk>.Default.Equals(left.Value, right!);
    
    public static bool operator >(Ok<TOk> left, Ok<TOk> right) => Comparer<TOk>.Default.Compare(left.Value, right.Value) > 0;
    public static bool operator >(Ok<TOk> left, TOk right) => Comparer<TOk>.Default.Compare(left.Value, right) > 0;
    public static bool operator >=(Ok<TOk> left, Ok<TOk> right) => Comparer<TOk>.Default.Compare(left.Value, right.Value) >= 0;
    public static bool operator >=(Ok<TOk> left, TOk right) => Comparer<TOk>.Default.Compare(left.Value, right) >= 0;
    public static bool operator <(Ok<TOk> left, Ok<TOk> right) => Comparer<TOk>.Default.Compare(left.Value, right.Value) < 0;
    public static bool operator <(Ok<TOk> left, TOk right) => Comparer<TOk>.Default.Compare(left.Value, right) < 0;
    public static bool operator <=(Ok<TOk> left, Ok<TOk> right) => Comparer<TOk>.Default.Compare(left.Value, right.Value) <= 0;
    public static bool operator <=(Ok<TOk> left, TOk right) => Comparer<TOk>.Default.Compare(left.Value, right) <= 0;

    /// <summary>
    /// The underlying Ok value
    /// </summary>
    public readonly TOk Value;

    /// <summary>
    /// Construct a new <see cref="Ok{TOk}"/> containing the <paramref name="okValue"/>
    /// </summary>
    public Ok(TOk okValue)
    {
        this.Value = okValue;
    }
    public void Deconstruct(out TOk okValue)
    {
        okValue = Value;
    }

    public int CompareTo(Ok<TOk> other) => Comparer<TOk>.Default.Compare(this.Value!, other.Value!);
    public int CompareTo(TOk? other) => Comparer<TOk>.Default.Compare(this.Value!, other!);

    public bool Equals(Ok<TOk> ok) => EqualityComparer<TOk>.Default.Equals(Value!, ok.Value!);
    public bool Equals(TOk? okValue) => EqualityComparer<TOk>.Default.Equals(Value!, okValue!);
    public bool Equals(bool isOkay) => isOkay;
    public override bool Equals(object? obj) => obj switch
    {
        Ok<TOk> ok => Equals(ok),
        TOk okValue => Equals(okValue),
        bool isOkay => isOkay,
        _ => false,
    };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TOk> GetEnumerator()
    {
        // Same behavior as the Result this should end up as
        yield return Value;
    }

    public override int GetHashCode() => Hasher.GetHashCode<TOk>(Value);
    public override string ToString() => $"Ok({Value})";
}