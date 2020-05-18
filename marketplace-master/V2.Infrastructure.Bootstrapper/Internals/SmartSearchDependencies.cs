using Domain.Interfaces.Repositories;
using Infra.SmartSearchAdapter;
using Infra.SmartSearchAdapter.Products;
using Infra.SmartSearchAdapter.Quotes;
using Infra.SmartSearchAdapter.Sellers;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Modules.Search;
using V2.Application.Modules.Search.Products;
using V2.Application.Modules.Search.Quotes;
using V2.Application.Modules.Search.Sellers;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class SmartSearchDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<QuoteIndexHelper>()
                .RegisterType<SellerIndexHelper>()
                .RegisterType<ProductIndexHelper>()

                .RegisterType<IndexUpdater>();

            container
                .RegisterType<SearchAppService>();

            container
                .RegisterType<IQuoteSearchService, QuoteSearchService>()
                .RegisterType<ISellerSearchService, SellerSearchService>()
                .RegisterType<IProductSearchService, ProductSearchService>();
        }
    }
}