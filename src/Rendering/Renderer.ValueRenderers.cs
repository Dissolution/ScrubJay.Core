using ScrubJay.Collections.NonGeneric;
using ScrubJay.Iteration;

namespace ScrubJay.Rendering;

public partial class Renderer
{
    internal static void RenderGuidTo(Guid guid, TextBuilder builder)
    {
        var buffer = builder.Allocate(36);
#if NETFRAMEWORK || NETSTANDARD2_0
        string str = guid.ToString("N");
        TextHelper.Notsafe.CopyBlock(str, buffer, 36);
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
    }

    internal static void RenderTupleTo<T>(T tuple, TextBuilder builder)
        where T : ITuple
    {
        builder
            .Append('(')
            .Delimit(", ", tuple.AsIterable(), "@")
            .Append(')');
    }

    internal static void RenderEnumTo<E>(E @enum, TextBuilder builder)
        where E : struct, Enum
    {
        EnumMemberInfo.For<E>(@enum)!.RenderTo(builder);
    }


    internal static void RenderReadOnlySpanTo<T>(ReadOnlySpan<T> span, TextBuilder builder)
    {
        builder.Append('[')
            .Delimit<T>(", ", span, "@")
            .Write(']');
    }

    internal static void RenderSpanTo<T>(Span<T> span, TextBuilder builder)
    {
        builder.Append('[')
            .Delimit<T>(", ", span, "@")
            .Write(']');
    }

    internal static void RenderEnumerableTo<T>(IEnumerable<T> enumerable, TextBuilder builder)
    {
        if (enumerable is IList<T> list)
        {
            builder
                .Append('[')
                .Delimit(", ", list, "@")
                .Append(']');
        }
        else if (enumerable is ICollection<T> collection)
        {
            builder
                .Append('(')
                .Delimit(", ", collection, "@")
                .Append(')');
        }
        else
        {
            builder
                .Append('{')
                .Delimit(", ", enumerable, "@")
                .Append('}');
        }
    }

    internal static void RenderDictionaryTo<K, V>(IDictionary<K, V> dictionary, TextBuilder builder)
    {
        builder.Append('{')
            .Delimit(", ", dictionary,
                static (tb, pair) => tb.Append($"({pair.Key:@}: {pair.Value:@})"))
            .Append('}');
    }

    internal static void RenderRenderableTo<R>(TextBuilder builder, R renderable)
        where R : IRenderable
    {
        renderable.RenderTo(builder);
    }
}