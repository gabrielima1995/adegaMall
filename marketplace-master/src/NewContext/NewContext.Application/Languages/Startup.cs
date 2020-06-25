using CSharpFunctionalExtensions;
using MoreLinq;
using NewContext.Acl.Languages;
using NewContext.Application.Startup;
using NewContext.Domain.Languages;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public class Startup : IStartup
    {
        private readonly ILanguagesRepository languagesRepository;
        private readonly IStringsRepository stringsRepository;

        public Startup(ILanguagesRepository languagesRepository, IStringsRepository stringsRepository)
        {
            this.languagesRepository = languagesRepository;
            this.stringsRepository = stringsRepository;
        }

        public async Task<Result> RunAsync()
        {
            return await InitilizePlatformLanguageAsync()
                .OnSuccess(() => InitializeLanguageStringsAsync())
                .OnSuccess(() => ReplicateDefaultStringsToSupportedLanguagesAsync());
        }

        private async Task<Result> InitializeLanguageStringsAsync()
        {
            var existingLabels = await stringsRepository.ListExistingLabelsForLanguage(ApplicationLanguages.Default);
            var allStrings = await DefaultLanguageStrings.ListAllAsync();
            var newStrings = allStrings.Where(str => !existingLabels.Any(lbl => str.Label == lbl));

            var batches = newStrings.Batch(100);

            foreach (var batch in batches)
            {
                var result = await stringsRepository.InsertLocalizedStringsAsync(batch);

                if (result.IsFailure)
                    return result;
            }

            return Result.Ok();
        }

        private async Task<Result> InitilizePlatformLanguageAsync()
        {
            var exists = await languagesRepository.ExistsAsync();

            if (!exists)
            {
                var platformLanguageResult = PlatformLanguage.Create(ApplicationLanguages.Default);

                if (platformLanguageResult.IsFailure)
                    return platformLanguageResult;

                await languagesRepository.AddSingleAsync(platformLanguageResult.Value);
            }

            return Result.Ok();
        }

        private async Task<Result> ReplicateDefaultStringsToSupportedLanguagesAsync()
        {
            var result = Result.Ok();

            foreach (var lang in Language.SupportedLanguages)
            {
                if (lang == ApplicationLanguages.Default)
                    continue;

                var thisResult = await stringsRepository.ReplicateStringsBetweenLanguagesAsync(
                    ApplicationLanguages.Default, lang);

                result = Result.Combine(result, thisResult);
            }

            return result;
        }
    }
}
