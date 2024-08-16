namespace ScrubJay;

public static class ExceptionResultExtensions
{
    public static void ThrowIfError<TOk, TException>(this Result<TOk, TException> result)
        where TException : Exception
    {
        if (result.IsError(out var ex))
            throw ex;
    }
    
    public static TOk OkOrThrow<TOk, TException>(this Result<TOk, TException> result)
        where TException : Exception
    {
        if (result.IsOkIncludeError(out var ok, out var error))
            return ok;
        throw error;
    }
}