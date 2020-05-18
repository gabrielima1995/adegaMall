using Domain.Interfaces;
using System;
using Unity;

namespace Infrastructure
{
    public class UnityIocContainer : IIocContainer
    {
        public static IUnityContainer Instance { get; set; }

        public object Get(Type type)
        {
            return Instance.Resolve(type);
        }

        public T Get<T>()
        {
            return Instance.Resolve<T>();
        }
    }
}
