using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.LeadSales;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.LeadSales
{
    public class LeadSalesQueries : ILeadSalesQueries
    {
        private const string quoteStatusQuery =
                @"SELECT TOP 1 UnlockedQuoteId FROM UnlockedQuote 
                INNER JOIN LeadSalesProfile ON LeadSalesProfile.Id = LeadSalesProfileId 
                WHERE UnlockedQuoteId = @quoteId AND LeadSalesProfile.SellerId = @sellerId";

        private const string listGroupsQuery =
                @"SELECT DISTINCT Quote.GroupId FROM UnlockedQuote
                INNER JOIN Quote ON Quote.Id = UnlockedQuoteId
                INNER JOIN Person ON Person.Id = @sellerId
                WHERE LeadSalesProfileId = Person.LeadSalesProfile_Id AND GroupId IS NOT NULL;";

        public bool IsQuoteUnlocked(string sellerId, string quoteId)
        {
            using (var dbConn = MyDbConnection.Create())
            {
                var result = dbConn.Query(quoteStatusQuery, new { quoteId, sellerId });
                return result.Any();
            }
        }

        public List<Guid> ListUnlockedGroups(string sellerId)
        {
            using (var dbConn = MyDbConnection.Create())
            {
                var result = dbConn.Query<Guid>(listGroupsQuery, new { sellerId }).ToList();
                return result ?? new List<Guid>();
            }
        }
    }
}
