using Unity;
using Unity.Injection;

namespace NewContext.Queries
{
    public static class QueriesConfigurationExtensions
    {
        public static IUnityContainer AddNewContextQueries(this IUnityContainer container, string connectionString)
        {
            container
                .RegisterSingleton<QueriesConnectionString>(
                    new InjectionConstructor(connectionString));

            HandlerRegistration.AddQueryHandlers(container);

            return container;
        }
    }
}
