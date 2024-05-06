namespace ScrubJay;

public static class ResultExtensions
{
    public static void ThrowIfError<TOk, TError>(this Result<TOk, TError> result)
        where TError : Exception
    {
        if (result.IsError(out var ex))
            throw ex;
    }
    
    public static TOk OkOrThrow<TOk, TError>(this Result<TOk, TError> result)
        where TError : Exception
    {
        if (result.IsOkIncludeError(out var ok, out var error))
            return ok;
        throw error;
    }
}