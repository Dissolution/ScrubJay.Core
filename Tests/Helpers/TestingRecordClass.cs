namespace ScrubJay.Tests.Helpers;

public record class TestingRecordClass(int Id, string Name)
{
    public Guid InstanceGuid { get; } = new Guid();
}

public readonly record struct TestingReadonlyRecordStruct(int Id, string Name)
{
    public Guid InstanceGuid { get; } = new Guid();
}