using Domain.Entities;
using Domain.Events.QuoteBillingModule;
using Framework;
using System;

namespace Domain.Modules.QuoteBillingModule.Entities
{
    public class QuoteBilling : BaseEntity<Guid>
    {
        public Guid QuoteId { get; private set; }

        public decimal Amount { get; private set; }

        public DateTime Deadline { get; private set; }

        public string SellerId { get; private set; }

        public string BuyerId { get; private set; }

        public Guid? ProductId { get; private set; }

        public Guid? OrderId { get; private set; }

        public bool IsCancelled { get; private set; }

        public bool IsPaid { get; private set; }

        public bool IsRefunded { get; private set; }

        public static QuoteBilling CreateBilling(Guid id, Guid quoteId, decimal amount, DateTime deadline, string sellerId, string buyerId)
        {
            Guard.Ensure.NotEmpty(id, nameof(id));
            Guard.Ensure.NotEmpty(quoteId, nameof(quoteId));
            Guard.Ensure.NotEmpty(sellerId, nameof(sellerId));
            Guard.Ensure.NotEmpty(buyerId, nameof(buyerId));

            var billing = new QuoteBilling
            {
                Id = id,
                QuoteId = quoteId,
                Amount = amount,
                Deadline = deadline,
                SellerId = sellerId,
                BuyerId = buyerId,
            };

            billing.AddDomainEvent(new QuoteBillingCreatedEvent(billing));
            return billing;
        }

        /// <summary>
        /// To be called when an order is placed to settle this billing.
        /// </summary>
        /// <param name="orderId"></param>
        public void SetOrderId(Guid orderId)
        {
            Guard.Ensure.NotEmpty(orderId, nameof(orderId));
            Guard.Ensure.NotEmpty(ProductId, nameof(ProductId), "Cannot set an Order Id when the Product Id is not set.");
            OrderId = orderId;
        }

        /// <summary>
        /// To be called when a product is created to represent the billing.
        /// </summary>
        /// <param name="productId"></param>
        public void SetProductId(Guid productId)
        {
            Guard.Ensure.NotEmpty(productId, nameof(productId));
            ProductId = productId;
        }

        /// <summary>
        /// To be called when the quote is being cancelled and the product will be deleted.
        /// </summary>
        public void ClearProductId()
        {
            Guard.Ensure.Null(OrderId, nameof(OrderId), "Cannot clear the Product Id when an order has been placed.");
            ProductId = null;
        }

        /// <summary>
        /// To be called when the billing is cancelled (not the associated order).
        /// </summary>
        public void MarkCancelled()
        {
            Guard.Ensure.IsFalse(IsPaid, nameof(IsPaid), "A quote billing cannot be cancelled when it is marked as Paid.");

            IsCancelled = true;
            AddDomainEvent(new QuoteBillingCancelledEvent(this));
        }

        /// <summary>
        /// To be called when the associated order is paid.
        /// </summary>
        public void MarkPaid()
        {
            Guard.Ensure.IsFalse(IsCancelled, nameof(IsCancelled), "A quote billing cannot be paid when it is marked as Cancelled.");
            Guard.Ensure.NotEmpty(OrderId, nameof(OrderId), "A quote billing cannot be paid without an Order Id.");

            IsPaid = true;
            AddDomainEvent(new QuoteBillingPaidEvent(this));
        }

        /// <summary>
        /// To be called when the associated order is cancelled or rejected, which triggers a refund.
        /// </summary>
        public void MarkRefunded()
        {
            Guard.Ensure.IsTrue(IsPaid, nameof(IsPaid), "A quote billing cannot be refunded when it is not marked as Paid.");

            IsRefunded = true;
            AddDomainEvent(new QuoteBillingRefundedEvent(this));
        }
    }
}