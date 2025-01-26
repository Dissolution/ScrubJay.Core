// CA1716: Identifiers should not match keywords
// Error doesn't appear to be a keyword (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/)
#pragma warning disable CA1716

// CA1051: Do not declare visible instance fields
// Done for optimizations
#pragma warning disable CA1051

namespace ScrubJay.Functional;

/// <summary>
///
/// </summary>
/// <typeparam name="TError"></typeparam>
/// <remarks>
/// Error evaluates as <c>false</c>, all other methods are a direct pass-through to <see cref="Value"/><br/>
/// Error should ideally only be used as a pass-through to an end <see cref="Result{TOk,TError}"/> as it implicitly converts
/// </remarks>
[PublicAPI]
public readonly struct Error<TError> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Error<TError>, Error<TError>, bool>,
    IEqualityOperators<Error<TError>, TError, bool>,
    IComparisonOperators<Error<TError>, Error<TError>, bool>,
    IComparisonOperators<Error<TError>, TError, bool>,
#endif
    IEquatable<Error<TError>>,
    IEquatable<TError>,
    IComparable<Error<TError>>,
    IComparable<TError>,
    IEnumerable<TError>
{
    // Error is false
    public static implicit operator bool(Error<TError> _) => false;
    public static bool operator true(Error<TError> _) => false;
    public static bool operator false(Error<TError> _) => true;

    // Error is a pass-through for its Value
    public static bool operator ==(Error<TError> left, Error<TError> right) => EqualityComparer<TError>.Default.Equals(left.Value, right.Value);
    public static bool operator !=(Error<TError> left, Error<TError> right) => !EqualityComparer<TError>.Default.Equals(left.Value, right.Value);
    public static bool operator ==(Error<TError> error, TError? errorValue) => EqualityComparer<TError>.Default.Equals(error.Value, errorValue!);
    public static bool operator !=(Error<TError> error, TError? errorValue) => !EqualityComparer<TError>.Default.Equals(error.Value, errorValue!);

    public static bool operator >(Error<TError> left, Error<TError> right) => Comparer<TError>.Default.Compare(left.Value, right.Value) > 0;
    public static bool operator >(Error<TError> left, TError right) => Comparer<TError>.Default.Compare(left.Value, right) > 0;
    public static bool operator >=(Error<TError> left, Error<TError> right) => Comparer<TError>.Default.Compare(left.Value, right.Value) >= 0;
    public static bool operator >=(Error<TError> left, TError right) => Comparer<TError>.Default.Compare(left.Value, right) >= 0;
    public static bool operator <(Error<TError> left, Error<TError> right) => Comparer<TError>.Default.Compare(left.Value, right.Value) < 0;
    public static bool operator <(Error<TError> left, TError right) => Comparer<TError>.Default.Compare(left.Value, right) < 0;
    public static bool operator <=(Error<TError> left, Error<TError> right) => Comparer<TError>.Default.Compare(left.Value, right.Value) <= 0;
    public static bool operator <=(Error<TError> left, TError right) => Comparer<TError>.Default.Compare(left.Value, right) <= 0;


    public readonly TError Value;

    public Error(TError errorValue)
    {
        Value = errorValue;
    }
    public void Deconstruct(out TError errorValue) => errorValue = Value;

    public int CompareTo(Error<TError> other) => Comparer<TError>.Default.Compare(Value!, other.Value!);
    public int CompareTo(TError? other) => Comparer<TError>.Default.Compare(Value!, other!);


    public bool Equals(Error<TError> other) => EqualityComparer<TError>.Default.Equals(Value!, other.Value!);
    public bool Equals(TError? other) => EqualityComparer<TError>.Default.Equals(Value!, other!);
    public bool Equals(bool isOkay) => !isOkay;
    public override bool Equals(object? obj) => obj switch
    {
        Error<TError> error => Equals(error),
        TError errorValue => Equals(errorValue),
        bool isOkay => !isOkay,
        _ => false,
    };


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TError> GetEnumerator()
    {
        // Same behavior as the Result this should end up as
        yield break;
    }

    public override int GetHashCode() => Hasher.GetHashCode<TError>(Value);
    public override string ToString() => $"Error({Value})";
}
