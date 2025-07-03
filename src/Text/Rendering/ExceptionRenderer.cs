using System.Reflection;
using ScrubJay.Debugging;

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class ExceptionRenderer : Renderer<Exception>
{
    private static TextBuilder RenderMethod<M>(M? method, TextBuilder builder)
        where M : MethodBase
    {
        if (method is null)
            return builder.Append("null");

        if (method is MethodInfo methodInfo)
        {
            builder.Render(methodInfo.ReturnType)
                .Append(' ');
        }
        builder.Append(method.Name)
            .Append('(')
            .EnumerateAndDelimit(method.GetParameters(),
                static (tb, param) => tb.Render(param.ParameterType),
                ", ")
            .Append(')');

        return builder;
    }

    public override void RenderTo(Exception? exception, TextBuilder builder)
    {
        if (exception is null)
        {
            builder.Append("null");
            return;
        }

        HResult hresult = exception.HResult;
        string helpLink = exception.HelpLink ?? hresult.HelpLink.ToString();

        builder
            .RenderType(exception)
            .Append(':')
            .Indent()
            .NewLine()
            .AppendLine($"Message: {exception.Message}")
            .AppendLine($"HResult: {hresult:X}")
            .IfNotEmpty(helpLink,
                static (tb, hl) => tb
                    .AppendLine($"HelpLink: {hl}"))
            .IfNotEmpty(exception.Data,
                static (tb, data) => tb
                    .Append($"Data x{data.Count}")
                    .Indented(t => t
                        .NewLine()
                        .Enumerate(data.OfType<DictionaryEntry>(),
                            static (tb, entry) => tb.AppendLine($"{entry.Key}: {entry.Value}"))))
            .IfNotEmpty(exception.Source,
                static (tb, source) => tb.AppendLine($"Source: {source}"))
            .IfNotNull(exception.TargetSite,
                (tb, ts) => tb
                    .Append("Target Site: ")
                    .Invoke(b => RenderMethod(ts, b))
                    .NewLine())
            .IfNotEmpty(exception.StackTrace,
                static (tb, st) => tb.AppendLine($"Trace: {st}"))
            .IfNotNull(exception.InnerException,
                (tb, inner) => tb
                    .Append("Inner ")
                    .Indent()
                    .Invoke(b => RenderTo(inner, b))
                    .Dedent())
            .If(exception.Is<AggregateException>(),
                (tb, agg) => tb
                    .Append("Inner Exceptions:")
                    .Indent()
                    .NewLine()
                    .Enumerate(agg.InnerExceptions, (t, ex) => t.Render(ex))
                    .Dedent())
            .Dedent()
            .NewLine();
    }
}