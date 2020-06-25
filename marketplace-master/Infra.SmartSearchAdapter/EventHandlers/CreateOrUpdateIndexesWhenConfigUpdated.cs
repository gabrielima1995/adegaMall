using Domain.Events;
using Domain.Events.Config;
using Domain.Interfaces;
using Domain.Modules.Search;

namespace Infra.SmartSearchAdapter.EventHandlers
{
    internal class CreateOrUpdateIndexesWhenConfigUpdated : IDomainEventHandler<ConfigUpdatedEvent>
    {
        private readonly IBackgroundWorker backgroundWorker;

        public CreateOrUpdateIndexesWhenConfigUpdated(
            IBackgroundWorker backgroundWorker)
        {
            this.backgroundWorker = backgroundWorker;
        }

        public void Handle(ConfigUpdatedEvent domainEvent)
        {
            if (!SearchSpecs.MustCreateOrUpdateIndices(domainEvent.OldConfig, domainEvent.NewConfig)) return;

            backgroundWorker.Enqueue<IndexUpdater>(i => i.CreateOrUpdateIndexes());
        }
    }
}