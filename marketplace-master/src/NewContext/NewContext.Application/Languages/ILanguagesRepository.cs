using NewContext.Domain.Languages;
using System.Threading.Tasks;

namespace NewContext.Application.Languages
{
    public interface ILanguagesRepository
    {
        Task AddSingleAsync(PlatformLanguage platformLanguage);

        Task<bool> ExistsAsync();

        Task<PlatformLanguage> GetAsync();

        Task UpdateAsync(PlatformLanguage platformLanguage);
    }
}