using Unity;
using V2.Application.Modules.ProductTemplateModule;
using V2.Infrastructure.Modules.ProductTemplateModule;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    public class ProductTemplateDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<IProductTemplateAdminQueries, ProductTemplateAdminQueries>()
                .RegisterType<IProductTemplateStoreQueries, ProductTemplateStoreQueries>()
                .RegisterType<IProductTemplateRepository, ProductTemplateRepository>();
        }
    }
}
