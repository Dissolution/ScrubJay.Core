#pragma warning disable CA1715, IDE0250, IDE0251, MA0102, IDE0060, RCS1163

namespace ScrubJay.Text.Builders;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="FluentTextBuilder{B}"/>
/// </summary>
/// <typeparam name="B"></typeparam>
[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder<B>
    where B : FluentTextBuilder<B>
{
    private readonly B _builder;
    private readonly bool _disposeBuilder = false;

    [MustDisposeResource]
    public InterpolatedTextBuilder()
    {
        _builder = Activator.CreateInstance<B>();
        _disposeBuilder = true;
    }
    
    [MustDisposeResource]
    public InterpolatedTextBuilder(int literalLength, int formattedCount)
    {
        _builder = Activator.CreateInstance<B>();
        _disposeBuilder = true;
    }
    
    /// <summary>
    /// Construct a new <see cref="InterpolatedTextBuilder{B}"/> that writes to a <typeparamref name="B"/> <paramref name="builder"/>
    /// </summary>
    [MustDisposeResource(false)]
    public InterpolatedTextBuilder(B builder)
    {
        _builder = builder;
    }
    
    /// <summary>
    /// Construct a new <see cref="InterpolatedTextBuilder{B}"/> that writes to a <typeparamref name="B"/> <paramref name="builder"/>
    /// </summary>
    [MustDisposeResource(false)]
    public InterpolatedTextBuilder(int literalLength, int formattedCount, B builder)
    {
        _builder = builder;
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
    public void AppendFormatted<T>(T value) => _builder.Append(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format) => _builder.Format(value, format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment) => _builder.AlignFormat<T>(value, width: alignment);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment, string? format) => _builder.AlignFormat<T>(value, alignment, format);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(Action<B> build)
    {
        // pass-through to builder for execution
        _builder.InterpolatedExecute(build);
    }

    public void Dispose()
    {
        if (_disposeBuilder)
        {
            _builder.Dispose();
        }
    }
    
    public override string ToString() => _builder.ToString();
  
}