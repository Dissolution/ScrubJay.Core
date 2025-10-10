// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedVariable

using text = System.ReadOnlySpan<char>;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ScrubJay.Collections.Pooling;
using ScrubJay.Maths;
using ScrubJay.Scratch;
using ScrubJay.Text.Rendering;
using ScrubJay.Text.Scratch;

using var builder = new TextBuilder();

var super = new TestSuperClass1(147, "TRJ");

ScratchRenderer.AddRenderer<TestBaseClass>(static (builder, test) => builder.Append($"TestBaseClass({test.Id}, {test.Name})"));
ScratchRenderer.RenderTo(builder, super);

var str = builder.ToString();
Console.WriteLine(str);


//Console.WriteLine(Build($"Type: {output:@T}  Value: {output:@}"));
Console.WriteLine("-------");
Debugger.Break();
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

    public static partial class Util
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