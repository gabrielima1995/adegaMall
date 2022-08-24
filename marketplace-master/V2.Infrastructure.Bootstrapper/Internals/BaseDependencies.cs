using Application.Addresses;
using Application.Home;
using Application.Reports;
using Application.StaticPages;
using CloudStorage;
using Domain.Addresses;
using Domain.Config.ConfigMigrations;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Integration;
using Domain.Reports;
using Domain.Services;
using Framework;
using Geocoding;
using Infra.Crosscutting.ExchangeRate.USD;
using Infrastructure;
using Infrastructure.Caching;
using Infrastructure.Data;
using Infrastructure.Data.Context;
using Infrastructure.Data.Repositories;
using Infrastructure.Geocoding;
using Unity;
using Unity.Lifetime;
using V2.Application.Cities;
using V2.Application.Modules.Admin;
using V2.Application.Modules.PushNotifications;
using V2.Application.Modules.SiteMap;
using V2.Infrastructure.Data;
using V2.Infrastructure.General.Cities;
using V2.Infrastructure.Messages;
using V2.Infrastructure.Modules.Metrics;
using V2.Infrastructure.Modules.PushNotifications;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class BaseDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                // Database
                .RegisterType<ScaffoldContext>(new ScopedLifetimeManager())
                .RegisterType<SqlDbConnection>()

                // Config
                .RegisterType<ConfigMigrator>()
                .RegisterType<IApplicationConfigRepository, ApplicationConfigRepository>()
                .RegisterType<IUserConfigRepository, UserConfigRepository>()

                // General
                .RegisterType<ReportService>()
                .RegisterType<IAdminReportRepository, AdminReportRepository>()
                .RegisterType<HomeAppService>()
                .RegisterType<IReportAppService, ReportAppService>()
                .RegisterType<IStaticPagesAppService, StaticPagesAppService>()
                .RegisterType<ITimeZoneProvider, TimeZoneProvider>()
                .RegisterType(typeof(IService<>), typeof(ServiceBase<>))

                // Metrics
                .RegisterType<IMetricsQueries, MetricsQueries>()

                // Address
                .RegisterType<AddressService>()
                .RegisterType<IAddressAppService, AddressAppService>()
                .RegisterType<IGeocoder, Geocoder>(new ScopedLifetimeManager())
                .RegisterType<IGeocodingService, GeocodingService>()

                //Site Map
                .RegisterType<SiteMapAppService, SiteMapAppService>()

                // General infrastructure
                .RegisterType<ErrorReportingService>()
                .RegisterType<IBackgroundWorker, BackgroundWorker>()
                .RegisterType<ICache, MemoryCache>(new SingletonLifetimeManager())
                .RegisterType<IEmailSender, BackgroundEmailSender>().RegisterType<EmailSender>()
                .RegisterType<IIocContainer, UnityIocContainer>()
                .RegisterType<IUnitOfWork, EfUnitOfWork>()
                .RegisterType<IUSDExchangeRateService, USDExchangeRateService>()
                .RegisterType(typeof(IRepository<>), typeof(Repository<>))

                // Strings, globalization
                .RegisterType<GlobalizationService>()
                .RegisterType<StrService>()
                .RegisterType<IStringRepository, StringRepository>()

                // Cities database
                .RegisterType<ICityManager, CityManager>()
                .RegisterType<ICityQueries, CityQueries>()

                // Push Notification dependencies
                .RegisterType<IPushSubscriptionRepository, PushSubscriptionRepository>()
                .RegisterType<IPushNotificationService, PushNotificationService>()
                .RegisterType<IAdminNotificationQueries, AdminPushNotificationQueries>()

                // Cloud storage
                .RegisterType<ICloudStorage, AwsCloudStorage>()

                // Google Shopping
                .RegisterType<IntegrationParameterFromGoogleService>()
                .RegisterType<IIntegrationParameterFromGoogleRepository, IntegrationParameterFromGoogleRepository>();
        }
    }
}