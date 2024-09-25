namespace ScrubJay;

/// <summary>
/// Some represents the value for an <see cref="Option{T}"/> and implicitly converts to
/// <see cref="Option{T}"/>.<see cref="Option{T}.Some"/>
/// </summary>
/// <remarks>
/// <see cref="Some{T}"/> evaulates as <c>true</c>
/// </remarks>
[PublicAPI]
public readonly struct Some<TSome> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Some<TSome>, Some<TSome>, bool>,
    IEqualityOperators<Some<TSome>, TSome, bool>,
#endif
    IEquatable<Some<TSome>>,
    IEquatable<TSome>
{
    /// <summary>
    /// All <see cref="Some{TSome}"/>s are <c>true</c>
    /// </summary>
    public static implicit operator bool(Some<TSome> _) => true;
    /// <summary>
    /// All <see cref="Some{TSome}"/>s are <c>true</c>
    /// </summary>
    public static bool operator true(Some<TSome> _) => true;
    /// <summary>
    /// All <see cref="Some{TSome}"/>s are <c>true</c>
    /// </summary>
    public static bool operator false(Some<TSome> _) => false;
    
    // Some is a passthrough for its Value
    public static bool operator ==(Some<TSome> left, Some<TSome> right) => EqualityComparer<TSome>.Default.Equals(left.Value, right.Value);
    public static bool operator !=(Some<TSome> left, Some<TSome> right) => !EqualityComparer<TSome>.Default.Equals(left.Value, right.Value);
    public static bool operator ==(Some<TSome> some, TSome? someValue) => EqualityComparer<TSome>.Default.Equals(some.Value, someValue!);
    public static bool operator !=(Some<TSome> some, TSome? someValue) => !EqualityComparer<TSome>.Default.Equals(some.Value, someValue!);


    public readonly TSome Value;

    public Some(TSome someValue)
    {
        this.Value = someValue;
    }
    public void Deconstruct(out TSome someValue)
    {
        someValue = Value;
    }

    /// <summary>
    /// Do these <see cref="Some{TSome}"/>s contain the same value?
    /// </summary>
    public bool Equals(Some<TSome> some) => EqualityComparer<TSome>.Default.Equals(Value, some.Value);
    /// <summary>
    /// Does this <see cref="Some{TSome}"/> contain the <paramref name="value"/>?
    /// </summary>
    public bool Equals(TSome? value) => EqualityComparer<TSome>.Default.Equals(Value, value!);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj switch
    {
        Some<TSome> some => Equals(some),
        TSome someValue => Equals(someValue),
        bool isSome => isSome,
        _ => false,
    };

    /// <inheritdoc />
    public override int GetHashCode() => Hasher.GetHashCode(Value);
    
    /// <inheritdoc />
    public override string ToString() => $"Some({Value})";
}