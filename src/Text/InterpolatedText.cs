#pragma warning disable CA2213 // Disposable fields should be disposed
// ReSharper disable NotDisposedResource
namespace ScrubJay.Text;

/// <summary>
/// An minimal Interpolated String Handler
/// </summary>
/// <remarks>
/// This is a thin wrapper around <see cref="Buffer{T}"/>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
[InterpolatedStringHandler]
[MustDisposeResource(true)]
public ref struct InterpolatedText : IDisposable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetStartingCapacity(int literalLength, int formattedCount) => literalLength + (formattedCount * 16);


    // do not make readonly
    private Buffer<char> _buffer;

    public InterpolatedText()
    {
        _buffer = new Buffer<char>();
    }

    public InterpolatedText(int literalLength, int formattedCount)
    {
        _buffer = new Buffer<char>(GetStartingCapacity(literalLength, formattedCount));
    }

    public InterpolatedText(int literalLength, int formattedCount, Buffer<char> initialBuffer)
    {
        _buffer = initialBuffer;
        _buffer.GrowCapacity(GetStartingCapacity(literalLength, formattedCount));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string str)
    {
        _buffer.AddMany(str.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(char ch) => _buffer.Add(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(scoped ReadOnlySpan<char> text) => _buffer.AddMany(text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str)
    {
        if (str is not null)
        {
            _buffer.AddMany(str.AsSpan());
        }
    }

    public void AppendFormatted<T>(T value)
    {
        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, default, default))
                {
                    _buffer.Grow();
                }
                _buffer.Count += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            str = value?.ToString();
        }

        _buffer.AddMany(str.AsSpan());
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, format.AsSpan(), default))
                {
                    _buffer.Grow();
                }
                _buffer.Count += charsWritten;
                return;
            }

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, null);
        }
        else
        {
            str = value?.ToString();
        }

        _buffer.AddMany(str.AsSpan());
    }

    [HandlesResourceDisposal]
    public void Dispose()
    {
        _buffer.Dispose();
    }

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = this.ToString();
        Dispose();
        return str;
    }

    public override string ToString() => _buffer.Written.ToString();
}