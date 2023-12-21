using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Tests.Utilities;

public class ScaryTests
{
    
}

public class ScaryUnmanagedTests
{
    public struct TestUnmanagedStruct
    {
        public int Identifier;

        public TestUnmanagedStruct(int identifier)
        {
            this.Identifier = identifier;
        }
        
        public bool Equals(TestUnmanagedStruct other)
        {
            return other.Identifier == this.Identifier;
        }
        
        public override bool Equals(object? obj)
        {
            return obj is TestUnmanagedStruct testUnmanagedStruct && Equals(testUnmanagedStruct);
        }

        public override int GetHashCode()
        {
            return Identifier;
        }

        public override string ToString()
        {
            return $"#{Identifier}";
        }
    }


    public static IEnumerable<object[]> Data => TestHelper.ToEnumerableObjects<object>(arrayLength: 1,
        byte.MinValue, (int)0, Guid.NewGuid(), DateTime.Now, new TestUnmanagedStruct(147));
    
    
    [Theory]
    [MemberData(nameof(Data))]
    public void CloneWorks<T>(T value)
        where T : unmanaged
    {
        T clone = Scary.Unmanaged.Clone(value);
        Assert.True(EqualityComparer<T>.Default.Equals(value, clone));

        string? valueString = value.ToString();
        string? cloneString = clone.ToString();
        Assert.Equal(valueString, cloneString);
        
        var valueJson = JsonSerializer.Serialize(value, TestHelper.AggressiveSerialization);
        var cloneJson = JsonSerializer.Serialize(clone, TestHelper.AggressiveSerialization);
        Assert.Equal(valueJson, cloneJson);
    }
    
    [Theory]
    [MemberData(nameof(Data))]
    public void ToBytesWorks<T>(T value)
        where T : unmanaged
    {
        var bytes = Scary.Unmanaged.ToBytes(value);
        Assert.True(bytes.Length > 0);
    }
    
}