using System.Collections.Concurrent;
using System.Reflection;

namespace ScrubJay.Text.Scratch;

// Serilog Structured Data
// https://github.com/serilog/serilog/wiki/Structured-Data

// Scalar Types
// Booleans - bool
// Numerics - byte, short, ushort, int, uint, long, ulong, float, double, decimal
// Strings - string, byte[]
// Temporals - DateTime, DateTimeOffset, TimeSpan
// Others - Guid, Uri
// Nullables - nullable versions of any of the types above

// Collections
// IEnumerable
// Dictionary<K,V> where K is one of the Scalar Types above

// Defaults to ToString!

// @ - Destructure
// $ - Stringify

[PublicAPI]
public delegate void DumpTo<in T>(TextBuilder builder, T value, DumpMode mode)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
;

public interface IValueDumper<in T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    void DumpTo(TextBuilder builder, T value, DumpMode mode);
}

public sealed class EnumDumper : IValueDumper<Enum>, IHasDefault<EnumDumper>
{
    public static EnumDumper Default { get; } = new();

    public void DumpTo(TextBuilder builder, Enum value, DumpMode mode = DumpMode.Default)
    {
        throw new NotImplementedException();
    }

    public void DumpTo<E>(TextBuilder builder, E value, DumpMode mode = DumpMode.Default)
        where E : struct, Enum
    {
        // todo: change to dump
        EnumInfo.RenderTo(builder, value);
    }
}

[PublicAPI]
[Flags]
public enum DumpMode
{
    Default = 0,
    Extended = 1 << 0,
    Types = 1 << 1,
}

public static class Dumper
{
    extension(TextBuilder builder)
    {
        public TextBuilder Dump<T>(T? value, DumpMode mode = DumpMode.Default)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            DumpTo<T>(builder, value, mode);
            return builder;
        }
    }

    static Dumper()
    {

    }

    private static readonly ConcurrentBag<object> _dumpers = [];

    internal static readonly MethodInfo _dumpEnumMethod = typeof(EnumDumper)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(static method => method.Name == nameof(DumpTo) && method.GetGenericArguments().Length == 1)
        .FirstOrDefault()
        .ThrowIfNull();

    internal static readonly MethodInfo _findDumpMethod = typeof(Dumper)
        .GetMethod(nameof(FindDump), BindingFlags.NonPublic | BindingFlags.Static)
        .ThrowIfNull();


    private static DumpTo<T>? FindDump<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var type = typeof(T);

        if (type == typeof(object))
        {
            return static (tb, value, mode) => DumpTo(tb, Notsafe.Box(value), mode);
        }

        if (type.IsEnum)
        {
            return Delegate.CreateDelegate<DumpTo<T>>(_dumpEnumMethod.MakeGenericMethod(type));
        }

        foreach (var dumper in _dumpers)
        {
            // If this is a cached delegate that can handle this type (as the delegate is `in T`, delegates will handle supertypes)
            // use that delegate directly
            if (dumper is DumpTo<T> dumpTo)
            {
                return dumpTo;
            }

            // If this is an IDumper<T> instance (that can also handle supertypes as above)
            // we can use the delegate from that instance
            if (dumper is IValueDumper<T> valueDumper)
            {
                return valueDumper.DumpTo;
            }
        }

        // no dumper has been associated with this type
        return null;
    }

    private static void DumpNull<T>(TextBuilder builder, DumpMode mode)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (mode == DumpMode.Extended)
        {
            builder.Append("`null");
        }

        if (mode.HasFlags(DumpMode.Types))
        {
            builder.Append('(')
                .Dump(typeof(T))
                .Append(")null");
        }
    }


    public static void Add<T>(DumpTo<T> dumpTo)
    {
        Throw.IfNull(dumpTo);
        _dumpers.Add(dumpTo);
    }

    public static void Add<T>(IValueDumper<T> valueDumper)
    {
        Throw.IfNull(valueDumper);
        _dumpers.Add(valueDumper);
    }

    public static void DumpTo(TextBuilder builder, object? obj, DumpMode mode = DumpMode.Default)
    {
        if (obj is null)
        {
            DumpNull<object>(builder, mode);
            return;
        }

        Type  type = obj.GetType();
        if (type == typeof(object))
        {
            // stop infinite recursion
            builder.Append("(object)");
            return;
        }

        // search for a compat
        _findDumpMethod.MakeGenericMethod(type)
            .Invoke(null, null)
            // will be a delegate
            .ThrowIfNot<Delegate>()
            // invoke it
            .DynamicInvoke(builder, obj, mode);
        // return is void
    }

    public static void DumpTo<T>(TextBuilder builder, T? value, DumpMode mode = DumpMode.Default)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is null)
        {
            DumpNull<T>(builder, mode);
            return;
        }

        // Find a function that lets us dump this value
        var dumpTo = FindDump<T>();
        if (dumpTo is not null)
        {
            dumpTo(builder, value, mode);
            return;
        }

        // no registered function for dumping
        // just call ToString
        builder.Append<T>(value);
    }


    public static string Dump<T>(T? value, DumpMode mode = DumpMode.Default)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        using var builder = new TextBuilder();
        DumpTo<T>(builder, value, mode);
        return builder.ToString();
    }
}