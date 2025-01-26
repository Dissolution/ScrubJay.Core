namespace ScrubJay.Parsing;

[PublicAPI]
public static class Parsable
{
#if NET7_0_OR_GREATER
    public static TParsable Parse<TParsable>(string str, IFormatProvider? provider = default)
        where TParsable : IParsable<TParsable> => TParsable.Parse(str, provider);

    public static TSpanParsable Parse<TSpanParsable>(ReadOnlySpan<char> text, IFormatProvider? provider = default)
        where TSpanParsable : ISpanParsable<TSpanParsable> => TSpanParsable.Parse(text, provider);

    public static bool TryParse<TParsable>(
        [NotNullWhen(true)] string? str,
        [MaybeNullWhen(false)] out TParsable result)
        where TParsable : IParsable<TParsable> => TParsable.TryParse(str, default, out result);

    public static bool TryParse<TSpanParsable>(
        ReadOnlySpan<char> text,
        [MaybeNullWhen(false)] out TSpanParsable result)
        where TSpanParsable : ISpanParsable<TSpanParsable> => TSpanParsable.TryParse(text, default, out result);
#endif
}