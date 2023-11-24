using System.Runtime.InteropServices;

namespace ScrubJay.Utilities;

/// <summary>
/// A representation of a <c>void</c> value<br/>
/// Not the same as <c>null</c><br/>
/// Construct with <c>default</c> or <c>default(Nothing)</c>
/// </summary>
[StructLayout(LayoutKind.Auto, Size = 0)]
public readonly struct Nothing :
#if NET7_0_OR_GREATER
    IEqualityOperators<Nothing, Nothing, bool>,
    IEqualityOperators<Nothing, object, bool>,
#endif
    IEquatable<Nothing>
{
    public static bool operator ==(Nothing a, Nothing z) => true;
    public static bool operator !=(Nothing a, Nothing z) => false;
    public static bool operator ==(Nothing _, object? obj) => obj is Nothing;
    public static bool operator !=(Nothing _, object? obj) => obj is not Nothing;
    public static bool operator ==(object? obj, Nothing _) => obj is Nothing;
    public static bool operator !=(object? obj, Nothing _) => obj is not Nothing;

    public bool Equals(Nothing _) => true;

    public override bool Equals(object? obj) => obj is Nothing;

    public override int GetHashCode() => 0;

    public override string ToString() => nameof(Nothing);
}