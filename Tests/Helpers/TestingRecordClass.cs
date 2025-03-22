namespace ScrubJay.Tests.Helpers;

[PublicAPI]
public sealed record class TestingRecordClass(int Id, string Name)
{
    public Guid InstanceGuid { get; } = Guid.NewGuid();
}

[PublicAPI]
public readonly record struct TestingReadonlyRecordStruct(int Id, string Name)
{
    public Guid InstanceGuid { get; } = Guid.NewGuid();
}
