using Domain.Interfaces.Repositories;
using Domain.Modules.Integration;
using Infrastructure.Data.Repositories;
using Infrastructure.Logging;
using Unity;
using V2.Application.Webhooks;
using V2.Infrastructure.Webhooks;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class ApiDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                // Tokens
                .RegisterType<IApiTokenRepository, ApiTokenRepository>()

                // Webhooks
                .RegisterType<WebhookCallService>()
                .RegisterType<WebhookLogService>()
                .RegisterType<IWebhookCallService, WebhookCallService>()
                .RegisterType<IWebhookRepository, WebhookRepository>()

                // Logging
                .RegisterType<ApiLogService>()
                .RegisterType<IRequestReceivedApiLogRepository, RequestReceivedApiLogRepository>()
                .RegisterType<IWebhookCalledApiLogRepository, WebhookCalledApiLogRepository>()
            
                // Google Shopping
                .RegisterType<IntegrationParameterFromGoogleService>()
                .RegisterType<IIntegrationParameterFromGoogleRepository, IntegrationParameterFromGoogleRepository>();
        }
    }
}