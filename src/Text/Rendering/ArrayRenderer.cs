namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class ArrayRenderer : IRenderer<Array>
{
    public bool CanRender(Type? type) => type is not null && type.IsArray;

    public TextBuilder FluentRender(TextBuilder builder, Array? array)
    {
        if (array is null)
            return builder;

        int rank = array.Rank;

        if (array.Length == 0)
        {
            return builder
                .RepeatAppend(rank, '[')
                .RepeatAppend(rank, ']');
        }

        if (rank == 1)
        {
            builder
                .Append('[')
                .Render(array.GetValue(0));
            for (var i = 1; i < array.Length; i++)
            {
                builder.Append(", ").Render(array.GetValue(i));
            }

            return builder.Append(']');
        }

        throw new NotImplementedException();
    }
}