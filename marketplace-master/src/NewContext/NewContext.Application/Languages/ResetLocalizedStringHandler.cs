using CSharpFunctionalExtensions;
using NewContext.Acl.Languages;
using NewContext.Core;
using NewContext.Domain.Languages;
using System;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public class ResetLocalizedStringHandler : ICommandHandler<ResetLocalizedString>
    {
        private readonly IMessagesDispatcher dispatcher;
        private readonly IStringsRepository stringsRepository;

        public ResetLocalizedStringHandler(IMessagesDispatcher dispatcher, IStringsRepository stringsRepository)
        {
            this.dispatcher = dispatcher;
            this.stringsRepository = stringsRepository;
        }

        public async Task<Result> HandleAsync(ResetLocalizedString command)
        {
            string defaultStringValue = null;

            return await DefaultLanguageStrings.GetAsync(command.Label)
                .ToResult($"No default string has been found for label '{command.Label}'.")
                .OnFailure(error => throw new Exception(error))

                .OnSuccess(defaultString =>
                {
                    var result = stringsRepository.ChangeLocalizedStringAsync(command.Language, command.Label, defaultString.Value);
                    defaultStringValue = defaultString.Value;
                    return result;
                })

                .OnSuccess(() => dispatcher.PublishAsync(new LocalizedStringReset(
                    command.Language, command.Label, defaultStringValue)));
        }
    }
}
