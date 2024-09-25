namespace ScrubJay;

[PublicAPI]
public static class Option
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
        where T : notnull
    {
        if (value is not null)
            return Option<T>.Some(value);
        return default;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> AsOption<T>(this T? nullable)
        where T : struct
    {
        if (nullable.HasValue)
        {
            // fastest path to inner value
            return Option<T>.Some(nullable.GetValueOrDefault());
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None() => default(None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => default;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

}