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

    public void Dispose()
    {
        _newLineAndIndents.Dispose();
    }
}

public partial class TextBuilder
{
    private Whitespace? _whitespace;

#region NewLine

    public TextBuilder NewLine()
    {
        if (_whitespace is null)
            return Append(Environment.NewLine);

        _whitespace.WriteNewLine(this);
        return this;
    }

    public TextBuilder NewLines(int count)
    {
        if (_whitespace is null)
            return RepeatAppend(count, Environment.NewLine);
        for (int i = 0; i < count; i++)
        {
            NewLine();
        }

        return this;
    }

#endregion

#region Indents

    public TextBuilder Indent()
    {
        _whitespace ??= new Whitespace();
        _whitespace.AddIndent();
        return this;
    }

    public TextBuilder Dedent()
    {
        _whitespace ??= new Whitespace();
        _whitespace.RemoveIndent();
        return this;
    }

    public TextBuilder Indented(Action<TextBuilder>? buildText)
    {
        if (buildText is not null)
        {
            _whitespace ??= new Whitespace();
            _whitespace.AddIndent();
            buildText(this);
            _whitespace.RemoveIndent();
        }

        return this;
    }

#endregion

#region Blocks

    public sealed record class BlockSpec
    {
        // https://en.wikipedia.org/wiki/Indentation_style

        public static BlockSpec Allman { get; } = new BlockSpec()
        {
            Prefix = "{",
            IndentPrefix = false,
            NewLineBeforePrefix = true,
            NewLineAfterPrefix = true,

            Indent = "    ",

            Postfix = "}",
            IndentPostfix = false,
            NewLineBeforePostfix = true,
            NewLineAfterPostfix = true,
        };

        public static BlockSpec KnR { get; } = new BlockSpec()
        {
            Prefix = "{",
            IndentPrefix = false,
            NewLineBeforePrefix = false,
            NewLineAfterPrefix = true,

            Indent = "    ",

            Postfix = "}",
            IndentPostfix = false,
            NewLineBeforePostfix = true,
            NewLineAfterPostfix = true,
        };

        public string? Prefix { get; init; } = null;
        public bool NewLineBeforePrefix { get; init; } = false;
        public bool IndentPrefix { get; init; } = false;
        public bool NewLineAfterPrefix { get; init; } = false;

        public string? Indent { get; init; } = null;

        public string? Postfix { get; init; } = null;
        public bool NewLineBeforePostfix { get; init; } = false;
        public bool IndentPostfix { get; init; } = false;
        public bool NewLineAfterPostfix { get; init; } = false;

        public bool AutodetectPosition { get; init; } = true;
    }

    private bool OnStartOfNewLine()
    {
        if (_position == 0) return true;

        text nl;
        if (_whitespace is null)
        {
#if NETFRAMEWORK || NETSTANDARD2_0
            nl = Environment.NewLine.AsSpan();
#else
            nl = Environment.NewLine;
#endif
        }
        else
        {
            nl = _whitespace.NewLineAndIndents;
        }

        return Written.EndsWith(nl);
    }

    public TextBuilder Block(BlockSpec? spec, Action<TextBuilder>? buildBlock)
    {
        _whitespace ??= new Whitespace();
        spec ??= BlockSpec.Allman; // c# default

        bool onStart = OnStartOfNewLine();

        if (spec.NewLineBeforePrefix)
        {
            // do we add the indent now or later?
            if (spec.IndentPrefix)
            {
                // now
                _whitespace.AddIndent(spec.Indent);
            }

            if (!onStart)
            {
                _whitespace.WriteNewLine(this);
            }

            if (!spec.IndentPrefix)
            {
                // later
                _whitespace.AddIndent(spec.Indent);
            }
        }

        Write(spec.Prefix);

        if (spec.NewLineAfterPrefix)
        {
            _whitespace.WriteNewLine(this);
        }

        buildBlock?.Invoke(this);

        onStart = OnStartOfNewLine();

        if (spec.NewLineBeforePostfix)
        {
            // do we remove the indent now or later?
            if (!spec.IndentPostfix)
            {
                // now
                _whitespace.RemoveIndent();
            }

            if (!onStart)
            {
                _whitespace.WriteNewLine(this);
            }

            if (spec.IndentPostfix)
            {
                // later
                _whitespace.RemoveIndent();
            }
        }

        Write(spec.Postfix);

        if (spec.NewLineAfterPostfix)
        {
            _whitespace.WriteNewLine(this);
        }

        return this;
    }

#endregion
}