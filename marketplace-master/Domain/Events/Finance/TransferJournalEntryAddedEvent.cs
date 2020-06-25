using Domain.Modules.Finance;

namespace Domain.Events.Finance
{
    public class TransferJournalEntryAddedEvent : DomainEvent
    {
        public TransferJournalEntry Entry { get; private set; }

        public TransferJournalEntryAddedEvent(TransferJournalEntry entry)
        {
            Entry = entry;
        }
    }
}
