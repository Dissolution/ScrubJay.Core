namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class EnumRenderer : Renderer<Enum>
{
    public override bool CanRender(Type type)
    {
        return type.IsEnum;
    }

    public override void RenderTo(Enum? e, TextBuilder builder)
    {
        if (e is null) return;

        var enumInfo = EnumInfo.For(e.GetType());
        var enumMemberInfo = enumInfo.GetMemberInfo(e);
        if (enumMemberInfo is null)
        {
            Debugger.Break();
            return;
        }

        enumMemberInfo.RenderTo(builder);
    }
}