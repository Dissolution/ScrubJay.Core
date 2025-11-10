namespace ScrubJay.Text.Rendering;

[PublicAPI]
public static class RenderingExtensions
{
    extension<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        /// <summary>
        /// Render this value as a <see cref="string"/>
        /// </summary>
        public string Render()
        {
            using var builder = new TextBuilder();
            Renderer.WriteTo<T>(builder, value);
            return builder.ToString();
        }
    }

    extension<T>(ReadOnlySpan<T> value)
    {
        /// <summary>
        /// Render this value as a <see cref="string"/>
        /// </summary>
        public string Render()
        {
            using var builder = new TextBuilder();
            builder.Append("[")
                .Delimit(", ", value)
                .Append("]");
            return builder.ToString();
        }
    }

    extension<T>(Span<T> value)
    {
        /// <summary>
        /// Render this value as a <see cref="string"/>
        /// </summary>
        public string Render()
        {
            using var builder = new TextBuilder();
            builder.Append("[")
                .Delimit(", ", value)
                .Append("]");
            return builder.ToString();
        }
    }
}