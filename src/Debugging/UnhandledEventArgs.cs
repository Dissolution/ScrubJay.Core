namespace ScrubJay.Debugging;

/// <summary>
/// Event Arguments for <see cref="UnhandledEventWatcher"/> events
/// </summary>
[PublicAPI]
public class UnhandledEventArgs : EventArgs
{
    public required UnhandledEventSource Source { get; init; }
    public object? Sender { get; init; }
    public Exception? Exception { get; init; }
    public object? Data { get; init; }
    public bool? IsTerminating { get; init; }
    public bool? IsObserved { get; init; }
}
