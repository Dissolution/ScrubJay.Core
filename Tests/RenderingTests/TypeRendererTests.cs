using ScrubJay.Rendering;
using ScrubJay.Validation;
using ScrubJay.Validation.Demanding;

namespace ScrubJay.Tests.RenderingTests;

public class TypeRendererTests
{
    public static TypeRenderer TypeRenderer { get; } = new TypeRenderer();

    [Theory]
    [InlineData(typeof(byte), "byte")]
    [InlineData(typeof(sbyte), "sbyte")]
    [InlineData(typeof(short), "short")]
    [InlineData(typeof(ushort), "ushort")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(uint), "uint")]
    [InlineData(typeof(long), "long")]
    [InlineData(typeof(ulong), "ulong")]
    [InlineData(typeof(nint), "nint")]
    [InlineData(typeof(nuint), "nuint")]
    [InlineData(typeof(float), "float")]
    [InlineData(typeof(double), "double")]
    [InlineData(typeof(decimal), "decimal")]
    [InlineData(typeof(bool), "bool")]
    [InlineData(typeof(char), "char")]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(void), "void")]
    public void RenderCommonTypesWorks(Type type, string rendering)
    {
        string r = TypeRenderer.Render(type);
        Demand.That(r).IsEqualTo(rendering);
    }

    [Theory]
    [InlineData(typeof(byte), "byte&")]
    [InlineData(typeof(char), "char&")]
    public void RenderReferenceTypeWorks(Type type, string rendering)
    {
        var byRefType = type.MakeByRefType();
        string r = TypeRenderer.Render(byRefType);
        Demand.That(r).IsEqualTo(rendering);
    }
}