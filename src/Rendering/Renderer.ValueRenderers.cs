using ScrubJay.Collections.NonGeneric;
using ScrubJay.Iteration;

namespace ScrubJay.Rendering;

public partial class Renderer
{
    private static void RenderGuidTo(Guid guid, TextBuilder builder)
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

    private static void RenderTupleTo<T>(T tuple, TextBuilder builder)
        where T : ITuple
    {
        builder
            .Append('(')
            .Delimit(", ", tuple.AsIterable())
            .Append(')');
    }

    private static void RenderEnumTo<E>(E @enum, TextBuilder builder)
        where E : struct, Enum
    {
        // todo:
        // EnumInfo.For<E>().GetMemberInfo(@enum).RenderTo(builder);

#if NET8_0_OR_GREATER
        if (Enum.TryFormat(@enum, builder.Available, out int charsWritten))
        {
            builder.Length += charsWritten;
            return;
        }
#endif

//#if NETSTANDARD
        string name = Enum.GetName(typeof(E), @enum) ?? @enum.ToString()!;
// #else
//         string name = Enum.GetName<E>(@enum) ?? @enum.ToString();
// #endif
        builder.Write(name);
    }


    private static void RenderReadOnlySpanTo<T>(ReadOnlySpan<T> span, TextBuilder builder)
    {
        builder.Append('[')
            .Delimit<T>(", ", span, "@")
            .Write(']');
    }

    private static void RenderSpanTo<T>(Span<T> span, TextBuilder builder)
    {
        builder.Append('[')
            .Delimit<T>(", ", span, "@")
            .Write(']');
    }

    private static void RenderArrayTo<T>(T[] array, TextBuilder builder)
    {
        builder.Append('[')
            .Delimit<T>(", ", array, "@")
            .Write(']');
    }
}

