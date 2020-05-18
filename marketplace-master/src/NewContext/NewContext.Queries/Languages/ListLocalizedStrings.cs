using Dapper;
using NewContext.Acl.Languages;
using NewContext.Core;
using NewContext.Queries.Dtos;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Queries.Languages
{
    public class ListLocalizedStrings : IQuery<IEnumerable<LocalizedStringDto>>
    {
    }

    public class ListLocalizedStringsHandler : IQueryHandler<ListLocalizedStrings, IEnumerable<LocalizedStringDto>>
    {
        private readonly QueriesConnectionString connectionString;

        public ListLocalizedStringsHandler(QueriesConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<LocalizedStringDto>> HandleAsync(ListLocalizedStrings query)
        {
            using (var conn = new SqlConnection(connectionString.Value))
            {
                var sql = "SELECT Label, Lang, Value FROM [Str]";
                var lines = await conn.QueryAsync<DbModel>(sql);
                return lines.Select(line => Map(line)).Where(m => m != null).ToArray();
            }
        }

        private LocalizedStringDto Map(DbModel dbModel)
        {
            var language = LegacyLanguageCodes.GetLanguageByLegacyCode(dbModel.Lang);

            return new LocalizedStringDto
            {
                Language = language,
                Label = dbModel.Label,
                Value = dbModel.Value
            };
        }

        private class DbModel
        {
            public string Label { get; set; }
            public int Lang { get; set; }
            public string Value { get; set; }
        }
    }
}