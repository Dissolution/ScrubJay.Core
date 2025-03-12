namespace ScrubJay.Rustlike.mitm;

[PublicAPI]
public readonly ref struct Ok<T>
    where T : allows ref struct
{
    /// <summary>
    /// The underlying Ok value
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// Construct a new <see cref="Ok{TOk}"/> containing the <paramref name="value"/>
    /// </summary>
    public Ok(T value)
    {
        Value = value;
    }
    public void Deconstruct(out T value) => value = Value;
}
