using Domain.Entities.DisputeModule;

namespace Domain.Events
{
    public class DisputeStatusChangedEvent : DomainEvent
    {
        public DisputeStatusChangedEvent(Dispute dispute, DisputeStatus newStatus)
        {
            Dispute = dispute;
            NewStatus = newStatus;
        }

        public Dispute Dispute { get; private set; }
        public DisputeStatus NewStatus { get; private set; }
    }
}
