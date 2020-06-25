using Dapper;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.Subscriptions.Model;
using V2.Application.Modules.Subscriptions.Queries;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.Subscription.Queries
{
    public class SubscriptionQueries : ISubscriptionQueries
    {
        public List<SubscriptionListItemForAdminDto> ListSubscriptions()
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("SubscriptionQueries_ListSubscriptions");
                var query = conn.Query<SubscriptionListItemForAdminDto>(sql);
                return query.ToList();
            }
        }
    }
}