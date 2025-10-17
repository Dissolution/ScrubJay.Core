namespace ScrubJay.Exceptions;

partial class Ex
{
    private static string GetArgumentExceptionMessage(
        Type? argumentType,
        string? argumentName,
        Action<TextBuilder>? addArgumentValue,
        InterpolatedText additionalInfo)
    {
        return TextBuilder.New
            .Append("Invalid ")
            .IfNotNull(argumentType, static (tb, type) => tb.Render(type), static tb => tb.Write("unknown"))
            .Append(" argument \"")
            .Append(argumentName)
            .Append('"')
            .Invoke(addArgumentValue)
            .IfNotEmpty(additionalInfo, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
    }

    public static ArgumentException Arg<T>(
        T? argument,
        InterpolatedText info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Type argumentType = Type.GetType(argument);
        string message = GetArgumentExceptionMessage(argumentType, argumentName,
            tb => tb.Append(" - `").Append(argument).Append('`'),
            info);
    }
}