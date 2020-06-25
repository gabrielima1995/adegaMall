using CSharpFunctionalExtensions;
using NewContext.Acl.Languages;
using NewContext.Core;
using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public class ChangeLocalizedStringHandler : ICommandHandler<ChangeLocalizedString>
    {
        private readonly IMessagesDispatcher dispatcher;
        private readonly IStringsRepository stringsRepository;

        public ChangeLocalizedStringHandler(IMessagesDispatcher dispatcher, IStringsRepository stringsRepository)
        {
            this.dispatcher = dispatcher;
            this.stringsRepository = stringsRepository;
        }

        public async Task<Result> HandleAsync(ChangeLocalizedString command)
        {
            return await stringsRepository.ChangeLocalizedStringAsync(command.Language, command.Label, command.NewValue)
                .OnSuccess(() => dispatcher.PublishAsync(new LocalizedStringChanged(command.Language, command.Label, command.NewValue)));
        }
    }
}
