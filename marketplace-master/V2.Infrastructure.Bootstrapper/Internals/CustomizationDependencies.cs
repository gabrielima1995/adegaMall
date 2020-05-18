using Domain.Interfaces.Repositories;
using Domain.Modules.Customization;
using Domain.Modules.Customization.Interfaces;
using Domain.Services;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Messages;
using V2.Application.Modules.Customization.Queries;
using V2.Application.Modules.General.CustomFields;
using V2.Infrastructure.Messages;
using V2.Infrastructure.Modules.Customization.Queries;
using V2.Infrastructure.Modules.Customization.Repositories;
using V2.Infrastructure.Modules.General.CustomFields;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class CustomizationDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                // General
                .RegisterType<CustomizationService>()
                .RegisterType<ICustomizationRepository, CustomizationRepository>()
                .RegisterType<IPlatformCustomizationQueries, PlatformCustomizationQueries>()

                // Ecommerce home
                .RegisterType<EcommerceHomeService>()
                .RegisterType<IEcommerceHomeCategoryRepository, EcommerceHomeCategoryRepository>()
                .RegisterType<IEcommerceHomeRepository, EcommerceHomeRepository>()
                .RegisterType<ICustomItemRepository, CustomItemRepository>()
                .RegisterType<ICustomItemCategoryRepository, CustomItemCategoryRepository>()

                // Banners
                .RegisterType<BannerService>()
                .RegisterType<IBannerRepository, BannerRepository>()

                //Custom Entity Fields
                .RegisterType<ICustomFieldRepository, CustomFieldRepository>()
                .RegisterType<ICustomFieldQueries, CustomFieldQueries>()
                .RegisterType<ICustomFieldAppService, CustomFieldAppService>()

                // Email templates
                .RegisterType<MessageTemplateSeeder>()
                .RegisterType<IMessageTemplateRepository, MessageTemplateRepository>()
                .RegisterType<ITemplateEngine, DatabaseTemplateEngine>();
        }
    }
}