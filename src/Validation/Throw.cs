namespace ScrubJay.Validation;

public static class Throw
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IfNull<T>([AllowNull, NotNull] T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is null)
            throw new ArgumentNullException(valueName);
    }
}