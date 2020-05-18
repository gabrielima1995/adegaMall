namespace Domain.Events
{
    public class PersonUpdatedEvent : DomainEvent
    {
        public bool IsBuyer { get; }
        public bool IsSeller { get; }
        public string PersonId { get; set; }

        public PersonUpdatedEvent(string personId, bool isBuyer, bool isSeller)
        {
            PersonId = personId;
            IsBuyer = isBuyer;
            IsSeller = isSeller;
        }
    }
}