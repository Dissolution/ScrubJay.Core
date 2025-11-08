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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScrubJay.Collections.Pooling;
using ScrubJay.Debugging;
using ScrubJay.Dumping;
using ScrubJay.Maths;
using ScrubJay.Maths.Ternary;
using ScrubJay.Scratch;
using ScrubJay.Text.Rendering;
using static InlineIL.IL;

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

var a = typeof(SimpleClass.NestedSimpleClass);
var b = typeof(SimpleClass.NestedGenericClass<int>);
var c = typeof(GenericClass<byte>.NestedSimpleClass);
var d = typeof(GenericClass<byte>.NestedGenericClass<int>);

var s = TypeHelper.Display(d);
var s2 = TextBuilder.Build(tb => TypeRenderer.RenderTypeTo(tb, d));

var s3 = TypeHelper.Display(typeof(int?));
var s4 = TypeHelper.Display(typeof((int, string)));
var s5 = TypeHelper.Display(typeof(IList<>).MakeGenericType(typeof(string)).MakeByRefType());
var s6 = TypeHelper.Display(typeof(int****));
var s7 = TypeHelper.Display(typeof(Dictionary<,>));

AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(static ass => ass.GetTypes())
    .Select(static type => (type, type.Display()))
    .Consume(static tuple => Console.WriteLine($"{tuple.type}: {tuple.Item2}"));

Debugger.Break();


try
{
    throw new InvalidOperationException("Something went wrong");
}
catch (InvalidOperationException ex)
{
    var exdump = Dumper.Dump(ex);

    Console.WriteLine(exdump);
    Console.WriteLine();
}


Console.WriteLine("〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️");
Console.WriteLine("Press Enter to quit...");
Debugger.Break();
Console.ReadLine();
return 0;


namespace ScrubJay.Scratch
{
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
            [InterpolatedStringHandlerArgument(nameof(builder))] ref InterpolatedTextBuilder interpolated)
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