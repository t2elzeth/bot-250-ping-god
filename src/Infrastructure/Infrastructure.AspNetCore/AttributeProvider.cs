using System.Collections.Concurrent;
using System.Reflection;

namespace Infrastructure.AspNetCore;

internal static class AttributeProvider<T> where T : Attribute
{
    private static readonly ConcurrentDictionary<MemberInfo, T?> Cache = new();

    public static T? FirstOrDefault(MemberInfo methodInfo)
    {
        return Cache.GetOrAdd(methodInfo, m =>
        {
            var attribute = GetFirstOfType(m.GetCustomAttributes(true));

            if (attribute != null)
                return attribute;

            return GetFirstOfType(m.DeclaringType!.GetTypeInfo().GetCustomAttributes(true));
        });
    }

    private static T? GetFirstOfType(object[] arr)
    {
        foreach (var obj in arr)
        {
            if (obj is T marker)
                return marker;
        }

        return null;
    }
}