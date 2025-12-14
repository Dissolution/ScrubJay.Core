using System.Reflection;

namespace ScrubJay.Rendering;

[PublicAPI]
public sealed class MethodRenderer : ValueRendererBase<MethodRenderer, MethodBase>
{
    public override void RenderTo(MethodBase method, TextBuilder builder)
    {
        if (method is MethodInfo methodInfo)
        {
            builder.Append(methodInfo.ReturnType, '@')
                .Append(' ');
        }

        if (method.IsGenericMethod)
        {
            int i = method.Name.IndexOf('`');
            if (i >= 0)
            {
                builder.Append(method.Name.AsSpan(0, i));
            }
            else
            {
                builder.Append(method.Name);
            }

            builder.Append('<')
                .Delimit(", ", method.GetGenericArguments(), "@")
                .Append('>');
        }

        builder
            .Append('(')
            .Delimit(", ", method.GetParameters(), "@")
            .Append(')');
    }
}