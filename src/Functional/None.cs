namespace ScrubJay.Functional;

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
#endif
    IEquatable<None>
{
    /// <summary>
    /// <see cref="None"/> is <c>false</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(None _) => false;
    /// <summary>
    /// <see cref="None"/> is <c>false</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(None _) => false;
    /// <summary>
    /// <see cref="None"/> is <c>false</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(None _) => true;

    /// <summary>
    /// All <see cref="None"/>s are the same
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(None _, None __) => true;
    /// <summary>
    /// All <see cref="None"/>s are the same
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(None _, None __) => false;

    /// <summary>
    /// Gets the only value of <see cref="None"/>
    /// </summary>
    /// <remarks>
    /// <c>None.Default == default(None)</c><br/>
    /// <c>default(None)</c> is preferred
    /// </remarks>
    public static readonly None Default;

    /// <summary>
    /// All <see cref="None"/>s are the same
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(None _) => true;

    /// <summary>
    /// Checks for equality with an <see cref="object"/>:<br/>
    /// <see cref="None"/>: All <see cref="None"/>s are the same<br/>
    /// <see cref="bool"/>: <see cref="None"/> is <c>false</c><br/>
    /// <c>default</c>: <c>false</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is None)
            return true;
        if (obj is bool isSome)
            return !isSome;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => nameof(None);
}
