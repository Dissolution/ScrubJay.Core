// ReSharper disable UnusedVariable


//using static ScrubJay.Scratch.Gamma.Result;
//using ScrubJay.Scratch.Gamma;

using System.Diagnostics;
using ScrubJay;



Result<int, Exception> result = new InvalidOperationException();


Debugger.Break();



//
//
//
//
// Result<int, byte> result_ib_a = 147;
// Result<int, byte> result_ib_b = (byte)147;
// Result<byte, Exception> nope = new Exception();
// Result<byte, Exception> nope2 = new InvalidOperationException();
// Result<byte, Exception> nope3 = new InvalidOperationException();



/*

int k = 13;
if (k is IFormattable)
{
    IFormattable formattable = (IFormattable)k;
    var str = formattable.ToString("g", default);
}




internal static class Things
{
    public static bool TestIs<TIn, TOut>(this TIn input, [MaybeNullWhen(false)] out TOut output)
    {
        if (input is IFormattable)
        {
            IFormattable formattable = (IFormattable)input;
            var str = formattable.ToString("g", default);
        }
        else if (input is TOut out2)
        {
            output = out2;
            return true;
        }

        output = default;
        return false;
    }
    
    
    public static ScrubJay.Scratch.Gamma.Result<int, Exception> TryParse(string? str)
    {
        return 4;
        return new InvalidOperationException();
        return null!;
    }
}
*/