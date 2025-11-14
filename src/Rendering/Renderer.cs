using System.Collections.Concurrent;
using System.Reflection;
using System.Security;
using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Rendering;

/* Rendering needs to be relatively lightweight
 *
 */



[PublicAPI]
public static partial class Renderer
{
    private static ConcurrentTypeMap<object> _valueRenderers = [];
    private static List<IOpenRenderer> _openRenderers = [];

    static Renderer()
    {
        Add<byte>(static (u8, tb) => tb.Format(u8));
        Add<sbyte>(static (i8, tb) => tb.Format(i8));
        Add<short>(static (i16, tb) => tb.Format(i16));
        Add<ushort>(static (u16, tb) => tb.Format(u16));
        Add<int>(static (i32, tb) => tb.Format(i32));
        Add<uint>(static (u32, tb) => tb.Format(u32).Write('U'));
        Add<long>(static (i64, tb) => tb.Format(i64).Write('L'));
        Add<ulong>(static (u64, tb) => tb.Format(u64).Write("UL"));

        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#general-format-specifier-g
        Add<float>(static (f32, tb) => tb.Format(f32, "G9").Write('f'));
        Add<double>(static (f64, tb) => tb.Format(f64, "G17").Write('d'));
        Add<decimal>(static (dec, tb) => tb.Format(dec, "G").Write('m'));

        Add<TimeSpan>(static (ts, tb) => tb.Format(ts, "g"));
        Add<DateTime>(static (dt, tb) => tb.Format(dt, "yyyy-MM-dd HH:mm:ss"));
        Add<DateTimeOffset>(static (dto, tb) => tb.Format(dto, "yyyy-MM-dd HH:mm:ss"));

        Add<char>(static (ch, tb) => tb.Append('\'').Append(ch).Append('\''));
        Add<string>(static (str, tb) => tb.Append('"').Append(str).Append('"'));

        Add<DBNull>(static (_, tb) => tb.Write(nameof(DBNull)));
        Add<bool>(static (boolean, tb) => tb.If(boolean, "true", "false"));

        Add<Guid>(RenderGuidTo);
        Add<ITuple>(RenderTupleTo);
        Add<Array>(RenderArrayTo);
        Add(new ArrayRenderer());
    }

    private static ValueRenderer<T>? GetValueRenderer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (_valueRenderers.TryGetValue<T>(out var obj))
        {
            if (obj is ValueRenderer<T> vrDelegate)
                return vrDelegate;
            if (obj is IValueRenderer<T> vrtImpl)
                return vrtImpl.RenderTo;
        }

        if (typeof(T).IsRef)
        {
            //return static (v, t) => HandleRefStruct(v, t);
            return HandleRefStruct;
        }
        else
        {
            //return static (v, t) => NonRsPassthrough(v, t);
            return NonRsPassthrough;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NonRsPassthrough<T>(T value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        unsafe
        {
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(builder));
            Emit.Call(new MethodRef(typeof(Renderer), nameof(Handle)).MakeGenericMethod(typeof(T)));
        }
    }

    internal static void HandleRefStruct<RS>(RS value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where RS : allows ref struct
#endif
    {
        Debugger.Break();
    }

    internal static void Handle<T>(T value, TextBuilder builder)
    {
        if (value is Enum e)
        {
            string str = EnumInfo.For(e).GetMemberInfo(e)!.ToString();
            builder.Write(str);
            return;
        }

        Debugger.Break();
    }

    public static void FaceRenderTo<T>(T? value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is null)
        {
            builder.Write("〈null〉");
            return;
        }

        var valueRenderer = GetValueRenderer<T>();
        if (valueRenderer is not null)
        {
            valueRenderer.Invoke(value, builder);
            return;
        }

        Debugger.Break();
    }

    public static void Add<T>(ValueRenderer<T> valueRenderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _valueRenderers.AddOrUpdate<T>(valueRenderer);
    }

    public static void Add<T>(IValueRenderer<T> valueRenderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Throw.IfNull(valueRenderer);
        _valueRenderers.AddOrUpdate<T>(valueRenderer);
    }


    public static string Render<T>(this T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        using var builder = new TextBuilder();
        RenderTo<T>(value, builder);
        return builder.ToString();
    }

    public static void RenderTo<T>(this T? value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        FaceRenderTo<T>(value, builder);
    }
}