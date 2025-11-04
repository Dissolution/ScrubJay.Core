namespace ScrubJay.Functional.IMPL;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct Ok<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public static implicit operator Result(Ok<T> ok) => Result.Ok;

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
        return TextBuilder.Build($"Ok<{typeof(T):@}>({Value})");
    }
}