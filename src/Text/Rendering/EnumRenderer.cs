namespace ScrubJay.Text.Rendering;

[PublicAPI]
public class EnumRenderer : Renderer<Enum>
{
    public override bool CanRender(Type type) => type.IsEnum;

    public override TextBuilder RenderValue(TextBuilder builder, Enum? e)
    {
        if (e is null)
            return builder;

        var enumInfo = EnumInfo.For(e.GetType());
        var enumMemberInfo = enumInfo.GetMemberInfo(e);
        if (enumMemberInfo is null)
            return builder.Append(e.ToString());

        return enumMemberInfo.RenderTo(builder);
    }
}

[PublicAPI]
public class EnumRenderer<E> : EnumRenderer, IRenderer<E>
    where E : struct, Enum
{
    public TextBuilder RenderValue(TextBuilder builder, E @enum)
    {
        return EnumInfo.For<E>().GetMemberInfo(@enum)!.RenderTo(builder);
    }
}