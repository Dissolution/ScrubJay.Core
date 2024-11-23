using System.Reflection;
using ScrubJay.Utilities;

namespace ScrubJay.Tests.UtilitiesTests;

public class NotsafeTests
{
    [Fact]
    public void CanUnboxToSpanOfOne_Struct()
    {
        object obj;

        // int
        {
            int i = 147;
            obj = (object)i;
            ReadOnlySpan<int> span = Notsafe.AsReadOnlySpan<int>(obj);
            Assert.Equal(1, span.Length);
            Assert.Equal(i, span[0]);
        }
        // guid
        {
            Guid g = Guid.NewGuid();
            obj = (object)g;
            ReadOnlySpan<Guid> span = Notsafe.AsReadOnlySpan<Guid>(obj);
            Assert.Equal(1, span.Length);
            Assert.Equal(g, span[0]);
        }
        // datetime
        {
            DateTime dt = DateTime.Now;
            obj = (object)dt;
            ReadOnlySpan<DateTime> span = Notsafe.AsReadOnlySpan<DateTime>(obj);
            Assert.Equal(1, span.Length);
            Assert.Equal(dt, span[0]);
        }
        // enum
        {
            var e = BindingFlags.Public | BindingFlags.Instance;
            obj = (object)e;
            ReadOnlySpan<BindingFlags> span = Notsafe.AsReadOnlySpan<BindingFlags>(obj);
            Assert.Equal(1, span.Length);
            Assert.Equal(e, span[0]);
        }
    }

    [Fact]
    public void CanUnboxToSpanOfOne_Class()
    {
        object obj;

        // string
        {
            string str = "TJ";
            obj = (object)str;
            ReadOnlySpan<string> span = Notsafe.AsReadOnlySpan<string>(obj);
            Assert.Equal(1, span.Length);
            Assert.Equal(str, span[0]);
        }
        // Stopwatch
        {
            Stopwatch timer = Stopwatch.StartNew();
            obj = (object)timer;
            ReadOnlySpan<Stopwatch> span = Notsafe.AsReadOnlySpan<Stopwatch>(obj);
            Assert.Equal(1, span.Length);
            Assert.Equal(timer, span[0]);
        }
    }

    [Fact]
    public void CanUnboxValueType()
    {
        // int
        {
            int input = 147;
            object boxed = (object)input;
            int output = Notsafe.Unbox<int>(boxed);
            Assert.Equal(input, output);
        }
        // guid
        {
            Guid input = Guid.NewGuid();
            object boxed = (object)input;
            Guid output = Notsafe.Unbox<Guid>(boxed);
            Assert.Equal(input, output);
        }
        // datetime
        {
            DateTime input = DateTime.Now;
            object boxed = (object)input;
            DateTime output = Notsafe.Unbox<DateTime>(boxed);
            Assert.Equal(input, output);
        }
        // enum
        {
            BindingFlags input = BindingFlags.Public | BindingFlags.Instance;
            object boxed = (object)input;
            BindingFlags output = Notsafe.Unbox<BindingFlags>(boxed);
            Assert.Equal(input, output);
        }
        // TestingReadonlyRecordStruct
        {
            TestingReadonlyRecordStruct input = new(147, "TJ");
            object boxed = (object)input;
            TestingReadonlyRecordStruct output = Notsafe.Unbox<TestingReadonlyRecordStruct>(boxed);
            Assert.Equal(input, output);
        }
    }

    [Fact]
    public void CanUnboxClass()
    {
        // string
        {
            string input = "TJ";
            object boxed = (object)input;
            string output = Notsafe.Unbox<string>(boxed);
            Assert.Equal(input, output);
        }
        // Stopwatch
        {
            Stopwatch input = Stopwatch.StartNew();
            object boxed = (object)input;
            Stopwatch output = Notsafe.Unbox<Stopwatch>(boxed);
            Assert.Equal(input, output);
        }
        // TestingRecordClass
        {
            TestingRecordClass input = new TestingRecordClass(147, "TJ");
            object boxed = (object)input;
            TestingRecordClass output = Notsafe.Unbox<TestingRecordClass>(boxed);
            Assert.Equal(input, output);
        }
    }

    [Fact]
    public void CanUnboxRefValueType()
    {
        // int
        {
            int input = 147;
            object boxed = (object)input;
            ref int output = ref Notsafe.UnboxRef<int>(boxed);
            Assert.Equal(input, output);

            int input2 = 13;
            output = input2;
            Assert.Equal(input2, output);
            Assert.Equal(input2, (int)boxed);
        }
        // Guid
        {
            Guid input = Guid.NewGuid();
            object boxed = (object)input;
            ref Guid output = ref Notsafe.UnboxRef<Guid>(boxed);
            Assert.Equal(input, output);

            Guid input2 =  Guid.NewGuid();
            output = input2;
            Assert.Equal(input2, output);
            Assert.Equal(input2, (Guid)boxed);
        }
        // DateTime
        {
            DateTime input = DateTime.Now.AddSeconds(-10);
            object boxed = (object)input;
            ref DateTime output = ref Notsafe.UnboxRef<DateTime>(boxed);
            Assert.Equal(input, output);

            DateTime input2 = DateTime.Now.AddSeconds(10);
            output = input2;
            Assert.Equal(input2, output);
            Assert.Equal(input2, (DateTime)boxed);
        }
        // BindingFlags
        {
            BindingFlags input = BindingFlags.Public | BindingFlags.Instance;
            object boxed = (object)input;
            ref BindingFlags output = ref Notsafe.UnboxRef<BindingFlags>(boxed);
            Assert.Equal(input, output);

            BindingFlags input2 = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase;
            output = input2;
            Assert.Equal(input2, output);
            Assert.Equal(input2, (BindingFlags)boxed);
        }
        // TestingReadonlyRecordStruct
        {
            TestingReadonlyRecordStruct input = new(147, "TJ");
            object boxed = (object)input;
            ref TestingReadonlyRecordStruct output = ref Notsafe.UnboxRef<TestingReadonlyRecordStruct>(boxed);
            Assert.Equal(input, output);

            TestingReadonlyRecordStruct input2 = new(13, "MJ");
            output = input2;
            Assert.Equal(input2, output);
            Assert.Equal(input2, (TestingReadonlyRecordStruct)boxed);
        }
    }
}