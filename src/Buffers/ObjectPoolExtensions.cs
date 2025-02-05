namespace ScrubJay.Buffers;

[PublicAPI]
public static class ObjectPoolExtensions
{
    public static void Borrow<T>(
        this IObjectPool<T> pool,
        Action<T> borrowedInstanceAction)
        where T : class
    {
        T instance = pool.Rent();
        borrowedInstanceAction(instance);
        pool.Return(instance);
    }

    public static TReturn Borrow<T, TReturn>(
        this IObjectPool<T> pool,
        Func<T, TReturn> borrowedInstanceAction)
        where T : class
    {
        T instance = pool.Rent();
        TReturn result = borrowedInstanceAction(instance);
        pool.Return(instance);
        return result;
    }
}