namespace ScrubJay.Text;

/// <summary>
/// Manages whitespace for a <see cref="TextBuilder"/>
/// </summary>
public sealed class Whitespace : IDisposable
{
    private string _defaultNewLine = Environment.NewLine;
    private string _defaultIndent = "    "; // 4 spaces

    private readonly PooledList<char> _whitespace;
    private readonly List<int> _indentOffsets = [];


    [AllowNull]
    public string DefaultNewLine
    {
        get => _defaultNewLine;
        set
        {
            if (value is null)
            {
                _defaultNewLine = Environment.NewLine;
                return;
            }

            int valueLen = value.Length;
            int dnlLen = _defaultNewLine.Length;
            int delta = valueLen - dnlLen;
            if (delta == 0)
            {
                _defaultNewLine = value;
                Notsafe.Text.CopyBlock(value, _whitespace.Written, value.Length);
            }
            else
            {
                _whitespace.TryRemoveMany(..dnlLen);
                _whitespace.TryInsertMany(0, value);
                if (_indentOffsets.Count > 0)
                {
                    _indentOffsets.ForEach((ref int offset) => offset += delta);
                }
            }
        }
    }

    [AllowNull]
    public string DefaultIndent
    {
        get => _defaultIndent;
        set => _defaultIndent = value ?? "    "; // 4 spaces
    }

    public Whitespace()
    {
        _whitespace = new PooledList<char>();
        _whitespace.AddMany(_defaultNewLine);
    }


    internal void WriteFullNewLineTo(TextBuilder builder)
    {
        builder.Write(_whitespace.Written);
    }

    internal void WriteFixedTextTo(TextBuilder builder, scoped text text)
    {
        text newline = _defaultNewLine;
        if (text.Length < newline.Length)
        {
            builder.Write(text);
        }
        else if (text.Equate(newline))
        {
            WriteFullNewLineTo(builder);
        }
        else
        {
            var split = SpanSplitter<char>.Split(text, newline);
            if (!split.MoveNext())
                return;
            // write the first (only?) chunk
            builder.Write(split.Current);

            // do we have more?
            while (split.MoveNext())
            {
                // write the full newline
                WriteFullNewLineTo(builder);
                // write this chunk
                builder.Write(split.Current);
            }
        }
    }

    internal bool IsStartLine(TextBuilder builder)
    {
        if (builder.Length == 0)
            return true;

        var nli = _whitespace.Written;
        if (builder.Length < nli.Length)
            return false;
        return builder.Written.EndsWith(nli);
    }

    internal bool IsStartDedentedLine(TextBuilder builder)
    {
        if (builder.Length == 0)
            return true;

        var nli = _whitespace.Written;
        if (_indentOffsets.Count > 0)
        {
            nli = nli[.._indentOffsets[^1]];
        }

        if (builder.Length < nli.Length)
            return false;
        return builder.Written.EndsWith(nli);
    }

    public void AddIndent() => AddIndent(_defaultIndent);

    public void AddIndent(char indent)
    {
        int offset = _whitespace.Count;
        _whitespace.Add(indent);
        _indentOffsets.Add(offset);
    }

    public void AddIndent(string? indent)
    {
        int offset = _whitespace.Count;
        _whitespace.AddMany(indent);
        _indentOffsets.Add(offset);
    }

    public void RemoveIndent()
    {
        if (_indentOffsets.Count == 0)
            throw Ex.Invalid("There are no indents to remove");

        int offset = _indentOffsets[^1];
        _indentOffsets.RemoveAt(_indentOffsets.Count - 1);
        _whitespace.Count = offset;
    }

    public void Dispose()
    {
        _indentOffsets.Clear();
        _whitespace.Dispose();
    }
}