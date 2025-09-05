namespace ScrubJay.Text;

internal sealed class Whitespace : IDisposable
{
    private string _newLine = Environment.NewLine;

    public string DefaultNewLine
    {
        get => _newLine;
        set { throw new NotImplementedException(); }
    }

    public string DefaultIndent { get; set; } = "    "; // 4 spaces

    public text NewLineAndIndents => _newLineAndIndents.Written;

    private readonly PooledList<char> _newLineAndIndents;
    private readonly Stack<int> _indentOffsets;

    public Whitespace()
    {
        _indentOffsets = [];
        _newLineAndIndents = [];
        _newLineAndIndents.AddMany(DefaultNewLine.AsSpan());
    }

    internal text DedentNLI()
    {
        text nli = _newLineAndIndents.Written;
        if (_indentOffsets.Count > 0)
        {
            nli = nli[.._indentOffsets.Peek()];
        }

        return nli;
    }

    public void WriteNewLine(TextBuilder builder)
    {
        builder.Append(_newLineAndIndents.Written);
    }

    public void AddIndent(string? indent = null)
    {
        indent ??= DefaultIndent;
        int offset = _newLineAndIndents.Count;
        _newLineAndIndents.AddMany(indent.AsSpan());
        _indentOffsets.Push(offset);
    }

    public void RemoveIndent()
    {
        int offset = _indentOffsets.Pop();
        _newLineAndIndents.Count = offset;
    }

    public void RemoveIndent(out text indent)
    {
        int offset = _indentOffsets.Pop();
        indent = _newLineAndIndents.Written[offset..];
        _newLineAndIndents.Count = offset;
    }

    public void Dispose()
    {
        _newLineAndIndents.Dispose();
    }
}