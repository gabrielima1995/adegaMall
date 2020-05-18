using Application.Modules.BatchImport;
using Dapper;
using Framework;
using Infra.BatchImportAdapter.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.BatchImportAdapter
{
    public class BatchImportQueries : IBatchImportQueries
    {
        private readonly ConnectionString connectionString;

        public BatchImportQueries(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<List<string>> ListUserEmailsAsync()
        {
            using (var conn = DbConnectionHelper.GetConnection(connectionString.Value))
            {
                var sql = "SELECT Email FROM Person";
                var emails = await conn.QueryAsync<string>(sql);
                return emails.ToList();
            }
        }
    }
}
