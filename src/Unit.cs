using JetBrains.Annotations;

namespace ScrubJay;

/// <summary>
/// The unit type is a type that indicates the absence of a specific value; the unit type has only a single value, which acts as a placeholder when no other value exists or is needed.
/// </summary>
/// <remarks>
/// Based upon F# and Rust's Unit types<br/>
/// In C#, <c>void</c> cannot be used as a generic type, thus why <see cref="Func{TResult}"/> and <see cref="Action"/> exist<br/>
/// (as we cannot do <c>Func&lt;void&gt;</c>)<br/>
/// You can use <see cref="Unit"/> for this task
/// </remarks>
[PublicAPI]
public readonly struct Unit :
#if NET7_0_OR_GREATER
    IEqualityOperators<Unit, Unit, bool>,
    IComparisonOperators<Unit, Unit, bool>,
#endif
    IEquatable<Unit>,
    IComparable<Unit>
{
    // ValueTuple would be written as '()' (if the C# compiler allowed it), and is virtually the same as Unit already
    
    public static implicit operator Unit(ValueTuple _) => default;
    public static implicit operator ValueTuple(Unit _) => default;
    
    // All units are exactly the same
    
    public static bool operator ==(Unit first, Unit second) => true;
    public static bool operator !=(Unit first, Unit second) => false;
    public static bool operator >(Unit left, Unit right) => false;
    public static bool operator >=(Unit left, Unit right) => true;
    public static bool operator <(Unit left, Unit right) => false;
    public static bool operator <=(Unit left, Unit right) => true;

    /// <summary>
    /// The <see cref="Unit"/> value
    /// </summary>
    public static readonly Unit Default = default;

    public int CompareTo(Unit unit) => 0;
    public bool Equals(Unit unit) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";
}