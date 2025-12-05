namespace ScrubJay.Validation;

partial class Ex
{
    public static ArgumentNullException ArgNull<T>(T? argument,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        string message = TextBuilder
            .New
            .Append($"Argument \"{argumentName}\" ({typeof(T):@}) is null")
            .IfNotEmpty(info, static (tb, n) => tb.Append(": ").Append(ref n))
            .ToStringAndDispose();
        return new ArgumentNullException(message, innerException);
    }
}