using CSharpFunctionalExtensions;
using NewContext.Core;
using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public class ActivateLanguageHandler : ICommandHandler<ActivateLanguage>
    {
        private readonly ILanguagesRepository languagesRepository;

        public ActivateLanguageHandler(ILanguagesRepository languagesRepository)
        {
            this.languagesRepository = languagesRepository;
        }

        public async Task<Result> HandleAsync(ActivateLanguage command)
        {
            var platformLanguage = await languagesRepository.GetAsync();
            var result = platformLanguage.ActivateLanguage(command.Language);

            if (result.IsFailure)
                return result;

            await languagesRepository.UpdateAsync(platformLanguage);
            return Result.Ok();
        }
    }
}
