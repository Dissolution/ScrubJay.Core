// Exception to Identifiers Require Correct Suffix
#pragma warning disable CA1710
// Only implements IEnumerable for fluent collection initialization support
#pragma warning disable CA1010

// ReSharper disable MergeCastWithTypeCheck

namespace ScrubJay.Text;

[StructLayout(LayoutKind.Auto)]
public ref struct FormatWriter : IEnumerable
{
    private Span<char> _destination;
    private int _position;
    private Option<Exception> _hasFailed;

    public ref char this[Index index] => ref AsSpan()[index];

    public Span<char> this[Range range] => AsSpan()[range];

    private readonly int End
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination.Length;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _position = value.Clamp(0, _destination.Length);
    }

    public Span<char> WrittenSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination[.._position];
    }

    public Span<char> AvailableSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination[_position..];
    }

    public int AvailableCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination.Length - _position;
    }

    public FormatWriter(Span<char> destination)
    {
        _destination = destination;
        _position = 0;
        _hasFailed = None<Exception>();
    }

    public FormatWriter(Span<char> destination, int position)
    {
        if (position < 0 || position >= destination.Length)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must be inside of span");
        _destination = destination;
        _position = position;
        _hasFailed = None<Exception>();
    }

    public bool Add(char ch)
    {
        if (_hasFailed) return false;

        int pos = _position;
        int newPos = pos + 1;
        if (newPos <= End)
        {
            _destination[pos] = ch;
            _position = newPos;
            return true;
        }

        _hasFailed = Some<Exception>(
            new ArgumentException($"Cannot add '{ch}': 1 character is greater than remaining capacity {AvailableCount}",
                nameof(ch)));
        return false;
    }


    public bool Add(scoped text text)
    {
        if (_hasFailed) return false;

        int pos = _position;
        int newPos = pos + text.Length;
        if (newPos <= End)
        {
            Sequence.CopyTo(text, _destination[pos..]);
            _position = newPos;
            return true;
        }

        _hasFailed = Some<Exception>(
            new ArgumentException($"Cannot add \"{text}\": {text.Length} characters is greater than remaining capacity {AvailableCount}",
                nameof(text)));
        return false;
    }

    public bool Add(char[]? chars)
    {
        if (_hasFailed) return false;
        if (chars is null) return true;

        int pos = _position;
        int newPos = pos + chars.Length;
        if (newPos <= End)
        {
            Sequence.CopyTo(chars, _destination[pos..]);
            _position = newPos;
            return true;
        }

        _hasFailed = Some<Exception>(
            new ArgumentException($"Cannot add [{chars.AsString()}]: {chars.Length} characters is greater than remaining capacity {AvailableCount}",
                nameof(chars)));
        return false;
    }

    public bool Add(IEnumerable<char>? characters)
    {
        if (_hasFailed) return false;
        if (characters is null) return true;

        int pos = _position;

        if (characters is IList<char> list)
        {
            int count = list.Count;
            int newPos = pos + count;
            if (newPos <= End)
            {
                for (int i = 0; i < count; i++)
                {
                    _destination[pos++] = list[i];
                }
                Debug.Assert(pos == newPos);
                _position = newPos;
                return true;
            }

            _hasFailed = Some<Exception>(
                new ArgumentException($"Cannot add [{list.AsString()}]: {list.Count} characters is greater than remaining capacity {AvailableCount}",
                    nameof(characters)));
            return false;
        }

        if (characters is ICollection<char> collection)
        {
            int count = collection.Count;
            int newPos = pos + count;
            if (newPos <= End)
            {
                foreach (char ch in characters)
                {
                    _destination[pos++] = ch;
                }
                Debug.Assert(pos == newPos);
                _position = newPos;
                return true;
            }

            _hasFailed = Some<Exception>(
                new ArgumentException($"Cannot add ({collection.AsString()}): {collection.Count} characters is greater than remaining capacity {AvailableCount}",
                    nameof(characters)));
            return false;
        }

        int start = pos;
        foreach (char ch in characters)
        {
            if (pos >= End)
            {
                _hasFailed = Some<Exception>(
                    new ArgumentException($"Cannot add another character from IEnumerable<char>: wrote \"{_destination[start..]}\" before remaining capacity became 0",
                        nameof(characters)));
                return false;
            }
            _destination[pos++] = ch;
        }

        _position = pos;
        return true;
    }

    public bool Add(string? str)
    {
        if (_hasFailed) return false;
        if (str is null) return true;

        int pos = _position;
        int newPos = pos + str.Length;
        if (newPos <= End)
        {
            Sequence.CopyTo(str, _destination[pos..]);
            _position = newPos;
            return true;
        }

        _hasFailed = Some<Exception>(
            new ArgumentException($"Cannot add \"{str}\": {str.Length} characters is greater than remaining capacity {AvailableCount}",
                nameof(str)));
        return false;
    }

    public bool Add<T>(T? value)
    {
        if (_hasFailed) return false;

        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so
            if (value is ISpanFormattable)
            {
                if (!((ISpanFormattable)value).TryFormat(AvailableSpan, out int charsWritten, default, default))
                {
                    _hasFailed = Some<Exception>(
                        new ArgumentException($"Cannot format({value}): Will not fit in remaining capacity {AvailableCount}",
                            nameof(value)));
                    return false;
                }

                _position += charsWritten;
                return true;
            }

            str = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            str = value?.ToString();
        }

        return Add(str);
    }

    public bool Add<T>(T? value,
        string? format,
        IFormatProvider? provider = null)
    {
        if (_hasFailed) return false;

        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so
            if (value is ISpanFormattable)
            {
                if (!((ISpanFormattable)value).TryFormat(AvailableSpan, out int charsWritten, format.AsSpan(), provider))
                {
                    _hasFailed = Some<Exception>(
                        new ArgumentException($"Cannot format({value}, {format}, {provider}): Will not fit in remaining capacity {AvailableCount}",
                            nameof(value)));
                    return false;
                }

                _position += charsWritten;
                return true;
            }

            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        return Add(str);
    }

    public bool Add<T>(T? value,
        text format,
        IFormatProvider? provider = null)
    {
        if (_hasFailed) return false;

        string? str;
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so
            if (value is ISpanFormattable)
            {
                if (!((ISpanFormattable)value).TryFormat(AvailableSpan, out int charsWritten, format, provider))
                {
                    _hasFailed = Some<Exception>(
                        new ArgumentException($"Cannot format({value}, {format}, {provider}): Will not fit in remaining capacity {AvailableCount}",
                            nameof(value)));
                    return false;
                }

                _position += charsWritten;
                return true;
            }

            str = ((IFormattable)value).ToString(format.AsString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        return Add(str);
    }

    public bool Add<T>((T? Value, string? Format) valueTuple)
        => Add<T>(valueTuple.Value, valueTuple.Format);

    public bool Add<T>(Tuple<T?, string?> tuple)
        => Add<T>(tuple.Item1, tuple.Item2);

    public bool Add<T>((T? Value, string? Format, IFormatProvider? Provider) valueTuple)
        => Add<T>(valueTuple.Value, valueTuple.Format, valueTuple.Provider);

    public bool Add<T>(Tuple<T?, string?,IFormatProvider?> tuple)
        => Add<T>(tuple.Item1, tuple.Item2, tuple.Item3);


    public delegate bool FormatWriterAdd(ref FormatWriter writer);

    public bool Add(FormatWriterAdd del)
    {
        return del(ref this);
    }


    public void Clear() => WrittenSpan.Clear();

    public bool TryCopyTo(Span<char> span) => Sequence.TryCopyTo(WrittenSpan, span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => WrittenSpan;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char[] ToArray() => WrittenSpan.ToArray();

    public Result<int, Exception> GetResult() => _hasFailed.HasSome(out var error) ? error : _position;

    public bool GetResult(out int charsWritten)
    {
        if (!_hasFailed)
        {
            charsWritten = _position;
            return true;
        }

        // remove any trace of us
        _destination.Clear();
        charsWritten = 0;
        return false;
    }

    public string GetString()
    {
        if (_hasFailed)
            return string.Empty;
        return _destination[.._position].AsString();
    }

    public override string ToString() => WrittenSpan.AsString();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
