using Dapper;
using Domain.Modules.QuoteBillingModule.Entities;
using Domain.Modules.QuoteBillingModule.Interfaces;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using V2.Infrastructure.Data;
using V2.Infrastructure.Data.SqlScripts;

namespace V2.Infrastructure.Modules.QuoteBillingModule.Repositories
{
    public class QuoteBillingRepository : IQuoteBillingRepository
    {
        private readonly ScaffoldContext _context;

        public QuoteBillingRepository(ScaffoldContext context)
        {
            _context = context;
        }

        public void Add(QuoteBilling quoteBilling)
        {
            _context.QuoteBillings.Add(quoteBilling);
        }

        public void Delete(QuoteBilling quoteBilling)
        {
            _context.Entry(quoteBilling).State = EntityState.Deleted;
        }

        public QuoteBilling Get(Guid quoteBillingId)
        {
            return _context.QuoteBillings.Find(quoteBillingId);
        }

        public QuoteBilling GetForOrder(Guid orderId)
        {
            return _context.QuoteBillings.SingleOrDefault(x => x.OrderId == orderId);
        }

        public QuoteBilling GetForProduct(Guid productId)
        {
            return _context.QuoteBillings.SingleOrDefault(x => x.ProductId == productId);
        }

        public QuoteBilling GetForQuote(Guid quoteId)
        {
            return _context.QuoteBillings.SingleOrDefault(x => x.QuoteId == quoteId);
        }

        public IReadOnlyCollection<BillingQuoteProductIds> ListPendingBillingInfosByProductIds(IEnumerable<Guid> productIds)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    --DECLARE @productIds VARCHAR(MAX) SET @productIds = 'DBA689CE-37F9-4DAA-B2E6-C4B2812BE165,AA4FE926-99E1-4D38-83C6-40EB64E4BA26'
                    SELECT Id QuoteBillingId, QuoteId, ProductId
                    FROM QuoteBilling
                    WHERE OrderId IS NULL AND ProductId IN (SELECT value FROM STRING_SPLIT(@productIds, ','))";

                return conn.Query<BillingQuoteProductIds>(sql, new { productIds = string.Join(",", productIds) }).ToList();
            }
        }

        public IReadOnlyCollection<QuoteBillingIdOrderIdRelationDto> ListBillingVsOrderIdsFromCartId(Guid cartId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = SqlScriptHelper.GetScript("QuoteBillingRepository_ListBillingVsOrderIdsFromCartId");
                var resultList = conn.Query<QuoteBillingIdOrderIdRelationDto>(sql, new { cartId }).ToList();
                return resultList;
            }
        }

        public bool ListContainsBillingProducts(IEnumerable<Guid> productIds)
        {
            return _context.QuoteBillings.Any(b =>
                productIds.Any(id => b.ProductId == id));
        }

        public void Update(QuoteBilling quoteBilling)
        {
            _context.Entry(quoteBilling).State = EntityState.Modified;
        }
    }
}
