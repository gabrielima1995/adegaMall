using Dapper;
using NewContext.Core;
using NewContext.Domain.Languages;
using NewContext.Queries.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Queries.Languages
{
    public class ListLanguagesForConfiguration : IQuery<IEnumerable<LanguageForConfigurationDto>>
    {
    }

    public class ListLanguagesForConfigurationHandler : IQueryHandler<ListLanguagesForConfiguration, IEnumerable<LanguageForConfigurationDto>>
    {
        private readonly QueriesConnectionString connectionString;

        public ListLanguagesForConfigurationHandler(QueriesConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<LanguageForConfigurationDto>> HandleAsync(ListLanguagesForConfiguration query)
        {
            var platformLanguage = await GetPlatformLanguageAsync();
            var activeLanguages = platformLanguage.ActiveLanguages.Split(',');

            return Language.SupportedLanguages.Select(lang => new LanguageForConfigurationDto
            {
                Language = lang,
                IsActive = activeLanguages.Contains(lang),
                IsDefault = lang == platformLanguage.DefaultLanguage
            });
        }

        private async Task<PlatformLanguageDbModel> GetPlatformLanguageAsync()
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var sql = "SELECT ActiveLanguages, DefaultLanguage FROM nc.PlatformLanguage";
                return await conn.QuerySingleAsync<PlatformLanguageDbModel>(sql);
            }
        }

        private class PlatformLanguageDbModel
        {
            public string ActiveLanguages { get; set; }
            public string DefaultLanguage { get; set; }
        }
    }
}