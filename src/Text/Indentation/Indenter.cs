namespace ScrubJay.Text.Indentation;

[PublicAPI]
public class Indenter : IDisposable
{
    private string _newline = Environment.NewLine;
    private string _whitespace = " ";
    private string _halfIndent = "  ";
    private string _indent = "    ";

    private string _blockPrefix = "{";
    private string _blockPostfix = "}";

    private readonly PooledStack<string> _indents = new();


    [NotNull, AllowNull]
    public string NewLine
    {
        get => _newline;
        set => _newline = value ?? Environment.NewLine;
    }

    [NotNull, AllowNull]
    public string Whitespace
    {
        get => _whitespace;
        set => _whitespace = value ?? " ";
    }

    public string HalfIndent
    {
        get => _halfIndent;
        set => _halfIndent = value ?? "  ";
    }

    public string Indent
    {
        get => _indent;
        set => _indent = value ?? "    ";
    }

    [NotNull, AllowNull]
    public string BlockPrefix
    {
        get => _blockPrefix;
        set => _blockPrefix = value ?? "{";
    }

    [NotNull, AllowNull]
    public string BlockPostfix
    {
        get => _blockPostfix;
        set => _blockPostfix = value ?? "}";
    }

    public bool NewLineBeforePrefix { get; set; }
    public bool NewLineAfterPrefix { get; set; }
    public IndentAmount PrefixIndent { get; set; }

    public bool NewLineBeforePostfix { get; set; }
    public bool NewLineAfterPostfix { get; set; }
    public IndentAmount PostfixIndent { get; set; }

    public Indenter(IndentStyle style)
    {
        switch (style)
        {
            case IndentStyle.Allman:
            {
                NewLineBeforePrefix = true;
                NewLineAfterPrefix = true;
                PrefixIndent = IndentAmount.None;
                NewLineBeforePostfix = true;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.None;
                break;
            }
            case IndentStyle.GNU:
            {
                NewLineBeforePrefix = true;
                NewLineAfterPrefix = true;
                PrefixIndent = IndentAmount.Half;
                NewLineBeforePostfix = true;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.Half;
                break;
            }
            case IndentStyle.Whitesmiths:
            {
                NewLineBeforePrefix = true;
                NewLineAfterPrefix = true;
                PrefixIndent = IndentAmount.Full;
                NewLineBeforePostfix = true;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.Full;
                break;
            }
            case IndentStyle.KnR:
            {
                NewLineBeforePrefix = false;
                NewLineAfterPrefix = true;
                PrefixIndent = IndentAmount.None;
                NewLineBeforePostfix = true;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.None;
                break;
            }
            case IndentStyle.Ratliff:
            {
                NewLineBeforePrefix = false;
                NewLineAfterPrefix = true;
                PrefixIndent = IndentAmount.None;
                NewLineBeforePostfix = true;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.Full;
                break;
            }
            case IndentStyle.Horstmann:
            {
                NewLineBeforePrefix = true;
                NewLineAfterPrefix = false;
                PrefixIndent = IndentAmount.None;
                NewLineBeforePostfix = true;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.None;
                break;
            }
            case IndentStyle.Pico:
            {
                NewLineBeforePrefix = true;
                NewLineAfterPrefix = false;
                PrefixIndent = IndentAmount.None;
                NewLineBeforePostfix = false;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.None;
                break;
            }
            case IndentStyle.Lisp:
            {
                NewLineBeforePrefix = true;
                NewLineAfterPrefix = false;
                PrefixIndent = IndentAmount.Half;
                NewLineBeforePostfix = false;
                NewLineAfterPostfix = true;
                PostfixIndent = IndentAmount.None;
                break;
            }
            default:
                throw InvalidEnumException.New(style);
        }
    }

    private void AppendNewLineAndIndents(TextBuilder builder)
    {
        builder.Append(_newline);
        using var e = _indents.GetEnumerator(false);
        while (e.MoveNext())
        {
            builder.Append(e.Current);
        }
    }

    private Option<int> IsOnTheStartOfANewLine(TextBuilder builder)
    {
        // nothing has been written yet
        if (builder.Length == 0)
            return Some(0);

        // we must have a newline
        if (!builder.TryFindIndex(_newline, firstToLast: false).IsSome(out var index))
            return None();

        var written = builder.Written[index..];
        var nliLen = written.Length;

        // skip the newline
        written = written[_newline.Length..];

        // it must be followed by all our current indents
        using var e = _indents.GetEnumerator(popOrder: false);
        while (e.MoveNext())
        {
            if (!TextHelper.StartsWith(written, e.Current))
                return None();
            written = written[(e.Current!.Length)..];
        }

        // and then nothing else
        if (written.Length == 0)
        {
            return Some(nliLen);
        }

        return None();
    }

    public void AddIndent(string? indent = null)
    {
        indent ??= _indent;
        _indents.Push(indent);
    }

    public void RemoveIndent()
    {
        _indents.Pop();
    }

    public TextBuilder Block(TextBuilder builder, Action<TextBuilder> blockBuild)
    {
        // new line before prefix?
        if (this.NewLineBeforePrefix)
        {
            if (!IsOnTheStartOfANewLine(builder))
            {
                AppendNewLineAndIndents(builder);
            }
        }
        else
        {
            if (IsOnTheStartOfANewLine(builder).IsSome(out var len))
            {
                builder.Length -= len;
                if (builder.Length > 0 && !char.IsWhiteSpace(builder.Written[^1]))
                {
                    builder.Append(_whitespace);
                }
            }
        }

        // prefix indent
        if (PrefixIndent == IndentAmount.Half)
        {
            builder.Append(_halfIndent);
        }
        else if (PrefixIndent == IndentAmount.Full)
        {
            builder.Append(_indent);
        }

        // prefix
        builder.Append(_blockPrefix);
        // start our new indent immediately
        AddIndent();

        if (this.NewLineAfterPrefix)
        {
            AppendNewLineAndIndents(builder);
        }

        // body
        builder.Invoke(blockBuild);

        // remove our indent now
        RemoveIndent();

        // newline before postfix?
        if (this.NewLineBeforePostfix)
        {
            if (!IsOnTheStartOfANewLine(builder))
            {
                AppendNewLineAndIndents(builder);
            }
        }
        else
        {
            if (IsOnTheStartOfANewLine(builder).IsSome(out var len))
            {
                builder.Length -= len;
                if (builder.Length > 0 && !char.IsWhiteSpace(builder.Written[^1]))
                {
                    builder.Append(_whitespace);
                }
            }
        }

        // postfix indent
        if (PostfixIndent == IndentAmount.Half)
        {
            builder.Append(_halfIndent);
        }
        else if (PostfixIndent == IndentAmount.Full)
        {
            builder.Append(_indent);
        }

        // prefix
        builder.Append(_blockPostfix);

        // newline after?
        if (this.NewLineAfterPostfix)
        {
            AppendNewLineAndIndents(builder);
        }

        return builder;
    }


    public void Dispose()
    {
        _indents.Dispose();
    }
}