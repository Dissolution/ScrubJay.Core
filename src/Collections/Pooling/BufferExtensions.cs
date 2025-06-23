namespace ScrubJay.Collections.Pooling;

public static class BufferExtensions
{
    public static string ToStringAndDispose([HandlesResourceDisposal] this ref Buffer<char> buffer)
    {
        string str = buffer.Written.AsString();
        buffer.Dispose();
        return str;
    }
}