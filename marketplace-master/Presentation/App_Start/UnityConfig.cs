using System;
using Unity;

namespace Presentation
{
    public class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        public static object Resolve(Type type)
        {
            var container = GetConfiguredContainer();

            if (!container.IsRegistered(type))
                return null;

            return container.Resolve(type);
        }

        public static T Resolve<T>()
        {
            return GetConfiguredContainer().Resolve<T>();
        }
    }
}