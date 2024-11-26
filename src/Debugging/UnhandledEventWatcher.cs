// Sender parameter should be 'this' for instance events

#pragma warning disable MA0091
// Sender parameter should be 'null' for static events
#pragma warning disable S4220, MA0092

namespace ScrubJay.Debugging;

[PublicAPI]
public static class UnhandledEventWatcher
{
    public static IDisposable Start()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        return Disposable.Action(Stop);
    }

    private static void Stop()
    {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
        _ = Interlocked.Exchange(ref UnhandledException, null);
    }


    public static event EventHandler<UnhandledEventArgs>? UnhandledException;

    private static void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var unhandledEventArgs = new UnhandledEventArgs
        {
            Source = UnhandledEventSource.AppDomain,
            Exception = args.ExceptionObject as Exception,
            Data = (args.ExceptionObject is not Exception) ? args.ExceptionObject : null,
            IsTerminating = args.IsTerminating,
        };
        UnhandledException?.Invoke(sender, unhandledEventArgs);
    }

    private static void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        var unhandledEventArgs = new UnhandledEventArgs
        {
            Source = UnhandledEventSource.TaskScheduler,
            Exception = args.Exception,
            IsObserved = args.Observed,
        };
        UnhandledException?.Invoke(sender, unhandledEventArgs);
        // We observed this!
        args.SetObserved();
    }

    internal static void OnUnbreakableException(object? sender, Exception? exception)
    {
        var unhandledEventArgs = new UnhandledEventArgs
        {
            Source = UnhandledEventSource.Unbreakable,
            Exception = exception,
            Data = sender,
        };
        UnhandledException?.Invoke(sender, unhandledEventArgs);
    }
}