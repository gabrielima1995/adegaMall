using Dapper;
using System.Linq;
using V2.Application.Modules.UserAccounts;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.UserRegistration.Queries
{
    public class UserAccountQueries : IUserAccountQueries
    {
        public BasicAccountInfo GetBasicInfo(string userId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("UserAccountQueries_GetBasicInfo");
                var query = conn.Query<BasicAccountInfo>(sql, new { userId });
                return query.SingleOrDefault();
            }
        }
    }
}
