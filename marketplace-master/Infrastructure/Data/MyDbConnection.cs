using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Infrastructure.Data
{
    internal static class MyDbConnection
    {
        public static IDbConnection Create()
        {
            var connectionSettings = ConfigurationManager.ConnectionStrings["ScaffoldContext"];
            var connection = GetConnection(connectionSettings.ConnectionString, connectionSettings.ProviderName);
            return connection;
        }

        private static DbConnection GetConnection(string connectionString, string providerName)
        {
            var factory = DbProviderFactories.GetFactory(providerName);

            if (factory == null)
                throw new InvalidOperationException(
                    string.Format("DbProviderFactory not found for provider '{0}'.", providerName));

            var connection = factory.CreateConnection();

            if (connection == null)
                throw new InvalidOperationException("Could not create a database connection.");

            connection.ConnectionString = connectionString;

            return connection;
        }
    }
}