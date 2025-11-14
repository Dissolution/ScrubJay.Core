using ScrubJay.Iteration;

namespace ScrubJay.Text;

partial class TextBuilder
{
#region Enumerate

    public TextBuilder Enumerate<T>(scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            foreach (var value in values)
            {
                build(this, value);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(T[]? values, Action<TextBuilder, T>? build)
    {
        if (values is not null && build is not null)
        {
            foreach (var value in values)
            {
                build(this, value);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null && build is not null)
        {
            foreach (var value in values)
            {
                build(this, value);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<R,V>(Iterable<R,V> iterable, Action<TextBuilder, V>? build)
        where R : struct, IIterator<V>
    {
        if (build is not null)
        {
            var iterator = iterable.Iterator;
            while (iterator.TryMoveNext(out var value))
            {
                build(this, value);
            }
        }

        return this;
    }

#endregion

#region Enumerate w/Index

    public TextBuilder Enumerate<T>(scoped ReadOnlySpan<T> values, Action<TextBuilder, T, int>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                build(this, values[i], i);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(T[]? values, Action<TextBuilder, T, int>? build)
    {
        if (values is not null && build is not null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                build(this, values[i], i);
            }
        }

        return this;
    }

    public TextBuilder Enumerate<T>(IEnumerable<T>? values, Action<TextBuilder, T, int>? build)
    {
        if (values is not null && build is not null)
        {
            if (values is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    build(this, list[i], i);
                }
            }
            else
            {
                int i = 0;
                foreach (var value in values)
                {
                    build(this, value, i);
                    i++;
                }
            }
        }

        return this;
    }

    public TextBuilder Enumerate<R,V>(Iterable<R,V> iterable, Action<TextBuilder, V, int>? build)
        where R : struct, IIterator<V>
    {
        if (build is not null)
        {
            var iterator = iterable.Iterator;
            int index = 0;
            while (iterator.TryMoveNext(out var value))
            {
                build(this, value, index);
                index++;
            }
        }

        return this;
    }


#endregion
}