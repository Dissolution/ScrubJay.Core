// Members can be made readonly
// ReSharper disable NotDisposedResource

#pragma warning disable IDE0250, IDE0251, CA1001

namespace ScrubJay.Text;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="TextBuilder"/>
/// </summary>
[PublicAPI]
[InterpolatedStringHandler]
[MustDisposeResource(false)]
public ref struct InterpolatedTextBuilder
{
    private readonly TextBuilder _builder;
    private readonly bool _disposeBuilder;

    [MustDisposeResource(true)]
    public InterpolatedTextBuilder()
    {
        _builder = new TextBuilder();
        _disposeBuilder = true;
    }

    [MustDisposeResource(true)]
    public InterpolatedTextBuilder(int literalLength, int formattedCount)
    {
        _builder = new TextBuilder(literalLength + (formattedCount * 16));
        _disposeBuilder = true;
    }

    [MustDisposeResource(false)]
    public InterpolatedTextBuilder(TextBuilder builder)
    {
        _builder = builder;
        _disposeBuilder = false;
    }

    [MustDisposeResource(false)]
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
    public void AppendFormatted(scoped text txt) => _builder.Append(txt);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(scoped text txt, int alignment) => _builder.Align(txt, width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str) => _builder.Append(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str, int alignment) => _builder.Align(str.AsSpan(), width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => _builder.Format(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format) => _builder.Format(value, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, scoped text format) => _builder.Format(value, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment) => _builder.AlignFormat<T>(value, width: alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment, string? format) => _builder.AlignFormat<T>(value, alignment, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(Action<TextBuilder>? build)
    {
        // pass-through to builder for execution
        _builder.Invoke(build);
    }

    public void Dispose()
    {
        if (_disposeBuilder)
        {
            _builder.Dispose();
        }
    }

    public string ToStringAndDispose()
    {
        string str = ToString();
        Dispose();
        return str;
    }

    public override string ToString() => _builder.ToString();
}