namespace ScrubJay.Pooling;

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

    public static TReturn Borrow<T, TReturn>(
        this IInstancePool<T> pool,
        Func<T, TReturn> borrowedInstanceAction)
        where T : class
    {
        T instance = pool.Rent();
        TReturn result = borrowedInstanceAction(instance);
        pool.Return(instance);
        return result;
    }
}
