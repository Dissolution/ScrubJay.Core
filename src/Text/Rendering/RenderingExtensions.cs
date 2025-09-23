#pragma warning disable CA1823

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public static class RenderingExtensions
{
    extension<T>(T? value)
    {
        public string Render()
        {
            if (value is null)
            {
                return string.Empty;
            }

            using var builder = new TextBuilder();
            Rendering.Render.RenderValue(builder, value);
            return builder.ToString();
        }
    }

}