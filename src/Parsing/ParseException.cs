#pragma warning disable CA1032, CA1710, CA1010

using ScrubJay.Collections.NonGeneric;
using ScrubJay.Text;

namespace ScrubJay.Parsing;

[PublicAPI]
public class ParseException : InvalidOperationException, IEnumerable
{
    public static ParseException Create(ReadOnlySpan<char> input, Type? destType, string? additionalInfo = null)
    {
        return new ParseException(input, destType, additionalInfo);
    }

    public static ParseException Create(string? input, Type? destType, string? additionalInfo = null)
    {
        return new ParseException(input, destType, additionalInfo);
    }

    public static ParseException Create<TDest>(ReadOnlySpan<char> input, string? additionalInfo = null)
    {
        return new ParseException(input, typeof(TDest), additionalInfo);
    }

    public static ParseException Create<TDest>(string? input, string? additionalInfo = null)
    {
        return new ParseException(input, typeof(TDest), additionalInfo);
    }


    private static string GetMessage(ReadOnlySpan<char> input, Type? destType, string? info)
    {
        var text = new Buffer<char>();
        text.Write("Could not parse ");
        text.Write('\"');
        text.Write(input);
        text.Write('\"');
        text.Write(" into a ");
        text.Write(destType.NameOf());
        if (!string.IsNullOrEmpty(info))
        {
            text.Write(": ");
            text.Write(info!);
        }

        return text.ToStringAndDispose();
    }

    private static string GetMessage(string? input, Type? destType, string? info)
    {
        var text = new Buffer<char>();
        text.Write("Could not parse ");
        if (input is null)
        {
            text.Write("null");
        }
        else
        {
            text.Write('\"');
            text.Write(input);
            text.Write('\"');
        }
        text.Write(" into a ");
        text.Write(destType.NameOf());
        if (!string.IsNullOrEmpty(info))
        {
            text.Write(": ");
            text.Write(info!);
        }
        return text.ToStringAndDispose();
    }


    private DictionaryAdapter<string, object?>? _data;

    /// <summary>
    /// The input text that was being parsed
    /// </summary>
    public string? Input { get; }

    /// <summary>
    /// The <see cref="Type"/> the <see cref="Input"/> text was being converted into, if known
    /// </summary>
    public Type? DestType { get; }

    public override IDictionary Data => _data ??= new DictionaryAdapter<string, object?>(base.Data);

    public ParseException(ReadOnlySpan<char> input,
        Type? destType = null,
        string? additionalInfo = null,
        Exception? innerException = null)
        : base(GetMessage(input, destType, additionalInfo), innerException)
    {
        this.Input = input.AsString();
        this.DestType = destType;
    }

    public ParseException(string? input,
        Type? destType = null,
        string? additionalInfo = null,
        Exception? innerException = null)
        : base(GetMessage(input, destType, additionalInfo), innerException)
    {
        this.Input = input;
        this.DestType = destType;
    }

    /// <summary>
    /// Adds a key/value pair into <see cref="this.Data"/>
    /// </summary>
    /// <remarks>
    /// Support for fluent setting of Data during Exception creation
    /// </remarks>
    public void Add(string key, object? value)
    {
        Throw.IfEmpty(key);
        if (value is not null)
        {
            this.Data.Add(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        // do nothing
        yield break;
    }
}