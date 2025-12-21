namespace ScrubJay.Text;


/// <summary>
/// Common <b>T</b>ext<b>B</b>uilder <b>A</b>ctions
/// </summary>
[PublicAPI]
public static class TBA
{
    public static Action<TextBuilder> None => static tb => { };

    public static Action<TextBuilder> NewLine => static tb => tb.NewLine();

    public static Action<TextBuilder> Append(char ch) => tb => tb.Append(ch);

    public static Action<TextBuilder> Append(string? str) => tb => tb.Append(str);

    public static Action<TextBuilder> Append<T>(T value) => tb => tb.Append<T>(value);
}

[PublicAPI]
public static class TBA<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
}


[PublicAPI]
public static class TBAExtensions
{
    extension<T>(TBA<T>)
    {
        public static Action<TextBuilder, T> Append => static (tb, value) => tb.Append<T>(value);

        public static Action<TextBuilder, T> Render => static (tb, value) => tb.Render<T>(value);

        public static Action<TextBuilder, T> Format(string? format, IFormatProvider? provider = null)
        {
            return (tb, value) => tb.Format<T>(value, format, provider);
        }

        public static Action<TextBuilder, T> Format(char format)
        {
            return (tb, value) => tb.Format<T>(value, format);
        }
    }

#if NET9_0_OR_GREATER
    extension<T>(TBA<T>)
        where T : allows ref struct
    {
        public static Action<TextBuilder, T> Format(char format, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        {
            return (tb, value) => tb.Format<T>(value, format);
        }
    }
#endif
}