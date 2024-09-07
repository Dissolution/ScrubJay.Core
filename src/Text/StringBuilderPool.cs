using System.Text;
using ScrubJay.Buffers;

namespace ScrubJay.Text;

public static class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> _pool = ObjectPool.Create<StringBuilder>(
        factory: () => new StringBuilder(1024),
        clean: static sb => sb.Clear());

    public static StringBuilder Rent() => _pool.Rent();
    
    public static string ToStringAndReturn(this StringBuilder? builder)
    {
        var str = builder?.ToString();
        _pool.Return(builder);
        return str ?? "";
    }
    public static void Return(this StringBuilder? builder) => _pool.Return(builder);
    
    public static IDisposable Borrow(out StringBuilder builder)
    {
        var instance = _pool.GetInstance();
        builder = instance.Instance;
        return instance;
    }
    
    public static string Borrow(Action<StringBuilder> build)
    {
        var builder = _pool.Rent();
        build(builder);
        var str = builder.ToString();
        _pool.Return(builder);
        return str;
    }
}