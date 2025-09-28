using System.Globalization;

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class GuidRenderer : Renderer<Guid>
{
    public override void RenderValue(TextBuilder builder, Guid guid)
    {
        var buffer = builder.Allocate(36);
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
        builder.Write(buffer);
    }
}