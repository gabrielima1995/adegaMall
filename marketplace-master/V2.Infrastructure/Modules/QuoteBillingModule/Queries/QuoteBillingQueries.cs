using Domain.Modules.QuoteBillingModule.Entities;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.QuoteBillingModule.Queries;

namespace V2.Infrastructure.Modules.QuoteBillingModule.Queries
{
    public class QuoteBillingQueries : IQuoteBillingQueries
    {
        private readonly ScaffoldContext _context;

        public QuoteBillingQueries(ScaffoldContext context)
        {
            _context = context;
        }

        public QuoteBilling GetBillingByQuoteGroup(Guid quoteGroupId)
        {
            var quoteIds = _context.Quotes
                .Where(x => x.GroupId == quoteGroupId)
                .Select(x => x.Id)
                .ToList();

            return ListBillingsForQuoteIds(quoteIds).FirstOrDefault();
        }

        public bool HasAnyBillingProducts(IEnumerable<Guid> productIds)
        {
            return _context.QuoteBillings.Any(b =>
                productIds.Any(id => b.ProductId == id));
        }

        public List<QuoteBilling> ListBillingsForQuoteIds(IEnumerable<Guid> quoteIds)
        {
            if (quoteIds == null)
                return new List<QuoteBilling>(0);

            return _context.QuoteBillings
                .Where(x => quoteIds.Any(id => id == x.QuoteId))
                .ToList();
        }
    }
}
