using Domain.Modules.QuoteBillingModule.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Modules.QuoteBillingModule.Interfaces
{
    public interface IQuoteBillingRepository
    {
        QuoteBilling Get(Guid quoteBillingId);

        QuoteBilling GetForQuote(Guid quoteId);

        QuoteBilling GetForOrder(Guid orderId);

        QuoteBilling GetForProduct(Guid productId);

        void Add(QuoteBilling quoteBilling);

        void Update(QuoteBilling quoteBilling);

        void Delete(QuoteBilling quoteBilling);

        bool ListContainsBillingProducts(IEnumerable<Guid> productIds);

        IReadOnlyCollection<BillingQuoteProductIds> ListPendingBillingInfosByProductIds(IEnumerable<Guid> productIds);

        IReadOnlyCollection<QuoteBillingIdOrderIdRelationDto> ListBillingVsOrderIdsFromCartId(Guid cartId);
    }
}
