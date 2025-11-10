using System.Reflection;
using ScrubJay.Text.Rendering;

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
            inst.DumpKey(valueName)
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

    private static bool IsSimple(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(decimal) ||
               type == typeof(Guid) ||
               type == typeof(TimeSpan) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset);
    }

    private readonly TextBuilder _builder = new();
    private readonly HashSet<object> _visited = [];


    public DumpInst Append(scoped text text)
    {
        _builder.Write(text);
        return this;
    }

    public DumpInst DumpString(string str, bool escape)
    {
        if (!escape)
        {
            _builder.Write(str);
        }
        else
        {
            _builder.Append('"').Append(str).Write('"');
        }

        return this;
    }

    public DumpInst DumpKey(object? key)
    {
        if (key is string str)
        {
            return DumpString(str, false);
        }
        else if (key is null)
        {
            return Append("`null`");
        }
        else
        {
            return DumpString(key.ToString() ?? "`null", true);
        }
    }

    public DumpInst DumpEnum(Enum e)
    {
        EnumInfo.RenderTo(_builder, e);
        return this;
    }

    public DumpInst DumpTuple<T>(T tuple)
        where T : ITuple
    {
        _builder.Write('[');
        if (tuple.Length > 0)
        {
            DumpObject(tuple[0]);
            for (var i = 1; i < tuple.Length; i++)
            {
                _builder.Write(", ");
                DumpObject(tuple[i]);
            }
        }

        _builder.Append(']');
        return this;
    }

    public DumpInst DumpSimple(object obj)
    {
        if (obj is bool boolean)
        {
            _builder.If(boolean, "true", "false");
        }
        else if (obj is byte u8)
        {
            _builder.Format(u8);
        }
        else if (obj is sbyte i8)
        {
            _builder.Format(i8);
        }
        else if (obj is short i16)
        {
            _builder.Format(i16);
        }
        else if (obj is ushort u16)
        {
            _builder.Format(u16);
        }
        else if (obj is int i32)
        {
            _builder.Format(i32);
        }
        else if (obj is uint u32)
        {
            _builder.Format(u32).Write('U');
        }
        else if (obj is long i64)
        {
            _builder.Format(i64).Write('L');
        }
        else if (obj is ulong u64)
        {
            _builder.Format(u64).Write("UL");
        }
        else if (obj is IntPtr intptr)
        {
            _builder.Format(intptr).Write('*');
        }
        else if (obj is UIntPtr uintptr)
        {
            _builder.Format(uintptr).Write('*');
        }
        else if (obj is char ch)
        {
            _builder.Append('\'').Append(ch).Write('\'');
        }
        else if (obj is float f32)
        {
            _builder.Format(f32, "G").Write('f');
        }
        else if (obj is double f64)
        {
            _builder.Format(f64, "G").Write('d');
        }
        else if (obj is decimal dec)
        {
            _builder.Format(dec, "G").Write('m');
        }
        else if (obj is DateTime dt)
        {
            _builder.Append('"').Format(dt, "yyyy-MM-dd HH:mm:ss").Write('"');
        }
        else if (obj is DateTimeOffset dto)
        {
            _builder.Append('"').Format(dto, "yyyy-MM-dd HH:mm:ss").Write('"');
        }
        else if (obj is TimeSpan ts)
        {
            _builder.Append('"').Format(ts).Write('"');
        }
        else if (obj is Guid guid)
        {
            _builder.Render(guid);
        }
        else
        {
            throw Ex.Unreachable();
        }

        return this;
    }

    public DumpInst DumpDictionary(IDictionary dict)
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
                DumpKey(entry.Key);
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

        if (IsSimple(type))
        {
            DumpSimple(obj);
            return;
        }

        if (obj is string str)
        {
            DumpString(str, true);
            return;
        }

        if (obj is Enum @enum)
        {
            DumpEnum(@enum);
            return;
        }

        if (obj is ITuple tuple)
        {
            DumpTuple(tuple);
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
                        tb.NewLine();
                        DumpKey(prop.Name);
                        tb.Write(": ");
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
                            tb.NewLine();
                            DumpKey(field.Name);
                            tb.Write(": ");
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
                    .AppendName(methodInfo)
                    .AppendGenericTypes(methodInfo)
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