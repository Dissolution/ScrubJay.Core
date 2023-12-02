namespace ScrubJay.Debugging;

/// <summary>
/// <see cref="EventArgs"/> for unhandled <see cref="Exception">Exceptions</see>
/// </summary>
public class UnhandledExceptionArgs : EventArgs
{
    /// <summary>
    /// When did this unhandled exception occur?
    /// </summary>
    public DateTime Occurrence { get; } = DateTime.Now;
    
    /// <summary>
    /// The source of the unhandled exception
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// The unhandled <see cref="Exception"/>
    /// </summary>
    public required Exception Exception { get; init; }
    
    /// <summary>
    /// Is this unhandled exception causing this application to terminate?
    /// </summary>
    public bool? IsTerminating { get; init; } = null;

    /// <summary>
    /// Has this unhandled exception been observed?
    /// </summary>
    public bool? IsObserved { get; init; } = null;

    public override string ToString() => 
        $"""
        [{Occurrence:yyyy-MM-dd HH:mm:ss.ff}]: Unhandled {Source} Exception
        Observed: {IsObserved.ToString("Yes", "No", "?")}   Terminating: {IsTerminating.ToString("Yes", "No", "?")}
        {Exception.GetType().Name}:
        {Exception.Message}
        {Exception.StackTrace}
        """;
}