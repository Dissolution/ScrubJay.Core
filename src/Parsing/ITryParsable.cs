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
    static abstract Result<TSelf> TryParse(text text, IFormatProvider? provider = null);


    static Result<TSelf> ITryParsable<TSelf>.TryParse(string? str, IFormatProvider? provider)
        => TSelf.TryParse(str.AsSpan(), provider);

    static TSelf ISpanParsable<TSelf>.Parse(text text, IFormatProvider? provider)
        => TSelf.TryParse(text, provider).OkOrThrow();

    static bool ISpanParsable<TSelf>.TryParse(text text, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(text, provider).IsOk(out result);

    static TSelf IParsable<TSelf>.Parse(string? str, IFormatProvider? provider)
        => TSelf.TryParse(str.AsSpan(), provider).OkOrThrow();

    static bool IParsable<TSelf>.TryParse(string? str, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(str.AsSpan(), provider).IsOk(out result);
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
    static abstract Result<TSelf> TryParse(string? str, IFormatProvider? provider = null);


    static TSelf IParsable<TSelf>.Parse(string? str, IFormatProvider? provider)
        => TSelf.TryParse(str, provider).OkOrThrow();

    static bool IParsable<TSelf>.TryParse(string? str, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result)
        => TSelf.TryParse(str, provider).IsOk(out result);
#endif
}
