#pragma warning disable CA1040

namespace ScrubJay.Parsing;

[PublicAPI]
public interface ITryParsable<S>
#if NET7_0_OR_GREATER
    : IParsable<S>
#endif
    where S : ITryParsable<S>
{
#if NET7_0_OR_GREATER
    static abstract Result<S> TryParse(string? str, IFormatProvider? provider = null);


    static S IParsable<S>.Parse(string? str, IFormatProvider? provider)
        => S.TryParse(str, provider).OkOrThrow();

    static bool IParsable<S>.TryParse(string? str, IFormatProvider? provider, [MaybeNullWhen(false)] out S result)
        => S.TryParse(str, provider).IsOk(out result);
#endif
}
