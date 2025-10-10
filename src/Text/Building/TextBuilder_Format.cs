// ReSharper disable MergeCastWithTypeCheck

#if NET9_0_OR_GREATER
#endif
namespace ScrubJay.Text;

public partial class TextBuilder
{


#region Format

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Format<T>(T? value)
    {
        Write<T>(value);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        Write<T>(value, format, provider);
        return this;
    }

    public TextBuilder Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        Write<T>(value, format, provider);
        return this;
    }

#endregion

#region FormatMany

    public TextBuilder FormatMany<T>(params ReadOnlySpan<T> values)
    {
        if (!values.IsEmpty)
        {
            foreach (var value in values)
            {
                Write<T>(value);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(scoped ReadOnlySpan<T> values, string? format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(scoped ReadOnlySpan<T> values, scoped text format, IFormatProvider? provider = null)
    {
        if (!values.IsEmpty)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values, string? format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values, scoped text format, IFormatProvider? provider = null)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(Func<Option<T>>? iterator, string? format, IFormatProvider? provider = null)
    {
        if (iterator is not null)
        {
            while (iterator().IsSome(out var value))
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(Func<Option<T>>? iterator, scoped text format, IFormatProvider? provider = null)
    {
        if (iterator is not null)
        {
            while (iterator().IsSome(out var value))
            {
                Write<T>(value, format, provider);
            }
        }

        return this;
    }

#endregion

#region FormatLine

    public TextBuilder FormatLine<T>(T? value) => Format<T>(value).NewLine();

    public TextBuilder FormatLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

    public TextBuilder FormatLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

#endregion
}