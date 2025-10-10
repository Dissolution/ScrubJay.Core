namespace ScrubJay.Text;

public partial class TextBuilder
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
            return RepeatAppend(count, Environment.NewLine);
        }

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

    public TextBuilder Indent(string? indent)
    {
        _whitespace ??= new Whitespace();
        _whitespace.AddIndent(indent);
        return this;
    }

    public TextBuilder Dedent()
    {
        if (_whitespace is null)
        {
            throw Ex.Invalid();
        }

        _whitespace.RemoveIndent();

        return this;
    }

    public TextBuilder Indented(Action<TextBuilder>? buildText)
    {
        return Indent().Invoke(buildText).Dedent();
    }

#endregion


#region Blocks

    public sealed record class BlockFix
    {
        public required string Text { get; init; }

        //public bool IndentBefore { get; init; } = false;
        public bool NewLineBefore { get; init; } = false;
        //public bool NewLineAfter { get; init; } = false;

        public BlockFix()
        {
        }

        [SetsRequiredMembers]
        public BlockFix(string text)
        {
            Text = text;
        }
    }

    public sealed record class BlockSpec
    {
        // https://en.wikipedia.org/wiki/Indentation_style

        public static BlockSpec Allman { get; } = new BlockSpec()
        {
            Prefix = new()
            {
                Text = "{",
                NewLineBefore = true,
            },
            Indent = "    ",
            Postfix = new()
            {
                Text = "}",
                NewLineBefore = true,
            },
        };

        public static BlockSpec KnR { get; } = new BlockSpec()
        {
            Prefix = new()
            {
                Text = " {",
            },
            Indent = "    ",
            Postfix = new()
            {
                Text = "}",
                NewLineBefore = true,
            },
        };


        public string? Indent { get; init; } = null;
        public BlockFix? Prefix { get; init; } = null;
        public BlockFix? Postfix { get; init; } = null;

        public void Deconstruct(out BlockFix? prefix, out string? indent, out BlockFix? postfix)
        {
            prefix = Prefix;
            indent = Indent;
            postfix = Postfix;
        }
    }


    public TextBuilder Block(BlockSpec? spec, Action<TextBuilder>? buildBlock)
    {
        _whitespace ??= new Whitespace();
        spec ??= BlockSpec.Allman; // c# default
        (BlockFix? prefix, string? indent, BlockFix? postfix) = spec;

        if (prefix is not null)
        {
            // if we want to start on a newline, only if not already
            if (prefix.NewLineBefore && !_whitespace.IsStartLine(this))
            {
                NewLine();
            }

            // write the actual prefix, add the indent and [then a newline]
            Write(prefix.Text);
            Indent(indent);
            NewLine();
        }
        else
        {
            Indent(indent);
        }

        // build the block
        buildBlock?.Invoke(this);

        Dedent();

        if (postfix is not null)
        {
            // if we want to start on a newline, only if not already
            if (postfix.NewLineBefore)
            {
                if (_whitespace.IsStartDedentedLine(this))
                {
                    // do not add a newline, instead remove the indent
                    Debug.Assert(Written.EndsWith(indent));
                    Length -= (indent?.Length ?? 0);
                }
                else if (!_whitespace.IsStartLine(this))
                {
                    NewLine();
                }
            }

            // write the actual prefix, add the indent and [then a newline]
            Write(postfix.Text);
            NewLine();
        }

        return this;
    }

#endregion
}