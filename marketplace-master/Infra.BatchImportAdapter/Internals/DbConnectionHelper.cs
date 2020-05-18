using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace Infra.BatchImportAdapter.Internals
{
    internal static class DbConnectionHelper
    {
        public static DbConnection GetConnection(string connectionStringName)
        {
            var connStr = ConfigurationManager.ConnectionStrings[connectionStringName];
            return new SqlConnection(connStr.ConnectionString);
        }
    }
}
