using Application.Modules.BatchImport;
using Application.Modules.BatchImport.Events;
using Application.Modules.BatchImport.Products;
using Infra.BatchImportAdapter;
using Infra.BatchImportAdapter.Internals;
using Infra.BatchImportAdapter.Products;
using Infra.BatchImportAdapter.Sellers;
using System.Collections.Generic;
using System.Configuration;
using Unity;
using Unity.Injection;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class BatchImportDependencies : IDependenciesRegistration
    {
        private static IUnityContainer container;

        public void Register(IUnityContainer container)
        {
            BatchImportDependencies.container = container;

            container
                .RegisterType<BatchImportAppService>()
                .RegisterType<CustomFieldsAdapter>()
                .RegisterType<IProductImportQueries, ProductImportQueries>()
                .RegisterType<IProductImportRepository, ProductImportRepository>()
                .RegisterType<ISellerAvatarUpdater, SellerAvatarUpdater>()
                .RegisterType<IBatchImportQueries, BatchImportQueries>()
                .RegisterType<IBatchImportService, BatchImportService>()
                .RegisterType<ISocialImageProviderResolver, SocialImageProviderResolver>()
                .RegisterType<IProductImagesZipExtractor, ProductImagesZipExtractor>();

            container
                // Add image providers here...
                .RegisterType<ISocialImageProvider, FacebookImageProvider>(nameof(FacebookImageProvider));
        }

        private class SocialImageProviderResolver : ISocialImageProviderResolver
        {
            public IEnumerable<ISocialImageProvider> ResolveAll() =>
                container.ResolveAll<ISocialImageProvider>();
        }
    }
}
