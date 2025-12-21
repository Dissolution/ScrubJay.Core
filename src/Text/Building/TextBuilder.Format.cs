using InlineIL;
using static InlineIL.IL;
// ReSharper disable MergeCastWithTypeCheck

namespace ScrubJay.Text;

partial class TextBuilder
{
#region Format

    public TextBuilder Format<T>(T? value)
    {
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, null))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
            }
            else
            {
                Write(((IFormattable)value).ToString(null, null));
            }
#else
            Write(((IFormattable)value).ToString(null, null));
#endif
        }
        else if (value is not null)
        {
            Write(value.ToString());
        }

        return this;
    }

    public TextBuilder Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        if (format.Equate('@'))
        {
            Renderer.RenderTo<T>(value, this);
        }
        else if (format.Equate("@T"))
        {
            Renderer.RenderTo<Type>(Type.GetType<T>(value), this);
        }
        else if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
            }
            else
            {
                Write(((IFormattable)value).ToString(format, provider));
            }
#else
            Write(((IFormattable)value).ToString(format, provider));
#endif
        }
        else if (value is not null)
        {
            Write(value.ToString());
        }

        return this;
    }

    public TextBuilder Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        if (format.Equate('@'))
        {
            Renderer.RenderTo<T>(value, this);
        }
        else if (format.Equate("@T"))
        {
            Renderer.RenderTo<Type>(Type.GetType<T>(value), this);
        }
        else if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
            }
            else
            {
                Write(((IFormattable)value).ToString(format.AsString(), provider));
            }
#else
            Write(((IFormattable)value).ToString(format.AsString(), provider));
#endif
        }
        else if (value is not null)
        {
            Write(value.ToString());
        }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Format<T>(T? value, char format)
    {
        if (format == '@')
        {
            return Render<T>(value);
        }

        return Format<T>(value, format.AsString());
    }


#if NET9_0_OR_GREATER

    private TextBuilder CallFormat<T>(T? value, string? format)
        where T : allows ref struct
    {
        Emit.Ldarg_0(); // this
        Emit.Ldarg(nameof(value));
        Emit.Ldarg(nameof(format));
        Emit.Ldnull(); // IFormatProvider?
        Emit.Call(new MethodRef(typeof(TextBuilder), nameof(Format), 1, typeof(T), typeof(string), typeof(IFormatProvider))
            .MakeGenericMethod(typeof(T)));
        Emit.Ret();
        throw Unreachable();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder Format<T>(T? value, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        if (format == '@')
        {
            return Render<T>(value);
        }

        if (typeof(T).IsRef)
            throw Ex.Argument(value, "ref structs only support two formats: null and '@'");

        // we cannot defer to Format<T> as we have the `allows ref struct` constraint on our `T`
        // even though we know that this value is not a ref struct
        // The only way to bypass is to abuse Emission
        return CallFormat(value, format.AsString());
    }
#endif

#endregion /Format

#region FormatLine

    public TextBuilder FormatLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

    public TextBuilder FormatLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => Format<T>(value, format, provider).NewLine();

    public TextBuilder FormatLine<T>(T? value, char format)
        => Format<T>(value, format).NewLine();

#if NET9_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder FormatLine<T>(T? value, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
        => Format<T>(value, format).NewLine();
#endif

#endregion /FormatLine

#region FormatMany

    public TextBuilder FormatMany<T>(IEnumerable<T>? values)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Format<T>(value);
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
                Format<T>(value, format, provider);
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
                Format<T>(value, format, provider);
            }
        }

        return this;
    }

    public TextBuilder FormatMany<T>(IEnumerable<T>? values, char format)
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Format<T>(value, format);
            }
        }

        return this;
    }

#if NET9_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TextBuilder FormatMany<T>(IEnumerable<T>? values, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Format<T>(value, format);
            }
        }

        return this;
    }
#endif

#endregion /FormatMany
}