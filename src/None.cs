namespace ScrubJay;

/// <summary>
/// None represents the lack of a value for an <see cref="Option{T}"/> and implicitly converts to <see cref="Option{T}"/>.<see cref="Option{T}.None"/> for any T
/// </summary>
/// <remarks>
/// <see cref="None"/> <b>is</b> <c>false</c> in all behaviors
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 0)]
public readonly struct None :
#if NET7_0_OR_GREATER
    IEqualityOperators<None, None, bool>,
    IBitwiseOperators<None, None, bool>,
#endif
    IEquatable<None>
{
    // None is false
    
    public static implicit operator bool(None _) => false;
    public static bool operator true(None _) => false;
    public static bool operator false(None _) => true;

    public static bool operator &(None left, None right) => false;
    public static bool operator |(None left, None right) => false;
    public static bool operator ^(None left, None right) => false;

    [Obsolete("Cannot apply operator '~' to operand of type 'None'", true)]
    public static bool operator ~(None _) => throw new NotSupportedException();
    
    public static bool operator ==(None left, None right) => true;
    public static bool operator !=(None left, None right) => false;
        
    public bool Equals(None _) => true;
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is None;
    public override int GetHashCode() => 0;
    public override string ToString() => nameof(None);
}