namespace ScrubJay.Debugging;

[PublicAPI]
[MustDisposeResource]
public sealed class UnbreakableEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    private IEnumerator<T>? _enumerator;
    private Option<T> _current;

    object? IEnumerator.Current => Current;

    public T Current => _current.SomeOrThrow("Enumeration has no value to yield");


    public UnbreakableEnumerator(IEnumerator<T>? enumerator)
    {
        _enumerator = enumerator;
    }

    public bool MoveNext()
    {
        _current = TryMoveNext();
        return _current.IsSome;
    }

    public Option<T> TryMoveNext()
    {
        if (_enumerator is null)
        {
            UnhandledEventWatcher.Unbroken(this, new ObjectDisposedException(nameof(UnbreakableEnumerator<T>)));
            return None<T>();
        }

        bool moved;
        T current;

        while (true)
        {
            try
            {
                moved = _enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                UnhandledEventWatcher.Unbroken(this, ex);
                // We could not move; we are finished enumerating
                return None<T>();
            }

            // If we could not move next, we are done enumerating
            if (!moved)
                return None<T>();

            // Try to access current
            try
            {
                current = _enumerator.Current;
            }
            catch (Exception ex)
            {
                UnhandledEventWatcher.Unbroken(this, ex);
                // We need to try the next item
                continue;
            }

            // We found the next item
            return Some(current);
        }
    }

    public void Reset()
    {
        if (_enumerator is null)
        {
            UnhandledEventWatcher.Unbroken(this, new ObjectDisposedException(nameof(UnbreakableEnumerator<T>)));
            return;
        }

        try
        {
            _enumerator.Reset();
        }
        catch (Exception ex)
        {
            UnhandledEventWatcher.Unbroken(this, ex);
        }
    }

    public void Dispose()
    {
        _ = Disposable.TryNullDisposeRef(ref _enumerator);
    }
}