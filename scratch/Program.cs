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

Debugger.Break();

//
// // Example 1: Simple object
// var person = new Person
// {
//     Name = "John Doe",
//     Age = 30,
//     Email = "john@example.com",
//     IsActive = true
// };
//
// Console.WriteLine(Dumper.Dump(person, "Person"));
// Console.WriteLine();
//
// // Example 2: Nested object
// var order = new Order
// {
//     OrderId = 12345,
//     Customer = new Person { Name = "Jane Smith", Age = 28 },
//     Items = new List<string> { "Widget", "Gadget", "Doohickey" },
//     Total = 299.99m
// };
//
// Console.WriteLine(Dumper.Dump(order, "Order"));
// Console.WriteLine();

// Example 3: Exception
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