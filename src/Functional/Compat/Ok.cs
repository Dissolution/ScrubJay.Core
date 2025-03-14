namespace ScrubJay.Functional.Compat;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public readonly ref struct Ok<T>
    where T : allows ref struct
#else
public readonly struct Ok<T>
#endif
{
    internal readonly T _value;

    public Ok(T value)
    {
        _value = value;
    }
}
