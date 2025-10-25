#pragma warning disable CA1032, CA1710, CA1010

namespace ScrubJay.Parsing;

/// <summary>
/// A <see cref="ParseException"/> is a type of <see cref="InvalidOperationException"/>
/// with a default message that contains the <see cref="InputText"/> and <see cref="DestinationType"/>
/// </summary>
/// <remarks>
/// <see cref="ParseException"/> supports fluent instantiation of its <see cref="Exception.Data"/> with <see cref="Add"/>
/// </remarks>
[PublicAPI]
public class ParseException : InvalidOperationException, IEnumerable
{
    /// <summary>
    /// The input text that was being parsed
    /// </summary>
    public string? InputText { get; init; }

    /// <summary>
    /// The <see cref="Type"/> the <see cref="InputText"/> was being parsed into, if known
    /// </summary>
    public Type? DestinationType { get; init; }

    public ParseException(string? message) : base(message)
    {

    }

    public ParseException(string? message, Exception? innerException) : base(message, innerException)
    {

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