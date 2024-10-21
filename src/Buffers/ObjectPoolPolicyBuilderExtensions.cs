namespace ScrubJay.Buffers;

public static class ObjectPoolPolicyBuilderExtensions
{
    public static ObjectPoolPolicyBuilder<T> Create<T>(this ObjectPoolPolicyBuilder<T> builder)
        where T : class, new()
    {
        return builder.Create(static () => new T());
    }

    public static ObjectPoolPolicyBuilder<T> Clean<T>(this ObjectPoolPolicyBuilder<T> builder)
        where T : class, IList
    {
        return builder.Clean(static value => value.Clear());
    }

#pragma warning disable S2953
    public static ObjectPoolPolicyBuilder<T> Dispose<T>(this ObjectPoolPolicyBuilder<T> builder)
        where T : class, IDisposable
    {
        return builder.Dispose(static value => value.Dispose());
    }
}