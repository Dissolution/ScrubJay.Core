namespace ScrubJay.Text;

partial class TextBuilder
{
    /// <summary>
    /// Renders the given <paramref name="value"/> to this <see cref="TextBuilder"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Render<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Renderer.RenderTo<T>(value, this);
        return this;
    }
}