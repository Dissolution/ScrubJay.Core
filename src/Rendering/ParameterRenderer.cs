using System.Reflection;

namespace ScrubJay.Rendering;

[PublicAPI]
public sealed class ParameterRenderer : ValueRendererBase<ParameterRenderer, ParameterInfo>
{
    public override void RenderTo(ParameterInfo parameter, TextBuilder builder)
    {
        // type name = default
        builder.Append(parameter.ParameterType, '@')
            .Append(' ')
            .Append(parameter.Name)
            .If(parameter.HasDefaultValue,
                tb => tb.Append(" = ").Append(parameter.DefaultValue, '@'));
    }
}