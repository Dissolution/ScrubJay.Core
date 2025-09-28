namespace ScrubJay.Text.Rendering;

[PublicAPI]
public class EnumRenderer : Renderer<Enum>
{
    public bool CanRender(Type type) => type.IsEnum;

    public override void RenderValue(TextBuilder builder, Enum? e)
    {
        if (e is null)
            return;

        var enumInfo = EnumInfo.For(e.GetType());
        var enumMemberInfo = enumInfo.GetMemberInfo(e);
        if (enumMemberInfo is null)
        {
            builder.Append(e.ToString());
        }
        else
        {
            enumMemberInfo.RenderTo(builder);
        }
    }
}

[PublicAPI]
public class EnumRenderer<E> : EnumRenderer, IRenderer<E>
    where E : struct, Enum
{
    public void RenderValue(TextBuilder builder, E @enum)
    {
        EnumInfo.For<E>().GetMemberInfo(@enum)!.RenderTo(builder);
    }
}