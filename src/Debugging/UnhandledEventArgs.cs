namespace ScrubJay.Debugging;

/// <summary>
/// Event Arguments for <see cref="UnhandledEventWatcher"/> events
/// </summary>
[PublicAPI]
public abstract class UnhandledEventArgs : EventArgs
{
    public object? OriginalSender { get; init; }
}

[PublicAPI]
public class CurrentDomainUnhandledEventArgs : UnhandledEventArgs
{
    public required object ExceptionObject { get; init; }
    public required bool IsTerminating { get; init; }
}

[PublicAPI]
public class UnobservedTaskEventArgs : UnhandledEventArgs
{
    public required bool WasObserved { get; init; }
    public required AggregateException? Exception { get; init; }
}
