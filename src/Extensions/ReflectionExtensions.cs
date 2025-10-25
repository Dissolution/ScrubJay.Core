using System.Reflection;

namespace ScrubJay.Extensions;

[PublicAPI]
public static class ReflectionExtensions
{
    extension(MemberInfo? member)
    {
        public Type[] GenericTypes()
        {
            return member switch
            {
                Type type => type.GetGenericArguments(),
                MethodBase methodBase => methodBase.GetGenericArguments(),
                PropertyInfo propertyInfo =>
                    (propertyInfo.GetMethod ?? propertyInfo.SetMethod)?.GetGenericArguments() ?? [],
                EventInfo eventInfo =>
                    (eventInfo.AddMethod ?? eventInfo.RemoveMethod ?? eventInfo.RaiseMethod)?.GetGenericArguments() ?? [],
                _ => [],
            };
        }

        public bool IsGeneric
        {
            get
            {
                return member switch
                {
                    Type type => type.IsGenericType,
                    MethodBase methodBase => methodBase.IsGenericMethod,
                    PropertyInfo propertyInfo =>
                        (propertyInfo.GetMethod ?? propertyInfo.SetMethod)?.IsGenericMethod == true,
                    EventInfo eventInfo =>
                        (eventInfo.AddMethod ?? eventInfo.RemoveMethod ?? eventInfo.RaiseMethod)?.IsGenericMethod == true,
                    _ => false
                };
            }
        }
    }
}