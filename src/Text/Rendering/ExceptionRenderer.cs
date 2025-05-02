using System.Reflection;
using ScrubJay.Debugging;

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class ExceptionRenderer : Renderer<Exception>
{
    private const string INDENT = " - ";

    private static IndentTextBuilder AppendMethodDump<M>(IndentTextBuilder text, M? method)
        where M : MethodBase
    {
        if (method is null)
            return text.Append("null");

        if (method is MethodInfo methodInfo)
        {
            text.Append(methodInfo.ReturnType.NameOf())
                .Append(' ');
        }
        text.Append(method.Name)
            .Append('(')
            .Delimit(", ", method.GetParameters(), (tb, p) => tb.Append(p.ParameterType.NameOf()))
            .Append(')');

        return text;
    }

    public override void RenderTo<B>(Exception? exception, B text)
    {
        if (exception is null)
        {
            text.Append("null");
            return;
        }

        text.Append(exception.GetType().NameOf())
            .Append(':')
            .AsIndentTextBuilder(itb => itb
                .Block(INDENT, exBlock =>
                {
                    HResult hresult = exception.HResult;
                    string helpLink = exception.HelpLink ?? hresult.HelpLink.ToString();

                    exBlock
                        .AppendLine($"Message: {exception.Message}")
                        .AppendLine($"HResult: {hresult:X}")
                        .IfNotEmpty(helpLink,
                            static (tb, hl) => tb
                                .AppendLine($"HelpLink: {hl}"))
                        .IfNotEmpty(exception.Data,
                            static (tb, data) => tb
                                .Append($"Data x{data.Count}")
                                .Block(INDENT, dataBlock => dataBlock
                                    .Enumerate(data.OfType<DictionaryEntry>(),
                                        static (tb, entry) => tb.AppendLine($"{entry.Key}: {entry.Value}"))))
                        .IfNotEmpty(exception.Source,
                            static (tb, source) => tb.AppendLine($"Source: {source}"))
                        .IfNotNull(exception.TargetSite,
                            (tb, ts) => tb
                                .Append("Target Site: ")
                                .Invoke(b => AppendMethodDump(b, ts))
                                .NewLine())
                        .IfNotEmpty(exception.StackTrace,
                            static (tb, st) => tb.AppendLine($"Trace: {st}"))
                        .IfNotNull(exception.InnerException,
                            (tb, inner) => tb
                                .Append("Inner ").AddIndent(INDENT)
                                .Invoke(b => RenderTo(inner, b))
                                .RemoveIndent())
                        .If(exception.Is<AggregateException>(),
                            (tb, agg) => tb
                                .Append("Inner Exceptions:")
                                .Block(INDENT, exceptionsBlock => exceptionsBlock
                                    .Enumerate(agg.InnerExceptions, (t, ex) => t.Render(ex))))
                        ;
                }));
    }
}