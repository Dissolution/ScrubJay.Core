namespace ScrubJay.Functional;

/// <summary>
/// The unit type is a type that indicates the absence of a specific value; the unit type has only a single value, which acts as a placeholder when no other value exists or is needed.
/// </summary>
/// <remarks>
/// Based upon F# and Rust's Unit types.<br/>
/// In C#, <c>void</c> cannot be used as a generic type, thus why <see cref="Func{TResult}"/> and <see cref="Action"/> are separate<br/>
/// (as <c>Func&lt;void&gt;</c> is invalid)<br/>
/// You can use <see cref="Unit"/> for this and similar tasks:<br/>
/// - <c>Func&lt;Unit&gt;</c><br/>
/// - <c>Result&lt;Unit&gt;</c><br/>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto, Size = 0)]
public readonly struct Unit :
#if NET7_0_OR_GREATER
    IEqualityOperators<Unit, Unit, bool>,
    IComparisonOperators<Unit, Unit, bool>,
#endif
    IEquatable<Unit>,
    IComparable<Unit>
{
    // ValueTuple would be written as '()' (if the C# compiler allowed it), and is virtually the same as Unit already
    // so we support implicit conversions between them

    public static implicit operator Unit(ValueTuple _) => default;
    public static implicit operator ValueTuple(Unit _) => default;

    // All units are exactly the same

    public static bool operator ==(Unit _, Unit __) => true;
    public static bool operator !=(Unit _, Unit __) => false;
    public static bool operator >(Unit _, Unit __) => false;
    public static bool operator >=(Unit _, Unit __) => true;
    public static bool operator <(Unit _, Unit __) => false;
    public static bool operator <=(Unit _, Unit __) => true;

    /// <summary>
    /// Gets <see cref="Unit"/>
    /// </summary>
    public static readonly Unit Default;

    public int CompareTo(Unit unit) => 0;
    public bool Equals(Unit unit) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";
}