using InlineIL;
using ScrubJay.Extensions;

#if NETFRAMEWORK || NETSTANDARD
using ScrubJay.Text;
#endif

namespace ScrubJay.Benchmarks;

using static IL;

public class TextCopyBenchmarks
{
    public static IEnumerable<object[]> Args()
    {
        yield return ["", new char[1024]];
        yield return [Environment.NewLine, new char[1024]];
        yield return ["EE0A1999-5F25-411B-AA2C-ACB40D6778C1", new char[1024]];
        yield return [new string('x', 1000), new char[1024]];
    }


    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public void StringCopyTo(string str, char[] charArray)
    {
        str.CopyTo(0, charArray, 0, str.Length);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Args))]
    public void SpanCopyTo(string str, char[] charArray)
    {
        str.AsSpan().CopyTo(charArray);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public unsafe void UnsafeCopyBlockVoid(string str, char[] charArray)
    {
        fixed (char* source = str)
        fixed (char* dest = charArray)
        {
            uint byteCount = (uint)str.Length * sizeof(char);
            Unsafe.CopyBlock(dest, source, byteCount);
        }
    }

    // ReSharper disable EntityNameCapturedOnly.Local
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyBlock(
        in char source,
        ref char destination,
        int count)
        // ReSharper restore EntityNameCapturedOnly.Local
    {
        Emit.Ldarg(nameof(destination));
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(count));
        Emit.Sizeof<char>();
        Emit.Mul();
        Emit.Cpblk();
    }

    // Winner by ~2x faster than baseline
    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public void EmitCopyBlock(string str, char[] charArray)
    {
        CopyBlock(in str.GetPinnableReference(), ref charArray.GetPinnableReference(), str.Length);
    }
}
