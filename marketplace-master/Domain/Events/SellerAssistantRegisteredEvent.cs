using Domain.Entities;

namespace Domain.Events
{
    public class SellerAssistantRegisteredEvent : DomainEvent
    {
        public SellerAssistantRegisteredEvent(SellerAssistant assistant, string password)
        {
            Assistant = assistant;
            Password = password;
        }

        public SellerAssistant Assistant { get; }
        public string Password { get; }
    }
}
