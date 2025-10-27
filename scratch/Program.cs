#region Setup
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using text = System.ReadOnlySpan<char>;
using TextBuffer = ScrubJay.Collections.Pooling.Buffer<char>;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using InlineIL;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScrubJay.Collections.Pooling;
using ScrubJay.Maths;
using ScrubJay.Maths.Ternary;
using ScrubJay.Scratch;
using ScrubJay.Text.Rendering;
using static InlineIL.IL;

Console.OutputEncoding = Encoding.UTF8;
var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Logging.AddConsole();
using var host = hostBuilder.Build();

#endregion End Setup



using var builder = new TextBuilder();

var opt1 = Util.Accept($"Option 1: {args:@}");
var opt2 = Util.Accept(builder, $"Option 2: {args:@}");
builder.Clear();
builder.Append($"Option 3: {args:@}");
var opt3 = builder.ToString();

var k = new InterpolatedTextBuilder();
k.AppendLiteral("HEY!");
k.AppendFormatted(' ');
k.AppendFormatted($"l{1}st{3}n");

var opt4 = k.ToStringAndDispose();

InterpolatedTextBuilder l = "Five alive";
var opt5 = l.ToStringAndDispose();

Console.WriteLine($"""
    Option 1: {opt1}
    Option 2: {opt2}
    Option 3: {opt3}
    Option 4: {opt4}
    Option 5: {opt5}
    """);

Console.WriteLine("〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️");
Console.WriteLine("Press Enter to quit...");
Debugger.Break();
Console.ReadLine();
return 0;


namespace ScrubJay.Scratch
{
    public static partial class Util
    {
        public static string Accept(ref InterpolatedTextBuilder interpolatedTextBuilder)
        {
            using var builder = new TextBuilder();
            builder.Append(interpolatedTextBuilder);
            return builder.ToString();
        }


        public static string Accept(TextBuilder builder, [InterpolatedStringHandlerArgument(nameof(builder))] ref InterpolatedTextBuilder interpolated)
        {
            return interpolated.ToStringAndDispose();
        }

    }

    public class TestBaseClass
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public TestBaseClass(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class TestSuperClass1 : TestBaseClass
    {
        public TestSuperClass1(int id, string name)
            : base(id * 2, name)
        {

        }
    }


    partial class Util
    {
        public static Nullable<T> ToNullable<T>(T value)
            where T : struct
        {
            return value;
        }
    }

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

}