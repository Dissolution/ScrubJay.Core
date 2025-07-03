namespace ScrubJay.Collections.Pooling;

[PublicAPI]
public static class InstancePoolExtensions
{
    public static void Borrow<T>(
        this IInstancePool<T> pool,
        Action<T> borrowedInstanceAction)
        where T : class
    {
        T instance = pool.Rent();
        borrowedInstanceAction(instance);
        pool.Return(instance);
    }

    public static R Borrow<T, R>(
        this IInstancePool<T> pool,
        Func<T, R> borrowedInstanceAction)
        where T : class
    {
        T instance = pool.Rent();
        R result = borrowedInstanceAction(instance);
        pool.Return(instance);
        return result;
    }
}
