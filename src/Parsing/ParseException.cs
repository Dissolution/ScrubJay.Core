﻿using ScrubJay.Text.Rendering;

#pragma warning disable CA1032, CA1710, CA1010

namespace ScrubJay.Parsing;

/// <summary>
/// A <see cref="ParseException"/> is a type of <see cref="InvalidOperationException"/>
/// with a default message that contains the <see cref="InputText"/> and <see cref="ParseType"/>
/// </summary>
/// <remarks>
/// <see cref="ParseException"/> supports fluent instantiation of its <see cref="Exception.Data"/> with <see cref="Add"/>
/// </remarks>
[PublicAPI]
public class ParseException : InvalidOperationException, IEnumerable
{
    public static ParseException Create(text input, Type? destType, string? additionalInfo = null, Exception? innerException = null)
        => new ParseException(input, destType, additionalInfo, innerException);

    public static ParseException Create(string? input, Type? destType, string? additionalInfo = null, Exception? innerException = null)
        => new ParseException(input, destType, additionalInfo, innerException);

    public static ParseException<T> Create<T>(text input, string? additionalInfo = null, Exception? innerException = null)
        => ParseException<T>.Create(input, additionalInfo, innerException);

    public static ParseException<T> Create<T>(string? input, string? additionalInfo = null, Exception? innerException = null)
        => ParseException<T>.Create(input, additionalInfo, innerException);


    private static string GetMessage(text input, Type? destType, string? info)
    {
        return TextBuilder.New
            .Append($"Could not parse \"{input}\" into a {destType.Render()}")
            .If(Validate.IsNotEmpty(info),
                static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }

    private static string GetMessage(string? input, Type? destType, string? info)
    {
        return TextBuilder.New
            .Append("Could not parse ")
            .If(Validate.IsNotNull(input),
                static (tb, n) => tb.Append('"').Append(n).Append('"'),
                static (tb, _) => tb.Append("null"))
            .Append(" into a ")
            .Append(destType.Render())
            .If(Validate.IsNotEmpty(info),
                static (tb, nfo) => tb.Append(": ").Append(nfo))
            .ToStringAndDispose();
    }


    /// <summary>
    /// The input text that was being parsed
    /// </summary>
    public string? InputText { get; }

    /// <summary>
    /// The <see cref="Type"/> the <see cref="InputText"/> was being parsed into, if known
    /// </summary>
    public Type? ParseType { get; }

    public ParseException(
        text input,
        Type? destType = null,
        string? additionalInfo = null,
        Exception? innerException = null)
        : base(GetMessage(input, destType, additionalInfo), innerException)
    {
        InputText = input.AsString();
        ParseType = destType;
    }

    public ParseException(
        string? input,
        Type? destType = null,
        string? additionalInfo = null,
        Exception? innerException = null)
        : base(GetMessage(input, destType, additionalInfo), innerException)
    {
        InputText = input;
        ParseType = destType;
    }

    /// <summary>
    /// Adds a key/value pair into <see cref="Exception.Data"/>
    /// </summary>
    /// <remarks>
    /// Support for fluent setting of Data during Exception creation
    /// </remarks>
    public void Add(string key, object? value)
    {
        Throw.IfEmpty(key);
        if (value is not null)
        {
            Data.Add(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        // do nothing
        yield break;
    }
}

[PublicAPI]
public class ParseException<T> : ParseException
{
    public static ParseException<T> Create(text input, string? additionalInfo = null, Exception? innerException = null) => new ParseException<T>(input, additionalInfo, innerException);

    public static ParseException<T> Create(string? input, string? additionalInfo = null, Exception? innerException = null) => new ParseException<T>(input, additionalInfo, innerException);

    public ParseException(
        text input,
        string? additionalInfo = null,
        Exception? innerException = null)
        : base(input, typeof(T), additionalInfo, innerException) { }

    public ParseException(
        string? input,
        string? additionalInfo = null,
        Exception? innerException = null)
        : base(input, typeof(T), additionalInfo, innerException) { }
}
