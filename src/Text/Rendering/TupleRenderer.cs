#if !NETSTANDARD2_0
namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class TupleRenderer : Renderer<ITuple>
{
    public override void RenderValue(TextBuilder builder, ITuple tuple)
    {
        int len = tuple.Length;
        builder.Append('(');

        if (len > 0)
        {
            builder.Render(tuple[0]);
        }

        int i = 1;
        while (i < len)
        {
            builder.Append(", ").Render(tuple[i]);
            i += 1;
        }

        builder.Append(')');
    }
}
#endif