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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScrubJay.Collections.Pooling;
using ScrubJay.Maths;
using ScrubJay.Scratch;
using ScrubJay.Text.Rendering;
using ScrubJay.Text.Scratch;

Console.OutputEncoding = Encoding.UTF8;
var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Logging.AddConsole();
using var host = hostBuilder.Build();

#endregion End Setup

using var builder = new TextBuilder();

#if NET9_0_OR_GREATER
ScratchRenderer.AddRenderer<text>((tb, text) => tb.Append('"').Append(text).Append('"'));

ScratchRenderer.RenderTo(builder, "TRJ".AsSpan());
builder.Write(" - ");
ScratchRenderer.RenderTo(builder, new int[] { 1, 2, 3 }.AsSpan());
builder.Write(" - ");
#endif
ScratchRenderer.RenderTo(builder, "AHAB");
builder.Write(" - ");
ScratchRenderer.RenderTo(builder, 'x');
builder.Write(" - ");
ScratchRenderer.RenderTo(builder, 147.13m);
builder.Write(" - ");
ScratchRenderer.RenderTo<object>(builder, Guid.NewGuid());
builder.Write(" - ");
ScratchRenderer.RenderTo<object>(builder, DateTime.Now);

string str = builder.ToString();
Console.WriteLine(str);



Console.WriteLine("〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️");
Console.WriteLine("Press Enter to quit...");
Debugger.Break();
Console.ReadLine();
return 0;


namespace ScrubJay.Scratch
{
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