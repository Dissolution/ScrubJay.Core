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
using ScrubJay.Scratch;
using ScrubJay.Text.Rendering;
using ScrubJay.Text.Scratch;

using static InlineIL.IL;

Console.OutputEncoding = Encoding.UTF8;
var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Logging.AddConsole();
using var host = hostBuilder.Build();

#endregion End Setup



RS rs = new();

text text = "ABC";

string s_2 = Util.Dump(text);

Util.Accept(new(), $"Hey: {rs:@}");


using var builder = new TextBuilder();

TypeInfo ti = typeof(Guid).GetTypeInfo();

ScratchRenderer.RenderTo<TypeInfo>(builder, ti);

string str = builder.ToString();
Console.WriteLine(str);


Console.WriteLine("〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️");
Console.WriteLine("Press Enter to quit...");
Debugger.Break();
Console.ReadLine();
return 0;


namespace ScrubJay.Scratch
{
    public static partial class Util
    {
        public static void Accept(TextBuilder builder, [InterpolatedStringHandlerArgument(nameof(builder))] ref InterpolatedTextBuilder interpolated)
        {

        }

        public static string Dump<T>(T value)
            where T : allows ref struct
        {
            Emit.Ldarg(nameof(value));
            Emit.Call(MethodRef.Method(TypeRef.Type<T>(), "ToString", typeof(string), 0, []));
            Emit.Ret();
            throw Unreachable();
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