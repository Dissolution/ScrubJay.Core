namespace ScrubJay.Tests.Helpers;

public record class TestingRecordClass(int Id, string Name)
{
    public Guid InstanceGuid { get; } = Guid.NewGuid();
}

public readonly record struct TestingReadonlyRecordStruct(int Id, string Name)
{
    public Guid InstanceGuid { get; } = Guid.NewGuid();
}
