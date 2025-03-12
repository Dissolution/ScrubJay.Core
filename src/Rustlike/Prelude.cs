using ScrubJay.Rustlike.mitm;

namespace ScrubJay.Rustlike;

public static class RustlikePrelude
{
    public static None None
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    public static Option<T> Some<T>(T value)
        where T : allows ref struct
        => Option<T>.Some(value);

    public static Ok<T> Ok<T>(T value)
        where T : allows ref struct
        => new Ok<T>(value);

    public static Err<T> Err<T>(T value)
        where T : allows ref struct
        => new Err<T>(value);
}
