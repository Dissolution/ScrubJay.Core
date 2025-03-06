#pragma warning disable CA1002

namespace ScrubJay.Text.Builders;

public sealed class Whitespace : IDisposable
{
    private static readonly string _defaultNewLine = Environment.NewLine;

    private string _newLine = _defaultNewLine;
    private int _newLineLength = _defaultNewLine.Length;

    private readonly PooledList<char> _whitespace = new();
    private readonly Stack<int> _indentIndices = [];

    internal text NewLineSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _newLine.AsSpan();
    }

    [AllowNull, NotNull]
    public string NewLine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _newLine;
        set
        {
            var nl = value ?? _defaultNewLine;
            if (_newLine != nl)
            {
                _whitespace.TryRemoveMany(.._newLine.Length);
                _whitespace.TryInsertMany(0, nl.AsSpan());
                _newLine = nl;
                _newLineLength = nl.Length;
            }
        }
    }

    public Option<char> NewLineChar
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _newLineLength == 1 ? Some(_newLine[0]) : None();
    }

    public Option<string> NewLineTwoChars
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _newLineLength == 2 ? Some(_newLine) : None();
    }

    public text NewLineAndIndents
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _whitespace.Written;
    }

    public text Indents
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _whitespace.Written.Slice(_newLineLength);
    }

    public bool HasIndent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _indentIndices.Count > 0;
    }

    public Whitespace()
    {
        _whitespace.AddMany(_newLine.AsSpan());
    }

    public void AddIndent(string? indent = null)
    {
        text dent = indent.AsSpan();
        _indentIndices.Push(_whitespace.Count);
        _whitespace.AddMany(dent);
    }

    public void RemoveIndent()
    {
        if (!_indentIndices.TryPop(out var indentStart))
            throw new InvalidOperationException("There are no indents to remove");
        _whitespace.Count = indentStart; // easy remove
    }

    public Option<string> TryRemoveIndent()
    {
        if (!_indentIndices.TryPop(out var indentStart))
            return None();
        var str = _whitespace[indentStart..].ToString();
        _whitespace.Count = indentStart;
        return Some(str);
    }

    public void Dispose()
    {
        _indentIndices.Clear();
        _whitespace.Dispose();
    }
}
