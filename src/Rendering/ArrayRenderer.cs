namespace ScrubJay.Rendering;

internal class ArrayRenderer : IValueRenderer<Array>, IHasDefault<ArrayRenderer>
{
    public static ArrayRenderer Default { get; } = new();

    public virtual bool CanRenderType(Type instanceType)
    {
        return instanceType.IsArray;
    }

    public void RenderTo(Array array, TextBuilder builder)
    {
        int rank = array.Rank;
        if (rank == 1)
        {
            builder.Write('[');

            int low = array.GetLowerBound(0);
            int high = array.GetUpperBound(0);

            if (high < low)
            {
                // empty
            }
            else
            {
                object? obj = array.GetValue(low);
                obj.RenderTo(builder);
                for (var i = low + 1; i <= high; i++)
                {
                    builder.Write(", ");
                    obj = array.GetValue(low);
                    obj.RenderTo(builder);
                }
            }

            builder.Write(']');
        }
        else
        {
            throw Ex.NotImplemented();
        }


        // using var e = ArrayIndicesEnumerator.For(array);
    }
}