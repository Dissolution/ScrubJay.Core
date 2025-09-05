namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct Ok<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public readonly T Value;

    public Ok(T value)
    {
        Value = value;
    }

    public void Deconstruct(out T value)
    {
        value = Value;
    }

    public override string ToString()
    {
        return $"Ok<{typeof(T)}>({Any.ToString(Value)})";
    }
}