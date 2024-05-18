namespace ScrubJay.Validation;

public static class ThrowIf
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Null<T>([AllowNull, NotNull] T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is null)
            throw new ArgumentNullException(valueName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NotBetween<T>(T value, T inclusiveMinimum, T inclusiveMaximum,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (Comparer<T>.Default.Compare(value, inclusiveMinimum) < 0 ||
            Comparer<T>.Default.Compare(value, inclusiveMaximum) > 0)
        {
            throw new ArgumentOutOfRangeException(valueName, value,
                $"Value must be between in [{inclusiveMinimum}..{inclusiveMaximum}]");
        }
    }
}