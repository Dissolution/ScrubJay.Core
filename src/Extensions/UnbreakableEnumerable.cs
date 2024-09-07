#pragma warning disable CA1031

using ScrubJay.Collections;

namespace ScrubJay.Extensions;

[PublicAPI]
public sealed class UnbreakableEnumerable<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> _enumerable;

    public UnbreakableEnumerable(IEnumerable<T>? enumerable)
    {
        _enumerable = enumerable ?? EmptyEnumerable<T>.Instance;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public UnbreakableEnumerator GetEnumerator()
    {
        IEnumerator<T>? enumerator;
        try
        {
            enumerator = _enumerable!.GetEnumerator();
        }
        catch (Exception)
        {
            enumerator = EmptyEnumerator<T>.Instance;
        }

        return new UnbreakableEnumerator(enumerator);
    }


    public sealed class UnbreakableEnumerator : IEnumerator<T>
    {
        private IEnumerator<T>? _enumerator;
        private Option<T> _current;

        object? IEnumerator.Current => Current;

        public T Current => _current.SomeOrThrow("Enumeration has no value to yield");


        public UnbreakableEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public bool MoveNext()
        {
            return (_current = TryMoveNext());
        }

        public Option<T> TryMoveNext()
        {
            if (_enumerator is null)
                return None<T>();

            bool moved;
            T current;

            while (true)
            {
                try
                {
                    moved = _enumerator.MoveNext();
                }
                catch (Exception)
                {
                    return default;
                }

                // If we could not move next, we are done enumerating
                if (!moved)
                    return default;

                // Try to access current
                try
                {
                    current = _enumerator.Current;
                }
                catch (Exception)
                {
                    // We need to try the next item
                    continue;
                }

                // Have it!
                return Some(current);
            }
        }

        void IEnumerator.Reset() => TryReset();

        public Result<Unit, Exception> TryReset()
        {
            if (_enumerator is null)
                return new ObjectDisposedException(nameof(UnbreakableEnumerator));
            try
            {
                _enumerator.Reset();
                return Unit.Default;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public void Dispose()
        {
            Result.TryDispose(_enumerator);
            _enumerator = null;
        }
    }
}