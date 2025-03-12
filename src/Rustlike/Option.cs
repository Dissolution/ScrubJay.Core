namespace ScrubJay.Rustlike;

#pragma warning disable
[StructLayout(LayoutKind.Auto)]
public readonly ref struct Option<T>
    where T : allows ref struct
{
    public static bool operator ==(in Option<T> left, in Option<T> right)
    {
        throw new NotImplementedException();
    }
    public static bool operator !=(in Option<T> left, in Option<T> right)
    {
        throw new NotImplementedException();
    }

    public static Option<T> None
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default!;
    }

    public static Option<T> Some(T value)
    {
        throw new NotImplementedException();
    }
}
