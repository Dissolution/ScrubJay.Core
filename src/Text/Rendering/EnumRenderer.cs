namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class EnumRenderer : Renderer<Enum>
{
    public override bool CanRender(Type? type)
    {
        return type is not null && type.IsEnum;
    }

    public override TextBuilder FluentRender(TextBuilder builder, Enum? e)
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