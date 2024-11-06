// I do not want to rename UnbreakableEnumerator
#pragma warning disable CA1710


namespace ScrubJay.Debugging;

[PublicAPI]
public sealed class UnbreakableEnumerable<T> : IEnumerable<T>, IEnumerable
{
    private readonly IEnumerable<T> _enumerable;

    public UnbreakableEnumerable(IEnumerable<T>? enumerable)
    {
        if (enumerable is null)
        {
            UnhandledEventWatcher.Unbroken(this, new ArgumentNullException(nameof(enumerable)));
            _enumerable = [];
        }
        else
        {
            _enumerable = enumerable;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource]
    public UnbreakableEnumerator<T> GetEnumerator()
    {
        IEnumerator<T>? enumerator = null;
        try
        {
            enumerator = _enumerable.GetEnumerator();
        }
        catch (Exception ex)
        {
            UnhandledEventWatcher.Unbroken(this, ex);
        }

        return new UnbreakableEnumerator<T>(enumerator);
    }
}