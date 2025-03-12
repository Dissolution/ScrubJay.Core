#pragma warning disable CA1307, CA1715

namespace ScrubJay.Text;

public sealed class FluentIndentTextBuilder : FluentIndentTextBuilder<FluentIndentTextBuilder>
{
    /// <summary>
    /// Gets a <c>new</c> <see cref="FluentIndentTextBuilder"/> instance
    /// </summary>
    public static FluentIndentTextBuilder New
    {
        [MustDisposeResource]
        get => new();
    }
}

public class FluentIndentTextBuilder<B> : TextBuilderBase<B>
    where B : FluentIndentTextBuilder<B>
{
    private Whitespace _whitespace = new();

    public FluentIndentTextBuilder() : base() { }

    protected internal override void InterpolatedExecute(Action<B> build)
    {
        // Special execution logic
        // We capture our current position as an indent
        // perform the build
        // and then reset our indent

        // If we're on a new line, use default logic
        if (IsOnStartOfNewLine())
        {
            build.Invoke(_builder);
            return;
        }

        text newLine = _whitespace.NewLine.AsSpan();

        var lastNewLineIndex = _text.TryFindIndex(newLine, firstToLast: false);
        // If we don't have one, nothing to capture
        if (!lastNewLineIndex.HasSome(out int i))
        {
            build.Invoke(_builder);
            return;
        }

        //string str = _text.ToString();
        //int dbgIndex = i + newLine.Length;
        //string dbStr = str.Substring(dbgIndex);

        string indent = _text.Slice(i + newLine.Length).ToString();
        // We need to start a new indent from this point
        Whitespace newWhiteSpace = new Whitespace();
        newWhiteSpace.AddIndent(indent);
        Interlocked.Exchange(ref _whitespace, newWhiteSpace);
        build.Invoke(_builder);
        Interlocked.Exchange(ref _whitespace, newWhiteSpace);
#if DEBUG
        var removed = newWhiteSpace.TryRemoveIndent();
        Debug.Assert(removed.HasOk(out string? removedIndent));
        Debug.Assert(TextHelper.Equal(indent, removedIndent));
#else
        // no need to remove indent, will be discarded
#endif
        newWhiteSpace.Dispose();
    }

    public override B NewLine()
    {
        _whitespace.WriteNewLineAndIndentsTo(_text);
        return _builder;
    }

    public override B Append(char ch)
    {
        // special behavior only if we're indented
        if (_whitespace.HasIndent && _whitespace.NewLineIsOneChar(out char nl) && nl == ch)
            return NewLine();
        _text.Add(ch);
        return _builder;
    }

    public override B Append(scoped text text)
    {
        // special behavior only if we're indented
        if (_whitespace.HasIndent)
        {
            if (TextHelper.Equal(text, _whitespace.NewLine))
                return NewLine();
            return _builder.Delimit<B, char>(
                static b => b.NewLine(),
                text.Splitter(_whitespace.NewLine.AsSpan(), SpanSplitterOptions.IgnoreEmpty),
                (b, seg) => b._text.AddMany(seg));
        }

        _text.AddMany(text);
        return _builder;
    }

    /// <summary>
    /// Is this Code at the start of a new line? (accounting for indents)
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnStartOfNewLine() => _text.Count == 0 || _text.Written.EndsWith(_whitespace.NewLineAndIndents());

    /// <summary>
    /// Ensures that this Code is at the start of a new line (accounting for indents)
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public B NewLineIfNeeded()
    {
        return IsOnStartOfNewLine() ? _builder : _builder.NewLine();
    }

    /// <summary>
    /// Adds a new indent to the Code
    /// </summary>
    /// <param name="indent"></param>
    /// <returns></returns>
    /// <remarks>
    /// Any further operations that append a NewLine will also include this indent
    /// </remarks>
    public B AddIndent(string indent)
    {
        _whitespace.AddIndent(indent);
        return _builder;
    }

    /// <summary>
    /// Removes the last Code indent
    /// </summary>
    /// <returns></returns>
    public B RemoveIndent()
    {
        _whitespace.RemoveIndent();
        return _builder;
    }

    public B RemoveIndent(out string indent)
    {
        indent = _whitespace.TryRemoveIndent().OkOrThrow();
        return _builder;
    }

    /// <summary>
    /// Builds some Code in an indent
    /// </summary>
    /// <param name="indent"></param>
    /// <param name="indentedCode"></param>
    /// <returns></returns>
    public B Indented(string indent, Action<B> indentedCode)
    {
        _whitespace.AddIndent(indent);
        indentedCode.Invoke(_builder);
        _whitespace.RemoveIndent();
        return _builder;
    }


    public B Block(Action<B> blockCode)
    {
        // A block starts on a new line
        return NewLineIfNeeded()
            // starting brace
            .Append('{')
            // start a default indent
            .AddIndent("    ")
            // Newline (which will start indented)
            .NewLine()
            // write the block code
            .Invoke(blockCode)
            // If we're on the start of a newline
            .InvokeIf(IsOnStartOfNewLine(),
                // Remove the excess indent, decrement the indent
                b => b.RemoveLast(4).RemoveIndent(),
                // otherwise remove the indent and start a new line
            b => b.RemoveIndent().NewLine())
            // ending brace
            .Append('}')
            // start a new line for other code
            .NewLine();
    }


    public override void Dispose()
    {
        _whitespace.Dispose();
        base.Dispose();
    }
}
