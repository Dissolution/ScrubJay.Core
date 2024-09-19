namespace ScrubJay;

[PublicAPI]
public readonly struct Ok<TOk> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Ok<TOk>, Ok<TOk>, bool>,
    IEqualityOperators<Ok<TOk>, TOk, bool>,
#endif
    IEquatable<Ok<TOk>>,
    IEquatable<TOk>
{
    // Ok is true
    public static implicit operator bool(Ok<TOk> _) => true;
    public static bool operator true(Ok<TOk> _) => true;
    public static bool operator false(Ok<TOk> _) => false;
    
    // Ok is a passthrough for its Value
    public static bool operator ==(Ok<TOk> left, Ok<TOk> right) => EqualityComparer<TOk>.Default.Equals(left.Value, right.Value);
    public static bool operator !=(Ok<TOk> left, Ok<TOk> right) => !EqualityComparer<TOk>.Default.Equals(left.Value, right.Value);
    public static bool operator ==(Ok<TOk> ok, TOk? okValue) => EqualityComparer<TOk>.Default.Equals(ok.Value, okValue!);
    public static bool operator !=(Ok<TOk> ok, TOk? okValue) => !EqualityComparer<TOk>.Default.Equals(ok.Value, okValue!);


    public readonly TOk Value;

    public Ok(TOk okValue)
    {
        this.Value = okValue;
    }
    public void Deconstruct(out TOk okValue)
    {
        okValue = Value;
    }


    public bool Equals(Ok<TOk> ok) => EqualityComparer<TOk>.Default.Equals(Value, ok.Value);
    public bool Equals(TOk? okValue) => EqualityComparer<TOk>.Default.Equals(Value, okValue!);

    public override bool Equals(object? obj) => obj switch
    {
        Ok<TOk> ok => Equals(ok),
        TOk okValue => Equals(okValue),
        _ => false,
    };

    public override int GetHashCode() => Hasher.GetHashCode(Value);
    public override string ToString() => $"Ok({Value})";
}