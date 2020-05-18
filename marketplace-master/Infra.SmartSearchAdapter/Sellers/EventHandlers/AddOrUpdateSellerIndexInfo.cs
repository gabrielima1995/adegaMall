using Domain.Events;
using Domain.Events.PortfolioModule;
using Domain.Events.Subscriptions;

namespace Infra.SmartSearchAdapter.Sellers.EventHandlers
{
    public class AddOrUpdateSellerIndexInfo :
        IDomainEventHandler<PortfolioCreatedEvent>,
        IDomainEventHandler<PortfolioItemAddedEvent>,
        IDomainEventHandler<PortfolioItemRemovedEvent>,
        IDomainEventHandler<PortfolioItemUpdatedEvent>,
        IDomainEventHandler<PortfolioUpdatedEvent>,
        IDomainEventHandler<SellerRegisteredEvent>,
        IDomainEventHandler<SellerUpdatedEvent>,
        IDomainEventHandler<SubscriptionAddedEvent>,
        IDomainEventHandler<SubscriptionCancelledEvent>,
        IDomainEventHandler<PersonUpdatedEvent>
    {
        public void Handle(PersonUpdatedEvent domainEvent)
        {
            if (domainEvent.IsSeller)
                UpdateSellerIndex(domainEvent.PersonId);
        }

        public void Handle(SellerRegisteredEvent domainEvent)
            => new SellerIndexHelper().AddItem(domainEvent.SellerId);

        public void Handle(SellerUpdatedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.SellerId);

        public void Handle(SubscriptionCancelledEvent domainEvent)
            => UpdateSellerIndex(domainEvent.Subscription.SellerId);

        public void Handle(SubscriptionAddedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.PersonId);

        public void Handle(PortfolioItemRemovedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.SellerId);

        public void Handle(PortfolioItemAddedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.SellerId);

        public void Handle(PortfolioItemUpdatedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.SellerId);

        public void Handle(PortfolioCreatedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.Portfolio.SellerId);

        public void Handle(PortfolioUpdatedEvent domainEvent)
            => UpdateSellerIndex(domainEvent.Portfolio.SellerId);

        private void UpdateSellerIndex(string sellerId)
            => new SellerIndexHelper().UpdateItem(sellerId);
    }
}