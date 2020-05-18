using Domain.Entities;

namespace Domain.Events
{
    public class SellerRegisteredEvent : DomainEvent
    {
        public string SellerId => SellerPerson.Id;

        public Person SellerPerson { get; }

        public SellerRegisteredEvent(Person seller)
        {
            SellerPerson = seller ?? throw new System.ArgumentNullException(nameof(seller));
        }
    }
}
