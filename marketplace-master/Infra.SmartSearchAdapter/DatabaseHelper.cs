using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Infra.SmartSearchAdapter
{
    internal class DatabaseHelper
    {
        public static IDbConnection CreateConnection()
        {
            var connectionSettings = ConfigurationManager.ConnectionStrings["ScaffoldContext"];
            var connection = DbProviderFactories.GetFactory(connectionSettings.ProviderName).CreateConnection();
            connection.ConnectionString = connectionSettings.ConnectionString;
            return connection;
        }
    }
}