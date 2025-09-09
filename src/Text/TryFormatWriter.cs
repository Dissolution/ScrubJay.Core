#pragma warning disable CA1010, CA1710

using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

/// <summary>
///
/// </summary>
/// <remarks>
/// Supports <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#collection-initializers">
/// fluent collection initialization</a>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public ref struct TryFormatWriter : IEnumerable
{
    private Span<char> _destination;
    private int _position;
    private Option<Exception> _hasFailed;

    private readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination.Length;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _position = value.Clamp(0, Capacity);
    }

    public Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination[.._position];
    }

    public Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination[_position..];
    }

    public readonly int RemainingCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _destination.Length - _position;
    }

    public TryFormatWriter(Span<char> destination)
    {
        _destination = destination;
        _position = 0;
        _hasFailed = None;
    }

    public TryFormatWriter(Span<char> destination, int position)
    {
        Throw.IfNotBetween(position, 0, destination.Length);
        _destination = destination;
        _position = position;
        _hasFailed = None;
    }

#region Private Methods

    private bool Errored(Exception error)
    {
        Debug.Assert(error is not null);
        Debug.Assert(_hasFailed.IsNone());
        _hasFailed = Some<Exception>(error!);
        return false;
    }


    private bool AddImpl(char ch,
        [CallerMemberName] string? callingMethod = null,
        [CallerArgumentExpression(nameof(ch))] string? charName = null)
    {
        if (_hasFailed)
            return false;

        int pos = _position;
        int newPos = pos + 1;
        if (newPos <= Capacity)
        {
            _destination[pos] = ch;
            _position = newPos;
            return true;
        }

        string message = TextBuilder
            .Build($"Could not {callingMethod} '{ch}': Will not fit in remaining Capacity of {RemainingCount}");
        return Errored(new ArgumentException(message, charName));
    }

    private bool AddImpl(scoped text text,
        [CallerMemberName] string? callingMethod = null,
        [CallerArgumentExpression(nameof(text))]
        string? textName = null)
    {
        if (_hasFailed)
            return false;

        int pos = _position;
        int len = text.Length;
        int newPos = pos + len;
        if (newPos <= Capacity)
        {
            Notsafe.Text.CopyBlock(text, _destination[pos..], len);
            _position = newPos;
            return true;
        }

        string message = TextBuilder
            .Build($"Could not {callingMethod} \"{text}\": Will not fit in remaining Capacity of {RemainingCount}");
        return Errored(new ArgumentException(message, textName));
    }

    private bool AddImpl(string? str,
        [CallerMemberName] string? callingMethod = null,
        [CallerArgumentExpression(nameof(str))]
        string? strName = null)
    {
        if (_hasFailed)
            return false;
        if (str is null)
            return true;
        return AddImpl(str.AsSpan(), callingMethod, strName);
    }

    private bool AddImpl(IEnumerable<char>? characters,
        [CallerMemberName] string? callingMethod = null,
        [CallerArgumentExpression(nameof(characters))]
        string? charactersName = null)
    {
        if (_hasFailed)
            return false;
        if (characters is null)
            return true; // add nothing

        int pos = _position;
        var dest = _destination;

        // Some collection types we can shortcut

        if (characters is IList<char> list)
        {
            int len = list.Count;
            int newPos = pos + len;
            if (newPos <= Capacity)
            {
                for (int i = 0; i < len; i++, pos++)
                {
                    dest[pos] = list[i];
                }

                Debug.Assert(pos == newPos);
                _position = newPos;
                return true;
            }

            string message = TextBuilder.New
                .Append($"Could not {callingMethod} [")
                .FormatMany(list)
                .Append($"]: Will not fit in remaining Capacity of {RemainingCount}")
                .ToStringAndDispose();
            return Errored(new ArgumentException(message, charactersName));
        }

        if (characters is ICollection<char> collection)
        {
            int len = collection.Count;
            int newPos = pos + len;
            if (newPos <= Capacity)
            {
                foreach (char ch in characters)
                {
                    dest[pos++] = ch;
                }

                Debug.Assert(pos == newPos);
                _position = newPos;
                return true;
            }

            string message = TextBuilder.New
                .Append($"Could not {callingMethod} (")
                .FormatMany(collection)
                .Append($"): Will not fit in remaining Capacity of {RemainingCount}")
                .ToStringAndDispose();
            return Errored(new ArgumentException(message, charactersName));
        }

        // slow path
        using var e = characters.GetEnumerator();
        while (e.MoveNext())
        {
            if (!AddImpl(e.Current, callingMethod, charactersName))
            {
                var builder = TextBuilder.New
                    .Append($"Could not {callingMethod} the rest of the enumerable ..")
                    .Append(e.Current);
                while (e.MoveNext())
                    builder.Append(", ").Append(e.Current);
                string message = builder
                    .Append($": Will not fit in remaining Capacity of {RemainingCount}")
                    .ToStringAndDispose();
                // override what AddImpl set with better information
                _hasFailed = Option<Exception>.Some(new ArgumentException(message, charactersName));
                return false;
            }
        }

        // Position has already been updated by AddImpl
        return true;
    }

    private bool AddImpl<T>(T? value,
        string? format,
        IFormatProvider? provider = null,
        [CallerMemberName] string? callingMethod = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (_hasFailed)
            return false;
        if (value is null)
            return true; // add nothing

        // Special Rendering
        if (format == "@")
            return AddImpl(value.Render(), callingMethod, valueName);
        if (format == "@T")
            return AddImpl(value.GetType().Render(), callingMethod, valueName);

        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                if (!((ISpanFormattable)value).TryFormat(Available, out int charsWritten, format.AsSpan(), provider))
                {
                    string message = TextBuilder.New
                        .Append($"Could not {callingMethod} `{value:@}`")
                        .IfNotEmpty(format, static (tb, fmt) => tb.Append($" with format \"{fmt}\""))
                        .IfNotNull(provider, static (tb, prov) => tb.Append($" with a {prov:@T} provider"))
                        .Append($": Will not fit in remaining Capacity of {RemainingCount}")
                        .ToStringAndDispose();
                    return Errored(new ArgumentException(message, valueName));
                }

                _position += charsWritten;
                return true;
            }
#endif

            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value.ToString();
        }

        return AddImpl(str, callingMethod, valueName);
    }

    private bool AddImpl<T>(T? value,
        scoped text format,
        IFormatProvider? provider = null,
        [CallerMemberName] string? callingMethod = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (_hasFailed)
            return false;
        if (value is null)
            return true; // add nothing

        // Special Rendering
        if (format.Equate("@"))
            return AddImpl(value.Render(), callingMethod, valueName);
        if (format.Equate("@T"))
            return AddImpl(value.GetType().Render(), callingMethod, valueName);

        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                if (!((ISpanFormattable)value).TryFormat(Available, out int charsWritten, format, provider))
                {
                    string message = TextBuilder.New
                        .Append($"Could not {callingMethod} `{value:@}`")
                        .IfNotEmpty(format, static (tb, fmt) => tb.Append($" with format \"{fmt}\""))
                        .IfNotNull(provider, static (tb, prov) => tb.Append($" with a {prov:@T} provider"))
                        .Append($": Will not fit in remaining Capacity of {RemainingCount}")
                        .ToStringAndDispose();
                    return Errored(new ArgumentException(message, valueName));
                }

                _position += charsWritten;
                return true;
            }
#endif

            str = ((IFormattable)value).ToString(format.AsString(), provider);
        }
        else
        {
            str = value.ToString();
        }

        return AddImpl(str, callingMethod, valueName);
    }

#endregion

#region Fluent Collection Instantiation Methods

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(char ch) => AddImpl(ch);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(scoped text text) => AddImpl(text);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(string? str) => AddImpl(str);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(char[]? chars) => AddImpl(chars.AsSpan());

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IEnumerable<char>? chars) => AddImpl(chars);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(T? value) => AddImpl<T>(value, default(text));

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(T? value, string? format) => AddImpl<T>(value, format);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(T? value, string? format, IFormatProvider? provider) => AddImpl<T>(value, format, provider);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(T? value, scoped text format) => AddImpl<T>(value, format);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(T? value, scoped text format, IFormatProvider? provider) => AddImpl<T>(value, format, provider);


    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>((T? Value, string? Format) valueTuple)
        => AddImpl<T>(valueTuple.Value, valueTuple.Format);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(Tuple<T?, string?> tuple)
        => AddImpl<T>(tuple.Item1, tuple.Item2);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>((T? Value, string? Format, IFormatProvider? Provider) valueTuple)
        => AddImpl<T>(valueTuple.Value, valueTuple.Format, valueTuple.Provider);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add<T>(Tuple<T?, string?, IFormatProvider?> tuple)
        => AddImpl<T>(tuple.Item1, tuple.Item2, tuple.Item3);

#endregion

    public bool Write(char ch) => AddImpl(ch);

    public bool Write(scoped text text) => AddImpl(text);

    public bool Write(string? str) => AddImpl(str);

    public bool Write(char[]? chars) => AddImpl(chars.AsSpan());

    public bool Write(IEnumerable<char>? chars) => AddImpl(chars);

    public bool Write<T>(T? value) => AddImpl<T>(value, default(text));

    public bool Write<T>(T? value, string? format) => AddImpl<T>(value, format);

    public bool Write<T>(T? value, string? format, IFormatProvider? provider) => AddImpl<T>(value, format, provider);

    public bool Write<T>((T? Value, string? Format) valueTuple)
        => AddImpl<T>(valueTuple.Value, valueTuple.Format);

    public bool Write<T>(Tuple<T?, string?> tuple)
        => AddImpl<T>(tuple.Item1, tuple.Item2);

    public bool Write<T>((T? Value, string? Format, IFormatProvider? Provider) valueTuple)
        => AddImpl<T>(valueTuple.Value, valueTuple.Format, valueTuple.Provider);

    public bool Write<T>(Tuple<T?, string?, IFormatProvider?> tuple)
        => AddImpl<T>(tuple.Item1, tuple.Item2, tuple.Item3);

    public readonly Result<int> Wrote()
    {
        if (!_hasFailed.IsSome(out var error))
        {
            return Ok(_position);
        }

        // remove any trace of us
        _destination.Clear();
        return error;
    }

    public readonly bool Wrote(out int charsWritten)
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

    public void Clear()
    {
        Written.Clear();
        _position = 0;
    }

    public bool TryCopyTo(Span<char> span) => TextHelper.TryCopyTo(Written, span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => Written;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char[] ToArray() => Written.ToArray();

    public readonly IEnumerator GetEnumerator()
    {
        if (_hasFailed.IsSome(out var error))
            return Enumerator.One<Exception>(error);
        return Enumerator.Empty<Exception>();
    }

    public readonly override string ToString()
    {
        if (_hasFailed)
            return string.Empty;
        return _destination[.._position].AsString();
    }
}