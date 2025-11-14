using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using ScrubJay.Utilities;

namespace ScrubJay.Tests.Playground;

[StructLayout(LayoutKind.Explicit, Size = 3)]
public readonly struct RGB
{
    [FieldOffset(2)]
    public readonly byte Red;

    [FieldOffset(1)]
    public readonly byte Green;

    [FieldOffset(0)]
    public readonly byte Blue;

    public RGB(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
}

public class RGBTests
{
    public static TheoryData<RGB> TestColors { get; } = [];

    static RGBTests()
    {
        var rng = RandomNumberGenerator.Create();
#if NETFRAMEWORK
        byte[] buffer = new byte[3];
#else
        Span<byte> buffer = stackalloc byte[3];
#endif

        for (var i = 0; i < 100; i++)
        {
            rng.GetBytes(buffer);
            RGB rgb = new RGB(buffer[0], buffer[1], buffer[2]);
            TestColors.Add(rgb);
        }
    }

    [Theory]
    [MemberData(nameof(TestColors))]
    public void BitShiftWorks(RGB rgb)
    {
        uint argb = (uint)Color.FromArgb(0, rgb.Red, rgb.Green, rgb.Blue).ToArgb();

        uint value = (uint)rgb.Blue | (uint)(rgb.Green << 8) | (uint)(rgb.Red << 16);

        Assert.Equal(argb, value);
    }

    [Theory]
    [MemberData(nameof(TestColors))]
    public void UnsafeAsWorks(ref RGB rgb)
    {
        uint argb = (uint)Color.FromArgb(0, rgb.Red, rgb.Green, rgb.Blue).ToArgb();

        uint value = Unsafe.As<RGB, uint>(ref rgb);

        Assert.Equal(argb, value);
    }

    [Theory]
    [MemberData(nameof(TestColors))]
    public void UnsafeReadWorks(ref RGB rgb)
    {
        uint argb = (uint)Color.FromArgb(0, rgb.Red, rgb.Green, rgb.Blue).ToArgb();

        uint value = Unsafe.ReadUnaligned<uint>(ref Unsafe.As<RGB, byte>(ref rgb));

        Assert.Equal(argb, value);
    }

    [Theory]
    [MemberData(nameof(TestColors))]
    public void NotsafeInAsWorks(in RGB rgb)
    {
        uint argb = (uint)Color.FromArgb(0, rgb.Red, rgb.Green, rgb.Blue).ToArgb();

        uint value = Notsafe.InAsIn<RGB, uint>(in rgb);

        Assert.Equal(argb, value);
    }
}