// constrained call avoiding boxing for value types
// ReSharper disable MergeCastWithTypeCheck

using System.Buffers;
using JetBrains.Annotations;

namespace ScrubJay.Text;

[MustDisposeResource(true)]
[InterpolatedStringHandler]
public ref struct InterpolatedStringHandler // : IDisposable
{
    public static implicit operator InterpolatedStringHandler(Span<char> initialBuffer) => new InterpolatedStringHandler(initialBuffer);
    
    private const int HOLE_LENGTH = 16;
    private const int MIN_CAPACITY = 256;
    private const int MAX_CAPACITY = 0x3FFFFFDF; // string.MaxLength < Array.MaxLength < int.MaxLength

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetDefaultLength(int literalLength, int formattedCount) => (literalLength + (formattedCount * HOLE_LENGTH)).Clamp(MIN_CAPACITY, MAX_CAPACITY);


    private char[]? _charArray;
    private Span<char> _charSpan;
    private int _position;

    internal Span<char> Written => _charSpan.Slice(0, _position);
    internal Span<char> Available => _charSpan.Slice(_position);

    public InterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(GetDefaultLength(literalLength, formattedCount));
        _position = 0;
    }

    public InterpolatedStringHandler(int minCapacity)
    {
        _charSpan = _charArray = ArrayPool<char>.Shared.Rent(minCapacity.Clamp(MIN_CAPACITY, MAX_CAPACITY));
        _position = 0;
    }

    public InterpolatedStringHandler(Span<char> initialBuffer)
    {
        _charSpan = initialBuffer;
        _charArray = null;
        _position = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int count)
    {
        int newCapacity = ((_charSpan.Length + count) * 2).Clamp(256, 0x3FFFFFDF);

        char[] newArray = ArrayPool<char>.Shared.Rent(newCapacity);
        Written.CopyTo(newArray);

        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;

        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn, true);
        }
    }
    
    public void AppendLiteral(string str)
    {
        int pos = _position;
        int newPos = pos + str.Length;
        if (newPos > _charSpan.Length)
        {
            GrowBy(str.Length);
        }
        
        str.CopyTo(_charSpan.Slice(pos));
        _position = newPos;
    }

    public void AppendFormatted(char ch)
    {
        int pos = _position;
        if (pos >= _charSpan.Length)
        {
            GrowBy(1);
        }
        
        _charSpan[pos] = ch;
        _position = pos + 1;
    }
    
    public void AppendFormatted(scoped ReadOnlySpan<char> text)
    {
        int pos = _position;
        int newPos = pos + text.Length;
        if (newPos > _charSpan.Length)
        {
            GrowBy(text.Length);
        }
        
        text.CopyTo(_charSpan.Slice(pos));
        _position = newPos;
    }

    public void AppendFormatted(params char[]? characters) => AppendFormatted(characters.AsSpan());

    public void AppendFormatted(string? str)
    {
        if (str is not null)
        {
            int pos = _position;
            int newPos = pos + str.Length;
            if (newPos > _charSpan.Length)
            {
                GrowBy(str.Length);
            }
        
            str.CopyTo(_charSpan.Slice(pos));
            _position = newPos;
        }
    }
    
    public void AppendFormatted<T>(T value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                {
                    GrowBy(HOLE_LENGTH);
                }

                _position += charsWritten;
                return;
            }
#endif
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            AppendLiteral(str);
        }
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, default))
                {
                    GrowBy(HOLE_LENGTH);
                }

                _position += charsWritten;
                return;
            }
#endif
            str = ((IFormattable)value).ToString(format, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            AppendLiteral(str);
        }
    }
    
    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        if (toReturn is not null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    public string ToStringAndDispose()
    {
        string result = Written.ToString();
        Dispose();
        return result;
    }

    public override string ToString() => Written.ToString();
}