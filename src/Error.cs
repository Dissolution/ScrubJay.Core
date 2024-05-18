namespace ScrubJay;

public readonly struct Error :
#if NET7_0_OR_GREATER
    IEqualityOperators<Error, Error, bool>,
    IBitwiseOperators<Error, Error, bool>,
#endif
    IEquatable<Error>,
    IEquatable<bool>
{
    public static implicit operator Error(Exception ex) => new(ex.Message);
    
    public static bool operator true(Error _) => false;
    public static bool operator false(Error _) => true;

    public static bool operator &(Error left, Error right) => false;
    public static bool operator |(Error left, Error right) => false;
    public static bool operator ^(Error left, Error right) => false;
    [Obsolete("Cannot apply operator '~' to operand of type 'Error'", true)]
    public static bool operator ~(Error _) => throw new NotSupportedException();

    public static bool operator ==(Error left, Error right) => true;
    public static bool operator !=(Error left, Error right) => false;

    public readonly string? Message;

    public Error(string? message)
    {
        Message = message;
    }
    
    public bool Equals(Error _) => true;
    public bool Equals(bool ok) => ok == false;
    public override bool Equals(object? obj) => obj switch
    {
        Error => true,
        bool ok => ok == false,
        _ => false,
    };
    public override int GetHashCode() => 0;
    public override string ToString() => Message is null ? nameof(Error) : $"{nameof(Error)}({Message})";
}