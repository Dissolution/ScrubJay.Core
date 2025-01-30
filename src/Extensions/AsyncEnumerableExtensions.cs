namespace ScrubJay.Extensions;

public static class AsyncEnumerableExtensions
{
#if !(NET481 || NETSTANDARD2_0)
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable, CancellationToken token = default)
    {
        var list = new List<T>();
        await foreach (T item in asyncEnumerable.WithCancellation(token))
        {
            list.Add(item);
        }
        return list;
    }
#endif
}