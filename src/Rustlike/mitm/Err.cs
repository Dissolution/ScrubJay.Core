namespace ScrubJay.Rustlike.mitm;

[PublicAPI]
public readonly ref struct Err<T>
    where T : allows ref struct
{
    /// <summary>
    /// The underlying Err value
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// Construct a new <see cref="Err{TErr}"/> containing the <paramref name="value"/>
    /// </summary>
    public Err(T value)
    {
        Value = value;
    }
    public void Deconstruct(out T value) => value = Value;
}
