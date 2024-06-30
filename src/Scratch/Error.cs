using System.Diagnostics;
using System.Reflection;
using ScrubJay.Text;

namespace Scratch;

public class Error
{
    public static Error WithStack()
    {
        StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
        Debugger.Break();
        throw new NotImplementedException();
    }
    
    public IReadOnlyDictionary<string, object?>? Data { get; init; } = null;
    public Uri? HelpUri { get; init; } = null;
    public int HResult { get; init; } = 0;
    public Error? InnerError { get; init; } = null;
    public MemberInfo? Source { get; init; } = null;
    public MethodBase? Target { get; init; } = null;
    public StackTrace? StackTrace { get; init; } = null;
    
    public string Message { get; }

    public Error()
    {
        this.Message = nameof(Error);
    }

    public Error(string? message)
    {
        this.Message = message ?? nameof(Error);
    }

    public Uri GetHResultInfo()
    {
        return new Uri($@"https://www.hresult.info/Search?q=0x{HResult:X8}");
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        var hasher = new Hasher();
        hasher.Add(Message);
        hasher.Add(InnerError);
        hasher.Add(HResult);
        hasher.Add(Source);
        hasher.Add(Target);
        if (Data is not null)
        {
            foreach ((string key, object? value) in Data)
            {
                hasher.Add(key);
                hasher.Add(value);
            }
        }
        return hasher.ToHashCode();
    }

    public override string ToString()
    {
        var text = StringBuilderPool.Rent();
        text.Append($"{this.GetType().Name}: {this.Message}");
        // TODO
        return text.ToStringAndReturn();
    }
}