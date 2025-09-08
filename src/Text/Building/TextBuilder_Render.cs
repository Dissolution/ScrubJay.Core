using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

public partial class TextBuilder
{
#region Render

    public TextBuilder Render(scoped text text)
    {
        return Append('"').Append(text).Append('"');
    }

    public TextBuilder Render<T>(T? value)
    {
        return RendererCache.RenderTo<T>(this, value);
    }

    public TextBuilder Render<T>(scoped ReadOnlySpan<T> span)
    {
        return Append('[')
            .Delimit<T>(Delimiter.CommaSpace, span)
            .Append(']');
    }

    public TextBuilder Render<T>(scoped Span<T> span)
    {
        return Append('[')
            .Delimit<T>(Delimiter.CommaSpace, span)
            .Append(']');
    }

    public TextBuilder Render<T>(T[]? array)
    {
        return Append('[')
            .Delimit<T>(Delimiter.CommaSpace, array)
            .Append(']');
    }

    public TextBuilder Render<K, V>(IDictionary<K, V>? dictionary)
    {
        return
            Append('[')
                .Delimit(", ", dictionary, static (tb, kvp) => tb.Append($"({kvp.Key:@}: {kvp.Value:@})")
                .Append(']');
    }

    public TextBuilder Render<T>(IEnumerable<T>? values)
    {
        if (values is null)
            return this;

        if (values is IList<T> list)
        {
            return Append('[')
                .RenderMany(list, ", ")
                .Append(']');
        }
        else if (values is ICollection<T> collection)
        {
            return Append('(')
                .RenderMany(collection, ", ")
                .Append(')');
        }
        else
        {
            return Append('{')
                .RenderMany(values, ", ")
                .Append('}');
        }
    }

#endregion

#region RenderMany

    public TextBuilder RenderMany<T>(params ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            foreach (T? value in values)
            {
                RendererCache.RenderTo<T>(this, value);
            }
        }

        return this;
    }

    public TextBuilder RenderMany<T>(scoped Span<T> values)
    {
        if (!values.IsEmpty)
        {
            foreach (T? value in values)
            {
                RendererCache.RenderTo<T>(this, value);
            }
        }

        return this;
    }

    public TextBuilder RenderMany<T>(T[]? values)
    {
        if (!values.IsNullOrEmpty())
        {
            foreach (T? value in values)
            {
                RendererCache.RenderTo<T>(this, value);
            }
        }

        return this;
    }

    public TextBuilder RenderMany<T>(IEnumerable<T>? values)
    {
        if (values is not null)
        {
            foreach (T? value in values)
            {
                RendererCache.RenderTo<T>(this, value);
            }
        }

        return this;
    }

    public TextBuilder RenderMany<T>(Func<Option<T>>? iterator)
    {
        if (iterator is not null)
        {
            T? value;
            while (iterator().IsSome(out value))
            {
                RendererCache.RenderTo<T>(this, value);
            }
        }

        return this;
    }

#endregion

#region RenderLine

    public TextBuilder RenderLine<T>(T? value) => Render<T>(value).NewLine();

#endregion
}