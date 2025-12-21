namespace ScrubJay.Text;

partial class TextBuilder
{
    private Whitespace? _whitespace;

#region NewLine

    public TextBuilder NewLine()
    {
        if (_whitespace is null)
        {
            return Append(Environment.NewLine);
        }
        else
        {
            _whitespace.WriteFullNewLineTo(this);
            return this;
        }
    }

    public TextBuilder NewLines(int count)
    {
        if (_whitespace is null)
        {
            return Repeat(count, Environment.NewLine);
        }

        for (int i = 0; i < count; i++)
        {
            _whitespace.WriteFullNewLineTo(this);
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

    public TextBuilder Indent(string? indent)
    {
        _whitespace ??= new Whitespace();
        _whitespace.AddIndent(indent);
        return this;
    }

    public TextBuilder Dedent()
    {
        if (_whitespace is null)
            throw Ex.Invalid("There is no Indent to remove");

        _whitespace.RemoveIndent();
        return this;
    }

    public TextBuilder Indented(Action<TextBuilder>? build)
    {
        _whitespace ??= new Whitespace();
        _whitespace.AddIndent();
        Invoke(build);
        _whitespace.RemoveIndent();
        return this;
    }

#endregion

#region Blocks

    public TextBuilder BracesBlock(Action<TextBuilder>? indentedBlock)
    {
        _whitespace ??= new Whitespace();

        if (!_whitespace.IsStartLine(this))
        {
            _whitespace.WriteFullNewLineTo(this);
        }

        Write('{');
        _whitespace.AddIndent();
        _whitespace.WriteFullNewLineTo(this);

        Invoke(indentedBlock);

        _whitespace.RemoveIndent();

        if (_whitespace.IsStartLine(this))
        {
            TryRemoveLast(_whitespace.Indent.Length).ThrowIfError();
        }
        else
        {
            _whitespace.WriteFullNewLineTo(this);
        }

        Write('}');
        _whitespace.WriteFullNewLineTo(this);

        return this;
    }

#endregion
}