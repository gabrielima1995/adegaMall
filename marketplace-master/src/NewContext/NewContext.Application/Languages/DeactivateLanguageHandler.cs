using CSharpFunctionalExtensions;
using NewContext.Core;
using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public class DeactivateLanguageHandler : ICommandHandler<DeactivateLanguage>
    {
        private readonly ILanguagesRepository languagesRepository;

        public DeactivateLanguageHandler(ILanguagesRepository languagesRepository)
        {
            this.languagesRepository = languagesRepository;
        }

        public async Task<Result> HandleAsync(DeactivateLanguage command)
        {
            var platformLanguage = await languagesRepository.GetAsync();
            var result = platformLanguage.DeactivateLanguage(command.Language);

            if (result.IsFailure)
                return result;

            await languagesRepository.UpdateAsync(platformLanguage);
            return Result.Ok();
        }
    }
}
