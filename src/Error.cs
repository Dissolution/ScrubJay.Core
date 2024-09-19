namespace ScrubJay;

[PublicAPI]
public readonly struct Error<TError> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Error<TError>, Error<TError>, bool>,
    IEqualityOperators<Error<TError>, TError, bool>,
#endif
    IEquatable<Error<TError>>,
    IEquatable<TError>
{
    // Error is false
    public static implicit operator bool(Error<TError> _) => false;
    public static bool operator true(Error<TError> _) => false;
    public static bool operator false(Error<TError> _) => true;
    
    // Error is a passthrough for its Value
    public static bool operator ==(Error<TError> left, Error<TError> right) => EqualityComparer<TError>.Default.Equals(left.Value, right.Value);
    public static bool operator !=(Error<TError> left, Error<TError> right) => !EqualityComparer<TError>.Default.Equals(left.Value, right.Value);
    public static bool operator ==(Error<TError> error, TError? errorValue) => EqualityComparer<TError>.Default.Equals(error.Value, errorValue!);
    public static bool operator !=(Error<TError> error, TError? errorValue) => !EqualityComparer<TError>.Default.Equals(error.Value, errorValue!);


    public readonly TError Value;

    public Error(TError errorValue)
    {
        this.Value = errorValue;
    }
    public void Deconstruct(out TError errorValue)
    {
        errorValue = Value;
    }


    public bool Equals(Error<TError> error) => EqualityComparer<TError>.Default.Equals(Value, error.Value);
    public bool Equals(TError? errorValue) => EqualityComparer<TError>.Default.Equals(Value, errorValue!);

    public override bool Equals(object? obj) => obj switch
    {
        Error<TError> error => Equals(error),
        TError errorValue => Equals(errorValue),
        _ => false,
    };

    public override int GetHashCode() => Hasher.GetHashCode(Value);
    public override string ToString() => $"Error({Value})";
}