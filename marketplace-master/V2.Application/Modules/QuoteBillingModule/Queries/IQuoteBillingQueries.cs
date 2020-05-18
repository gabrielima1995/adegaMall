using Domain.Modules.QuoteBillingModule.Entities;
using System;
using System.Collections.Generic;

namespace V2.Application.Modules.QuoteBillingModule.Queries
{
    public interface IQuoteBillingQueries
    {
        QuoteBilling GetBillingByQuoteGroup(Guid quoteGroupId);

        List<QuoteBilling> ListBillingsForQuoteIds(IEnumerable<Guid> quoteIds);
    }
}
