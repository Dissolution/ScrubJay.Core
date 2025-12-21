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
    private static readonly TypeMap<Delegate> _exactValueRenderers = [];
    private static readonly List<IValueRenderer> _openValueRenderers = [];
    static Renderer()
    {
        Register<byte>(static (u8, tb) => tb.Append(u8));
        Register<sbyte>(static (i8, tb) => tb.Append(i8));
        Register<short>(static (i16, tb) => tb.Append(i16));
        Register<ushort>(static (u16, tb) => tb.Append(u16));
        Register<int>(static (i32, tb) => tb.Append(i32));
        Register<uint>(static (u32, tb) => tb.Append(u32).Write('U'));
        Register<long>(static (i64, tb) => tb.Append(i64).Write('L'));
        Register<ulong>(static (u64, tb) => tb.Append(u64).Write("UL"));

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
        //Register<ITuple>(RenderTupleTo);
        //Register<Array>(RenderArrayTo);
        Register<object>(RenderObjectTo);

        Register(new ExceptionRenderer());
        Register(new TypeRenderer());
        Register(new MethodRenderer());
        Register(new ParameterRenderer());
    }

    internal static ValueRenderer<T> GetValueRenderer<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        // exact?
        if (_exactValueRenderers.TryGetValue<T>(out var render) &&
            render.Is<ValueRenderer<T>>(out var valueRenderer))
            return valueRenderer;

        // open?
        foreach (var openRenderer in _openValueRenderers)
        {
            if (openRenderer.CanRender(typeof(T)) &&
                openRenderer.Is<IValueRenderer<T>>(out var impl))
            {
                return impl.RenderTo;
            }
        }

        var type = typeof(T);

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
                return BindRenderMethod<T>(nameof(Render2DArrayTo), type);
            }
        }

        if (type.HasGenericTypeDefinition(typeof(Span<>)))
        {
            return BindRenderMethod<T>(nameof(RenderSpanTo), type);

        }

        if (type.HasGenericTypeDefinition(typeof(ReadOnlySpan<>)))
        {
            return BindRenderMethod<T>(nameof(RenderSpanTo), type);
        }

        if (type.HasGenericTypeDefinition(typeof(IDictionary<,>)))
        {
            return BindRenderMethod<T>(nameof(RenderDictionaryTo), type);
        }

        if (type.HasGenericTypeDefinition(typeof(IEnumerable<>)))
        {
            return BindRenderMethod<T>(nameof(RenderEnumerableTo), type);
        }

        if (type.IsByRef)
        {
            return DefaultRefRenderTo;
        }
        else
        {
            return DefaultNonRefRenderToShim;
        }
    }

    /// <summary>
    /// Binds one of <see cref="Renderer"/>'s methods to a <see cref="ValueRenderer{T}"/> delegate
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static ValueRenderer<T> BindRenderMethod<T>(string methodName, Type? type = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        var renderMethod = typeof(Renderer).GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull();

        if (type is not null)
        {
            var genericTypes = type.GetGenericArguments();
            // var elementType = type.GetElementType();
            if (genericTypes.Length > 0)
            {
                renderMethod = renderMethod.MakeGenericMethod(genericTypes);
            }
        }

        var valueRenderer = Delegate.CreateDelegate<ValueRenderer<T>>(renderMethod)
            .ThrowIfNull();
        return valueRenderer!;
    }

    /// <summary>
    /// Registers a new <see cref="ValueRenderer{T}"/>
    /// </summary>
    /// <param name="valueRenderer"></param>
    /// <typeparam name="T"></typeparam>
    public static void Register<T>(ValueRenderer<T> valueRenderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _exactValueRenderers.AddOrUpdate<T>(valueRenderer);
    }

    /// <summary>
    /// Registers a new <see cref="IValueRenderer{T}"/>
    /// </summary>
    /// <param name="valueRenderer"></param>
    /// <typeparam name="T"></typeparam>
    public static void Register<T>(IValueRenderer<T> valueRenderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _openValueRenderers.Add(valueRenderer);
    }

    public static void RenderTo<T>(T? value, TextBuilder builder)
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
            GetValueRenderer<T>().Invoke(value, builder);
        }
    }

    public static string Render<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is null)
            return "〈null〉";

        var renderer = GetValueRenderer<T>();
        using var builder = new TextBuilder();
        renderer(value, builder);
        return builder.ToString();
    }
}