using System.Collections.Concurrent;
using System.Reflection;
using System.Security;
using InlineIL;
using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Local

namespace ScrubJay.Rendering;

/* Rendering needs to be relatively lightweight
 *
 */

[PublicAPI]
public static partial class Renderer
{
    private static readonly ConcurrentTypeMap<object> _valueRenderers = [];

    static Renderer()
    {
        Register<byte>(static (u8, tb) => tb.Format(u8));
        Register<sbyte>(static (i8, tb) => tb.Format(i8));
        Register<short>(static (i16, tb) => tb.Format(i16));
        Register<ushort>(static (u16, tb) => tb.Format(u16));
        Register<int>(static (i32, tb) => tb.Format(i32));
        Register<uint>(static (u32, tb) => tb.Format(u32).Write('U'));
        Register<long>(static (i64, tb) => tb.Format(i64).Write('L'));
        Register<ulong>(static (u64, tb) => tb.Format(u64).Write("UL"));

        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#general-format-specifier-g
        Register<float>(static (f32, tb) => tb.Format(f32, "G9").Write('f'));
        Register<double>(static (f64, tb) => tb.Format(f64, "G17").Write('d'));
        Register<decimal>(static (dec, tb) => tb.Format(dec, "G").Write('m'));

        Register<TimeSpan>(static (ts, tb) => tb.Format(ts, "g"));
        Register<DateTime>(static (dt, tb) => tb.Format(dt, "yyyy-MM-dd HH:mm:ss"));
        Register<DateTimeOffset>(static (dto, tb) => tb.Format(dto, "yyyy-MM-dd HH:mm:ss"));

        Register<char>(static (ch, tb) => tb.Append('\'').Append(ch).Append('\''));
        Register<string>(static (str, tb) => tb.Append('"').Append(str).Append('"'));

        Register<DBNull>(static (_, tb) => tb.Write(nameof(DBNull)));
        Register<bool>(static (boolean, tb) => tb.If(boolean, "true", "false"));

        Register<Guid>(RenderGuidTo);
        Register<ITuple>(RenderTupleTo);
        Register<Array>(RenderArrayTo);
        Register<object>(RenderObjectTo);
    }





    private static ValueRenderer<T> BindRenderMethod<T>(string methodName)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var renderMethod = typeof(Renderer).GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull();
        var del = Delegate.CreateDelegate(typeof(ValueRenderer<T>), renderMethod, true)
            .ThrowIfNull();
        var valueRenderer = del.ThrowIfNot<ValueRenderer<T>>();
        return valueRenderer!;
    }

    private static ValueRenderer<T> BindRenderMethod<T>(Type type, string methodName)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var elementType = type.GetElementType() ?? type.GetGenericArguments().Single();
        var renderMethod = typeof(Renderer).GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(elementType);
        var del = Delegate.CreateDelegate(typeof(ValueRenderer<T>), renderMethod, true)
            .ThrowIfNull();
        var valueRenderer = del.ThrowIfNot<ValueRenderer<T>>();
        return valueRenderer!;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueRenderer<T>? Shim<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        unsafe
        {
            //Emit.Ldarg(nameof(value));
            //Emit.Ldarg(nameof(builder));
            //Emit.Call(new MethodRef(typeof(Renderer), nameof(Handle)).MakeGenericMethod(typeof(T)));
            Emit.Call(new MethodRef(typeof(Renderer), nameof(FindNonRefStructRenderer)).MakeGenericMethod(typeof(T)));
            return Return<ValueRenderer<T>>();
        }
    }

    private static ValueRenderer<T>? FindRefStructRenderer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Debugger.Break();
        throw Ex.NotImplemented();
    }

    private static ValueRenderer<T>? FindNonRefStructRenderer<T>()
    {
        var type = typeof(T);
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var renderArrayMethod = typeof(Renderer).GetMethod(
                nameof(RenderGenericArrayTo),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(elementType);
            var del = Delegate.CreateDelegate(typeof(ValueRenderer<T>), renderArrayMethod);
            var valueRenderer = del as ValueRenderer<T>;
            return valueRenderer!;
        }

        return DefaultNonRefStructRenderTo;
    }

    private static void DefaultNonRefStructRenderTo<T>(T value, TextBuilder builder)
    {
        if (value is Enum e)
        {
            builder.Write(e.Display());
            return;
        }

        Debugger.Break();
        throw Ex.NotImplemented();
    }







    private static ValueRenderer<T> CreateValueRenderer<T>(Type type)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (type.IsEnum)
        {
            var renderMethod = typeof(Renderer).GetMethod(
                    nameof(RenderEnumTo),
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type);
            return Delegate.CreateDelegate<ValueRenderer<T>>(renderMethod);
        }

        if (type.IsArray)
        {
            if (type.GenericTypeArguments.Length != 1)
            {
                return BindRenderMethod<T>(nameof(RenderArrayTo));
            }
            else
            {
                return BindRenderMethod<T>(type, nameof(RenderGenericArrayTo));
            }
        }

        if (type.HasGenericTypeDefinition(typeof(Span<>)))
        {
            return BindRenderMethod<T>(type, nameof(RenderSpanTo));
        }

        if (type.HasGenericTypeDefinition(typeof(ReadOnlySpan<>)))
        {
            return BindRenderMethod<T>(type, nameof(RenderSpanTo));
        }

        // tostring renderer
        return static (value, builder) => builder.Write(value.Stringify());
    }


    private static ValueRenderer<T> FindValueRenderer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var renderer = _valueRenderers.GetOrAdd<T>(CreateValueRenderer<T>);

        if (renderer is ValueRenderer<T> valueRenderer)
            return valueRenderer;
        if (renderer is IValueRenderer<T> interfacedValueRenderer)
            return interfacedValueRenderer.RenderTo;

        throw Ex.Unreachable();
    }



    public static void Register<T>(ValueRenderer<T> valueRenderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _valueRenderers.AddOrUpdate<T>(valueRenderer);
    }

    public static void Register<T>(IValueRenderer<T> valueRenderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _valueRenderers.AddOrUpdate<T>(valueRenderer);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderTo<T>(this T? value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is null)
        {
            builder.Write("〈null〉");
        }
        else
        {
            FindValueRenderer<T>()(value, builder);
        }
    }

    public static string Render<T>(this T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return TextBuilder.New.Invoke<T?>(value, static (tb,v) => RenderTo<T>(v,tb)).ToStringAndDispose();
    }
}