#pragma warning disable

using System.CodeDom.Compiler;
using System.Reflection;
using NotImplementedException = System.NotImplementedException;

namespace ScrubJay.Debugging;

[PublicAPI]
public static class ExDump
{
    private const string INDENT = " - ";

    private static IndentTextBuilder AppendMethodDump<M>(this IndentTextBuilder text, M? method)
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

    private static IndentTextBuilder AppendExceptionDump<X>(this IndentTextBuilder text, X? exception)
        where X : Exception
    {
        if (exception is null)
            return text.Append("null");

        text.Append(exception.GetType().NameOf())
            .Append(':')
            .Block(INDENT, exBlock =>
            {
                HResult hresult = exception.HResult;
                string helpLink = exception.HelpLink ?? hresult.HelpLink.ToString();

                exBlock
                    .AppendLine($"Message: {exception.Message}")
                    .AppendLine($"HResult: {hresult:X}")
                    .If(Validate.IsNotEmpty(helpLink),
                        static (tb, hl) => tb
                            .AppendLine($"HelpLink: {hl}"))
                    .If(Validate.IsNotEmpty(exception.Data),
                        static (tb, data) => tb
                            .Append($"Data x{data.Count}")
                            .Block(INDENT, dataBlock => dataBlock
                                .Enumerate(data.OfType<DictionaryEntry>(),
                                    static (tb, entry) => tb.AppendLine($"{entry.Key}: {entry.Value}"))))
                    .If(Validate.IsNotEmpty(exception.Source),
                        static (tb, source) => tb.AppendLine($"Source: {source}"))
                    .If(Validate.IsNotNull(exception.TargetSite),
                        (tb, ts) => tb
                            .Append("Target Site: ")
                            .AppendMethodDump(ts)
                            .NewLine())
                    .If(Validate.IsNotEmpty(exception.StackTrace),
                        static (tb, st) => tb.AppendLine($"Trace: {st}"))
                    .If(Validate.IsNotNull(exception.InnerException),
                        (tb, inner) => tb
                            .Append("Inner ").AddIndent(INDENT)
                            .AppendExceptionDump(inner)
                            .RemoveIndent())
                    .If(exception.As<AggregateException>(),
                        (tb, agg) => tb
                            .Append("Inner Exceptions:")
                            .Block(INDENT, exceptionsBlock => exceptionsBlock
                                .Enumerate(agg.InnerExceptions, (t, ex) => t.AppendExceptionDump(ex))))
                    ;
            });

        // string str = text.ToString();
        // Dbg.Break();
        return text;
    }


    public static string Dump<X>(this X? exception)
        where X : Exception
    {
        using var text = new IndentTextBuilder();
        AppendExceptionDump<X>(text, exception);
        return text.ToString();
    }
}
