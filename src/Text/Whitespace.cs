namespace ScrubJay.Text;

internal class Whitespace : IDisposable
{
    protected string _newLine = Environment.NewLine;
    protected readonly PooledStack<string> _indents = [];

    [AllowNull, NotNull]
    public string NewLine
    {
        get => _newLine;
        set => _newLine = value ?? Environment.NewLine;
    }

    public bool HasIndent => _indents.Count > 0;

    public Whitespace()
    {

    }

    public void AddIndent(string? indent) => _indents.Push(indent ?? "");
    public void RemoveIndent() => _indents.Pop();

    public Result<string, Exception> TryRemoveIndent() => _indents.TryPop();

    public bool NewLineIsOneChar(out char ch)
    {
        if (_newLine.Length == 1)
        {
            ch = _newLine[0];
            return true;
        }
        ch = default;
        return false;
    }

    public bool NewLineIsTwoChars(out char char1, out char char2)
    {
        if (_newLine.Length == 2)
        {
            char1 = _newLine[0];
            char2 = _newLine[1];
            return true;
        }

        char1 = default;
        char2 = default;
        return false;
    }

    public void WriteNewLineAndIndentsTo(PooledList<char> list)
    {
        list.AddMany(_newLine.AsSpan());
        using var e = _indents.GetEnumerator(false);
        while (e.MoveNext())
        {
            list.AddMany(e.Current.AsSpan());
        }
    }

    public string NewLineAndIndents()
    {
        var buffer = new Buffer<char>();
        buffer.Write(_newLine);
        using var e = _indents.GetEnumerator(false);
        while (e.MoveNext())
        {
            buffer.Write(e.Current);
        }
        return buffer.ToStringAndDispose();
    }

    public void Dispose() => _indents.Dispose();
}
