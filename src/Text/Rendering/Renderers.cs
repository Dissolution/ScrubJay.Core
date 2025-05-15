using ScrubJay.Collections.NonGeneric;

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public static class Renderers
{
    private static readonly List<IRenderer> _renderers;

    static Renderers()
    {
        _renderers = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(static assembly => Result.TryInvoke(assembly.GetTypes).OkOr([]))
            .Where(static type => type.Implements(typeof(Renderer<>)))
            .Where(static type => !type.IsAbstract)
            .SelectWhere(static type => Result.TryInvoke(type, static t => Activator.CreateInstance(t)))
            .OfType<IRenderer>()
            .ToList();
    }


    internal static B RenderValueTo<T, B>(T value, B builder)
        where B : TextBuilderBase<B>
    {
        if (value is IRenderable)
        {
            ((IRenderable)value).RenderTo<B>(builder);
            return builder;
        }

        var hasRenderer = _renderers.TryGetFirst(r => r.CanRender(typeof(T)));
        if (hasRenderer.IsOk(out var renderer))
        {
            if (renderer.Is<IRenderer<T>>(out var typedRenderer))
            {
                typedRenderer.RenderTo(value, builder);
                return builder;
            }
            else
            {
                typedRenderer = renderer as IRenderer<T>;
                throw new InvalidOperationException();
            }
        }

        // fallbacks
        switch (value)
        {
            case null:
                return builder.Append("`null`");
            case DBNull:
                return builder.Append(nameof(DBNull));
            case bool b:
                return builder.AppendIf(b, bool.TrueString, bool.FalseString);
            case byte u8:
                return builder.Append("(byte)").Append(u8);
            case sbyte i8:
                return builder.Append("(sbyte)").Append(i8);
            case short i16:
                return builder.Append("(short)").Append(i16);
            case ushort u16:
                return builder.Append("(ushort)").Append(u16);
            case int i32:
                return builder.Append(i32);
            case uint u32:
                return builder.Append(u32).Append('U');
            case long i64:
                return builder.Append(i64).Append('L');
            case ulong u64:
                return builder.Append(u64).Append("UL");
            case float f32:
                return builder.Append(f32, "N1").Append('f');
            case double f64:
                return builder.Append(f64, "N1").Append('d');
            case decimal dec:
                return builder.Append(dec, "N1").Append('m');
            case TimeSpan ts:
                return builder.Append(ts, "g");
            case DateTime dt:
                return builder.Append(dt, "yyyy-MM-dd HH:mm:ss");
            case char ch:
                return builder.Append('\'').Append(ch).Append('\'');
            case string str:
                return builder.Append('"').Append(str).Append('"');
#if !NETSTANDARD2_0
            case ITuple tuple:
            {
                return builder
                    .Append('(')
                    .EnumerateAndDelimit(Enumerable.Range(0, tuple.Length),
                        (tb, i) => RenderValueTo(tuple[i],tb),
                        tb => tb.Append(", "))
                    .Append(')');
            }
#endif
            case Enum e:
            {
                return builder.Append(e.ToString());
            }
            case Array array:
            {
                var wrapper = new ArrayWrapper<object?>(array);
                return builder.Append('[')
                    .EnumerateAndDelimit(wrapper,
                        (tb, item) => RenderValueTo(item, tb),
                        tb => tb.Append(", "))
                    .Append(']');
            }
            case IEnumerable enumerable:
            {
                return builder.Append('{')
                    .EnumerateAndDelimit(enumerable.OfType<object?>(),
                        (tb, item) => RenderValueTo(item, tb),
                        tb => tb.Append(", "))
                    .Append('}');
            }
            default:
            {
                break;
            }
        }

        return builder.Append(value);
    }


    public static B Render<B, T>(this B builder, T? value)
        where B : TextBuilderBase<B>
    {
        return RenderValueTo(value, builder);
    }

    public static B Render<B, T>(this B builder, T[]? array)
        where B : TextBuilderBase<B>
    {
        if (array is null)
            return builder.Append("`null`");

        return builder.Append('[')
            .EnumerateAndDelimit(array,
                (tb, item) => RenderValueTo(item, tb),
                tb => tb.Append(", "))
            .Append(']');
    }

    public static B Render<B>(this B builder, text text)
        where B : TextBuilderBase<B>
    {
        return builder.Append('"').Append(text).Append('"');
    }

    public static B Render<B, T>(this B builder, ReadOnlySpan<T> span)
        where B : TextBuilderBase<B>
    {
        return builder.Append('[')
            .EnumerateAndDelimit(span,
                (tb, item) => RenderValueTo(item, tb),
                tb => tb.Append(", "))
            .Append(']');
    }

    public static B Render<B, T>(this B builder, Span<T> span)
        where B : TextBuilderBase<B>
    {
        return builder.Append('[')
            .EnumerateAndDelimit((ReadOnlySpan<T>)span,
                (tb, item) => RenderValueTo(item, tb),
                tb => tb.Append(", "))
            .Append(']');
    }
}