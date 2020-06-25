using Domain.Events;
using Domain.Events.Config;
using Domain.Interfaces;
using Domain.Modules.StoreModule.Price;

namespace V2.Application.Modules.StoreModule.EventHandlers
{
    public class CalculatePriceOfProductsWhenConfigUpdated : IDomainEventHandler<ConfigUpdatedEvent>
    {
        private readonly IBackgroundWorker backgroundWorker;

        public CalculatePriceOfProductsWhenConfigUpdated(
            IBackgroundWorker backgroundWorker)
        {
            this.backgroundWorker = backgroundWorker;
        }

        public void Handle(ConfigUpdatedEvent domainEvent)
        {
            if (!ProductPriceSpecs.MustCalculatePriceAllProducts(domainEvent.OldConfig, domainEvent.NewConfig)) return;

            backgroundWorker.Enqueue<CalculatePriceOfProductsJob>(i => i.Run());
        }
    }
}