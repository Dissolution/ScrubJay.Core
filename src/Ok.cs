namespace ScrubJay;

// Generic Ok helper struct
public readonly struct Ok :
#if NET7_0_OR_GREATER
    IEqualityOperators<Ok, Ok, bool>,
    IBitwiseOperators<Ok, Ok, bool>,
#endif
    IEquatable<Ok>,
    IEquatable<bool>
{
    public static bool operator true(Ok _) => true;
    public static bool operator false(Ok _) => false;

    public static bool operator &(Ok left, Ok right) => true;
    public static bool operator |(Ok left, Ok right) => true;
    public static bool operator ^(Ok left, Ok right) => false;
    [Obsolete("Cannot apply operator '~' to operand of type 'Ok'", true)]
    public static bool operator ~(Ok _) => throw new NotSupportedException();

    public static bool operator ==(Ok left, Ok right) => true;
    public static bool operator !=(Ok left, Ok right) => false;

    public bool Equals(Ok _) => true;
    public bool Equals(bool ok) => ok;
    public override bool Equals(object? obj) => obj switch
    {
        Ok => true,
        bool ok => ok,
        _ => false,
    };
    public override int GetHashCode() => 1;
    public override string ToString() => nameof(Ok);
}

// Generic Error (non-Exception) helper struct
/* Some of the Interfaces are commented out to prevent compiler errors */