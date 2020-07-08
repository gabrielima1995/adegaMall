using Application.Modules.Products;
using Application.Modules.Products.Categories;
using Application.Modules.Products.Orders;
using Application.Modules.Products.Queries;
using Application.Modules.Products.StockManagement;
using Application.Modules.Products.Store;
using Application.Modules.Products.Variants;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.Orders;
using Domain.Modules.Products.StockManagement;
using Domain.Modules.Products.Variants;
using Domain.Services.StoreModule;
using Domain.StoreModule.Interfaces;
using Framework.AsapLog.ConsultaFrete;
using Framework.Json;
using HttpClientFactoryLite;
using Infra.Crosscutting.AsapLog.ConsultaFrete;
using Infrastructure.Data.Repositories;
using Infrastructure.Shipping;
using System;
using Unity;
using Unity.Lifetime;
using V2.Application.Modules.StoreModule;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Json;
using V2.Infrastructure.Modules.Search;
using V2.Infrastructure.Modules.StoreModule.Queries;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class ProductDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            var httpClientFactory = new HttpClientFactory();

            httpClientFactory.Register("asaplog", builder => builder
                // Configure HttpClient before it's returned
                .ConfigureHttpClient(c => { })
                // Set HttpMessageHandler lifetime
                .SetHandlerLifetime(TimeSpan.FromMinutes(10)));

            container
                .RegisterInstance<IHttpClientFactory>(httpClientFactory, new SingletonLifetimeManager())
                .RegisterType<IJsonProvider, JsonProvider>()
                .RegisterType<IAsapLogConsultaFreteProvider, AsapLogConsultaFreteProvider>()

                // Store
                .RegisterType<StoreService>()
                .RegisterType<IStoreAppService, StoreAppService>()
                .RegisterType<IStoreRepository, StoreRepository>()
                .RegisterType<IStoreProductQueries, StoreProductQueries>()

                // Product registration
                .RegisterType<ProductSearchStore>()
                .RegisterType<ProductService>()
                .RegisterType<ProductService_Obsolete>()
                .RegisterType<IProductAppService, ProductAppService>()
                .RegisterType<IProductCategoryAppService, ProductCategoryAppService>()
                .RegisterType<IProductQueries, ProductQueries>()
                .RegisterType<IProductSearchQueries, ProductSearchQueries>()
                .RegisterType<IProductRepository, ProductRepository>()
                .RegisterType<ProductSmartSearchAppService>()

                // Product categorization
                .RegisterType<ProductCategoryService>()
                .RegisterType<ProductVariantService>()
                .RegisterType<ProductVariantTypeService>()
                .RegisterType<IProductCategoryRepository, ProductCategoryRepository>()
                .RegisterType<IProductVariantAppService, ProductVariantAppService>()
                .RegisterType<IProductVariantRepository, ProductVariantRepository>()
                .RegisterType<IProductVariantTypeAppService, ProductVariantTypeAppService>()

                // Order, cart, checkout
                .RegisterType<OrderService_Obsolete>()
                .RegisterType<OrderTransaction>()
                .RegisterType<OrderTransactionService>()
                .RegisterType<ICartQueries, CartQueries>()
                .RegisterType<ICartRepository, CartRepository>()
                .RegisterType<OrderAppService>()
                .RegisterType<IOrderQueries, OrderQueries>()
                .RegisterType<IOrderRepository, OrderRepository>()
                .RegisterType<IOrderTransactionRepository, OrderTransactionRepository>()
                .RegisterType<IPostalService, CorreiosPostalService>()
                .RegisterType<IShippingService, ShippingService>()
                .RegisterType<RefundOrderOnOrderCancelledOrRejected>()

                // Stock management
                .RegisterType<StockTransactionService>()
                .RegisterType<IStockTransactionAppService, StockTransactionAppService>()
                .RegisterType<IStockTransactionRepository, StockTransactionRepository>()

                .RegisterType<CalculatePriceOfProductsVersionOutJob>()
                .RegisterType<CalculatePriceOfProductsJob>();
        }
    }
}