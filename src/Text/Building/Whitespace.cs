namespace ScrubJay.Text;

/// <summary>
/// Manages whitespace for a <see cref="TextBuilder"/>
/// </summary>
public sealed class Whitespace : IDisposable
{
    public static class Defaults
    {
        /// <summary>
        /// Gets or sets the default <see cref="string"/> NewLine that <see cref="Whitespace"/> instances will use.
        /// </summary>
        [AllowNull, NotNull]
        public static string NewLine
        {
            get => field;
            set => field = value ?? Environment.NewLine;
        } = Environment.NewLine;

        /// <summary>
        /// Gets or sets the default <see cref="string"/> Indent that <see cref="Whitespace"/> instances will use.
        /// </summary>
        [AllowNull, NotNull]
        public static string Indent
        {
            get => field;
            set => field = value ?? "    ";
        } = "    "; // 4 spaces
    }


    private readonly PooledList<char> _whitespace;
    private readonly PooledList<int> _indentOffsets;


    [AllowNull]
    public string NewLine
    {
        get => field;
        set => SetNewLine(ref field, value);
    } = Defaults.NewLine;

    [AllowNull]
    public string Indent
    {
        get => field;
        set => field = value ?? "    "; // 4 spaces
    } = Defaults.Indent;

    public Whitespace()
    {
        _whitespace = new PooledList<char>();
        _whitespace.AddMany(NewLine);
        _indentOffsets = new PooledList<int>();
    }

    private void SetNewLine(ref string field, string? newline)
    {
        if (newline is null)
        {
            field = Defaults.NewLine;
            return;
        }

        int oldLength = field.Length;
        int newLength = newline.Length;

        // We need to account for a change in length
        int delta = newLength - oldLength;

        // Smaller newline?
        if (delta < 0)
        {
            // copy the new newline into the space (we know is available)
            TextHelper.Notsafe.CopyBlock(newline, _whitespace.Written, newline.Length);
            // remove the rest of the old newline
            _whitespace.TryRemoveMany(newLength..oldLength);
            // If we have any indents, adjust their offset down by delta
            _indentOffsets.ForEach((ref int offset) => offset += delta); // delta is negative
        }
        // Bigger newline?
        else if (delta > 0)
        {
            // shift written over to make room
            _whitespace.GrowBy(delta);
            _whitespace._array.SelfCopy(oldLength.._whitespace.Count, newLength..);
            TextHelper.Notsafe.CopyBlock(newline, _whitespace.Written, newLength);
            _whitespace._position += delta;
            // If we have any indents, adjust their offset up by delta
            _indentOffsets.ForEach((ref int offset) => offset += delta);
        }
        else
        {
            // If they are the same, we can swap in-place
            TextHelper.Notsafe.CopyBlock(newline, _whitespace.Written, newLength);
        }

        field = newline;
    }


    internal void WriteFullNewLineTo(TextBuilder builder)
    {
        builder.Write(_whitespace.Written);
    }

    internal void ReplaceNewLinesAndWrite(TextBuilder builder, scoped text text)
    {
        text newline = NewLine;
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
            builder.Delimit(
                delimit: tb => WriteFullNewLineTo(tb),
                iterator: text.Split(newline),
                static (tb, segment) => tb.Write(segment.Span));
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

    public void AddIndent(string? indent = null)
    {
        int offset = _whitespace.Count;
        _whitespace.AddMany(indent ?? Indent);
        _indentOffsets.Add(offset);
    }

    public void RemoveIndent()
    {
        if (_indentOffsets.Count == 0)
            throw Ex.Invalid("There are no indents to remove");

        int offset = _indentOffsets[^1];
        _indentOffsets.TryRemoveAt(^1);
        _whitespace.Count = offset;
    }

    public void Dispose()
    {
        _indentOffsets.Dispose();
        _whitespace.Dispose();
    }
}