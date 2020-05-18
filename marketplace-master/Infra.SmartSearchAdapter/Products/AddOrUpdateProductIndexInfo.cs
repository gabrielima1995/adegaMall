using Domain.Entities.StoreModule;
using Domain.Events;
using Domain.Events.PortfolioModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Events.StoreModule;
using Domain.Events.Subscriptions;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Entities;
using System;

namespace Infra.SmartSearchAdapter.Products
{
    public class AddOrUpdateProductIndexInfo :
        IDomainEventHandler<SellerUpdatedEvent>,
        IDomainEventHandler<SellerProfileUpdated>,
        IDomainEventHandler<PersonUpdatedEvent>,
        IDomainEventHandler<PortfolioUpdatedEvent>,
        IDomainEventHandler<SubscriptionAddedEvent>,
        IDomainEventHandler<SubscriptionCancelledEvent>,
        IDomainEventHandler<StoreUpdatedEvent>,

        IDomainEventHandler<ProductAddedEvent>,
        IDomainEventHandler<ProductUpdatedEvent>,
        IDomainEventHandler<ProductDeletedEvent>,
        IDomainEventHandler<ProductAvailabilityAdded>,
        IDomainEventHandler<ProductAvailabilityUpdated>,
        IDomainEventHandler<ProductAvailabilityDeleted>,
        IDomainEventHandler<ProductCurrentStockChanged>,
        IDomainEventHandler<ProductPriceChangedEvent>,

        IDomainEventHandler<OrderStatusChangedEvent>,
        IDomainEventHandler<NewOrderRegisteredEvent>,
        IDomainEventHandler<RentalInfoCreated>,
        IDomainEventHandler<RentalInfoUpdated>,
        IDomainEventHandler<RentalCancelled>,

        IDomainEventHandler<RentalLockAdded>,
        IDomainEventHandler<RentalLockDeleted>
    {
        private readonly IBackgroundWorker _backgroundWorker;
        private readonly IOrderRepository _orderRepository;

        public AddOrUpdateProductIndexInfo(
            IBackgroundWorker backgroundWorker,
            IOrderRepository orderRepository)
        {
            _backgroundWorker = backgroundWorker;
            _orderRepository = orderRepository;
        }

        private void UpdateItemsBySeller(string sellerId)
            => _backgroundWorker.Schedule<ProductIndexHelper>(x => x.UpdateItemsBySeller(sellerId),
                TimeSpan.FromSeconds(10));

        private void UpdateProductIndex(Guid id)
            => _backgroundWorker.Schedule<ProductIndexHelper>(x => x.UpdateItem(id),
                TimeSpan.FromSeconds(10));

        public void Handle(SellerUpdatedEvent domainEvent)
            => UpdateItemsBySeller(domainEvent.SellerId);

        public void Handle(SellerProfileUpdated domainEvent)
            => UpdateItemsBySeller(domainEvent.SellerId);

        public void Handle(PersonUpdatedEvent domainEvent)
            => UpdateItemsBySeller(domainEvent.PersonId);

        public void Handle(PortfolioUpdatedEvent domainEvent)
            => UpdateItemsBySeller(domainEvent.Portfolio.SellerId);

        public void Handle(SubscriptionAddedEvent domainEvent)
            => UpdateItemsBySeller(domainEvent.PersonId);

        public void Handle(SubscriptionCancelledEvent domainEvent)
            => UpdateItemsBySeller(domainEvent.Subscription.SellerId);

        public void Handle(StoreUpdatedEvent domainEvent)
            => UpdateItemsBySeller(domainEvent.SellerId);

        public void Handle(ProductUpdatedEvent domainEvent)
            => UpdateProductIndex(domainEvent.Product.Id);

        public void Handle(ProductAddedEvent domainEvent)
            => UpdateProductIndex(domainEvent.Product.Id);

        public void Handle(ProductDeletedEvent domainEvent)
            => UpdateProductIndex(domainEvent.ProductId);

        public void Handle(ProductAvailabilityAdded domainEvent)
            => UpdateProductIndex(domainEvent.ProductAvailability.Product_Id);

        public void Handle(ProductAvailabilityUpdated domainEvent)
            => UpdateProductIndex(domainEvent.ProductAvailability.Product_Id);

        public void Handle(ProductAvailabilityDeleted domainEvent)
            => UpdateProductIndex(domainEvent.ProductAvailability.Product_Id);

        public void Handle(ProductCurrentStockChanged domainEvent)
            => UpdateProductIndex(domainEvent.Product.Id);

        public void Handle(ProductPriceChangedEvent domainEvent)
            => UpdateProductIndex(domainEvent.ProductId);

        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.NewStatus.In(OrderStatus.Canceled, OrderStatus.Rejected))
            {
                foreach (var orderProduct in domainEvent.Order.Products)
                {
                    UpdateProductIndex(orderProduct.Product_Id.Value);
                }
            }
        }

        public void Handle(NewOrderRegisteredEvent domainEvent)
        {
            var order = _orderRepository.GetDetails(domainEvent.OrderId);

            foreach (var orderProduct in order.Products)
            {
                UpdateProductIndex(orderProduct.Product_Id.Value);
            }
        }

        public void Handle(RentalInfoCreated domainEvent)
        {
            UpdateProductIndex(domainEvent.OrderProduct.Product_Id.Value);
        }

        public void Handle(RentalInfoUpdated domainEvent)
        {
            UpdateProductIndex(domainEvent.OrderProduct.Product_Id.Value);
        }

        public void Handle(RentalCancelled domainEvent)
        {
            var order = domainEvent.Order;

            foreach (var orderProduct in order.Products)
            {
                UpdateProductIndex(orderProduct.Product_Id.Value);
            }
        }

        public void Handle(RentalLockAdded domainEvent)
        {
            var rentalLock = domainEvent.RentalLock;

            if (rentalLock.Product_Id.HasValue)
                UpdateProductIndex(rentalLock.Product_Id.Value);
            else
                UpdateItemsBySeller(rentalLock.Seller_Id);
        }

        public void Handle(RentalLockDeleted domainEvent)
        {
            var rentalLock = domainEvent.RentalLock;

            if (rentalLock.Product_Id.HasValue)
                UpdateProductIndex(rentalLock.Product_Id.Value);
            else
                UpdateItemsBySeller(rentalLock.Seller_Id);
        }
    }
}