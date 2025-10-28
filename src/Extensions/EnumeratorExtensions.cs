namespace ScrubJay.Extensions;

[PublicAPI]
public static class EnumeratorExtensions
{
    extension<E>(E enumerator)
        where E : IEnumerator
    {
        public Option<object?> Next()
        {
            if (enumerator.MoveNext())
            {
                return Some<object?>(enumerator.Current);
            }

            return None;
        }

        public void Dispose()
        {
            if (enumerator is IDisposable)
            {
                ((IDisposable)enumerator).Dispose();
            }
        }
    }
}

[PublicAPI]
public static class EnumeratorTExtensions
{
    extension<E, T>(E enumerator)
        where E : IEnumerator<T>
    {
        public Option<T> Next()
        {
            if (enumerator.MoveNext())
            {
                return Some<T>(enumerator.Current);
            }

            return None;
        }
    }
}