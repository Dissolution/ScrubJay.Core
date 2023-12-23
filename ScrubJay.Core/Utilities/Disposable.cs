using JetBrains.Annotations;

namespace ScrubJay.Utilities;

/// <summary>
/// A utility for working with <see cref="IDisposable"/>
/// </summary>
public static class Disposable
{
    /// <summary>
    /// Gets an <see cref="IDisposable"/> value that executes the given <paramref name="action"/>
    /// when its Dispose method is called
    /// </summary>
    /// <param name="action">
    /// The <see cref="Action"/> to invoke when the returned <see cref="IDisposable"/> is disposed
    /// </param>
    /// <returns>
    /// An <see cref="IDisposable"/> instance that will execute the given <see cref="Action"/>
    /// exactly once when it is disposed
    /// </returns>
    public static IDisposable FromAction(Action? action)
    {
        return new ActionDisposable(action);
    }

    /// <summary>
    /// Tries to dispose of <paramref name="value"/>, ignoring all exceptions
    /// </summary>
    public static void Dispose<T>([HandlesResourceDisposal] T? value)
    {
        if (value is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // Ignore all exceptions
            }
        }
    }

    /// <summary>
    /// Sets <paramref name="value"/> to <c>null</c> and disposes it, ignoring all exceptions
    /// </summary>
    public static void DisposeRef<T>([HandlesResourceDisposal] ref T? value)
        where T : class
    {
        var toDispose = Interlocked.Exchange(ref value, null);
        if (toDispose is IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // Ignore all exceptions
            }
        }
    }
}