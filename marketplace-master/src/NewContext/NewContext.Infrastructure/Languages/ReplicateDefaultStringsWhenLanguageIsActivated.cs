using NewContext.Acl.Languages;
using NewContext.Application.Languages;
using NewContext.Core;
using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Infrastructure.Languages
{
    public class ReplicateDefaultStringsWhenLanguageIsActivated : IDomainEventHandler<LanguageActivated>
    {
        private readonly IStringsRepository stringsRepository;

        public ReplicateDefaultStringsWhenLanguageIsActivated(IStringsRepository stringsRepository)
        {
            this.stringsRepository = stringsRepository;
        }

        public async Task HandleAsync(LanguageActivated domainEvent)
        {
            await stringsRepository.ReplicateStringsBetweenLanguagesAsync(
                ApplicationLanguages.Default, domainEvent.Language);
        }
    }
}
