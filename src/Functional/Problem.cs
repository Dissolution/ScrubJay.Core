// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScrubJay.Functional;

/// <summary>
/// A generic Problem for use as a non-<see cref="Exception"/> Error in a <see cref="Result{T,E}"/>.<br/>
/// This is a rough approximation of <see href="https://www.rfc-editor.org/rfc/rfc9457.html">Problem Details</see> without the overhead of any ASP or Web related properties.<br/>
/// </summary>
public class Problem : IEnumerable
{
    public string? Details { get; set; }

    public string? Title { get; set; }

    public Exception? Exception { get; set; }

    public Dictionary<string, object?> Data { get; } = new(0, StringComparer.OrdinalIgnoreCase);


    public Problem(Exception exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        this.Exception = exception;
        this.Title = exception.GetType().Name;
        this.Details = exception.Message;
        foreach (DictionaryEntry data in exception.Data)
        {
            this.Data[data.Key.Stringify()] = data.Value;
        }
    }

    public Problem(string? details)
        : this(details, null, null) { }

    public Problem(string? details, Exception? exception)
        : this(details, null, exception) { }

    public Problem(string? details, string? title)
        : this(details, title, null) { }

    public Problem(string? details, string? title, Exception? exception)
    {
        this.Details = details;
        this.Exception = exception;
        this.Title = title;
    }


    public void Add(string key, object? value)
    {
        this.Data[key] = value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Data.GetEnumerator();
    }
}