using ScrubJay.Text.Rendering;

namespace ScrubJay.Text;

[PublicAPI]
public readonly ref struct Payload<T>
{
    public static implicit operator Payload<T>(T value)
        => Render(value);

    public static implicit operator Payload<T>((T Value, string? Format) tuple)
        => Format(tuple.Value, tuple.Format);

    public static implicit operator Payload<T>(Tuple<T, string?> tuple)
        => Format(tuple.Item1, tuple.Item2);

    public static implicit operator Payload<T>((T Value, string? Format, IFormatProvider? Provider) tuple)
        => Format(tuple.Value, tuple.Format, tuple.Provider);

    public static implicit operator Payload<T>(Tuple<T, string?, IFormatProvider?> tuple)
        => Format(tuple.Item1, tuple.Item2, tuple.Item3);


    public static Payload<T> Render(T value) => new(value);

    public static Payload<T> Format(T value, string? format = null, IFormatProvider? provider = null) =>
        new(value, format ?? string.Empty, provider);

    internal readonly T _value;
    internal readonly string? _format;
    internal readonly IFormatProvider? _provider;

    private Payload(T value, string? format = null, IFormatProvider? provider = null)
    {
        _value = value;
        _format = format;
        _provider = provider;
    }

    public void Deconstruct(out T value, out string? format, out IFormatProvider? provider)
    {
        value = _value;
        format = _format;
        provider = _provider;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is T value)
            return Equate.Values(_value, value);
        if (obj is ValueTuple<T, string?> valueFormat)
            return Equate.Values(_value, valueFormat.Item1) && _format.Equate(valueFormat.Item2);
        if (obj is ValueTuple<T, string?, IFormatProvider?> valueFormatProvider)
            return Equate.Values(_value, valueFormatProvider.Item1)
                   && _format.Equate(valueFormatProvider.Item2)
                   && Equate.Values(_provider, valueFormatProvider.Item3);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.HashMany(_value, _format, _provider);
    }

    public override string ToString()
    {
        using var builder = TextBuilder.New;
        if (_provider is not null)
        {
            builder.Render(_provider.GetType()).Append('(');
        }

        builder.Append(_value?.ToString());

        if (_format is not null)
        {
            builder.Append(':').Append(_format);
        }

        if (_provider is not null)
        {
            builder.Append(')');
        }

        return builder.ToString();
    }
}

[PublicAPI]
public readonly ref struct Payload
{
    public static implicit operator Payload(in char ch) => New(in ch);
    public static implicit operator Payload(text text) => New(text);
    public static implicit operator Payload(string? str) => New(str);
    public static implicit operator Payload(char[]? chars) => New(chars);

    public static Payload New(in char ch) => new(ch.AsSpan());

    public static Payload New(string? str) => new(str.AsSpan());

    public static Payload New(params char[]? chars) => new(new text(chars));

    public static Payload New(text text) => new(text);

    public static Payload New(Span<char> chars) => new(chars);

    public static Payload New(IEnumerable<char>? characters)
    {
        using var buffer = new Buffer<char>();
        buffer.AddMany(characters);
        return new(buffer.AsSpan());
    }

    public static Payload New(IEnumerable<string?>? strings)
    {
        if (strings is null)
            return default;

        using var buffer = new Buffer<char>();
        foreach (string? str in strings)
        {
            buffer.AddMany(str.AsSpan());
        }

        return new(buffer.AsSpan());
    }

    internal readonly text _text;

    internal Payload(text text)
    {
        _text = text;
    }

    public void Deconstruct(out text text)
    {
        text = _text;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is char ch)
            return _text.Equate(ch);
        if (obj is string str)
            return _text.Equate(str);
        if (obj is char[] chars)
            return _text.Equate(chars);
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.HashMany<char>(_text);
    }

    public override string ToString()
    {
        return _text.AsString();
    }
}