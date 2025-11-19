#region Setup

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using text = System.ReadOnlySpan<char>;
using TextBuffer = ScrubJay.Collections.Pooling.Buffer<char>;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using InlineIL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScrubJay.Collections.NonGeneric;
using ScrubJay.Collections.Pooling;
using ScrubJay.Debugging;
using ScrubJay.Dumping;
using ScrubJay.Maths;
using ScrubJay.Maths.Ternary;
using ScrubJay.Rendering;
using ScrubJay.Scratch;

using static InlineIL.IL;
using Renderer = ScrubJay.Rendering.Renderer;
using TypeRenderer = ScrubJay.Rendering.TypeRenderer;

Console.OutputEncoding = Encoding.UTF8;
var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Logging.AddConsole();
using var host = hostBuilder.Build();
using var watcher = new UnhandledEventWatcher();
watcher.UnhandledException += (sender, args) =>
{
    var logger = host.Services.GetService<ILogger<UnhandledEventWatcher>>();
    logger.LogError("Unhandled Exception - sender: {@sender} args: {@args}", sender, args);
};

#endregion End Setup


using var builder = new TextBuilder();

var allMethods =
    AppDomain.CurrentDomain
        .GetAssemblies()
        .SelectMany(static ass => ass.GetTypes())
        .SelectMany(static t =>
            t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
        .ToHashSet();

var renderer = new MethodRenderer();
foreach (var method in allMethods)
{
    builder.Clear();
    renderer.RenderTo(method, builder);
    string? typeString = method.ToString();
    string display = builder.ToString();

    Console.WriteLine(display);
    // using var tx = new TextBuilder();
    // tx.Write(typeString);
    // tx.Written.Replace('+', '.');
    // tx.Written.Replace('[', '<');
    // tx.Written.Replace(']', '>');
    //
    // var txStr = tx.ToString();
    // if (txStr != display)
    // {
    //     Debugger.Break();
    // }
}

Debugger.Break();

Pair<int, string> pair = new(147, "TRJ");
builder.Write("Pair<int, string> (IRenderable):  ");
Renderer.RenderTo(pair, builder);
builder.NewLine();
Debugger.Break();



Dictionary<int, string> dict = new()
{
    { 4, "Four" },
    { 5, "Five" },
};
builder.Write("Dictionary<int,string>:  ");
Renderer.RenderTo(dict, builder);
builder.NewLine();
Debugger.Break();



Array arr = Array.CreateInstance(typeof(string), 3);
arr.SetValue("Zero", 0);
arr.SetValue("One", 1);
arr.SetValue("Two", 2);

var mda = new int[2, 2, 2];
Array.Clear(mda);
foreach (var i in ArrayIndicesEnumerator.For(mda))
{
    mda.SetValue(Random.Shared.Next(), i);
}

builder.Write("3d Array:  ");
Renderer.RenderTo(mda, builder);
builder.NewLine();


builder.Write("Object:  ");
object obj = Guid.NewGuid();
Renderer.RenderTo(obj, builder);
builder.NewLine();

char[] array = ['T', 'R', 'J'];


Renderer.RenderArrayTo(arr, builder);
builder.Write("  | ");
Renderer.RenderTo(array, builder);
builder.Write("  | ");
Renderer.RenderTo(array.AsSpan(), builder);
builder.Write(" | ");
Renderer.RenderTo(BindingFlags.Public, builder);
builder.Write("  | ");

string output = builder.ToString();
Console.WriteLine(output);
Debugger.Break();


Console.WriteLine("〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️");
Console.WriteLine("Press Enter to quit...");
Debugger.Break();
Console.ReadLine();
return 0;


namespace ScrubJay.Scratch
{
    partial class Util
    {
        public static int Check1(object? obj)
        {
            switch (obj)
            {
                case null:
                    ThrowNullException();
                    return 0;
                case string str:
                    return int.Parse(str);
                default:
                    throw new Exception();
            }
        }

        public static int Check2(object? obj)
        {
            return obj switch
            {
                null => throw GetNullReferenceException(),
                string str => int.Parse(str),
                _ => throw new Exception()
            };
        }

        [DoesNotReturn]
        public static void ThrowNullException()
        {
            throw new NullReferenceException();
        }

        public static NullReferenceException GetNullReferenceException()
        {
            return new NullReferenceException();
        }
    }

    public class SimpleClass
    {
        public class NestedSimpleClass
        {
        }

        public class NestedGenericClass<T>
        {
        }
    }

    public class GenericClass<T>
    {
        public class NestedSimpleClass
        {
        }

        public class NestedGenericClass<U>
        {
            public class FurtherNestedTuple<W, X, Y> : ITuple
            {
                public W Dubya { get; }
                public X Exx { get; }
                public Y Why { get; }

                object? ITuple.this[int index]
                    => index switch
                    {
                        0 => Dubya,
                        1 => Exx,
                        2 => Why,
                        _ => throw Ex.Argument(index),
                    };

                int ITuple.Length => 3;
            }
        }
    }

    public static partial class Util
    {
        public static string Accept(ref InterpolatedTextBuilder interpolatedTextBuilder)
        {
            using var builder = new TextBuilder();
            builder.Append(ref interpolatedTextBuilder);
            return builder.ToString();
        }


        public static string Accept(TextBuilder builder,
            [InterpolatedStringHandlerArgument(nameof(builder))]
            ref InterpolatedTextBuilder interpolated)
        {
            return interpolated.ToStringAndClear();
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

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public Person Customer { get; set; }
        public List<string> Items { get; set; }
        public decimal Total { get; set; }
    }
}