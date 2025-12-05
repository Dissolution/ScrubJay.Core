using System.Text;

namespace ScrubJay.Functional.Tests;


public class DemandException<L, R> : DemandException
#if NET9_0_OR_GREATER
    where L : allows ref struct
    where R : allows ref struct
#endif
{
    private static string GetMessage(
        CapturedValue<L> left,
        CapturedValue<R> right,
        string? info)
    {
        var builder = new StringBuilder();
        left.AppendTo(builder)
            .Append(" and ");
        right.AppendTo(builder)
            .Append(" are invalid");

        if (!string.IsNullOrEmpty(info))
        {
            builder.Append(": ")
                .Append(info);
        }

        return builder.ToString();
    }

    public (Type ValueType, string ValueString) LeftArgument { get; init; }
    public (Type ValueType, string ValueString) RightArgument { get; init; }

    public DemandException(
        CapturedValue<L> left,
        CapturedValue<R> right,

        string? info = null,
        Exception? innerException = null)
        : base(GetMessage(left, right,info), innerException)
    {
        this.LeftArgument = left.ToTuple();
        this.RightArgument = right.ToTuple();
    }
}