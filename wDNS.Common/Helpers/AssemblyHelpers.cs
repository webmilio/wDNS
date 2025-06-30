using System;
using System.Collections.Generic;
using System.Reflection;

namespace wDNS.Common.Helpers;

public static class AssemblyHelpers
{
    public static IEnumerable<TypeInfo> Get(TypeInfo parent)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(parent))
                {
                    yield return (TypeInfo) type;
                }
            }
        }
    }

    public static IEnumerable<TypeInfo> GetConcrete(TypeInfo parent)
    {
        foreach (var type in Get(parent))
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            yield return type;
        }
    }
}
