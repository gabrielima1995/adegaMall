using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Infrastructure.Data.Context
{
    public class MyDbConfiguration : DbConfiguration
    {
        protected internal MyDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
