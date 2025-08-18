namespace ScrubJay.Text.Rendering;

[PublicAPI]
public class EnumRenderer : Renderer<Enum>
{
    public override bool CanRender(Type type) => type.IsEnum;

    public override TextBuilder RenderTo(TextBuilder builder, Enum? e)
    {
        if (e is null)
            return builder;

        var enumInfo = EnumInfo.For(e.GetType());
        var enumMemberInfo = enumInfo.GetMemberInfo(e);
        if (enumMemberInfo is null)
        {
            Debugger.Break();
            return builder;
        }

        enumMemberInfo.RenderTo(builder);
        return builder;
    }
}

[PublicAPI]
public class EnumRenderer<E> : EnumRenderer, IRenderer<E>
    where E : struct, Enum
{
    public TextBuilder RenderTo(TextBuilder builder, E @enum)
    {
        return EnumInfo.For<E>().GetMemberInfo(@enum)!.RenderTo(builder);
    }
}