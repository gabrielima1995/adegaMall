using Dapper;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.StoreModule.Queries
{
    public class CartQueries : ICartQueries
    {
        public int GetProductsCount(string buyerId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("CartQueries_GetProductsCount");
                return conn.ExecuteScalar<int>(sql, new { buyerId });
            }
        }
    }
}
