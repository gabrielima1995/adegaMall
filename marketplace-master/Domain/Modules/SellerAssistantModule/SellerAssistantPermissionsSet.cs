using Domain.Events;

namespace Domain.Modules.SellerAssistantModule
{
    public class SellerAssistantPermissionsSet : DomainEvent
    {
        public SellerAssistantPermissionsSet(string assistantUserId, string assistantPersonId)
        {
            AssistantUserId = assistantUserId;
            AssistantPersonId = assistantPersonId;
        }

        public string AssistantUserId { get; }
        public string AssistantPersonId { get; }
    }
}
