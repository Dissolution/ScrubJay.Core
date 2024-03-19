using static ScrubJay.Option;
using static ScrubJay.Scratch.Result;
using ScrubJay.Scratch;



Result<int, byte> result = Ok(147);
Result<byte, Exception> nope = Error(new Exception());





internal static class Things
{
    public static Result<int, Exception> TryParse(string? str)
    {
        //return 4;
        //return new InvalidOperationException();
        return null!;
    }
}