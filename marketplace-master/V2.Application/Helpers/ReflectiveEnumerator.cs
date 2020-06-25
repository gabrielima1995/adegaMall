using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace V2.Application.Helpers
{
    public static class ReflectiveEnumerator
    {
        public static List<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            var objects = Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                .Select(type => (T)Activator.CreateInstance(type, constructorArgs))
                .ToList();

            return objects;
        }
    }
}