namespace ScrubJay.Text;

internal partial class Whitespace
{
    private static string _defaultNewLine = Environment.NewLine;
    private static string _defaultIndent = "    "; // 4 spaces

    [NotNull, AllowNull]
    public static string DefaultNewLine
    {
        get => _defaultNewLine;
        set => _defaultNewLine = value ?? Environment.NewLine;
    }

    [NotNull, AllowNull]
    public static string DefaultIndent
    {
        get => _defaultIndent;
        set => _defaultIndent = value ?? "    "; // 4 spaces
    }
}


/// <summary>
/// Manages whitespace for a <see cref="TextBuilder"/>
/// </summary>
internal sealed partial class Whitespace
{
    private string _newLine = _defaultNewLine;
    private string _indent = _defaultIndent;
    private readonly List<string> _indents = [];

    [NotNull, AllowNull]
    public string NewLine
    {
        get => _newLine;
        set => _newLine = value ?? _defaultNewLine;
    }

    [NotNull, AllowNull]
    public string Indent
    {
        get => _indent;
        set => _indent = value ?? _defaultIndent;
    }

    internal void WriteFullNewLineTo(TextBuilder builder)
    {
        builder.Write(_newLine);
        foreach (var indent in _indents)
        {
            builder.Write(indent);
        }
    }

    internal void WriteFixedTextTo(TextBuilder builder, scoped text text)
    {
        text newline = _newLine;
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

    public void AddIndent() => _indents.Add(_indent);

    public void AddIndent(char indent) => _indents.Add(indent.AsString());

    public void AddIndent(string? indent) => _indents.Add(indent ?? string.Empty);

    public void RemoveIndent()
    {
        if (_indents.Count == 0)
        {
            throw new InvalidOperationException();
        }
        _indents.RemoveAt(_indents.Count - 1);
    }
}