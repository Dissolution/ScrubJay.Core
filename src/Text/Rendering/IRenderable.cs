namespace ScrubJay.Text.Rendering;

/// <summary>
/// Similar to <see cref="IFormattable"/>, provides functionality to render a value to a <see cref="TextBuilderBase{B}"/>
/// </summary>
/// <remarks>
/// 💡- <see cref="object.ToString"/> can be implemented as:<br/>
/// <code>
/// public override string ToString() => TextBuilder.Build(RenderTo);
/// </code>
/// </remarks>
[PublicAPI]
public interface IRenderable
{
    void RenderTo<B>(B textBuilder)
        where B : TextBuilderBase<B>;
}

