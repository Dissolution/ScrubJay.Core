using System.Text;

namespace ScrubJay.Functional.Tests;

public class DemandException : InvalidOperationException
{
    public DemandException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}