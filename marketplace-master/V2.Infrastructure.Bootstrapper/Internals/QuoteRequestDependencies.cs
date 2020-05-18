using Domain.Interfaces.Repositories;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Modules.QuoteRequests.Queries;
using V2.Infrastructure.Modules.QuoteRequests.Queries;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class QuoteRequestDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<IQuoteQueries, QuoteQueries>()
                .RegisterType<IQuoteAnswerRepository, QuoteAnswerRepository>()
                .RegisterType<IQuoteRepository, QuoteRepository>();
        }
    }
}