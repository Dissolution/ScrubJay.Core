namespace ScrubJay.Text;

public partial class TextBuilder
{
    public TextBuilder Wrap<W, T>(W? wrapper, T? value)
    {
        return Append<W>(wrapper).Append<T>(value).Append<W>(wrapper);
    }

    public TextBuilder Wrap<W>(W? wrapper, Action<TextBuilder>? center)
    {
        return Append<W>(wrapper).Invoke(center).Append<W>(wrapper);
    }

    public TextBuilder Wrap<T>(Action<TextBuilder>? wrapper, T? value)
    {
        return Invoke(wrapper).Append<T>(value).Invoke(wrapper);
    }

    public TextBuilder Wrap(Action<TextBuilder>? wrapper, Action<TextBuilder>? center)
    {
        return Invoke(wrapper).Invoke(center).Invoke(wrapper);
    }

    public TextBuilder Wrap<W, T>((W Pre, W Post) wrapper, T? value)
    {
        return Append<W>(wrapper.Pre).Append<T>(value).Append<W>(wrapper.Post);
    }

    public TextBuilder Wrap<W>((W Pre, W Post) wrapper, Action<TextBuilder>? center)
    {
        return Append<W>(wrapper.Pre).Invoke(center).Append<W>(wrapper.Post);
    }

    public TextBuilder Wrap<WA, WZ, T>((WA Pre, WZ Post) wrapper, T? value)
    {
        return Append<WA>(wrapper.Pre).Append<T>(value).Append<WZ>(wrapper.Post);
    }

    public TextBuilder Wrap<WA, WZ>((WA Pre, WZ Post) wrapper, Action<TextBuilder>? center)
    {
        return Append<WA>(wrapper.Pre).Invoke(center).Append<WZ>(wrapper.Post);
    }
}