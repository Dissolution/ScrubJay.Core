// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using ScrubJay.Debugging;
using ScrubJay.Extensions;
using ScrubJay.Memory;
using ScrubJay.Randomization.Seeding;
using ScrubJay.Validation;
using ScrubJay.Text;
using ScrubJay.Text.Rendering;
using static InlineIL.IL;

var seed = new PhraseSeed("Hello, how are you?");
var seeds = seed.GetSeeds(4);
var str = seed.ToString();


Debugger.Break();
return;



[StructLayout(LayoutKind.Explicit, Size = 3)]
public readonly struct RGB
{
    [FieldOffset(2)]
    public readonly byte Red;

    [FieldOffset(1)]
    public readonly byte Green;

    [FieldOffset(0)]
    public readonly byte Blue;

    public RGB(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
}





namespace ScrubJay.Scratch
{
    public static class Util_2
    {
        private static Result<double> parse(string input) => Result.TryInvoke(() => double.Parse(input));

        private static Result<double> divide(double x, double y) => Result.TryInvoke(() =>
        {
            if (y == 0)
                throw new DivideByZeroException();
            return x / y;
        });

        public static async Result<double> parseDivideAsync(string a, string b)
        {
            var x = await parse(a);
            var y = await Result.TryInvoke(() => double.Parse(b));
            Console.WriteLine("Successfully parsed inputs");
            return await divide(x, y);
        }
    }
}



/*

static AsyncResult<double> Parse(string input)
{
    try
    {
        double f64 = double.Parse(input);
        return AsyncResult<double>.Success(f64);
    }
    catch (Exception ex)
    {
        return AsyncResult<double>.Fail(ex);
    }
}

static async AsyncResult<double> Add(string a, string b)
{
    var ad = await Parse(a);
    var bd = await Parse(b);
    return ad + bd;
}
*/










/*
object obj = (decimal)147.13m;
// if (obj is char)
// {
//     char ch = (char)obj;
//     Console.WriteLine(ch);
// }
// else
// {
//     Console.WriteLine("IS NOT");
// }

ref char ch = ref Notsafe.TryUnboxRef<char>(obj);
if (Notsafe.IsNullRef(ref ch))
{
    Console.WriteLine("Null ref");
}
else
{
    Console.WriteLine(ch);
}

ReadOnlySpan<char> text = new ReadOnlySpan<char>(in ch);
var str = text.AsString();
Console.WriteLine(str);
*/
/*
ReadOnlySpan<char> left = ['a', 'b', 'c'];
ReadOnlySpan<char> right = "abc".ToCharArray().AsSpan();

var eq = Equate
    .GetEqualityComparer<ReadOnlySpan<char>>()
    .Equals(left, right);
Debugger.Break();


char[] chars = new char[128];
var writer = new TryFormatWriter(chars)
{
    '(',
    (""),
    (147),
    {147, "F1"},
    (147, "F2", (IFormatProvider)CultureInfo.CurrentCulture),
    (ref TryFormatWriter w) => w.Add("^_^"),
};
string str = writer.GetString();
var result = writer.GetResult();

Debugger.Break();



Console.WriteLine(str);

Point pt = new(1, 2);

var property = typeof(Point).GetProperty(nameof(Point.X), BindingFlags.Public | BindingFlags.Instance);

object? x = property!.GetValue((object?)pt);

var getMethod = property.GetMethod;
object? x2 = getMethod!.Invoke((object?)pt, null);

DynamicMethod dyn = default!;
var gen = dyn.GetILGenerator();

gen.Emit(OpCodes.Ldarg_0);
gen.Emit(OpCodes.Call, getMethod);
gen.Emit(OpCodes.Ret);

var getter = dyn.CreateDelegate<Func<Point, int>>();


int x3 = getter(pt);

*/
