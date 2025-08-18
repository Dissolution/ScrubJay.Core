namespace ScrubJay.Text;

public partial class TextBuilder
{
    /// <summary>
    /// Creates a <c>new</c> <see cref="TextBuilder"/> instance
    /// </summary>
    public static TextBuilder New
    {
        [MustDisposeResource(true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new();
    }

    /// <summary>
    /// Builds a <see cref="string"/> using a temporary <see cref="TextBuilder"/> instance
    /// </summary>
    /// <param name="buildText">
    /// The <see cref="Action{T}"/> to invoke on a temporary <see cref="TextBuilder"/> instance
    /// </param>
    /// <returns>
    /// The <see cref="string"/> produced by calling <see cref="TextBuilder.ToString"/> on the temporary instance before disposing it
    /// </returns>
    public static string Build(Action<TextBuilder>? buildText)
    {
        if (buildText is null)
            return string.Empty;
        using var builder = new TextBuilder();
        buildText(builder);
        return builder.ToString();
    }

    public static string Build<S>(S state, Action<TextBuilder, S>? buildStatefulText)
    {
        if (buildStatefulText is null)
            return string.Empty;
        using var builder = new TextBuilder();
        buildStatefulText(builder, state);
        return builder.ToString();
    }

    public static string Build(ref InterpolatedTextBuilder interpolatedText)
    {
        return interpolatedText.ToStringAndDispose();
    }

}