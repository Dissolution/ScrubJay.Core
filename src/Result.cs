global using Result = ScrubJay.Result<ScrubJay.Ok, System.Exception>;
//global using Result<T> = ScrubJay.Result<T, System.Exception>;

namespace ScrubJay;

public static class ImportResult
{
    public static Ok Ok() => default;
    
    public static Error Error(string? message = null) => new Error(message);
}

