namespace ScrubJay.Text;

partial class TextBuilder
{
#region Delimit

#region Values: Append

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Append<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter).Append<T>(values[i]);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Append<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter).Append<T>(values[i]);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            Append<T>(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                Invoke(delimit).Append<T>(values[i]);
            }
        }

        return this;
    }

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

    public TextBuilder Delimit<T>(char delimiter, T[]? values)
        => Delimit<T>(delimiter, values.AsSpan());

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values)
        => Delimit<T>(delimiter, values.AsSpan());

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan());
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values)
        => Delimit<T>(delimit, values.AsSpan());

#endregion Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Append<T>(e.Current);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Append<T>(e.Current);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Append<T>(e.Current);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Append<T>(e.Current);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Append<T>(e.Current);
                while (e.MoveNext())
                {
                    Invoke(delimit).Append<T>(e.Current);
                }
            }
        }

        return this;
    }

#endregion Values: IEnumerable<T>

#region Values: Iterable<T>

    public TextBuilder Delimit<T>(char delimiter, Iterable<T>? values)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Append<T>(value);
                while (values().IsSome(out value))
                {
                    Append(delimiter).Append<T>(value);
                }
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, Iterable<T>? values)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Append<T>(value);
                while (values().IsSome(out value))
                {
                    Append(delimiter).Append<T>(value);
                }
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, Iterable<T>? values)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, Iterable<T>? values)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Append<T>(value);
                while (values().IsSome(out value))
                {
                    Invoke(delimit).Append<T>(value);
                }
            }
        }

        return this;
    }

#endregion Values: Iterable<T>

#endregion Values: Append

#region Values: Format

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter)
                    .Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter)
                    .Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values,
        string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            Format<T>(values[0], format, provider);
            for (int i = 1; i < values.Length; i++)
            {
                Invoke(delimit)
                    .Format<T>(values[i], format, provider);
            }
        }

        return this;
    }

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

    public TextBuilder Delimit<T>(char delimiter, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter, values.AsSpan(), format, provider);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter, values.AsSpan(), format, provider);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan(), format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimit, values.AsSpan(), format, provider);

#endregion Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Format<T>(e.Current, format, provider);
            }

            while (e.MoveNext())
            {
                Invoke(delimit).Format<T>(e.Current, format, provider);
            }
        }

        return this;
    }

#endregion Values: IEnumerable<T>

#region Values: Iterable<T>

    public TextBuilder Delimit<T>(char delimiter, Iterable<T>? values, string? format, IFormatProvider? provider = null)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Format<T>(value, format, provider);
                while (values().IsSome(out value))
                {
                    Append(delimiter)
                        .Format<T>(value, format, provider);
                }
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, Iterable<T>? values, string? format, IFormatProvider? provider = null)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Format<T>(value, format, provider);
                while (values().IsSome(out value))
                {
                    Append(delimiter).Format<T>(value, format, provider);
                }
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, Iterable<T>? values, string? format, IFormatProvider? provider = null)
        => Delimit<T>(delimiter.AsSpan(), values, format, provider);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, Iterable<T>? values, string? format,
        IFormatProvider? provider = null)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Format<T>(value, format, provider);
                while (values().IsSome(out value))
                {
                    Invoke(delimit).Format<T>(value, format, provider);
                }
            }
        }

        return this;
    }

#endregion Values: Iterable<T>

#endregion Values: Format

#region Values: Build

#region Values: ReadOnlySpan<T>

    public TextBuilder Delimit<T>(char delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            Invoke(values[0], build);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter).Invoke(values[i], build);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            Invoke(values[0], build);
            for (int i = 1; i < values.Length; i++)
            {
                Append(delimiter).Invoke(values[i], build);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, scoped ReadOnlySpan<T> values, Action<TextBuilder, T>? build)
    {
        if (!values.IsEmpty && build is not null)
        {
            Invoke(values[0], build);
            for (int i = 1; i < values.Length; i++)
            {
                Invoke(delimit).Invoke(values[i], build);
            }
        }

        return this;
    }

#endregion Values: ReadOnlySpan<T>

#region Values: T[]

    public TextBuilder Delimit<T>(char delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, values.AsSpan(), build);

    public TextBuilder Delimit<T>(scoped text delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter, values.AsSpan(), build);

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values.AsSpan(), build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, T[]? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimit, values.AsSpan(), build);

#endregion Values: T[]

#region Values: IEnumerable<T>

    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null && build is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Invoke(e.Current, build);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Invoke(e.Current, build);
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Invoke(e.Current, build);
            }

            while (e.MoveNext())
            {
                Append(delimiter).Invoke(e.Current, build);
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T>? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, IEnumerable<T>? values, Action<TextBuilder, T>? build)
    {
        if (values is not null)
        {
            using var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                Invoke(e.Current, build);
                while (e.MoveNext())
                {
                    Invoke(delimit).Invoke(e.Current, build);
                }
            }
        }

        return this;
    }

#endregion Values: IEnumerable<T>

#region Values: Iterable<T>

    public TextBuilder Delimit<T>(char delimiter, Iterable<T>? values, Action<TextBuilder, T>? build)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Invoke(value, build);
                while (values().IsSome(out value))
                {
                    Append(delimiter).Invoke(value, build);
                }
            }
        }

        return this;
    }

    public TextBuilder Delimit<T>(scoped text delimiter, Iterable<T>? values, Action<TextBuilder, T>? build)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Invoke(value, build);
                while (values().IsSome(out value))
                {
                    Append(delimiter).Invoke(value, build);
                }
            }
        }

        return this;
    }

#if NETFRAMEWORK || NETSTANDARD2_0
    public TextBuilder Delimit<T>(string? delimiter, Iterable<T>? values, Action<TextBuilder, T>? build)
        => Delimit<T>(delimiter.AsSpan(), values, build);
#endif

    public TextBuilder Delimit<T>(Action<TextBuilder>? delimit, Iterable<T>? values, Action<TextBuilder, T>? build)
    {
        T? value;

        if (values is not null)
        {
            if (values().IsSome(out value))
            {
                Invoke(value, build);
                while (values().IsSome(out value))
                {
                    Invoke(delimit).Invoke(value, build);
                }
            }
        }

        return this;
    }

#endregion Values: Iterable<T>

#endregion Values: Build

#endregion Delimit
}