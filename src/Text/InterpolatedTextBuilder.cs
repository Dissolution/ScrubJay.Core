// Members can be made readonly
// ReSharper disable NotDisposedResource
#pragma warning disable IDE0250, IDE0251, CA1001

namespace ScrubJay.Text;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="TextBuilder"/>
/// </summary>
[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder // : IDisposable
{
    private readonly TextBuilder _builder;
    private readonly bool _disposeBuilder;

    public InterpolatedTextBuilder()
    {
        _builder = new TextBuilder();
        _disposeBuilder = true;
    }

    public InterpolatedTextBuilder(int literalLength, int formattedCount)
    {
        _builder = new TextBuilder(literalLength + (formattedCount * 16));
        _disposeBuilder = true;
    }

    public InterpolatedTextBuilder(TextBuilder builder)
    {
        _builder = builder;
        _disposeBuilder = false;
    }

    public InterpolatedTextBuilder(int literalLength, int formattedCount, TextBuilder builder)
    {
        _builder = builder;
        _disposeBuilder = false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string str) => _builder.Append(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(char ch) => _builder.Append(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(char ch, int alignment) => _builder.Align(ch, width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(text txt) => _builder.Append(txt);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(text txt, int alignment) => _builder.Align(txt, width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str) => _builder.Append(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str, int alignment) => _builder.Align(str.AsSpan(), width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value) => _builder.Format(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format) => _builder.Format(value, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, scoped text format) => _builder.Format(value, format);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment) => _builder.AlignFormat<T>(value, width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment, string? format) => _builder.AlignFormat<T>(value, alignment, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(Action<TextBuilder>? buildText)
    {
        // pass-through to builder for execution
        _builder.Invoke(buildText);
    }
    //
    // [HandlesResourceDisposal]
    // public void Dispose()
    // {
    //     if (_disposeBuilder)
    //     {
    //         _builder.Dispose();
    //     }
    // }

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = ToString();
        if (_disposeBuilder)
        {
            _builder.Dispose();
        }
        return str;
    }

    public override string ToString() => _builder.ToString();
}
