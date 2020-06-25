using Infra.Crosscutting.Globalization;
using NewContext.Core;
using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Acl.Languages
{
    public class UpdateGlobalizationHelperWhenLocalizeStringChanges :
        IDomainEventHandler<LocalizedStringChanged>,
        IDomainEventHandler<LocalizedStringReset>
    {
        public Task HandleAsync(LocalizedStringReset domainEvent)
        {
            GlobalizationHelper.UpdateStringInMemory(
                domainEvent.Label, domainEvent.Label, domainEvent.DefaultValue);

            return Task.CompletedTask;
        }

        public Task HandleAsync(LocalizedStringChanged domainEvent)
        {
            GlobalizationHelper.UpdateStringInMemory(
                domainEvent.Label, domainEvent.Language, domainEvent.NewValue);

            return Task.CompletedTask;
        }
    }
}
