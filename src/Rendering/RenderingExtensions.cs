namespace ScrubJay.Rendering;

[PublicAPI]
public static class RenderingExtensions
{
    extension(Enum e)
    {
        public string Display()
        {
            var memberInfo = EnumMemberInfo.For(e);
            if (memberInfo is not null)
            {
                return memberInfo.Display;
            }

            return Enum.GetName(e.GetType(), e) ?? e.ToString();
        }
    }
    extension<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        /// <summary>
        /// Render this <typeparamref name="T"/> <paramref name="value"/> to a <see cref="TextBuilder"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RenderTo(TextBuilder builder)
        {
            if (value is null)
            {
                builder.Write("〈null〉");
            }
            else
            {
                Renderer.GetValueRenderer<T>().Invoke(value, builder);
            }
        }

        /// <summary>
        /// Gets a <see cref="string"/> Rendering of this <typeparamref name="T"/> <paramref name="value"/>
        /// </summary>
        public string Render()
        {
            if (value is null)
                return "〈null〉";

            var renderer = Renderer.GetValueRenderer<T>();
            using var builder = new TextBuilder();
            renderer(value, builder);
            return builder.ToString();
        }
    }

#if !NET9_0_OR_GREATER
    public static string Render<T>(this Span<T> span)
    {
        using var builder = new TextBuilder();
        Renderer.RenderSpanTo<T>(span, builder);
        return builder.ToString();
    }

    public static string Render<T>(this ReadOnlySpan<T> span)
    {
        using var builder = new TextBuilder();
        Renderer.RenderReadOnlySpanTo<T>(span, builder);
        return builder.ToString();
    }
#endif
}