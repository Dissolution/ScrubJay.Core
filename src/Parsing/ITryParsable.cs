#pragma warning disable CA1040

namespace ScrubJay.Parsing;

[PublicAPI]
public interface ITrySpanParsable<TSelf> : ITryParsable<TSelf>
#if NET7_0_OR_GREATER
    , ISpanParsable<TSelf>
    , IParsable<TSelf>
#endif
    where TSelf : ITrySpanParsable<TSelf>
{
#if NET7_0_OR_GREATER
    static abstract Result<TSelf, ParseException> TryParse(ReadOnlySpan<char> text, IFormatProvider? provider = null);


    static Result<TSelf, ParseException> ITryParsable<TSelf>.TryParse(string? str, IFormatProvider? provider)
        => TSelf.TryParse(str.AsSpan(), provider);

    static TSelf ISpanParsable<TSelf>.Parse(ReadOnlySpan<char> text, IFormatProvider? provider)
        => TSelf.TryParse(text, provider).OkOrThrow();

    static bool ISpanParsable<TSelf>.TryParse(ReadOnlySpan<char> text, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(text, provider).HasOk(out result);

    static TSelf IParsable<TSelf>.Parse(string? str, IFormatProvider? provider)
        => TSelf.TryParse(str.AsSpan(), provider).OkOrThrow();

    static bool IParsable<TSelf>.TryParse(string? str, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(str.AsSpan(), provider).HasOk(out result);
#endif
}

[PublicAPI]
public interface ITryParsable<TSelf>
#if NET7_0_OR_GREATER
    : IParsable<TSelf>
#endif
    where TSelf : ITryParsable<TSelf>
{
#if NET7_0_OR_GREATER
    static abstract Result<TSelf, ParseException> TryParse(string? str, IFormatProvider? provider = null);


    static TSelf IParsable<TSelf>.Parse(string? str, IFormatProvider? provider)
        => TSelf.TryParse(str, provider).OkOrThrow();

    static bool IParsable<TSelf>.TryParse(string? str, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(str, provider).HasOk(out result);
#endif
}
