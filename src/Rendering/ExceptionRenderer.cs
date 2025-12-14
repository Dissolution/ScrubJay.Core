using ScrubJay.Debugging;

namespace ScrubJay.Rendering;

[PublicAPI]
public sealed class ExceptionRenderer : ValueRendererBase<ExceptionRenderer, Exception>
{
    public override void RenderTo(Exception exception, TextBuilder builder)
    {
        HResult hresult = exception.HResult;
        string helpLink = exception.HelpLink ?? hresult.HelpLink.ToString();

        builder
            .Append($"{exception:@T}:")
            .Indented(propBuilder => propBuilder
                .NewLine()
                .AppendLine($"Message: \"{exception.Message}\"")
                .AppendLine($"HResult: {hresult:X}")
                .IfNotEmpty(helpLink, static (tb, hl) => tb.AppendLine($"HelpLink: {hl}"))
                .If(exception.Data, static data => data.Count > 0,
                    static (tb, data) => tb
                        .Append($"Data x{data.Count}")
                        .Indented(t => t
                            .NewLine()
                            .Enumerate(data.OfType<DictionaryEntry>(),
                                static (tb, entry) => tb.AppendLine($"{entry.Key:@}: {entry.Value:@}"))))
                .IfNotEmpty(exception.Source,
                    static (tb, source) => tb.AppendLine($"Source: \"{source}\""))
                .IfNotNull(exception.TargetSite,
                    static (tb, ts) => tb.Append("Target Site: ").Append(ts,'@').NewLine())
                .IfNotEmpty(exception.StackTrace,
                    static (tb, st) => tb
                        .Append("StackTrace:")
                        .Indented(ib => ib.NewLine().Append(st))
                        .NewLine())
                .IfNotNull(exception.InnerException,
                    (tb, inner) => tb
                        .Append("Inner Exception:")
                        .Indented(ib => ib.NewLine().Invoke(inner, RenderTo))
                        .NewLine())
                .If(exception.Is<AggregateException>(),
                    (tb, agg) => tb
                        .Append("Inner Exceptions:")
                        .Indented(ib => ib
                            .Enumerate(agg.InnerExceptions, (ab, ex) => ab.NewLine()
                                .Invoke(ex, RenderTo)))));
    }
}