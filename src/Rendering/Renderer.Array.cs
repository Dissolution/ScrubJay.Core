using ScrubJay.Collections.NonGeneric;

namespace ScrubJay.Rendering;

partial class Renderer
{
    internal static void RenderArrayTo(Array array, TextBuilder builder)
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
                    obj = array.GetValue(i);
                    obj.RenderTo(builder);
                }
            }

            builder.Write(']');
        }
        else
        {
            int[] indices = new int[rank];
            Array.Clear(indices, 0, indices.Length);
            iterateDimension(0);
            return;


            void iterateDimension(int dim)
            {
                int start = array.GetLowerBound(dim);
                int end = array.GetUpperBound(dim);

                if (end < start)
                {
                    builder.Write("[]");
                    return;
                }

                // any dimension other than the last
                if (dim < (rank - 1))
                {
                    // bracket all my children
                    builder.Write('[');

                    // iterate them
                    for (var i = start; i <= end; i++)
                    {
                        indices[dim] = i;
                        iterateDimension(dim + 1);
                    }

                    builder.Write(']');
                    return;
                }

                // last dimension, we actually write values
                builder.Write('[');
                indices[dim] = start;
                object? obj = array.GetValue(indices);
                RenderTo<object>(obj, builder);
                for (var i = start + 1; i <= end; i++)
                {
                    indices[dim] = i;
                    builder.Write(", ");
                    obj = array.GetValue(indices);
                    RenderTo<object>(obj, builder);
                }
                builder.Write(']');
            }
        }
    }

    internal static void RenderGenericArrayTo<T>(T[] array, TextBuilder builder)
    {
        builder.Append('[')
            .Delimit<T>(", ", array, "@")
            .Write(']');
    }
}