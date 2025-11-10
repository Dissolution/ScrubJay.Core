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

try
{
    Util.Check1(null);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    Console.WriteLine(ex.StackTrace);
    Debugger.Break();
}


try
{
    Util.Check2(null);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    Console.WriteLine(ex.StackTrace);
    Debugger.Break();
}



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