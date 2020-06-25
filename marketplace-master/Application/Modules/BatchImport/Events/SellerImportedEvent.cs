using Domain.Entities;
using Domain.Events;

namespace Application.Modules.BatchImport.Events
{
    public class SellerImportedEvent : DomainEvent
    {
        public Seller Seller { get; }

        public SellerImportedEvent(Seller seller)
        {
            Seller = seller;
        }
    }
}
