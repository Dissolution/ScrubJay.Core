namespace scratch;

public class TB : TB<TB>;

public class ITB : ITB<ITB>;

public class ITB<B> : TB<B>, IIndentableTextBuilder<B>
    where B : IIndentableTextBuilder<B>
{
}

public class TB<B> : ITextBuilder<B>
    where B : ITextBuilder<B>
{
    public static B New { get; }

    public static string Build(Action<B>? buildText)
    {
        throw new NotImplementedException();
    }

    public static string Build<S>(S state, Action<B, S>? buildText)
    {
        throw new NotImplementedException();
    }

    public static string Build(ref InterpolatedTextBuilder<B> interpolatedText)
    {
        throw new NotImplementedException();
    }

    public ref char this[Index index] => throw new NotImplementedException();

    public Span<char> this[Range range] => throw new NotImplementedException();

    public int Length { get; }
    public B Self { get; }

    public B NewLine()
    {
        throw new NotImplementedException();
    }

    public B NewLines(int count)
    {
        throw new NotImplementedException();
    }

    public B Append(char ch)
    {
        throw new NotImplementedException();
    }

    public B Append(params ReadOnlySpan<char> text)
    {
        throw new NotImplementedException();
    }

    public B Append(string? str)
    {
        throw new NotImplementedException();
    }

    public B Append(char[]? chars)
    {
        throw new NotImplementedException();
    }

    public B Append(IEnumerable<char>? characters)
    {
        throw new NotImplementedException();
    }

    public B Append(ref InterpolatedTextBuilder<B> interpolatedText)
    {
        throw new NotImplementedException();
    }

    public B Format<T>(T? value)
    {
        throw new NotImplementedException();
    }

    public B Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        throw new NotImplementedException();
    }

    public B Format<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        throw new NotImplementedException();
    }

    public B Render(scoped ReadOnlySpan<char> text)
    {
        throw new NotImplementedException();
    }

    public B Render<T>(T? value)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
    {
        throw new NotImplementedException();
    }

    public B Render<T>(T[]? array)
    {
        throw new NotImplementedException();
    }

    public B Render<T>(scoped ReadOnlySpan<T> span)
    {
        throw new NotImplementedException();
    }

    public B Render<T>(scoped Span<T> span)
    {
        throw new NotImplementedException();
    }

    public IIteratingTextBuilder<B, T> Iterate<T>(IEnumerable<T>? enumerable)
    {
        throw new NotImplementedException();
    }
}

[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder<B>
    where B : ITextBuilder<B>
{
}

[PublicAPI]
public static class TextBuilderExtensions
{
    public static B NewLines<B>(this B builder, int count)
        where B : ITextBuilder<B>
    {
        for (var i = 0; i < count; i++)
        {
            builder.NewLine();
        }

        return builder;
    }

    public static B AppendLine<B>(this B builder, char ch)
        where B : ITextBuilder<B>
        => builder.Append(ch).NewLine();

    public static B AppendLine<B>(this B builder, params text text)
        where B : ITextBuilder<B>
        => builder.Append(text).NewLine();

    public static B AppendLine<B>(this B builder, string? str)
        where B : ITextBuilder<B>
        => builder.Append(str).NewLine();

    public static B AppendLine<B>(this B builder, char[]? chars)
        where B : ITextBuilder<B>
        => builder.Append(chars).NewLine();

    public static B AppendLine<B>(this B builder, IEnumerable<char>? characters)
        where B : ITextBuilder<B>
        => builder.Append(characters).NewLine();

    public static B AppendLine<B>(this B builder,
        [InterpolatedStringHandlerArgument(nameof(builder))]
        ref InterpolatedTextBuilder<B> interpolatedText)
        where B : ITextBuilder<B>
        => builder.Append(ref interpolatedText).NewLine();

    public static B FormatLine<B, T>(this B builder, T? value)
        where B : ITextBuilder<B>
        => builder.Format<T>(value).NewLine();

    public static B FormatLine<B, T>(this B builder, T? value, string? format, IFormatProvider? provider = null)
        where B : ITextBuilder<B>
        => builder.Format<T>(value, format, provider).NewLine();

    public static B FormatLine<B, T>(this B builder, T? value, scoped text format, IFormatProvider? provider = null)
        where B : ITextBuilder<B>
        => builder.Format<T>(value, format, provider).NewLine();


    public static B RenderLine<B>(this B builder, scoped text text)
        where B : ITextBuilder<B>
        => builder.Render(text).NewLine();

    public static B RenderLine<B, T>(this B builder, T? value)
        where B : ITextBuilder<B>
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        => builder.Render<T>(value).NewLine();

    public static B RenderLine<B, T>(this B builder, T[]? array)
        where B : ITextBuilder<B>
        => builder.Render<T>(array).NewLine();

    public static B RenderLine<B, T>(this B builder, scoped ReadOnlySpan<T> span)
        where B : ITextBuilder<B>
        => builder.Render<T>(span).NewLine();

    public static B RenderLine<B, T>(this B builder, scoped Span<T> span)
        where B : ITextBuilder<B>
        => builder.Render<T>(span).NewLine();
}

public interface ITextBuilder<B>
    where B : ITextBuilder<B>
{
#if NET6_0_OR_GREATER
    static abstract B New { get; }

    static abstract string Build(Action<B>? buildText);

    static abstract string Build<S>(S state, Action<B, S>? buildText);

    static abstract string Build(ref InterpolatedTextBuilder<B> interpolatedText);
#endif

    ref char this[Index index] { get; }
    Span<char> this[Range range] { get; }
    int Length { get; }

    B Self { get; }

    B NewLine();
    B NewLines(int count);

    B Append(char ch);
    B Append(params text text);
    B Append(string? str);
    B Append(char[]? chars);
    B Append(IEnumerable<char>? characters);
    B Append([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder<B> interpolatedText);

    B Format<T>(T? value);
    B Format<T>(T? value, string? format, IFormatProvider? provider = null);
    B Format<T>(T? value, scoped text format, IFormatProvider? provider = null);

    B Render(scoped text text);

    B Render<T>(T? value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;

    B Render<T>(T[]? array);
    B Render<T>(scoped ReadOnlySpan<T> span);
    B Render<T>(scoped Span<T> span);

    ITB Indent();

    IIteratingTextBuilder<B, T> Iterate<T>(IEnumerable<T>? enumerable);
}

public interface IIndentableTextBuilder<B> : ITextBuilder<B>
    where B : IIndentableTextBuilder<B>
{
}

public interface IIteratedTextBuilder<B> : ITextBuilder<B>
    where B : ITextBuilder<B>
{
    B Delimited(char delimiter);
    B Delimited(scoped text delimiter);
    B Delimited(string? delimiter);
    B Delimited(Action<B>? delimit);
}

public interface IIteratingTextBuilder<B, T>
    where B : ITextBuilder<B>
{
    IIteratedTextBuilder<B> WithAppend();
    IIteratedTextBuilder<B> WithRender();
    IIteratedTextBuilder<B> WithFormat(string? format = null, IFormatProvider? provider = null);
    IIteratedTextBuilder<B> With(Action<B, T>? perItem);
}

public interface IDelimitingTextBuilder<B, T> : IIteratingTextBuilder<B, T>
    where B : ITextBuilder<B>
{
}