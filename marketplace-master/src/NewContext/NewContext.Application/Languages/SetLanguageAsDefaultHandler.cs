using CSharpFunctionalExtensions;
using NewContext.Core;
using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public class SetLanguageAsDefaultHandler : ICommandHandler<SetLanguageAsDefault>
    {
        private readonly ILanguagesRepository languagesRepository;

        public SetLanguageAsDefaultHandler(ILanguagesRepository languagesRepository)
        {
            this.languagesRepository = languagesRepository;
        }

        public async Task<Result> HandleAsync(SetLanguageAsDefault command)
        {
            var platformLanguage = await languagesRepository.GetAsync();
            var result = platformLanguage.ChangeDefaultLanguage(command.Language);

            if (result.IsFailure)
                return result;

            await languagesRepository.UpdateAsync(platformLanguage);
            return Result.Ok();
        }
    }
}
