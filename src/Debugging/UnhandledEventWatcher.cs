// Sender parameter should be 'this' for instance events
#pragma warning disable MA0091
// Sender parameter should be 'null' for static events
#pragma warning disable S4220, MA0092

namespace ScrubJay.Debugging;

[PublicAPI]
public sealed class UnhandledEventWatcher : IDisposable
{
    public event EventHandler<UnhandledEventArgs>? UnhandledException;

    public UnhandledEventWatcher()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        UnhandledEventArgs unhandledArgs = new CurrentDomainUnhandledEventArgs()
        {
            OriginalSender = sender,
            ExceptionObject = args.ExceptionObject,
            IsTerminating = args.IsTerminating,
        };
        UnhandledException?.Invoke(sender, unhandledArgs);
    }

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        UnhandledEventArgs unhandledArgs = new UnobservedTaskEventArgs()
        {
            OriginalSender = sender,
            Exception = args.Exception,
            WasObserved = args.Observed,
        };
        UnhandledException?.Invoke(sender, unhandledArgs);
        // We observed this!
        args.SetObserved();
    }

    public void Dispose()
    {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
        _ = Interlocked.Exchange(ref UnhandledException, null);
    }
}
