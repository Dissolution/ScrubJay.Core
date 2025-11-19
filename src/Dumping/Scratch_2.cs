using System.Reflection;


namespace ScrubJay.Dumping;

public partial class Dumper
{
    public static string Dump<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        using var inst = new DumpInst();
        if (!string.IsNullOrEmpty(valueName))
        {
            inst.Append(valueName)
                .Append(": ");
        }

        try
        {
            inst.DumpObject(value);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Debugger.Break();
            throw;
        }


        return inst.ToString();
    }
}

internal sealed class DumpInst : IDisposable
{
    private const string INDENT = "  "; // 2 spaces

    private static bool UseRender(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(nint) ||
               type == typeof(nuint) ||
               type == typeof(decimal) ||
               type == typeof(Guid) ||
               type == typeof(TimeSpan) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type.Implements<ITuple>() ||
               type.IsEnum ||
               type == typeof(Type);
    }

    private readonly TextBuilder _builder = new();
    private readonly HashSet<object> _visited = [];

    internal TextBuilder Append(string? str) => _builder.Append(str);

    private DumpInst DumpDictionary(IDictionary dict)
    {
        _builder.Append($"{dict:@T}[{dict.Count}]");

        if (dict.Count == 0)
        {
            return this;
        }

        _builder.Append(':')
            .Indent(INDENT)
            .Delimit(TBA.NewLine, dict.OfType<DictionaryEntry>(), (tb, entry) =>
            {
                DumpObject(entry.Key);
                tb.Write(": ");
                DumpObject(entry.Value);
            })
            .Dedent();
        return this;
    }

    private void DumpEnumerable(IEnumerable enumerable)
    {
        _builder.Render(enumerable.GetType());

        var e = enumerable.GetEnumerator();
        if (!e.MoveNext())
        {
            _builder.Write(": {}");
            return;
        }

        _builder.Append(':')
            .Indent(INDENT)
            .NewLine();
        DumpObject(e.Current);
        while (e.MoveNext())
        {
            _builder.NewLine();
            DumpObject(e.Current);
        }

        _builder.Dedent();
        e.Dispose();
    }


    public void DumpObject(object? obj)
    {
        if (obj is null)
        {
            _builder.Append("`null");
            return;
        }

        var type = obj.GetType();

        if (UseRender(type))
        {
            _builder.Render(obj);
            return;
        }

        // we have to avoid circular references
        if (!type.IsValueType)
        {
            if (!_visited.Add(obj))
            {
                _builder.Append("<🔁>");
                return;
            }
        }

        // Handle dictionaries
        if (obj is IDictionary dict)
        {
            DumpDictionary(dict);
            return;
        }

        // Handle enumerables (lists, arrays, etc.)
        if (obj is IEnumerable enumerable)
        {
            DumpEnumerable(enumerable);
            return;
        }

        if (obj is MemberInfo member)
        {
            DumpMember(member);
            return;
        }

        // Handle complex objects
        DumpComplex(obj);
    }

    private void DumpComplex(object obj)
    {
        var type = obj.GetType();

        _builder.Render(type);

        var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToList();

        if (properties.Count > 0)
        {
            _builder
                .Append(':')
                .Indent(INDENT)
                .Enumerate(properties, (tb, prop) =>
                {
                    // can we even get this property value?
                    var res = Result.Try(() => prop.GetValue(obj));
                    if (res.IsOk(out var value))
                    {
                        tb.NewLine()
                            .Append(prop.Name)
                            .Write(": ");
                        DumpObject(value);
                    }
                })
                .Dedent();
        }
        else
        {
            var fields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .ToList();

            if (fields.Count > 0)
            {
                _builder
                    .Append(':')
                    .Indent(INDENT)
                    .Enumerate(fields, (tb, field) =>
                    {
                        // can we even get this property value?
                        var res = Result.Try(() => field.GetValue(obj));
                        if (res.IsOk(out var value))
                        {
                            tb.NewLine()
                                .Append(field.Name)
                                .Write(": ");
                            DumpObject(value);
                        }
                    })
                    .Dedent();
            }
            else
            {
                throw Ex.Unreachable();
            }
        }
    }

    private void DumpParameter(ParameterInfo parameter)
    {
        _builder.Render(parameter.ParameterType)
            .IfNotNull(parameter.Name, static (tb, n) => tb.Append(' ').Write(n))
            .If(parameter.HasDefaultValue, tb =>
            {
                tb.Write(" = ");
                DumpObject(parameter.DefaultValue);
            });
    }

    private void DumpMember(MemberInfo member)
    {
        switch (member)
        {
            case Type type:
                _builder.Render(type);
                return;
            case MethodInfo methodInfo:
            {
                var owner = methodInfo.DeclaringType ?? methodInfo.ReflectedType ?? methodInfo.Module.GetType();
                _builder.Render(owner)
                    .Append('.')
                    //.AppendName(methodInfo)
                    //.AppendGenericTypes(methodInfo)
                    .Append('(')
                    .Delimit(", ", methodInfo.GetParameters(), (tb, param) => DumpParameter(param))
                    .Append(')');
                return;
            }
            case EventInfo eventInfo:
                break;
            case ConstructorInfo constructorInfo:
                break;
            case FieldInfo fieldInfo:
                break;
            case PropertyInfo propertyInfo:
                break;
            default:
                throw Ex.Argument(member);
        }

        throw Ex.NotImplemented();
    }

    public void Dispose()
    {
        _builder.Dispose();
        _visited.Clear();
    }


    public override string ToString()
    {
        return _builder.ToString();
    }
}