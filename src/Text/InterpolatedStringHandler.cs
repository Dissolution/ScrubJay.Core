namespace ScrubJay.Text;

[InterpolatedStringHandler]
public ref struct InterpolatedStringHandler
{
    public static string Render(ref InterpolatedStringHandler interpolatedText)
    {
        return interpolatedText.ToStringAndDispose();
    }
    
    private TextBuffer _textBuffer;

    public InterpolatedStringHandler()
    {
        _textBuffer = new();
    }

    public InterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _textBuffer = new(literalLength + (formattedCount * 16));
    }

    public void AppendLiteral(string str)
    {
        _textBuffer.Append(str.AsSpan());
    }

    public void AppendFormatted<T>(ReadOnlySpan<T> span)
    {
        _textBuffer.Append(span.ToString());
    }

    public void AppendFormatted<T>(T? value)
    {
        _textBuffer.AppendFormatted<T>(value);
    }


    [HandlesResourceDisposal]
    public void Dispose()
    {
        _textBuffer.Dispose();
    }

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}