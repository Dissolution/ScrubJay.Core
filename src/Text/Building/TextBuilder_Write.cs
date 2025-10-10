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

            Notsafe.Text.CopyBlock(text, _chars.AsSpan(_position), text.Length);
            _position += text.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string? str) => Write(str.AsSpan());

    public void Write<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (value is not null)
        {
            Write(TextHelper.ToString(value));
        }
    }

#if NET9_0_OR_GREATER
    public void Write<T>(T? value,
        string? format,
        IFormatProvider? provider = null,
        GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        where T : allows ref struct
    {
        // render?
        if (format.Equate('@'))
        {

        }
    }
#endif

    public void Write<T>(T? value,
        string? format,
        IFormatProvider? provider = null)
    {
        // special format codes for Rendering to support InterpolatedTextHandler

        // render this value
        if (format.Equate('@'))
        {
            Rendering.Renderer.RenderValue<T>(this, value);
            return;
        }

        // render this value's type
        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            Rendering.Renderer.RenderValue<Type>(this, value?.GetType() ?? typeof(T));
            return;
        }

        if (value is IFormattable)
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
                return;
            }
#endif

            Write(((IFormattable)value).ToString(format, provider));
            return;
        }

        if (value is not null)
        {
            Write(value.ToString());
        }
    }

    public void Write<T>(T? value, scoped text format, IFormatProvider? provider)
    {
        // special format codes for Rendering
        // this is to support interpolated text

        // render this value
        if (format.Equate('@'))
        {
            Rendering.Renderer.RenderValue<T>(this, value);
            return;
        }

        // render this value's type
        if (format.Equate("@T", StringComparison.OrdinalIgnoreCase))
        {
            Rendering.Renderer.RenderValue<Type>(this, value?.GetType() ?? typeof(T));
            return;
        }

        if (value is IFormattable)
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
                return;
            }
#endif

            Write(((IFormattable)value).ToString(format.AsString(), provider));
            return;
        }

        if (value is not null)
        {
            Write(value.ToString());
        }
    }
}