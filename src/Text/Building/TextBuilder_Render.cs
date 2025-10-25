using ScrubJay.Text.Scratch;

namespace ScrubJay.Text;

public partial class TextBuilder
{
    public TextBuilder Render<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Renderer.WriteTo(this, value);
        return this;
    }
}