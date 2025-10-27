namespace ScrubJay.Exceptions;

partial class Ex
{
    public static ArgumentOutOfRangeException Enum<E>(
        E @enum,
        [HandlesResourceDisposal] InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
    {
        var message = TextBuilder.New
            .Append($"Invalid {typeof(E):@} Enum \"{enumName}\" `")
            .Render(@enum)
            .Append('`')
            .IfNotEmpty(info, static (tb, info) => tb.Append(": ").Append(info))
            .ToStringAndDispose();
        if (innerException is not null)
            return new ArgumentOutOfRangeException(message, innerException);
        return new ArgumentOutOfRangeException(enumName, @enum, message);
    }
}