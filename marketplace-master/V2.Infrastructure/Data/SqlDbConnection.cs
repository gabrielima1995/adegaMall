using Framework;
using System.Data.SqlClient;

namespace V2.Infrastructure.Data
{
    public class SqlDbConnection
    {
        public SqlDbConnection(ConnectionString connectionString)
        {
            Value = connectionString.Value;
        }

        public string Value { get; }

        public SqlConnection Connect() => new SqlConnection(Value);
    }
}
