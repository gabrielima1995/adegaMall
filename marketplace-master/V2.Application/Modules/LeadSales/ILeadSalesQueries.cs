using System;
using System.Collections.Generic;

namespace V2.Application.Modules.LeadSales
{
    public interface ILeadSalesQueries
    {
        bool IsQuoteUnlocked(string sellerId, string quoteId);

        List<Guid> ListUnlockedGroups(string sellerId);
    }
}
