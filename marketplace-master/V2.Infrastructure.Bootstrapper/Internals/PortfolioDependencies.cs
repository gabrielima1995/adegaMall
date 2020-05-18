using Application.Modules.Portfolio;
using Application.Modules.Portfolio.Categories;
using Application.Modules.Portfolio.SubCategories;
using Domain.Categories;
using Domain.Interfaces.Repositories;
using Domain.Modules.Portfolio;
using Domain.SubCategories;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Infrastructure.Modules.Portfolio;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class PortfolioDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                // Categorization
                .RegisterType<CategoryService_Obsolete>()
                .RegisterType<SubCategoryService_Obsolete>()
                .RegisterType<ICategoryAppService, CategoryAppService>()
                .RegisterType<IPortfolioCategoryRepository, PortfolioCategoryRepository>()
                .RegisterType<IPortfolioSubCategoryRepository, PortfolioSubCategoryRepository>()
                .RegisterType<ISubCategoryAppService, SubCategoryAppService>()

                // Portfolio
                .RegisterType<PortfolioItemService>()
                .RegisterType<PortfolioService>()
                .RegisterType<IPortfolioAppService, PortfolioAppService>()
                .RegisterType<IPortfolioRepository, PortfolioRepository>()
                .RegisterType<IPortfolioQueries, PortfolioQueries>();
        }
    }
}