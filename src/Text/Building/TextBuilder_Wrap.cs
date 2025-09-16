namespace ScrubJay.Text;

public partial class TextBuilder
{

#region single

    public TextBuilder Wrap<T>(char wrap, T? value)
        => Append(wrap).Add<T>(value).Append(wrap);

    public TextBuilder Wrap(char wrapChar, Action<TextBuilder>? buildText)
        => Append(wrapChar).Invoke(buildText).Append(wrapChar);

    public TextBuilder WrapAppend(char wrapChar, char ch)
        => Append(wrapChar).Append(ch).Append(wrapChar);

    public TextBuilder WrapAppend(char wrapChar, scoped text text)
        => Append(wrapChar).Append(text).Append(wrapChar);

    public TextBuilder WrapAppend(char wrapChar, string? str)
        => Append(wrapChar).Append(str).Append(wrapChar);

    public TextBuilder WrapFormat<T>(char wrapChar, T? value, string? format = null, IFormatProvider? provider = null)
        => Append(wrapChar).Format(value, format, provider).Append(wrapChar);


    public TextBuilder Wrap(scoped text wrapText, Action<TextBuilder>? buildText)
        => Append(wrapText).Invoke(buildText).Append(wrapText);

    public TextBuilder WrapAppend(scoped text wrapText, char ch)
        => Append(wrapText).Append(ch).Append(wrapText);

    public TextBuilder WrapAppend(scoped text wrapText, scoped text text)
        => Append(wrapText).Append(text).Append(wrapText);

    public TextBuilder WrapAppend(scoped text wrapText, string? str)
        => Append(wrapText).Append(str).Append(wrapText);

    public TextBuilder WrapFormat<T>(scoped text wrapText, T? value, string? format = null, IFormatProvider? provider = null)
        => Append(wrapText).Format(value, format, provider).Append(wrapText);


    public TextBuilder Wrap(string? wrapString, Action<TextBuilder>? buildText)
        => Append(wrapString).Invoke(buildText).Append(wrapString);

    public TextBuilder WrapAppend(string? wrapString, char ch)
        => Append(wrapString).Append(ch).Append(wrapString);

    public TextBuilder WrapAppend(string? wrapString, scoped text text)
        => Append(wrapString).Append(text).Append(wrapString);

    public TextBuilder WrapAppend(string? wrapString, string? str)
        => Append(wrapString).Append(str).Append(wrapString);

    public TextBuilder WrapFormat<T>(string? wrapString, T? value, string? format = null, IFormatProvider? provider = null)
        => Append(wrapString).Format(value, format, provider).Append(wrapString);

    public TextBuilder Wrap(Action<TextBuilder>? wrapBuild, Action<TextBuilder>? buildText)
        => Invoke(wrapBuild).Invoke(buildText).Invoke(wrapBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? wrapBuild, char ch)
        => Invoke(wrapBuild).Append(ch).Invoke(wrapBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? wrapBuild, scoped text text)
        => Invoke(wrapBuild).Append(text).Invoke(wrapBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? wrapBuild, string? str)
        => Invoke(wrapBuild).Append(str).Invoke(wrapBuild);

    public TextBuilder WrapFormat<T>(Action<TextBuilder>? wrapBuild, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Invoke(wrapBuild).Format(value, format, provider).Invoke(wrapBuild);

#endregion

#region double

    public TextBuilder WrapAppend(char preChar, char postChar, char ch)
        => Append(preChar).Append(ch).Append(postChar);

    public TextBuilder WrapAppend(char preChar, char postChar, scoped text text)
        => Append(preChar).Append(text).Append(postChar);

    public TextBuilder WrapAppend(char preChar, char postChar, string? str)
        => Append(preChar).Append(str).Append(postChar);

    public TextBuilder WrapFormat<T>(char preChar, char postChar, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Append(preChar).Format(value, format, provider).Append(postChar);

    public TextBuilder Wrap(char preChar, char postChar, Action<TextBuilder>? buildText)
        => Append(preChar).Invoke(buildText).Append(postChar);


    public TextBuilder WrapAppend(scoped text preText, scoped text postText, char ch)
        => Append(preText).Append(ch).Append(postText);

    public TextBuilder WrapAppend(scoped text preText, scoped text postText, scoped text text)
        => Append(preText).Append(text).Append(postText);

    public TextBuilder WrapAppend(scoped text preText, scoped text postText, string? str)
        => Append(preText).Append(str).Append(postText);

    public TextBuilder WrapFormat<T>(scoped text preText, scoped text postText, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Append(preText).Format(value, format, provider).Append(postText);

    public TextBuilder Wrap(scoped text preText, scoped text postText, Action<TextBuilder>? buildText)
        => Append(preText).Invoke(buildText).Append(postText);

    public TextBuilder WrapAppend(string? preString, string? postString, char ch)
        => Append(preString).Append(ch).Append(postString);

    public TextBuilder WrapAppend(string? preString, string? postString, scoped text text)
        => Append(preString).Append(text).Append(postString);

    public TextBuilder WrapAppend(string? preString, string? postString, string? str)
        => Append(preString).Append(str).Append(postString);

    public TextBuilder WrapFormat<T>(string? preString, string? postString, T? value, string? format = null,
        IFormatProvider? provider = null)
        => Append(preString).Format(value, format, provider).Append(postString);

    public TextBuilder Wrap(string? preString, string? postString, Action<TextBuilder>? buildText)
        => Append(preString).Invoke(buildText).Append(postString);

    public TextBuilder WrapAppend(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, char ch)
        => Invoke(preBuild).Append(ch).Invoke(postBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, scoped text text)
        => Invoke(preBuild).Append(text).Invoke(postBuild);

    public TextBuilder WrapAppend(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, string? str)
        => Invoke(preBuild).Append(str).Invoke(postBuild);

    public TextBuilder WrapFormat<T>(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, T? value,
        string? format = null,
        IFormatProvider? provider = null)
        => Invoke(preBuild).Format(value, format, provider).Invoke(postBuild);

    public TextBuilder Wrap(Action<TextBuilder>? preBuild, Action<TextBuilder>? postBuild, Action<TextBuilder>? buildText)
        => Invoke(preBuild).Invoke(buildText).Invoke(postBuild);

#endregion


}