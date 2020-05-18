using System.Linq;
using System.Reflection;

namespace System
{
    public static class AssemblyExtensions
    {
        public static Type[] GetTypesImplementing(this Assembly assembly, Type interfaceType, bool includeAbstractTypes = false)
        {
            return assembly.GetTypes()
                .Where(type => type.IsClass && interfaceType.IsAssignableFrom(type))
                .Where(type => includeAbstractTypes || !type.IsAbstract)
                .ToArray();
        }
    }
}
