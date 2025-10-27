using ScrubJay.Parsing;

namespace ScrubJay.Exceptions;

partial class Ex
{
    private static string GetMessage(scoped text input, Type? destType, InterpolatedTextBuilder info)
    {
        return TextBuilder.New
            .Append($"Could not parse \"{input}\" into a {destType:@}")
            .IfNotEmpty(info, static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }

    private static string GetMessage(string? input, Type? destType, InterpolatedTextBuilder info)
    {
        return TextBuilder.New
            .Append("Could not parse ")
            .IfNotNull(input, static (tb, n) => tb.Append('"').Append(n).Append('"'),
                static tb => tb.Append("`null"))
            .Append(" into a ")
            .Render(destType)
            .IfNotEmpty(info,
                static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }


    public static ParseException Parse(
        scoped text input,
        Type? destType,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        var message = GetMessage(input, destType, info);
        return new ParseException(message, innerException)
        {
            InputText = input.AsString(),
            DestinationType = destType,
        };
    }

    public static ParseException Parse(
        string? input,
        Type? destType,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
    {
        var message = GetMessage(input, destType, info);
        return new ParseException(message, innerException)
        {
            InputText = input.AsString(),
            DestinationType = destType,
        };
    }

    public static ParseException Parse<T>(
        scoped text input,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var message = GetMessage(input, typeof(T), info);
        return new ParseException(message, innerException)
        {
            InputText = input.AsString(),
            DestinationType = typeof(T),
        };
    }

    public static ParseException Parse<T>(
        string? input,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var message = GetMessage(input, typeof(T), info);
        return new ParseException(message, innerException)
        {
            InputText = input.AsString(),
            DestinationType = typeof(T),
        };
    }
}