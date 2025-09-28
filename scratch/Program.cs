// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using text = System.ReadOnlySpan<char>;

using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using ScrubJay.Collections.Pooling;
using ScrubJay.Maths;
using ScrubJay.Scratch;
using ScrubJay.Text.Rendering;

object[] objects = [147.13m, DateTime.Now, Guid.NewGuid(), "jkl", null!, BindingFlags.CreateInstance, new Exception()];

foreach (object? obj in objects)
{
    string r = obj.Render();
    Console.WriteLine($"{r}  |  {obj}");
}


Console.WriteLine("-------");
Debugger.Break();
return 0;


namespace ScrubJay.Scratch
{


    public ref struct RS
    {
        private ReadOnlySpan<char> _text;

        public RS()
        {
            _text = default;
        }

        public override string ToString()
        {
            return nameof(RS);
        }
    }

    public class Thing
    {
        public object Object { get; set; }

        public object? ObjectQ { get; set; }
    }

    public static class Util
    {
        public static void Accept<T>(T value)
            where T : allows ref struct
        {
            // var a = value.Equals(default);
            // var b = value.GetHashCode();
            // var c = value.GetType();
            // var d = value.ToString();
            //
            // var f = Any<T>.Equals(value, default);
            // var g = Any<T>.GetHashCode(value);
            // var h = Any<T>.GetType(value);
            // var i = Any<T>.ToString(value);
        }
    }
}