namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class EnumerableRenderer : Renderer<IEnumerable>
{
    public override TextBuilder FluentRender(TextBuilder builder, IEnumerable? enumerable)
    {
        if (enumerable is null)
            return builder;

        builder.Render(enumerable.GetType());

        if (enumerable is IDictionary dictionary)
        {
            builder
                .Append('[')
                .EnumerateAndDelimit(
                    dictionary.OfType<DictionaryEntry>(),
                    static (tb, entry) => tb.Append('(').Render(entry.Key).Append(": ").Render(entry.Value).Append(')'),
                    ", ")
                .Append(']');
        }
        else if (enumerable is IList list)
        {
            builder
                .Append('[')
                .EnumerateAndDelimit(
                    list.OfType<object?>(),
                    static (tb, item) => tb.Render(item),
                    ", ")
                .Append(']');
        }
        else if (enumerable is ICollection collection)
        {
            builder
                .Append('(')
                .EnumerateAndDelimit(
                    collection.OfType<object?>(),
                    static (tb, item) => tb.Render(item),
                    ", ")
                .Append(')');
        }
        else
        {
            builder
                .Append('(')
                .EnumerateAndDelimit(
                    enumerable.OfType<object?>(),
                    static (tb, item) => tb.Render(item),
                    ", ")
                .Append(')');
        }

        return builder;
    }
}