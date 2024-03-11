namespace ScrubJay;

/// <summary>
/// <see cref="Option{T}"/>.None
/// </summary>
/// <remarks>
/// None exists to support <see cref="Option"/>'s ability to return a <see cref="M:Option{T}.None"/>
/// without needing the generic type information 
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public readonly struct None :
#if NET7_0_OR_GREATER
    IEqualityOperators<None, None, bool>,
#endif
    IEquatable<None>
{
    // All Nones are the same
    public static bool operator ==(None left, None right) => true;
    public static bool operator !=(None left, None right) => false;

    // All Nones are the same
    public bool Equals(None _) => true;
    public override bool Equals(object? obj) => obj is None;
    public override int GetHashCode() => 0;
    public override string ToString() => nameof(None);
}