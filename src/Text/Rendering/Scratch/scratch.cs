using System.Collections.Concurrent;
using System.Reflection;
using InlineIL;

namespace ScrubJay.Text.Scratch;

public delegate void RenderTo<in T>(TextBuilder builder, T value)
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
;

public delegate bool TryRenderObjectTo(TextBuilder builder, object value);

public static class ScratchRenderer
{
    private static readonly ConcurrentBag<Delegate> _renderers = [];

    static ScratchRenderer()
    {
        AddRenderer<TimeSpan>(static (tb, timespan) => tb.Format(timespan, "g"));
        AddRenderer<DateTime>(static (tb, datetime) => tb.Format(datetime, "yyyy-MM-dd HH:mm:ss"));
        AddRenderer<DateTimeOffset>(static (tb, datetime) => tb.Format(datetime, "yyyy-MM-dd HH:mm:ss"));
        AddRenderer<Guid>(static (tb, guid) =>
        {
            var buffer = tb.Allocate(36);
#if NETFRAMEWORK || NETSTANDARD2_0
            string str = guid.ToString("N");
            Notsafe.Text.CopyBlock(str, buffer, 36);
#else
            guid.TryFormat(buffer, out _, format: "D");
#endif
            buffer.ForEach((ref char ch) =>
            {
                if (ch == 'a')
                    ch = 'A';
                else if (ch == 'b')
                    ch = 'B';
                else if (ch == 'c')
                    ch = 'C';
                else if (ch == 'd')
                    ch = 'D';
                else if (ch == 'e')
                    ch = 'E';
                else if (ch == 'f')
                    ch = 'F';
            });

            tb.Write(buffer);
        });
        AddRenderer<Enum>(static (tb, e) =>
        {
            EnumInfo.For(e).GetMemberInfo(e)!.RenderTo(tb);
            tb.Append("_TESTING");
        });
    }

    private static TextBuilder ObjectRenderTo(TextBuilder builder, object? obj)
    {
        if (obj is null)
        {
            return builder;
        }

        var type = obj.GetType();
        var rendererType = typeof(RenderTo<>).MakeGenericType(type);
        var renderer = _renderers.FirstOrDefault(renderer => renderer.GetType() == rendererType);
        if (renderer is null)
        {
            // fallback
#if NET9_0_OR_GREATER
            return builder.Append(TextHelper.ToString(obj));
#else
            return builder.Append(obj.ToString());
#endif
        }

        var output = renderer.DynamicInvoke(builder, obj);
        Debug.Assert(output is null);
        return builder;
    }

    public static void AddRenderer<T>(RenderTo<T> renderer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        _renderers.Add(renderer);
    }

    public static void AddRenderer(TryRenderObjectTo renderer)
    {
        _renderers.Add(renderer);
    }


    public static TextBuilder RenderTo<T>(TextBuilder builder, T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is null)
        {
            return builder.Append("null");
        }

        if (typeof(T) == typeof(object))
        {
            return ObjectRenderTo(builder, Notsafe.As<T?, object?>(value));
        }

        foreach (var renderer in _renderers)
        {
            if (renderer.Is<RenderTo<T>>(out var renderTo))
            {
                renderTo(builder, value);
                return builder;
            }
        }

        if (!typeof(T).IsRef)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer.Is<TryRenderObjectTo>(out var renderTo))
                {
                    if (renderTo(builder, Notsafe.Box(value)))
                        return builder;
                }
            }
        }

        // no registered renderers can handle this value
#if NET9_0_OR_GREATER
        return builder.Append(TextHelper.ToString(value));
#else
        return builder.Format<T>(value);
#endif
    }

    public static TextBuilder RenderTo(TextBuilder builder, scoped text text)
    {
        return builder.Append(text);
    }

    public static TextBuilder RenderTo<T>(TextBuilder builder, scoped ReadOnlySpan<T> span)
    {
        return builder
            .Append('[')
            .Delimit(", ", span)
            .Append(']');
    }

    public static TextBuilder RenderTo<T>(TextBuilder builder, scoped Span<T> span)
    {
        return builder
            .Append('[')
            .Delimit(", ", span)
            .Append(']');
    }

    public static TextBuilder RenderTo<T>(TextBuilder builder, T[]? array)
    {
        return builder
            .Append('[')
            .Delimit(", ", array)
            .Append(']');
    }

    public static TextBuilder RenderTo<T>(TextBuilder builder, IEnumerable<T>? values)
    {
        if (values is null)
        {
            return builder;
        }
        else if (values is IList<T> list)
        {
            return builder
                .Append('[')
                .Delimit(", ", list)
                .Append(']');
        }
        else if (values is ICollection<T> collection)
        {
            return builder
                .Append('(')
                .Delimit(", ", collection)
                .Append(')');
        }
        else
        {
            return builder
                .Append('{')
                .Delimit(", ", values)
                .Append('}');
        }
    }

    public static TextBuilder RenderTo<K,V>(TextBuilder builder, IReadOnlyDictionary<K,V>? dictionary)
    {
        return builder
            .Append('{')
            .Delimit(", ", dictionary, static (tb, kvp) => tb.Append($"({kvp.Key:@}: {kvp.Value:@})"))
            .Append('}');
    }
}