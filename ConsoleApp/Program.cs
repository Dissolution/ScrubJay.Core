// ReSharper disable UnusedVariable


//using static ScrubJay.Scratch.Gamma.Result;
//using ScrubJay.Scratch.Gamma;


using System.Diagnostics;
using ScrubJay;

bool? a = true;
bool? b = null;
bool? c = false;


bool ab = (bool)a;
bool bb = (bool)b;
bool cb = (bool)c;

Debugger.Break();

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