namespace ScrubJay.Functional;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/> and similar types
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T>? enumerable)
    {
        public IEnumerable<N> SelectWhere<N>(Func<T, Option<N>>? selectWhere)
        {
            if (enumerable is null || selectWhere is null)
                yield break;

            foreach (T value in enumerable)
            {
                if (selectWhere(value).IsSome(out var newValue))
                {
                    yield return newValue;
                }
            }
        }

        public IEnumerable<N> SelectWhere<N>(Func<T, Result<N>>? selectWhere)
        {
            if (enumerable is null || selectWhere is null)
                yield break;

            foreach (T value in enumerable)
            {
                if (selectWhere(value).IsOk(out var newValue))
                {
                    yield return newValue;
                }
            }
        }

        public IEnumerable<N> SelectWhere<N, E>(Func<T, Result<N, E>>? selectWhere)
        {
            if (enumerable is null || selectWhere is null)
                yield break;

            foreach (T value in enumerable)
            {
                if (selectWhere(value).IsOk(out var newValue))
                {
                    yield return newValue;
                }
            }
        }

        public IEnumerable<N> TrySelect<N>(Func<T, N>? selector)
        {
            if (enumerable is null || selector is null)
                yield break;

            foreach (T value in enumerable)
            {
                N newValue;
                try
                {
                    newValue = selector(value);
                }
                catch (Exception)
                {
                    continue;
                }

                yield return newValue;
            }
        }
    }
}