using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NewContext.Application.Languages;
using NewContext.Core;
using NewContext.Infrastructure.Data;
using NewContext.Infrastructure.Languages;
using NewContext.Infrastructure.Util;
using Owin;
using Unity;
using Unity.Injection;

namespace NewContext.Infrastructure
{
    public static class InfrastructureConfigurationExtensions
    {
        public static IUnityContainer AddNewContextInfrastructure(this IUnityContainer container, string connectionString)
        {
            container
                .AddCaching()
                .AddDbContext(connectionString)
                .RegisterSingleton<CommandsConnectionString>(new InjectionConstructor(connectionString))
                .RegisterSingleton<IMessagesDispatcher, MessagesDispatcher>(new InjectionConstructor(container))
                .RegisterType<ILanguagesRepository, LanguagesRepository>();

            HandlerRegistration.AddDomainEventHandlers(container);

            LanguagesHelper.Container = container;
            container.RegisterSingleton<LanguagesHelperInternal>();

            return container;
        }

        public static IAppBuilder UseNewContextInfrastructure(this IAppBuilder app, IUnityContainer container)
        {
            container.Resolve<AppDbContext>().Database.Migrate();
            return app;
        }

        private static IUnityContainer AddCaching(this IUnityContainer container)
        {
            return container
                .RegisterInstance<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()))
                .RegisterSingleton<ICachingProvider, MemoryCachingProvider>();
        }

        private static IUnityContainer AddDbContext(this IUnityContainer container, string connectionString)
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString, (sql) => sql.MigrationsHistoryTable("__MigrationHistory", DefaultSchema.Value))
                .Options;

            return container
                .RegisterInstance<DbContextOptions>(dbContextOptions)
                .RegisterType<AppDbContext>();
        }
    }
}
