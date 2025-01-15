// Standard Exceptions
#pragma warning disable CA1032, RCS1194

namespace ScrubJay.Text.Parsing;

[PublicAPI]
public class ParseException : InvalidOperationException
{
    private static string GetMessage(string? input, Type destType, string? info)
    {
        using InterpolatedText text = new();
        text.AppendLiteral("Could not parse ");
        if (input is null)
        {
            text.AppendLiteral("null");
        }
        else
        {
            text.AppendFormatted('\"');
            text.AppendLiteral(input);
            text.AppendFormatted('\"');
        }
        text.AppendLiteral(" into a ");
        text.AppendFormatted(destType);
        if (!string.IsNullOrEmpty(info))
        {
            text.AppendLiteral(": ");
            text.AppendLiteral(info!);
        }
        return text.ToString();
    }

    private static string GetMessage(text input, Type destType, string? info)
    {
        using InterpolatedText text = new();
        text.AppendLiteral("Could not parse ");
        text.AppendFormatted('\"');
        text.AppendFormatted(input);
        text.AppendFormatted('\"');
        text.AppendLiteral(" into a ");
        text.AppendFormatted(destType);
        if (!string.IsNullOrEmpty(info))
        {
            text.AppendLiteral(": ");
            text.AppendLiteral(info!);
        }
        return text.ToString();
    }


    public static ParseException Create(string? input, Type destType, string? info = null)
    {
        Throw.IfNull(destType);
        return new ParseException(input, destType, info);
    }

    public static ParseException Create(text input, Type destType, string? info = null)
    {
        Throw.IfNull(destType);
        return new ParseException(input, destType, info);
    }

    public string? Input { get;}
    public Type? DestType { get; }

    private ParseException(string? input, Type destType, string? info)
        : base(GetMessage(input, destType, info))
    {
        this.Input = input;
        this.DestType = destType;
    }

    public ParseException(text input, Type destType, string? info)
        : base(GetMessage(input, destType, info))
    {
        this.Input = input.AsString();
        this.DestType = destType;
    }
}