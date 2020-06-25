using Application.Config;
using Application.Modules.Ratings;
using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Modules.QuoteBillingModule.Interfaces;
using System;

namespace Application.Modules.Products.Orders.EventHandlers
{
    public class RequestUserRatingOnOrderCompleted : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly IUserRatingAppService ratingAppService;
        private readonly IQuoteBillingRepository billingRepository;

        public RequestUserRatingOnOrderCompleted(
            IUserRatingAppService ratingAppService,
            IQuoteBillingRepository billingRepository)
        {
            this.ratingAppService = ratingAppService;
            this.billingRepository = billingRepository;
        }

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (ShouldSkip(domainEvent))
                return;

            var quoteId = GetQuoteIdIfIsBilling(domainEvent);
            var sellerRating = GenerateSellerRatingIfEnabled(domainEvent, quoteId);
            var buyerRating = GenerateBuyerRatingIfEnabled(domainEvent, quoteId);

            AssignPairsIfEnabled(sellerRating, buyerRating);
        }

        private string GetQuoteIdIfIsBilling(OrderStatusChangedEvent domainEvent)
        {
            if (IsQuoteBilling())
            {
                var billing = billingRepository.GetForOrder(domainEvent.Order.Id);
                if (billing != null && billing.QuoteId != null)
                    return billing.QuoteId.ToString();
            }
            return null;
        }

        private Guid? GenerateSellerRatingIfEnabled(OrderStatusChangedEvent domainEvent, string quoteId)
        {
            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
                return ratingAppService.NewPendingRating(domainEvent.Order.Buyer.Id, domainEvent.Order.Seller.Id, "Seller", domainEvent.Order.OrderId, quoteId).Result;
            return null;
        }

        private Guid? GenerateBuyerRatingIfEnabled(OrderStatusChangedEvent domainEvent, string quoteId)
        {
            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
                return ratingAppService.NewPendingRating(domainEvent.Order.Seller.Id, domainEvent.Order.Buyer.Id, "Buyer", domainEvent.Order.OrderId, quoteId).Result;
            return null;
        }

        private void AssignPairsIfEnabled(Guid? sellerRating, Guid? buyerRating)
        {
            if (MaaSConfig.Current.Ratings.DoubleBlindRatings && sellerRating.HasValue && buyerRating.HasValue)
                ratingAppService.AssignRatingPairs(sellerRating.Value.ToString(), buyerRating.Value.ToString());
        }

        private bool ShouldSkip(OrderStatusChangedEvent domainEvent)
        {
            var shouldHandle = domainEvent.NewStatus.In(OrderStatus.Delivered, OrderStatus.Completed) ||
                (IsQuoteBilling() && domainEvent.NewStatus == OrderStatus.Completed);
            return !shouldHandle;
        }

        private bool IsQuoteBilling() => MaaSConfig.Current.Quote.Enabled && MaaSConfig.Current.Quote.QuoteBilling.Enabled;
    }
}
