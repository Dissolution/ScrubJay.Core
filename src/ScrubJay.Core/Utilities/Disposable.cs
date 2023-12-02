namespace ScrubJay.Utilities;

/// <summary>
/// A utility for working with <see cref="IDisposable"/>
/// </summary>
public static class Disposable
{
    public static IDisposable FromAction(Action? action)
    {
        return new ActionDisposable(action);
    }

    /// <summary>
    /// Tries to dispose of <paramref name="value"/>, ignoring all exceptions
    /// </summary>
    public static void Dispose<T>(T? value)
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
    public static void DisposeRef<T>(ref T? value)
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