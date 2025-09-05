namespace ScrubJay.Functional.IMPL;

/// <summary>
/// <b>None</b> represents the lack of a value in an <see cref="Option{T}"/>,<br/>
/// and implicitly converts to and from a <see cref="Option{T}.None"/>
/// </summary>
/// <remarks>
/// <see cref="None"/> behaves exactly as <c>false</c>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto, Size = 0)]
public readonly struct None :
#if NET7_0_OR_GREATER
    IEqualityOperators<None, None, bool>,
    IComparisonOperators<None,None,bool>,
#endif
    IEquatable<None>,
    IComparable<None>
{
    public static implicit operator bool(None _) => false;
    public static bool operator true(None _) => false;
    public static bool operator false(None _) => true;
    
    public static bool operator ==(None _, None __) => true;
    public static bool operator !=(None _, None __) => false;
    public static bool operator >(None left, None right) => false;
    public static bool operator >=(None left, None right) => true;
    public static bool operator <(None left, None right) => false;
    public static bool operator <=(None left, None right) => true;

    
    /// <summary>
    /// Gets the default <see cref="None"/> instance
    /// </summary>
    public static readonly None Default;

    /// <summary>
    /// Gets a <c>ref readonly </c> to the default <see cref="None"/> instance
    /// </summary>
    public static ref readonly None Ref => ref Default;
    
    public int CompareTo(None _) => 0;
    
    public bool Equals(None _) => true;

    public override bool Equals([NotNullWhen(true)] object? obj) 
        => obj switch
        {
            None => true,
            bool boolean => boolean == false,
            _ => false,
        };
    
    public override int GetHashCode() => 0;
    
    public override string ToString() => nameof(None);
}
