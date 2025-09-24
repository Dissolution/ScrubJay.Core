namespace ScrubJay.Exceptions;

[PublicAPI]
public class ArgException : Exception
{
    private static string GetMessage(object? value, string? info, string? valueName)
    {
        return TextBuilder.New
            .Append($"Invalid {value:@T} {valueName} `{value:@}`")
            .IfNotEmpty(info, static (tb, n) => tb.Append(": ").Write(n))
            .ToStringAndDispose();
    }

    public object? ArgumentValue { get; }

    public string? ArgumentName { get; }

    public ArgException(object? argumentValue,
        string? message,
        [CallerArgumentExpression(nameof(argumentValue))] string? argumentName = null)
        : base(GetMessage(argumentValue, message, argumentName))
    {
        this.ArgumentValue = argumentValue;
        this.ArgumentName = argumentName;
    }

}