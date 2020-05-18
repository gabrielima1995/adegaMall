using Dapper;
using NewContext.Acl.Languages;
using NewContext.Core;
using NewContext.Queries.Dtos;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Queries.Languages
{
    public class ListStringsForAdminTranslation : IQuery<StringsForAdminTranslationDto>
    {
    }

    public class ListStringsForAdminTranslationHandler : IQueryHandler<ListStringsForAdminTranslation, StringsForAdminTranslationDto>
    {
        private readonly QueriesConnectionString connectionString;

        public ListStringsForAdminTranslationHandler(QueriesConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<StringsForAdminTranslationDto> HandleAsync(ListStringsForAdminTranslation query)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var entries = (await conn.QueryAsync<DbModel>(sql)).ToArray();

                var langGroups = entries.GroupBy(entry => entry.Lang);
                var languages = langGroups.Select(group => LegacyLanguageCodes.GetLanguageByLegacyCode(group.Key).Specifier);

                var labelGroups = entries.GroupBy(entry => entry.Label);
                var labelsWithTranslations = labelGroups.Select(group =>
                {
                    var translations = group.Select(g => new StringsForAdminTranslationDto.Translation
                    {
                        Language = LegacyLanguageCodes.GetLanguageByLegacyCode(g.Lang).Specifier,
                        Value = g.Value
                    });

                    return new StringsForAdminTranslationDto.LabelWithTranslations
                    {
                        Label = group.Key,
                        Translations = translations.ToList().AsReadOnly()
                    };
                });

                return new StringsForAdminTranslationDto
                {
                    Languages = languages.ToList().AsReadOnly(),
                    LabelsWithTranslations = labelsWithTranslations.ToList().AsReadOnly()
                };
            }
        }

        private class DbModel
        {
            public string Label { get; set; }
            public int Lang { get; set; }
            public string Value { get; set; }
        }

        private const string sql = @"
            DROP TABLE IF EXISTS #tmpLanguages

            SELECT 
	            [Value] Specifier,
	            CONVERT(INT, 0) LegacyCode
            INTO #tmpLanguages
            FROM STRING_SPLIT((SELECT TOP 1 ActiveLanguages FROM nc.PlatformLanguage), ',')

            UPDATE #tmpLanguages
            SET LegacyCode = CASE
	            WHEN Specifier = 'en-US' THEN 1
	            WHEN Specifier = 'pt-BR' THEN 2
	            WHEN Specifier = 'es-ES' THEN 3
	            WHEN Specifier = 'ca-ES' THEN 4 END

            SELECT [Str].*
            FROM [Str]
            INNER JOIN #tmpLanguages ON [Str].Lang = #tmpLanguages.LegacyCode

            DROP TABLE IF EXISTS #tmpLanguages";
    }
}
