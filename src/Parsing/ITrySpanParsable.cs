namespace ScrubJay.Parsing;

[PublicAPI]
public interface ITrySpanParsable<S> : ITryParsable<S>
#if NET7_0_OR_GREATER
    , ISpanParsable<S>
    , IParsable<S>
#endif
    where S : ITrySpanParsable<S>
{
#if NET7_0_OR_GREATER
    static abstract Result<S> TryParse(text text, IFormatProvider? provider = null);


    static Result<S> ITryParsable<S>.TryParse(string? str, IFormatProvider? provider)
        => S.TryParse(str.AsSpan(), provider);

    static S ISpanParsable<S>.Parse(text text, IFormatProvider? provider)
        => S.TryParse(text, provider).OkOrThrow();

    static bool ISpanParsable<S>.TryParse(text text, IFormatProvider? provider, [MaybeNullWhen(false)] out S result)
        => S.TryParse(text, provider).IsOk(out result);

    static S IParsable<S>.Parse(string? str, IFormatProvider? provider)
        => S.TryParse(str.AsSpan(), provider).OkOrThrow();

    static bool IParsable<S>.TryParse(string? str, IFormatProvider? provider, [MaybeNullWhen(false)] out S result)
        => S.TryParse(str.AsSpan(), provider).IsOk(out result);
#endif
}