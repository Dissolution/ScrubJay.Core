namespace ScrubJay.Text;

/* This portion of TextBuilder contains the underlying methods that write text directly to the rented array
 * These are designed for efficiency and do not return TextBuilder fluently for better inlining
 */

public partial class TextBuilder
{
    public void Write(char ch)
    {
        if (_position >= Capacity)
        {
            GrowBy(1);
        }

        _chars[_position] = ch;
        _position++;
    }

    public void Write(scoped text text)
    {
        if (!text.IsEmpty)
        {
            if (_position + text.Length > Capacity)
            {
                GrowBy(text.Length);
            }

            TextHelper.Notsafe.CopyBlock(text, _chars.AsSpan(_position), text.Length);
            _position += text.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string? str) => Write(str.AsSpan());


#if NET9_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void Write<T>(T? value, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        // stringify can be called on any value, including ref structs
        Write(value.Stringify());
    }

    public void Write<T>(T? value, char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        if (format == '@')
        {
            Renderer.RenderTo<T>(value, this);
        }
        else
        {
            Write(value.Stringify());
        }
    }
#endif

    public void Write<T>(T? value)
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
    }

    public void Write<T>(T? value,
        string? format,
        IFormatProvider? provider = null)
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
    }

    public void Write<T>(T? value, char format, IFormatProvider? provider = null)
    {
        if (format == '@')
        {
            Renderer.RenderTo<T>(value, this);
        }
        else if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format.AsSpan(), provider))
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
    }

    public void Write<T>(T? value, scoped text format, IFormatProvider? provider = null)
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
    }
}