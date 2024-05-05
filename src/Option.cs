namespace ScrubJay;

public static class Option
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
        where T : notnull
    {
        if (value is not null)
            return Option<T>.Some(value);
        return Option<T>.None;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> From<T>(T? nullable)
        where T : struct
    {
        if (nullable.HasValue)
            return Option<T>.Some(nullable.Value);
        return Option<T>.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> From<T>(Result<T, Error> result)
    {
        if (result.IsOk(out var value))
            return Some<T>(value);
        return Option<T>.None;
    }
}