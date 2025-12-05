using System.Text;

namespace ScrubJay.Functional.Tests;

public class DemandException<T> : DemandException
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private static string GetMessage(CapturedValue<T> argument, string? info)
    {
        var builder = new StringBuilder();
        argument.AppendTo(builder)
            .Append(" is invalid");

        if (!string.IsNullOrEmpty(info))
        {
            builder.Append(": ")
                .Append(info);
        }

        return builder.ToString();
    }

    public (Type ValueType, string ValueString) Argument { get; init; }

    public DemandException(
        CapturedValue<T> argument,
        string? info = null,
        Exception? innerException = null)
        : base(GetMessage(argument, info), innerException)
    {
        this.Argument = argument.ToTuple();
    }
}