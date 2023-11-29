#if !(NET48 || NETSTANDARD2_0)

namespace ScrubJay.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(
        this IAsyncEnumerable<T> asyncEnumerable,
        CancellationToken token = default)
    {
        var list = new List<T>();
        await foreach (T item in asyncEnumerable.WithCancellation(token))
        {
            list.Add(item);
        }
        return list;
    }
}

#endif