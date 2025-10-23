using ScrubJay.Text.Scratch;

namespace ScrubJay.Text;

public partial class TextBuilder
{
#region Append

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(char ch)
    {
        Write(ch);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(scoped text text)
    {
        Write(text);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(string? str)
    {
        Write(str);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder Append(char[]? chars)
    {
        Write(chars);
        return this;
    }

#pragma warning disable IDE0060, CA2000
    public TextBuilder Append(
        [HandlesResourceDisposal] [InterpolatedStringHandlerArgument("")] // pass this TextBuilder instance in as an argument
        InterpolatedText interpolatedText)
    {
        // as this TextBuilder instance was passed into the InterpolatedTextBuilder's constructor,
        // all the writing has already occurred

        // not needed!
        //interpolatedText.Dispose();

        return this;
    }
#pragma warning restore IDE0060, CA2000

    public TextBuilder Append<T>(T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        string? str = TextHelper.ToString<T>(value);
        return Append(str);
    }

#endregion

#region AppendMany

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TextBuilder AppendMany(params text characters) => Append(characters);

    public TextBuilder AppendMany(char[]? characters) => Append(new text(characters));

    public TextBuilder AppendMany(IEnumerable<char>? characters)
    {
        if (characters is not null)
        {
            foreach (char ch in characters)
            {
                Write(ch);
            }
        }

        return this;
    }

    public TextBuilder AppendMany(IEnumerable<string?>? strings)
    {
        if (strings is not null)
        {
            foreach (string? str in strings)
            {
                Write(str);
            }
        }

        return this;
    }

    public TextBuilder AppendMany<T>(IEnumerable<T>? values)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        if (values is not null)
        {
            foreach (var value in values)
            {
                Write(TextHelper.ToString(value));
            }
        }

        return this;
    }

#endregion

#region AppendLine

    public TextBuilder AppendLine(char ch) => Append(ch).NewLine();

    public TextBuilder AppendLine(params text text) => Append(text).NewLine();

    public TextBuilder AppendLine(string? str) => Append(str).NewLine();

    public TextBuilder AppendLine(char[]? chars) => Append(chars).NewLine();

#pragma warning disable IDE0060, CA2000
    public TextBuilder AppendLine(
        [InterpolatedStringHandlerArgument("")] [HandlesResourceDisposal]
        ref InterpolatedText interpolatedText) => Append( interpolatedText).NewLine();
#pragma warning restore IDE0060, CA2000

#endregion
}