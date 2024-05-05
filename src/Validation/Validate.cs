namespace ScrubJay.Validation;

public static class Validate
{
    public static Result<T, ArgumentNullException> NotNull<T>(
        [AllowNull, NotNullWhen(true)] T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value is null)
            return new ArgumentNullException(valueName);
        return value;
    }
}