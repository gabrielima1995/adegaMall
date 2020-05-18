using Domain.Entities;
using Domain.Entities.DisputeModule;

namespace Domain.Events
{
    public class DisputeOpenedEvent : DomainEvent
    {
        public Dispute Dispute { get; set; }
        public Person OpenedBy { get; set; }

        //public Order Order { get; set; }

        public DisputeOpenedEvent(Dispute dispute, Person openedBy)
        {
            this.Dispute = dispute;
            this.OpenedBy = openedBy;

            //this.Order = order;
        }
    }
}
