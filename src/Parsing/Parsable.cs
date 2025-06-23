namespace ScrubJay.Parsing;

[PublicAPI]
public static class Parsable
{
#if NET7_0_OR_GREATER
    public static TParsable Parse<TParsable>(string str, IFormatProvider? provider = null)
        where TParsable : IParsable<TParsable> => TParsable.Parse(str, provider);

    public static TSpanParsable Parse<TSpanParsable>(text text, IFormatProvider? provider = null)
        where TSpanParsable : ISpanParsable<TSpanParsable> => TSpanParsable.Parse(text, provider);

    public static bool TryParse<TParsable>(
        [NotNullWhen(true)] string? str,
        [MaybeNullWhen(false)] out TParsable result)
        where TParsable : IParsable<TParsable> => TParsable.TryParse(str, null, out result);

    public static bool TryParse<TSpanParsable>(
        text text,
        [MaybeNullWhen(false)] out TSpanParsable result)
        where TSpanParsable : ISpanParsable<TSpanParsable> => TSpanParsable.TryParse(text, null, out result);
#endif
}