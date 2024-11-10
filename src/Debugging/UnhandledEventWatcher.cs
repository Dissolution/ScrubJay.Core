namespace ScrubJay.Debugging;

[PublicAPI]
[MustDisposeResource]
public sealed class UnhandledEventWatcher : IDisposable
{
    // There may only be a single instance of this class at one time
    private static readonly Mutex _mutex = new Mutex(initiallyOwned: false);
    private static UnhandledEventWatcher? _instance;

    internal static void Unbroken(object sender, Exception exception)
    {
        _instance?.OnException(sender, exception);
    }

    public event EventHandler<UnhandledEventArgs>? UnhandledException;

    public UnhandledEventWatcher()
    {
        if (!_mutex.WaitOne(0))
            throw new InvalidOperationException($"There may only be a single instance of {nameof(UnhandledEventWatcher)}");

        _instance = this;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
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

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
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

    private void OnException(object? sender, Exception? exception)
    {
        var unhandledEventArgs = new UnhandledEventArgs
        {
            Source = UnhandledEventSource.Unbreakable,
            Exception = exception,
            Data = sender,
        };
        UnhandledException?.Invoke(sender, unhandledEventArgs);
    }

    public void Dispose()
    {
        AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
        _ = Interlocked.Exchange(ref UnhandledException, null);
        _mutex.ReleaseMutex();
    }
}