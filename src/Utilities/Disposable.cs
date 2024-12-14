using ScrubJay.Debugging;

#pragma warning disable CA1031, MA0048, CA1045

namespace ScrubJay.Utilities;

public abstract class Disposable : IDisposable
{
    public static IDisposable Action(Action onDispose)
    {
        Throw.IfNull(onDispose);
        return new ActionDisposable(onDispose);
    }

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
    public static IAsyncDisposable AsyncAction(Func<ValueTask> onDispose)
    {
        Throw.IfNull(onDispose);
        return new ActionAsyncDisposable(onDispose);
    }
#endif

    public static bool TryDispose([HandlesResourceDisposal] IDisposable? disposable)
    {
        if (disposable is not null)
        {
            try
            {
                disposable.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                UnhandledEventWatcher.OnUnbreakableException($"{nameof(Disposable)}.{nameof(TryDispose)}", ex);
                // Suppress any errors
            }
        }

        return false;
    }

    public static bool TryDisposeRef<TDisposable>([HandlesResourceDisposal] ref TDisposable disposable)
        where TDisposable : IDisposable
    {
        try
        {
            disposable.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            UnhandledEventWatcher.OnUnbreakableException($"{nameof(Disposable)}.{nameof(TryDisposeRef)}", ex);
            // Suppress any errors
            return false;
        }
    }



    public static bool TryNullDisposeRef<TDisposable>(
        [HandlesResourceDisposal]
        ref TDisposable? disposable)
        where TDisposable : class, IDisposable
    {
        // Acquire and immediately set to null
        var localDisposable = Interlocked.Exchange<TDisposable?>(ref disposable, null);
        if (localDisposable is not null)
        {
            try
            {
                localDisposable.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                UnhandledEventWatcher.OnUnbreakableException($"{nameof(Disposable)}.{nameof(TryNullDisposeRef)}", ex);
                // Suppress any errors
                return false;
            }
            finally
            {
                // always set to null
                disposable = null;
            }
        }

        return true;
    }


    private bool _disposed; // false

    protected abstract void OnDispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (!_disposed)
        {
            OnDispose();
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}

internal sealed class ActionDisposable : Disposable
{
    private readonly Action _onDispose;

    internal ActionDisposable(Action onDispose)
    {
        _onDispose = onDispose;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void OnDispose() => _onDispose();
}

#if NETSTANDARD2_1 || NET6_0_OR_GREATER
internal sealed class ActionAsyncDisposable : IAsyncDisposable
{
    private readonly Func<ValueTask> _onAsyncDispose;

    public ActionAsyncDisposable(Func<ValueTask> onAsyncDispose)
    {
        _onAsyncDispose = onAsyncDispose;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask DisposeAsync() => _onAsyncDispose(); // no await, direct pass-through
}
#endif
