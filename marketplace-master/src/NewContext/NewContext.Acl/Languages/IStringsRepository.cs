using CSharpFunctionalExtensions;
using Dapper;
using NewContext.Domain.Languages;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Acl.Languages
{
    public interface IStringsRepository
    {
        Task<Result> ChangeLocalizedStringAsync(Language language, Label label, string newValue);

        Task<Result> InsertLocalizedStringsAsync(IEnumerable<LocalizedString> localizedStrings);

        Task<IReadOnlyList<string>> ListExistingLabelsForLanguage(Language language);

        Task<Result> ReplicateStringsBetweenLanguagesAsync(Language referenceLanguage, Language targetLanguage);
    }

    public class StringsRepository : IStringsRepository
    {
        private readonly ConnectionString connectionString;

        public StringsRepository(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Result> ChangeLocalizedStringAsync(Language language, Label label, string newValue)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var sql = "UPDATE [Str] SET Value = @newValue WHERE Lang = @lang AND Label = @label";
                var lang = LegacyLanguageCodes.GetLecacyCode(language);
                await conn.ExecuteAsync(sql, new { lang, label = label.ToString(), newValue });
            }

            return Result.Ok();
        }

        public async Task<Result> InsertLocalizedStringsAsync(IEnumerable<LocalizedString> localizedStrings)
        {
            var array = localizedStrings.ToArray();
            var valueStatements = new List<string>(array.Length);

            using (var conn = new SqlConnection(connectionString.Value))
            {
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        valueStatements.Add($"(@lbl{i}, @lng{i}, @val{i}, GETUTCDATE())");
                        cmd.Parameters.Add($"lbl{i}", SqlDbType.VarChar, 8000).Value = array[i].Label.Value;
                        cmd.Parameters.Add($"lng{i}", SqlDbType.Int).Value = LegacyLanguageCodes.GetLecacyCode(array[i].Language);
                        cmd.Parameters.Add($"val{i}", SqlDbType.VarChar, 8000).Value = array[i].Value;
                    }

                    var allValues = string.Join(", ", valueStatements);
                    cmd.CommandText = "INSERT INTO [Str] VALUES " + allValues;

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Result.Ok();
        }

        public async Task<IReadOnlyList<string>> ListExistingLabelsForLanguage(Language language)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var legacyCode = LegacyLanguageCodes.GetLecacyCode(language);
                var sql = "SELECT Label FROM [Str] WHERE Lang = @lang";
                var result = await conn.QueryAsync<string>(sql, new { lang = legacyCode });
                return result.ToList().AsReadOnly();
            }
        }

        public async Task<Result> ReplicateStringsBetweenLanguagesAsync(Language referenceLanguage, Language targetLanguage)
        {
            var sqlCommand = @"
                -- DECLARE @referenceLang INT SET @referenceLang = 1
                -- DECLARE @newLang INT SET @newLang = 1

                INSERT INTO [Str] (Lang, Label, [Value], CreatedAt)
                SELECT @newLang, Reference.Label, Reference.[Value], GETUTCDATE()
	                FROM [Str] Reference
	                LEFT JOIN [Str] NewLang ON Reference.Label = NewLang.Label AND NewLang.Lang = @newLang
	                WHERE Reference.Lang = @referenceLang
		                AND NewLang.Label IS NULL";

            using (var conn = new SqlConnection(connectionString.Value))
            {
                var referenceLang = LegacyLanguageCodes.GetLecacyCode(referenceLanguage);
                var newLang = LegacyLanguageCodes.GetLecacyCode(targetLanguage);
                await conn.ExecuteAsync(sqlCommand, new { referenceLang, newLang });
            }

            return Result.Ok();
        }
    }
}
